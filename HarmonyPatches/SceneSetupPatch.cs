using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Config_Share.Configuration;
using HarmonyLib;
using UnityEngine;

/// <summary>
/// See https://github.com/pardeike/Harmony/wiki for a full reference on Harmony.
/// </summary>
namespace Config_Share.HarmonyPatches
{
    [HarmonyPatch]
    public class SceneSetupPatch
    {
        // ReSharper disable once UnusedMember.Local
        private static IEnumerable<MethodBase> TargetMethods()
        {
            Type targetType = typeof(StandardLevelScenesTransitionSetupDataSO);
            return targetType.GetMethods()
                .Where(m => m.Name == "Init");
        }

        // ReSharper disable once UnusedMember.Local
        [HarmonyPrefix]
        private static void Prefix(ref ColorScheme overrideColorScheme)
        {
            if (PluginConfig.Instance.Enabled)
            {
                var scheme = Manager.Instance.GetCurrentScheme();
                overrideColorScheme  = scheme.ToColorScheme();
            }
        }
    }
}