using AsmResolver.Collections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomPlaylists.Plugins
{
    internal class CustomPlaylistJsonLoading
    {
        public static void LoadCustomPlaylists()
        {
            var dirInfo = new DirectoryInfo(Plugin.Instance.ConfigCustomPlaylistDirectory.Value);
            if (!dirInfo.Exists)
            {
                Directory.CreateDirectory(Plugin.Instance.ConfigCustomPlaylistDirectory.Value);
                return;
            }

            var files = dirInfo.GetFiles("*.json", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name == "Template.json")
                {
                    continue;
                }

                LoadPlaylist(files[i].FullName);
            }
        }

        static void LoadPlaylist(string filePath)
        {
            var node = JsonNode.Parse(File.ReadAllText(filePath));
            if (node == null)
            {
                return;
            }

            string name;
            bool enabled = false;
            CategoryPanelData panel = new CategoryPanelData();
            List<string> songIds = new List<string>();

            if (node["Enabled"] is not null)
            {
                enabled = node["Enabled"].GetValue<bool>();
            }
            if (enabled == false)
            {
                // No need to parse any further
                return;
            }
            if (node["Name"] is not null)
            {
                name = node["Name"]!.GetValue<string>();
            }
            if (node["CategoryPanelData"] is not null)
            {
                var panelNode = node["CategoryPanelData"];
                if (panelNode["Name"] is not null)
                {
                    panel.Name = panelNode["Name"].GetValue<string>();
                }
                if (panelNode["BgColor"] is not null)
                {
                    var color = panelNode["BgColor"].GetValue<string>();
                    if (ColorUtility.TryParseHtmlString(color, out var newColor))
                    {
                        panel.BgColor = newColor;
                    }
                }
                if (panelNode["FrameType"] != null)
                {
                    var frameType = panelNode["FrameType"].GetValue<string>();
                    if (Enum.TryParse<FrameType>(frameType, true, out var newFrameType))
                    {
                        panel.FrameType = newFrameType;
                    }
                }
                if (panelNode["FrameColor"] is not null)
                {
                    var color = panelNode["FrameColor"].GetValue<string>();
                    if (ColorUtility.TryParseHtmlString(color, out var newColor))
                    {
                        panel.FrameColor = newColor;
                    }
                }
                if (panelNode["Order"] is not null)
                {
                    panel.Order = panelNode["Order"].GetValue<int>();
                }
            }
            if (node["SongIds"] is not null)
            {
                var array = node["SongIds"].AsArray();
                for (int i = 0; i < array.Count; i++)
                {
                    songIds.Add(array[i].GetValue<string>());
                }
            }

            panel.InitializeCallback(delegate { return songIds; });
            panel.AddToManager();
        }
    }
}
