﻿using IPA.Loader;

public static class BSMLWrapper
{
    public static void EnableUI()
    {
        if (hasBsml)
        {
            ColorSectionFlowCoordinator.Initialize();
        }
    }

    public static void DisableUI()
    {
        if (hasBsml)
        {
            ColorSectionFlowCoordinator.Deinit();
        }
    }

    private static readonly bool hasBsml = PluginManager.GetPluginFromId("BeatSaberMarkupLanguage") != null;
}