using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using Config_Share.MonoBehaviours;
using HMUI;
using Plugin = Config_Share.Plugin;

#if !V_1_29_1
using BeatSaberMarkupLanguage.Util;
#endif

public class ColorSectionFlowCoordinator : 
#if !V_1_29_1
    FlowCoordinator
    #else
    FlowCoordinator

#endif
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

    protected override void BackButtonWasPressed(
#if !V_1_29_1
        ViewController
#else
        ViewController
#endif
        _
        )
    {
        BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
        ColorPreviewCubes.Instance.SetActive(false);
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
                    ColorPreviewCubes.Instance.SetActive(true);
                });

#if V_1_29_1
        MenuButtons.instance.RegisterButton(button);
        #else
        MenuButtons.Instance.RegisterButton(button);
        
#endif
        Plugin.Logger.Info("Initialized Flow Coordinator");
    }

    public static void Deinit()
    {
        if (button != null)
        {
#if V_1_29_1
            MenuButtons.instance.UnregisterButton(button);
#else
        MenuButtons.Instance.UnregisterButton(button);
        
#endif
        }
    }
}