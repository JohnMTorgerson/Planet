using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator
{
    ColorSettings settings;
    Texture2D texturePosElv; // texture for above sea level
    Texture2D textureNegElv; // texture for below sea level
    const int textureResolution = 50;
    int numBiomes = 1;
    INoiseFilter biomeNoiseFilter;

    public void UpdateSettings(ColorSettings settings)
    {
        this.settings = settings;
        numBiomes = settings.biomeColorSettings.biomes.Length;
        if (texturePosElv == null || texturePosElv.height != numBiomes) texturePosElv = new Texture2D(textureResolution, numBiomes);
        if (textureNegElv == null || textureNegElv.height != numBiomes) textureNegElv = new Texture2D(textureResolution, numBiomes);
        biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(settings.biomeColorSettings.noise);
    }

    public void UpdateColors() {
        Color[] colorsPosElv = new Color[texturePosElv.width * texturePosElv.height];
        Color[] colorsNegElv = new Color[texturePosElv.width * texturePosElv.height];
        int colorIndex = 0;
        foreach (var biome in settings.biomeColorSettings.biomes) {
            for (int i = 0; i < textureResolution; i++) {
                Color colorPosElv = biome.gradientPosElv.Evaluate(i / (textureResolution - 1f));
                Color colorNegElv = biome.gradientNegElv.Evaluate(i / (textureResolution - 1f));
                Color tintColor = biome.tint;
                
                colorsPosElv[colorIndex] = colorPosElv * (1-biome.tintPercent) + tintColor * biome.tintPercent;
                colorsNegElv[colorIndex] = colorNegElv * (1-biome.tintPercent) + tintColor * biome.tintPercent;

                colorIndex++;
            }
        }
        texturePosElv.SetPixels(colorsPosElv);
        textureNegElv.SetPixels(colorsNegElv);
        texturePosElv.Apply();
        textureNegElv.Apply();
        settings.planetMaterial.SetTexture("_texture_pos", texturePosElv);
        settings.planetMaterial.SetTexture("_texture_neg", textureNegElv);
    }

    // return 0 if in first biome, 1 if in last biome, and somewhere in between for others
    public float BiomePercentFromPoint(Vector3 pointOnUnitSphere) {
        // float latitude = Mathf.Sin(pointOnUnitSphere.y * Mathf.PI / 2) * 0.5f + 0.5f; // Range(0,1)
        float latitude = pointOnUnitSphere.y * 0.5f + 0.5f; 
        latitude += (biomeNoiseFilter.Evaluate(pointOnUnitSphere) - settings.biomeColorSettings.noiseOffset) * settings.biomeColorSettings.noiseStrength;
        float blendRange = settings.biomeColorSettings.blendAmount / 2f + 0.0001f;
        float biomeIndex = 0;
        for (int i = 0; i < numBiomes; i++) {
            float dst = latitude - settings.biomeColorSettings.biomes[i].startLatitude;
            float weight = Mathf.InverseLerp(-blendRange,blendRange,dst);
            biomeIndex *= 1 - weight;
            biomeIndex += i * weight;
        }
        // for (int i = numBiomes - 1; i >= 0; i--) {
        //     if (settings.biomeColorSettings.biomes[i].startLatitude < latitude) {
        //         biomeIndex = i;
        //         break;
        //     }
        // }
        return biomeIndex / Mathf.Max(1,(float) numBiomes - 1); // Mathf.Max to avoid dividing by 0
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
