using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config_Share;
using Config_Share.Configuration;
using UnityEngine;
using UnityEngine.Networking;

public class Manager
{
    private static string _baseUrl = "https://config-share.lambourne.at/api/";
    private static string _colorSchemesEndpoint = "colors";
    public static Manager Instance { get; private set; } = new Manager();

    public Dictionary<string, CustomColorScheme> CustomColorSchemes { get; } =
        new Dictionary<string, CustomColorScheme>();


    public CustomColorScheme CurrentScheme => GetCurrentScheme();

    public CustomColorScheme GetCurrentScheme()
    {
        return CustomColorSchemes[PluginConfig.Instance.SelectedColorSchemeId];
    }

    public void SetCurrentScheme(string colorSchemeId)
    {
        var colorScheme = CustomColorSchemes.FirstOrDefault(x => x.Key == colorSchemeId).Value;
        PluginConfig.Instance.SelectedColorSchemeId = colorScheme.colorSchemeId;
    }

    private Manager()
    {
        Instance = this;
    }

    public void AddColorScheme(CustomColorScheme colorScheme)
    {
        CustomColorSchemes.Add(colorScheme.colorSchemeId, colorScheme);
    }

    public IEnumerator RequestAllColorSchemes(System.Action<string> callback)
    {
        var url = _baseUrl + _colorSchemesEndpoint;

        using (var request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Plugin.Logger.Error(request.error);
                callback?.Invoke(null);
            }
            else
            {
                var json = request.downloadHandler.text;
                callback?.Invoke(json);
            }
        }
    }
}