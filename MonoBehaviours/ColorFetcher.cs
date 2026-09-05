using System;
using Config_Share;
using Config_Share.Configuration;
using Newtonsoft.Json;
using UnityEngine;

public class ColorFetcher : MonoBehaviour
{
    public static ColorFetcher Instance { get; private set; }

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

    public void FetchColorSchemes(
        string searchQuery = null,
        string sortBy = null,
        string sortDirection = null,
        int page = 1,
        int? pageSize = null,
        Action<bool> onCompleted = null)
    {
        var config = PluginConfig.Instance;
        var query = searchQuery ?? config.SearchQuery;
        var sortField = string.IsNullOrWhiteSpace(sortBy) ? config.SortBy : sortBy;
        var sortOrder = string.IsNullOrWhiteSpace(sortDirection) ? config.SortDirection : sortDirection;
        var size = pageSize ?? (config.PageSize <= 0 ? 15 : config.PageSize);

        StartCoroutine(Manager.Instance.RequestAllColorSchemes(json =>
        {
            var success = !string.IsNullOrEmpty(json);
            if (success)
            {
                Plugin.Logger.Info("Received color schemes.");
                var colorWebResponse = JsonConvert.DeserializeObject<ColorWebResponse>(json);

                Manager.Instance.CustomColorSchemes.Clear();
                Manager.Instance.LastTotalCount = colorWebResponse.total;

                foreach (var colorScheme in colorWebResponse.items)
                {
                    if (string.IsNullOrEmpty(config.SelectedColorSchemeId))
                    {
                        config.SelectedColorSchemeId = colorScheme.colorSchemeId;
                    }

                    Manager.Instance.AddColorScheme(colorScheme);
                }
            }
            else
            {
                Plugin.Logger.Debug("Failed to retrieve color schemes.");
            }

            onCompleted?.Invoke(success);
        },
            query,
            sortField,
            sortOrder,
            page,
            size));
    }
}