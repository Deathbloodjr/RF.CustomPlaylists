using AsmResolver.Collections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomPlaylists.Plugins
{
    internal class CustomPlaylistJsonLoading
    {
        static List<CategoryPanelData> LoadedPlaylists = new List<CategoryPanelData>();

        public static void LoadCustomPlaylists()
        {
            var dirInfo = new DirectoryInfo(Plugin.Instance.ConfigCustomPlaylistDirectory.Value);
            if (!dirInfo.Exists)
            {
                Directory.CreateDirectory(Plugin.Instance.ConfigCustomPlaylistDirectory.Value);
                CreateTemplateJson();
            }

            bool templateFound = false;
            var files = dirInfo.GetFiles("*.json", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name == "Template.json")
                {
                    templateFound = true;
                }
                LoadPlaylist(files[i].FullName);
            }
            if (!templateFound)
            {
                CreateTemplateJson();
                LoadPlaylist(Path.Combine(Plugin.Instance.ConfigCustomPlaylistDirectory.Value, "Template.json"));
            }
        }

        public static void UnloadCustomPlaylists()
        {
            for (int i = 0; i < LoadedPlaylists.Count; i++)
            {
                LoadedPlaylists[i].RemoveFromManager();
                LoadedPlaylists.RemoveAt(0);
                i--;
            }
        }

        public static void ReloadCustomPlaylists()
        {
            UnloadCustomPlaylists();
            LoadCustomPlaylists();
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
            LoadedPlaylists.Add(panel);
        }

        static void CreateTemplateJson()
        {
            JsonObject obj = new JsonObject()
            {
                ["Name"] = "Template",
                ["Enabled"] = false,
                ["CategoryPanelData"] = new JsonObject()
                {
                    ["Name"] = "Base Game",
                    ["BgColor"] = "#F7F1E1",
                    ["FrameType"] = "SingleColor",
                    ["FrameColor"] = "#B8DCF2",
                    ["Order"] = 60,
                },
                ["SongIds"] = new JsonArray()
                {
                    "natsu",
                    "twcfsp",
                    "ddudu",
                    "ohdlov",
                    "yoruk2",
                    "yrsda2",
                    "mrgold",
                    "mgaaot",
                    "zense",
                    "xjapan",
                    "ohdmi2",
                    "kimetu",
                    "eveka2",
                    "anaint",
                    "shing2",
                    "dora4",
                    "eva",
                    "digmon",
                    "smoon2",
                    "dball2",
                    "totoro",
                    "snhero",
                    "roki",
                    "gumikg",
                    "vfvill",
                    "gumids",
                    "vfshrr",
                    "tmbeat",
                    "csmclr",
                    "csmmon",
                    "pirate",
                    "pixelg",
                    "fdive",
                    "mope",
                    "th7171",
                    "thflnd",
                    "astero",
                    "clsw",
                    "clsbu",
                    "clsmil",
                    "cls12r",
                    "clsff",
                    "clscam",
                    "rockmw",
                    "monhnw",
                    "p5last",
                    "p5life",
                    "udtmgl",
                    "pacm40",
                    "kata",
                    "klwind",
                    "crtesc",
                    "syairl",
                    "sw2op",
                    "ssn3rd",
                    "cs7op",
                    "kumokn",
                    "kappa",
                    "yumeut",
                    "fmod",
                    "insidm",
                    "solstr",
                    "hold",
                    "1234dk",
                    "nijbtn",
                    "ptptpk",
                    "ponpok",
                    "8ka7ki",
                    "vixtor",
                    "dptfct",
                    "zerosy",
                    "flyagn",
                    "gnkcrt",
                    "freewy",
                    "ai7ndz",
                    "tkmdst",
                    "lactea",
                    "struck",
                    "dragoo",
                    "ycoast",
                    "goget",
                    "59shin",
                    "kalice",
                    "mnpure",
                    "hayabu",
                    "tokkyo",
                    "7fuku"
                },
            };


            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                //Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
            string outputPath = Path.Combine(Plugin.Instance.ConfigCustomPlaylistDirectory.Value, "Template.json");
            File.WriteAllText(outputPath, obj.ToJsonString(options));
        }
    }
}
