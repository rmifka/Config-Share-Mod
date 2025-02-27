using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ColorWebResponse
{
    public List<CustomColorScheme> items;
}

[Serializable]
public class CustomColorScheme
{
    public string colorSchemeId;
    public string colorSchemeName;
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
}