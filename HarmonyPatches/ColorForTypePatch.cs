using Config_Share.Configuration;
using HarmonyLib;

/// <summary>
/// See https://github.com/pardeike/Harmony/wiki for a full reference on Harmony.
/// </summary>
namespace Config_Share.HarmonyPatches
{
    [HarmonyPatch(typeof(StandardLevelScenesTransitionSetupDataSO), "Init")]
    public class ColorForTypePatch
    {
        // Resharper disable UnusedMember.Local
        private static void Prefix(ref ColorScheme overrideColorScheme)
        {
            if (PluginConfig.Instance.Enabled)
            {
                var scheme = Manager.Instance.GetCurrentScheme();
                overrideColorScheme = new ColorScheme("ConfigShare", "ConfigShare", true, "ConfigShare", false,
                    scheme.saberAColor, scheme.saberBColor, scheme.environmentColor0, scheme.environmentColor1, true,
                    scheme.environmentColor0Boost, scheme.environmentColor1Boost, scheme.obstaclesColor);
            }
        }
    }
}