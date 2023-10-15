using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    ShapeSettings settings;
    INoiseFilter[] noiseFilters;
    public MinMax elevationMinMax;

    public void UpdateSettings(ShapeSettings settings)
    {
        this.settings = settings;
        this.elevationMinMax = new MinMax();
        
        noiseFilters = new INoiseFilter[settings.noiseLayers.Length];

        for (int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings);
        }
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere) {
        float elevation = 0;
        for (int i = 0; i < noiseFilters.Length; i++)
        {
            if (settings.noiseLayers[i].enabled) {
                float current = noiseFilters[i].Evaluate(pointOnUnitSphere);
                if (settings.noiseLayers[i].usePreviousLayersAsMask) {
                    current *= elevation * 10; // the constant is arbitrary, just to keep the values reasonable
                }
                elevation += current;
            }
        }
        elevationMinMax.AddValue(elevation); // feed point into the MinMax calculator to store the lowest and highest values planet-wide

        return pointOnUnitSphere * settings.planetRadius * (1 + elevation);
    }
}
