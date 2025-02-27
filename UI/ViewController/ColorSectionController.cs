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
    [UIAction("on-scheme-selected")]
    private void OnSchemeSelected(TableView tb, object row)
    {
        var selected = row as ColorListItem;
        Manager.Instance.SetCurrentScheme(selected?.index);
        Plugin.Logger.Info($"Selected color scheme: {selected?.index}");
        tb.ReloadDataKeepingPosition();
        selected.schemeText.color = Color.green;
    }

    [UIValue("scheme-list")] private List<object> SchemeList => new List<object>();

    [UIValue("enabled")]
    private bool Enabled
    {
        get => PluginConfig.Instance.Enabled;
        set => PluginConfig.Instance.Enabled = value;
    }

    [UIComponent("list")] public CustomCellListTableData presetListDisplay;


    [UIAction("#post-parse")]
    public void UpdatePresetList()
    {
        SetColorList();
    }

    private void SetColorList()
    {
        List<ColorListItem> colorList = new List<ColorListItem>();
        colorList.AddRange(
            Manager.Instance.CustomColorSchemes
                .Select(x =>
                    new ColorListItem(x.Value.colorSchemeId, x.Value.colorSchemeName, x.Value.GetColors())));
        presetListDisplay.data = colorList.Cast<object>().ToList();
        presetListDisplay.tableView.ReloadData();
    }
}

internal class ColorListItem
{
    public string index;

    [UIValue("scheme-name")] public readonly string colorSchemeName;

    [UIComponent("saberAColor")] public ImageView saberAColor;
    [UIComponent("saberBColor")] public ImageView saberBColor;
    [UIComponent("obstacleColor")] public ImageView obstacleColor;
    [UIComponent("environment0Color")] public ImageView environment0Color;
    [UIComponent("environment1Color")] public ImageView environment1Color;

    [UIComponent("environment0ColorBoost")]
    public ImageView environment0ColorBoost;

    [UIComponent("environment1ColorBoost")]
    public ImageView environment1ColorBoost;

    [UIComponent("scheme-text")] public TextMeshProUGUI schemeText;

    private readonly IEnumerable<Color> colors;

    public ColorListItem(string index, string colorSchemeName, IEnumerable<Color> colors)
    {
        this.index = index;
        this.colorSchemeName = colorSchemeName;
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

        if (index == Manager.Instance.CurrentScheme.colorSchemeId)
        {
            schemeText.color = Color.green;
        }
    }

    private void DeSelect()
    {
        schemeText.color = Color.grey;
    }
}