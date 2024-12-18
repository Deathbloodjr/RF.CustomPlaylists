using HarmonyLib;
using Scripts.OutGame.Common;
using Scripts.OutGame.SongSelect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomPlaylists.Plugins
{
    internal class CategoryPanelPatch
    {
        internal static CategoryPanelDataInterface? CategoryPanelData = null;

        [HarmonyPatch(typeof(DataManager))]
        [HarmonyPatch(nameof(DataManager.Awake))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void DataManager_Awake_Postfix(DataManager __instance)
        {
            if (__instance.CategoryPanelData != null)
            {
                CategoryPanelData = __instance.CategoryPanelData;
                CategoryPanelManager.AddCategoryPanels();
            }
        }

        [HarmonyPatch(typeof(SongScroller))]
        [HarmonyPatch(nameof(SongScroller.CreateItemList))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static void SongScroller_CreateItemList_Prefix(SongScroller __instance, Il2CppSystem.Collections.Generic.List<MusicDataInterface.MusicInfoAccesser> list)
        {
            if (CategoryPanelManager.CategoryPanels.ContainsKey((int)__instance.filter))
            {
                var panel = CategoryPanelManager.CategoryPanels[(int)__instance.filter];
                list.Clear();
                var musicInfos = panel.GetMusicInfos();
                for (int i = 0; i < musicInfos.Count; i++)
                {
                    list.Add(musicInfos[i]);
                }
            }
        }


        [HarmonyPatch(typeof(UiFilterButton))]
        [HarmonyPatch(nameof(UiFilterButton.SetPanel))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void UiFilterButton_SetPanel_Postfix(UiFilterButton __instance)
        {
            if (CategoryPanelManager.CategoryPanels.ContainsKey((int)__instance.filter))
            {
                var panel = CategoryPanelManager.CategoryPanels[(int)__instance.filter];
                __instance.textOn.SetTextRawOnly(panel.Name);
                __instance.textOff.SetTextRawOnly(panel.Name);
            }
        }

        [HarmonyPatch(typeof(SongSelectUtility))]
        [HarmonyPatch(nameof(SongSelectUtility.FilterToTheme))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void SongSelectUtility_FilterToTheme_Postfix(ref ThemeTypes __result, FilterTypes filter)
        {
            if (CategoryPanelManager.CategoryPanels.ContainsKey((int)filter))
            {
                var panel = CategoryPanelManager.CategoryPanels[(int)filter];
                __result = panel.ThemeId;
            }
        }
    }
}
