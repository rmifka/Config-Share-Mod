using Config_Share.Configuration;
using HarmonyLib;
using System;
using UnityEngine;

/// <summary>
/// See https://github.com/pardeike/Harmony/wiki for a full reference on Harmony.
/// </summary>
namespace Config_Share.HarmonyPatches
{
    [HarmonyPatch(typeof(StandardLevelScenesTransitionSetupDataSO), "Init")]
    public class ColorForTypePatch
    {
        private static void Prefix(ref ColorScheme overrideColorScheme)
        {
            Plugin.Logger.Info("ColorAffinityPatch");

            if (PluginConfig.Instance.Enabled)
            {
                Plugin.Logger.Info("ColorAffinityPatch true");

                var scheme = Manager.Instance.GetCurrentScheme();
                overrideColorScheme = new ColorScheme("ConfigShare", "ConfigShare", true, "ConfigShare", false,
                    scheme.saberAColor, scheme.saberBColor, scheme.environmentColor0, scheme.environmentColor1, true,
                    scheme.environmentColor0Boost, scheme.environmentColor1Boost, scheme.obstaclesColor);
            }
        }
    }
}