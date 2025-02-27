using IPA;
using IPA.Config.Stores;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Config = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

namespace Config_Share
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Logger { get; private set; }

        [Init]
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
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Logger.Debug("Config loaded");
        }

        #endregion

        [OnStart]
        public void OnApplicationStart()
        {
            Logger.Debug("OnApplicationStart");
            new GameObject("Config_ShareController").AddComponent<Config_ShareController>();
            new GameObject("ConfigShareMainBehaviour").AddComponent<MainBehaviour>();
            BSMLWrapper.EnableUI();
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Logger.Debug("OnApplicationQuit");
            BSMLWrapper.DisableUI();
            _harmony.UnpatchSelf();
        }

        private readonly Harmony _harmony = new Harmony("com.renschi.configshare");
    }
}