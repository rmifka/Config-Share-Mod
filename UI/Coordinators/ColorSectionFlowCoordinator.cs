using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using BeatSaberMarkupLanguage.Util;
using HMUI;
using Plugin = Config_Share.Plugin;

public class ColorSectionFlowCoordinator : FlowCoordinator
{
    private static ColorSectionFlowCoordinator flow;

    private static MenuButton button;

    private ColorSectionController view;

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        if (firstActivation)
        {
            SetTitle("Config Share");
            showBackButton = true;
            if (view == null) view = BeatSaberUI.CreateViewController<ColorSectionController>();

            ProvideInitialViewControllers(view);
            Plugin.Logger.Info("Successfully instantiated Flow Coordinator");
        }
    }

    protected override void BackButtonWasPressed(ViewController _)
    {
        BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
    }

    public void ShowFlow()
    {
        var _parentFlow = BeatSaberUI.MainFlowCoordinator.YoungestChildFlowCoordinatorOrSelf();
        _parentFlow.PresentFlowCoordinator(this);
    }

    public static void Initialize()
    {
        if (button == null)
            button = new MenuButton("Config Share",
                "Config Share", delegate
                {
                    if (flow == null)
                        flow =
                            BeatSaberUI.CreateFlowCoordinator<ColorSectionFlowCoordinator>();

                    flow.ShowFlow();
                });

        MenuButtons.Instance.RegisterButton(button);
        Plugin.Logger.Info("Initialized Flow Coordinator");
    }

    public static void Deinit()
    {
        if (button != null) MenuButtons.Instance.UnregisterButton(button);
        ;
    }
}