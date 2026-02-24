using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config_Share;
using Config_Share.Configuration;
using UnityEngine.Networking;

public class Manager
{
    private static readonly string _baseUrl = "https://config-share-api.lambourne.at/api/";
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
        var selectedId = PluginConfig.Instance.SelectedColorSchemeId;
        if (!string.IsNullOrEmpty(selectedId) && CustomColorSchemes.TryGetValue(selectedId, out var scheme))
        {
            return scheme;
        }

        var fallback = CustomColorSchemes.Values.FirstOrDefault();
        if (fallback != null && fallback.colorSchemeId != selectedId)
        {
            Plugin.Logger.Warn($"Selected color scheme '{selectedId}' missing, falling back to '{fallback.colorSchemeId}'.");
            PluginConfig.Instance.SelectedColorSchemeId = fallback.colorSchemeId;
        }
        else if (fallback == null)
        {
            Plugin.Logger.Error("No custom color schemes available to select.");
        }

        return fallback;
    }

    public void SetCurrentScheme(string colorSchemeId)
    {
        if (string.IsNullOrEmpty(colorSchemeId))
        {
            Plugin.Logger.Warn("Attempted to set an empty color scheme id.");
            return;
        }

        if (!CustomColorSchemes.TryGetValue(colorSchemeId, out var colorScheme))
        {
            Plugin.Logger.Warn($"Color scheme '{colorSchemeId}' not found, keeping current selection.");
            return;
        }

        PluginConfig.Instance.SelectedColorSchemeId = colorScheme.colorSchemeId;
    }

    public void AddColorScheme(CustomColorScheme colorScheme)
    {
        CustomColorSchemes.Add(colorScheme.colorSchemeId, colorScheme);
    }

    public IEnumerator RequestAllColorSchemes(
        Action<string> callback,
        string searchQuery = "",
        string sortBy = "CreatedAt",
        string sortDirection = "Desc",
        int page = 1,
        int pageSize = 15)
    {
        var safePage = Math.Max(1, page);
        var safePageSize = Math.Max(1, pageSize);
        var queryParams = new List<string>
        {
            $"page={safePage}",
            $"pageSize={safePageSize}"
        };

        if (!string.IsNullOrWhiteSpace(sortBy))
            queryParams.Add($"sortBy={Uri.EscapeDataString(sortBy)}");

        if (!string.IsNullOrWhiteSpace(sortDirection))
            queryParams.Add($"sortDir={Uri.EscapeDataString(sortDirection)}");

        if (!string.IsNullOrWhiteSpace(searchQuery))
            queryParams.Add($"search={Uri.EscapeDataString(searchQuery)}");

        var url = _baseUrl + _colorSchemesEndpoint + "?" + string.Join("&", queryParams);

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