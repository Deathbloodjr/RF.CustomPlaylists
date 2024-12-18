﻿using Scripts.OutGame.SongSelect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomPlaylists.Plugins
{
    public static class CategoryPanelManager
    {
        internal static Dictionary<int, CategoryPanelData> CategoryPanels = new Dictionary<int, CategoryPanelData>();

        static List<CategoryPanelData> CategoriesToAdd = new List<CategoryPanelData>();
        static List<CategoryPanelData> DuplicateIdPanels = new List<CategoryPanelData>();

        public static void AddCategoryPanel(CategoryPanelData categoryPanelData)
        {
            CategoriesToAdd.Add(categoryPanelData);

            if (CategoryPanelPatch.CategoryPanelData != null)
            {
                AddCategoryPanels();
            }


            // Don't bother with error checking or anything right now

            //var existingGenreId = CategoriesToAdd.Find((x) => x.GenreId == categoryPanelData.GenreId);
            //if (existingGenreId != null)
            //{
            //    DuplicateIdPanels.Add(categoryPanelData);
            //    DuplicateIdPanels.Add(existingGenreId);
            //}
            //else
            //{
            //    CategoriesToAdd.Add(categoryPanelData);
            //}


            //if (!CategoryPanels.ContainsKey(categoryPanelData.GenreId))
            //{
            //    CategoryPanels.Add(categoryPanelData.GenreId, categoryPanelData);
            //}
            //else
            //{
            //    List<string> logOutput = new List<string>()
            //    {
            //        "Duplicate Category Panel Id found: " + categoryPanelData.GenreId,
            //        CategoryPanels[categoryPanelData.GenreId].Text,
            //        categoryPanelData.Text,
            //    };

            //    Logger.Log(logOutput, LogType.Warning);
            //    DuplicateIdPanels.Add(categoryPanelData);
            //}
        }

        internal static void AddCategoryPanels()
        {
            var panelData = CategoryPanelPatch.CategoryPanelData;
            for (int i = 0; i < CategoriesToAdd.Count; i++)
            {
                var category = CategoriesToAdd[i].CreateCategoryPanel();
                panelData.CategoryPanelInfoAccessers.Add(category);
                panelData.GenerFilterPairs.Add(category.Genre, (FilterTypes)category.Genre);

                CategoryPanels.Add(category.Genre, CategoriesToAdd[i]);
            }
        }
    }
}
