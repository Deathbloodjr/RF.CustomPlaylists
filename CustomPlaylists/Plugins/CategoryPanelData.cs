using Scripts.OutGame.SongSelect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomPlaylists.Plugins
{
    public delegate List<MusicDataInterface.MusicInfoAccesser> CategoryPanelMusicInfoCallback();
    public delegate List<string> CategoryPanelSongIdCallback();
    public delegate List<int> CategoryPanelUniqueIdCallback();

    public enum FrameType
    {
        None = 0,
        SingleColor = 1,
        Gradient = 2,
    }

    public class CategoryPanelData
    {
        /// <summary>
        /// Minimum 100<br />
        /// Maximum Unknown, 900 works, possibly int.MaxValue
        /// </summary>
        public int GenreId = -1;
        public string Name = string.Empty;
        public ThemeTypes ThemeId = ThemeTypes.Library;
        public int Order = 0;

        public int BgType = 1;
        public Color BgColor = Color.white;

        /// <summary>
        /// 0 = No border<br />
        /// 1 = Single color border<br />
        /// 2 = Gradient border<br />
        /// </summary>
        public FrameType FrameType = FrameType.None;
        public Color FrameColor = Color.white;

        /// <summary>
        /// I don't think changing this affects anything.
        /// </summary>
        public Color TextFaceColor = Color.white;

        /// <summary>
        /// I don't think changing this affects anything.
        /// </summary>
        public Color TextUnderlayColor = Color.white;

        CategoryPanelMusicInfoCallback? CallbackMusicInfo = null;
        CategoryPanelSongIdCallback? CallbackSongId = null;
        CategoryPanelUniqueIdCallback? CallbackUniqueId = null;

        public void InitializeCallback(CategoryPanelMusicInfoCallback Callback)
        {
            CallbackMusicInfo = Callback;
        }
        public void InitializeCallback(CategoryPanelSongIdCallback Callback)
        {
            CallbackSongId = Callback;
        }
        public void InitializeCallback(CategoryPanelUniqueIdCallback Callback)
        {
            CallbackUniqueId = Callback;
        }

        public void AddToManager()
        {
            CategoryPanelManager.AddCategoryPanel(this);
        }

        internal CategoryPanelDataInterface.CategoryPanelInfoAccessor CreateCategoryPanel()
        {
            var result = new CategoryPanelDataInterface.CategoryPanelInfoAccessor(GenreId,
                                                                                  (int)ThemeId,
                                                                                  Order,
                                                                                  "custom_category_empty_string",
                                                                                  BgType,
                                                                                  "#" + ColorUtility.ToHtmlStringRGB(BgColor),
                                                                                  (int)FrameType,
                                                                                  "#" + ColorUtility.ToHtmlStringRGB(FrameColor),
                                                                                  "#" + ColorUtility.ToHtmlStringRGB(TextFaceColor),
                                                                                  "#" + ColorUtility.ToHtmlStringRGB(TextUnderlayColor));

            return result;
        }

        internal List<MusicDataInterface.MusicInfoAccesser> GetMusicInfos()
        {
            if (CallbackMusicInfo != null)
            {
                return CallbackMusicInfo.Invoke();
            }
            else
            {
                var musicData = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.MusicData;
                List<MusicDataInterface.MusicInfoAccesser> results = new List<MusicDataInterface.MusicInfoAccesser>();
                if (CallbackSongId != null)
                {
                    var songIds = CallbackSongId.Invoke();
                    for (int i = 0; i < songIds.Count; i++)
                    {
                        var musicInfo = musicData.GetInfoById(songIds[i]);
                        // If null is added to the list, the game will softlock
                        if (musicInfo != null)
                        {
                            results.Add(musicInfo);
                        }
                        else
                        {
                            Logger.Log("Error getting musicInfo for songId: " + songIds[i]);
                        }
                    }
                }
                else if (CallbackUniqueId != null)
                {
                    var uniqueIds = CallbackUniqueId.Invoke();
                    for (int i = 0; i < uniqueIds.Count; i++)
                    {
                        var musicInfo = musicData.GetInfoByUniqueId(uniqueIds[i]);
                        if (musicInfo != null)
                        {
                            results.Add(musicInfo);
                        }
                        else
                        {
                            Logger.Log("Error getting musicInfo for uniqueId: " + uniqueIds[i]);
                        }
                    }
                }
                return results;
            }
        }

        internal bool Validate()
        {
            if (CallbackMusicInfo == null &&
                CallbackSongId == null &&
                CallbackUniqueId == null)
            {
                Logger.Log("Category Panel must have assigned Callback");
                return false;
            }

            return true;
        }
    }
}
