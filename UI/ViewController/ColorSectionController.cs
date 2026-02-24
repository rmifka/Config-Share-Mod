using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using Config_Share;
using Config_Share.Configuration;
using HMUI;
using TMPro;
using UnityEngine;

[ViewDefinition("Config_Share.UI.Views.ColorSelection.bsml")]
[HotReload(RelativePathToLayout = @"..\Views\ColorSelection.bsml")]
internal class ColorSectionController : BSMLAutomaticViewController
{
    [UIComponent("list")] public CustomCellListTableData presetListDisplay;

    // ReSharper disable once UnusedMember.Local
    [UIValue("scheme-list")] private List<object> SchemeList => []; // Empty List

    [UIValue("enabled")]
    // ReSharper disable once UnusedMember.Local
    private bool Enabled
    {
        get => PluginConfig.Instance.Enabled;
        set => PluginConfig.Instance.Enabled = value;
    }

    [UIValue("search-query")]
    // ReSharper disable once UnusedMember.Local
    private string SearchQuery
    {
        get => PluginConfig.Instance.SearchQuery;
        set => PluginConfig.Instance.SearchQuery = value ?? string.Empty;
    }

    [UIValue("sort-options")]
    // ReSharper disable once UnusedMember.Local
    private List<object> SortOptions => ["CreatedAt", "Name", "Description"];

    [UIValue("sort-dirs")]
    // ReSharper disable once UnusedMember.Local
    private List<object> SortDirections => ["Desc", "Asc"];

    [UIValue("sort-by")]
    // ReSharper disable once UnusedMember.Local
    private string SortBy
    {
        get => PluginConfig.Instance.SortBy;
        set => PluginConfig.Instance.SortBy = NormalizeSortBy(value);
    }

    [UIValue("sort-dir")]
    // ReSharper disable once UnusedMember.Local
    private string SortDir
    {
        get => PluginConfig.Instance.SortDirection;
        set => PluginConfig.Instance.SortDirection = NormalizeSortDir(value);
    }

    [UIAction("on-scheme-selected")]
    // ReSharper disable once UnusedMember.Local
    private void OnSchemeSelected(TableView tb, object row)
    {
        if (row is not ColorListItem selected)
        {
            Plugin.Logger.Warn("Received invalid color scheme selection row.");
            return;
        }

        Manager.Instance.SetCurrentScheme(selected.index);
        Plugin.Logger.Info($"Selected color scheme: {selected.index}");

        tb.ReloadDataKeepingPosition();
        if (Manager.Instance.CurrentScheme?.colorSchemeId == selected.index)
        {
            selected.schemeText.color = Color.green;
        }
        else
        {
            selected.schemeText.color = Color.grey;
        }
    }


    [UIAction("on-search-click")]
    // ReSharper disable once UnusedMember.Local
    private void OnSearchClick()
    {
        TriggerSearch();
    }

    [UIAction("#post-parse")]
    public void UpdatePresetList()
    {
        if (Manager.Instance.CustomColorSchemes.Count == 0)
        {
            TriggerSearch();
            return;
        }

        SetColorList();
    }

    private void TriggerSearch()
    {
        if (ColorFetcher.Instance == null)
        {
            Plugin.Logger.Warn("ColorFetcher.Instance is null; unable to fetch schemes.");
            return;
        }

        ColorFetcher.Instance.FetchColorSchemes(
            PluginConfig.Instance.SearchQuery,
            PluginConfig.Instance.SortBy,
            PluginConfig.Instance.SortDirection,
            1,
            PluginConfig.Instance.PageSize,
            success =>
            {
                if (success)
                {
                    SetColorList();
                }
            });
    }

    private static string NormalizeSortBy(string value)
    {
        return value switch
        {
            "Name" => "Name",
            "Description" => "Description",
            _ => "CreatedAt"
        };
    }

    private static string NormalizeSortDir(string value)
    {
        return value == "Asc" ? "Asc" : "Desc";
    }

    private void SetColorList()
    {
        var colorList = new List<ColorListItem>();
        colorList.AddRange(
            Manager.Instance.CustomColorSchemes
                .Select(x =>
                    new ColorListItem(
                        x.Value.colorSchemeId,
                        x.Value.colorSchemeName,
                        x.Value.colorSchemeDescription,
                        x.Value.GetColors())));
        presetListDisplay.Data = colorList.Cast<object>().ToList();
        presetListDisplay.TableView.ReloadData();
    }
}

internal class ColorListItem
{
    private readonly IEnumerable<Color> colors;
    private readonly string description;

    [UIValue("scheme-name")] public readonly string colorSchemeName;
    [UIComponent("obstacleColor")] private readonly ImageView obstacleColor = null;
    [UIComponent("environment0Color")] private readonly ImageView environment0Color = null;

    [UIComponent("environment0ColorBoost")]
    private readonly ImageView environment0ColorBoost = null;

    [UIComponent("environment1Color")] private readonly ImageView environment1Color = null;

    [UIComponent("environment1ColorBoost")]
    private readonly ImageView environment1ColorBoost = null;

    public string index;

    [UIComponent("saberAColor")] private readonly ImageView saberAColor = null;
    [UIComponent("saberBColor")] private readonly ImageView saberBColor = null;

    [UIComponent("scheme-text")] public readonly TextMeshProUGUI schemeText = null;

    [UIValue("scheme-description")]
    public string SchemeDescription => string.IsNullOrWhiteSpace(description)
        ? "No description available"
        : description;

    public ColorListItem(string index, string colorSchemeName, string colorSchemeDescription, IEnumerable<Color> colors)
    {
        this.index = index;
        this.colorSchemeName = colorSchemeName;
        description = colorSchemeDescription;
        this.colors = colors;
    }

    [UIAction("#post-parse")]
    public void Setup()
    {
        DeSelect();
        var colorList = colors.ToList();
        saberAColor.color = colorList[0];
        saberBColor.color = colorList[1];
        obstacleColor.color = colorList[2];
        environment0Color.color = colorList[3];
        environment1Color.color = colorList[4];
        environment0ColorBoost.color = colorList[5];
        environment1ColorBoost.color = colorList[6];

        var currentScheme = Manager.Instance.CurrentScheme;
        if (currentScheme != null && index == currentScheme.colorSchemeId) schemeText.color = Color.green;
    }

    private void DeSelect()
    {
        schemeText.color = Color.grey;
    }
}