using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config_Share;
using Config_Share.Configuration;
using UnityEngine.Networking;

public class Manager
{
    private static readonly string _baseUrl = "https://config-share.lambourne.at/api/";
    private static readonly string _colorSchemesEndpoint = "colors";

    private Manager()
    {
        Instance = this;
    }

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

    public void AddColorScheme(CustomColorScheme colorScheme)
    {
        CustomColorSchemes.Add(colorScheme.colorSchemeId, colorScheme);
    }

    public IEnumerator RequestAllColorSchemes(Action<string> callback)
    {
        var url = _baseUrl + _colorSchemesEndpoint;

        using (var request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
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