using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using BepInEx.Configuration;
using CustomPlaylists.Plugins;
using UnityEngine;
using System.Collections;
using SaveProfileManager.Plugins;
using System.Reflection;

namespace CustomPlaylists
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, ModName, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public const string ModName = "CustomPlaylists";

        public static Plugin Instance;
        private Harmony _harmony = null;
        public new static ManualLogSource Log;


        public ConfigEntry<bool> ConfigEnabled;
        public ConfigEntry<string> ConfigCustomPlaylistDirectory;



        public override void Load()
        {
            Instance = this;

            Log = base.Log;

            SetupConfig(Config, Path.Combine("BepInEx", "data", ModName));
            SetupHarmony();

            var isSaveManagerLoaded = IsSaveManagerLoaded();
            if (isSaveManagerLoaded)
            {
                AddToSaveManager();
            }
        }

        // Any data that's likely to be shared between multiple profiles should use the dataFolder path
        // Any data that's likely to be specific per profile should use the saveFolder path
        private void SetupConfig(ConfigFile config, string saveFolder, bool isSaveManager = false)
        {
            var dataFolder = Path.Combine("BepInEx", "data", ModName);

            if (!isSaveManager)
            {
                ConfigEnabled = config.Bind("General",
                   "Enabled",
                   true,
                   "Enables the mod.");
            }

            ConfigCustomPlaylistDirectory = config.Bind("General",
                "CustomPlaylistDirectory",
                Path.Combine(saveFolder, "Playlists"),
                "The directory containing custom playlists to be loaded.");
        }

        private void SetupHarmony()
        {
            // Patch methods
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

            LoadPlugin(Instance.ConfigEnabled.Value);
        }

        public static void LoadPlugin(bool enabled)
        {
            bool apiResult = true;

            apiResult &= Instance.PatchFile(typeof(CategoryPanelPatch));
            apiResult &= Instance.PatchFile(typeof(XmlSavePatch));

            if (apiResult)
            {
                Logger.Log($"Plugin {MyPluginInfo.PLUGIN_NAME} is loaded!");
            }
            else
            {
                Logger.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} failed to load.", LogType.Error);

            }

            if (enabled)
            {
                try
                {
                    CustomPlaylistJsonLoading.LoadCustomPlaylists();
                }
                catch
                {

                }
            }
            else
            {
                Logger.Log($"Plugin {MyPluginInfo.PLUGIN_NAME} is disabled.");
            }
        }

        private bool PatchFile(Type type)
        {
            if (_harmony == null)
            {
                _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            }
            try
            {
                _harmony.PatchAll(type);
#if DEBUG
                Logger.Log("File patched: " + type.FullName);
#endif
                return true;
            }
            catch (Exception e)
            {
                Logger.Log("Failed to patch file: " + type.FullName);
                Logger.Log(e.Message);
                return false;
            }
        }

        public static void UnloadPlugin()
        {
            //Instance._harmony.UnpatchSelf();
            //Logger.Log($"Plugin {MyPluginInfo.PLUGIN_NAME} has been unpatched.");
            CustomPlaylistJsonLoading.UnloadCustomPlaylists();
        }

        public static void ReloadPlugin()
        {
            // Reloading will always be completely different per mod
            // You'll want to reload any config file or save data that may be specific per profile
            // If there's nothing to reload, don't put anything here, and keep it commented in AddToSaveManager
            //SwapSongLanguagesPatch.InitializeOverrideLanguages();
            //TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.MusicData.Reload();

            CustomPlaylistJsonLoading.ReloadCustomPlaylists();
        }

        public void AddToSaveManager()
        {
            // Add SaveDataManager dll path to your csproj.user file
            // https://github.com/Deathbloodjr/RF.SaveProfileManager
            var plugin = new PluginSaveDataInterface(MyPluginInfo.PLUGIN_GUID);
            plugin.AssignLoadFunction(LoadPlugin);
            plugin.AssignUnloadFunction(UnloadPlugin);
            plugin.AssignReloadSaveFunction(ReloadPlugin);
            plugin.AssignConfigSetupFunction(SetupConfig);
            plugin.AddToManager();
            //Logger.Log("Plugin added to SaveDataManager");
        }

        private bool IsSaveManagerLoaded()
        {
            try
            {
                Assembly loadedAssembly = Assembly.Load("com.DB.RF.SaveProfileManager");
                return loadedAssembly != null;
            }
            catch
            {
                return false;
            }
        }

        public static MonoBehaviour GetMonoBehaviour() => TaikoSingletonMonoBehaviour<CommonObjects>.Instance;
        public Coroutine StartCoroutine(IEnumerator enumerator)
        {
            return GetMonoBehaviour().StartCoroutine(enumerator);
        }
    }
}
