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

        // This places all the custom categories in the mod's CategoryPanelManager Lists/Dictionaries
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

        // This sets the songs to be placed in the filter
        [HarmonyPatch(typeof(SongScroller))]
        [HarmonyPatch(nameof(SongScroller.CreateItemList))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static void SongScroller_CreateItemList_Prefix(SongScroller __instance, Il2CppSystem.Collections.Generic.List<MusicDataInterface.MusicInfoAccesser> list)
        {
            //Logger.Log("SongScroller_CreateItemList_Prefix");
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

        // This renames the filters
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

        // This lets the game properly remember where the filter is
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

        // This lets the random button appear
        [HarmonyPatch(typeof(SongSelectUtility))]
        [HarmonyPatch(nameof(SongSelectUtility.IsLibaray))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void SongSelectUtility_IsLibaray_Postfix(FilterTypes filter, ref bool __result)
        {
            //Logger.Log("SongSelectUtility_IsLibaray_Postfix");
            //Logger.Log("__result: " + __result);

            if (CategoryPanelManager.CategoryPanels.ContainsKey((int)filter))
            {
                __result = CategoryPanelManager.CategoryPanels[(int)filter].ThemeId == ThemeTypes.Library;
                //Logger.Log("Updated __result: " + __result);
            }
        }

        // This displays the sorting panel at the top (including song count)
        [HarmonyPatch(typeof(UiSongScroller))]
        [HarmonyPatch(nameof(UiSongScroller.IsViewSortingPanel))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void UiSongScroller_IsViewSortingPanel_Postfix(UiSongScroller __instance, ref bool __result)
        {
            //Logger.Log("UiSongScroller_IsViewSortingPanel_Postfix");
            // This is a little buggy, but it's mostly good
            // Definitely better than not having it
            __result = true;
        }
    }
}
