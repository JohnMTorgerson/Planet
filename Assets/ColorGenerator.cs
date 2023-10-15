using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator
{
    ColorSettings settings;
    Texture2D texturePosElv; // texture for above sea level
    Texture2D textureNegElv; // texture for below sea level
    const int textureResolution = 50;

    public void UpdateSettings(ColorSettings settings)
    {
        this.settings = settings;
        if (texturePosElv == null) texturePosElv = new Texture2D(textureResolution, 1);
        if (textureNegElv == null) textureNegElv = new Texture2D(textureResolution, 1);

    }

    public void UpdateColors() {
        Color[] colorsPosElv = new Color[textureResolution];
        Color[] colorsNegElv = new Color[textureResolution];
        for (int i = 0; i < textureResolution; i++)
        {
            colorsPosElv[i] = settings.gradientPosElv.Evaluate(i / (textureResolution - 1f));
            colorsNegElv[i] = settings.gradientNegElv.Evaluate(i / (textureResolution - 1f));
        }
        texturePosElv.SetPixels(colorsPosElv);
        textureNegElv.SetPixels(colorsNegElv);
        texturePosElv.Apply();
        textureNegElv.Apply();
        settings.planetMaterial.SetTexture("_texture_pos", texturePosElv);
        settings.planetMaterial.SetTexture("_texture_neg", textureNegElv);
    }

    public void UpdateElevationMinMax(MinMax elevationMinMax) {
        Debug.Log("Min: " + elevationMinMax.Min);
        Debug.Log("Max: " + elevationMinMax.Max);
        settings.planetMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min,elevationMinMax.Max));
    }

    public void UpdatePlanetRadius(float radius) {
        settings.planetMaterial.SetFloat("_radius", radius);
    }
}
