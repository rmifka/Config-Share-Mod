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
    [UIValue("scheme-list")] private List<object> SchemeList => new List<object>(); // Empty List

    [UIValue("enabled")]
    // ReSharper disable once UnusedMember.Local
    private bool Enabled
    {
        get => PluginConfig.Instance.Enabled;
        set => PluginConfig.Instance.Enabled = value;
    }

    [UIAction("on-scheme-selected")]
    // ReSharper disable once UnusedMember.Local
    private void OnSchemeSelected(TableView tb, object row)
    {
        var selected = row as ColorListItem;
        Manager.Instance.SetCurrentScheme(selected?.index);
        Plugin.Logger.Info($"Selected color scheme: {selected?.index}");
        tb.ReloadDataKeepingPosition();
        if (selected != null)
        {
            selected.schemeText.color = Color.green;
        }
    }


    [UIAction("#post-parse")]
    public void UpdatePresetList()
    {
        SetColorList();
    }

    private void SetColorList()
    {
        var colorList = new List<ColorListItem>();
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
    private readonly IEnumerable<Color> colors;

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

        if (index == Manager.Instance.CurrentScheme.colorSchemeId) schemeText.color = Color.green;
    }

    private void DeSelect()
    {
        schemeText.color = Color.grey;
    }
}