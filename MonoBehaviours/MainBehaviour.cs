using Config_Share;
using Config_Share.Configuration;
using Newtonsoft.Json;
using UnityEngine;

public class MainBehaviour : MonoBehaviour
{
    public static MainBehaviour Instance { get; private set; }

    private void Start()
    {
        Instance = this;
        if (Manager.Instance == null)
        {
            Plugin.Logger.Debug("Manager.Instance is null.");
            return;
        }

        FetchColorSchemes();
    }

    public void FetchColorSchemes()
    {
        StartCoroutine(Manager.Instance.RequestAllColorSchemes(json =>
        {
            Plugin.Logger.Info("Received color schemes.");
            if (json != null)
            {
                var a = JsonConvert.DeserializeObject<ColorWebResponse>(json);

                Manager.Instance.CustomColorSchemes.Clear();

                foreach (var colorScheme in a.items)
                {
                    if (string.IsNullOrEmpty(PluginConfig.Instance.SelectedColorSchemeId))
                    {
                        PluginConfig.Instance.SelectedColorSchemeId = colorScheme.colorSchemeId;
                    }

                    Manager.Instance.AddColorScheme(colorScheme);
                }
            }
            else
            {
                Plugin.Logger.Debug("Failed to retrieve color schemes.");
            }
        }));
    }
}