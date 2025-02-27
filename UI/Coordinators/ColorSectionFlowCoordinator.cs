using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using HMUI;
using Plugin = Config_Share.Plugin;

public class ColorSectionFlowCoordinator : FlowCoordinator
{
    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        if (firstActivation)
        {
            base.SetTitle("Config Share");
            showBackButton = true;
            if (view == null)
            {
                this.view = BeatSaberUI.CreateViewController<ColorSectionController>();
            }

            ProvideInitialViewControllers(this.view, null, null, null, null);
            Plugin.Logger.Info("Succesfully instantiated Flow Coordinator");
        }
    }

    protected override void BackButtonWasPressed(ViewController topViewController)
    {
        BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
    }

    public void ShowFlow()
    {
        FlowCoordinator _parentFlow = BeatSaberUI.MainFlowCoordinator.YoungestChildFlowCoordinatorOrSelf();
        _parentFlow.PresentFlowCoordinator(this);
    }

    public static void Initialize()
    {
        if (button == null)
        {
            button = new MenuButton("Config Share",
                "Config Share", delegate
                {
                    if (flow == null)
                    {
                        flow =
                            BeatSaberUI.CreateFlowCoordinator<ColorSectionFlowCoordinator>();
                    }

                    flow.ShowFlow();
                });
        }

        PersistentSingleton<MenuButtons>.instance.RegisterButton(button);
        Plugin.Logger.Info("Initialized Flow Coordinator");
    }

    public static void Deinit()
    {
        if (button != null)
        {
            PersistentSingleton<MenuButtons>.instance.UnregisterButton(button);
        }
    }

    // Token: 0x0400000B RID: 11
    private ColorSectionController view;

    // Token: 0x0400000C RID: 12
    private static ColorSectionFlowCoordinator flow;

    // Token: 0x0400000D RID: 13
    private static MenuButton button;
}