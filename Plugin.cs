using System.Reflection;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Util;
using Config_Share.Configuration;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using UnityEngine;
using Config = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

namespace Config_Share
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        private readonly Harmony _harmony = new Harmony("com.renschi.configshare");
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Logger { get; private set; }

        [Init]
        // Resharper disable once UnusedMember.Global
        public void Init(IPALogger logger)
        {
            Instance = this;
            Logger = logger;
            Logger.Info("Config-Share initialized.");
        }

        #region BSIPA Config

        [Init]
        public void InitWithConfig(Config conf)
        {
            PluginConfig.Instance = conf.Generated<PluginConfig>();
            Logger.Debug("Config loaded");
        }

        #endregion

        [OnStart]
        // Resharper disable once UnusedMember.Global
        public void OnApplicationStart()
        {
            Logger.Info("OnApplicationStart - Loading Config Share first!");

            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            GameObject controllerObject = new GameObject("Config_ShareController");
            Object.DontDestroyOnLoad(controllerObject);
            controllerObject.AddComponent<Config_ShareController>();

            GameObject colorFetcherObject = new GameObject("ConfigShareMainBehaviour");
            Object.DontDestroyOnLoad(colorFetcherObject);
            colorFetcherObject.AddComponent<ColorFetcher>();

            MainMenuAwaiter.MainMenuInitializing += BSMLWrapper.EnableUI;
        }

        [OnExit]
        // Resharper disable once UnusedMember.Global
        public void OnApplicationQuit()
        {
            Logger.Debug("OnApplicationQuit");
            BSMLWrapper.DisableUI();
            _harmony.UnpatchSelf();
        }
    }
}