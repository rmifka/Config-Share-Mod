using System.Reflection;
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
            Logger.Debug("OnApplicationStart");
            new GameObject("Config_ShareController").AddComponent<Config_ShareController>();
            new GameObject("ConfigShareMainBehaviour").AddComponent<ColorFetcher>();
            BSMLWrapper.EnableUI();
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
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