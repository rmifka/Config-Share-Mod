using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class ColorWebResponse
{
    public List<CustomColorScheme> items;
    public int page;
    public int pageSize;
    public int total;
}

[Serializable]
public class CustomColorScheme
{
    [JsonProperty("id")]
    public string colorSchemeId;
    
    [JsonProperty("name")]
    public string colorSchemeName;
    
    [JsonProperty("description")]
    public string colorSchemeDescription;
    public DateTime createdAt;
    public Color saberAColor;
    public Color saberBColor;
    public Color environmentColor0;
    public Color environmentColor1;
    public Color obstaclesColor;
    public Color environmentColor0Boost;
    public Color environmentColor1Boost;

    internal IEnumerable<Color> GetColors()
    {
        return new[]
        {
            saberAColor,
            saberBColor,
            environmentColor0,
            environmentColor1,
            obstaclesColor,
            environmentColor0Boost,
            environmentColor1Boost
        };
    }

    public ColorScheme ToColorScheme()
    {
        return new ColorScheme("ConfigShare", "ConfigShare",
            true, "ConfigShare", false, true,
            saberAColor, saberBColor, true, environmentColor0, environmentColor1,
            Color.clear, true,
            environmentColor0Boost, environmentColor1Boost, Color.clear, obstaclesColor);
    }
}