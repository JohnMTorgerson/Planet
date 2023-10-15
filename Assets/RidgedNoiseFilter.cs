using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidgedNoiseFilter : INoiseFilter
{
    Noise noise = new Noise();
    NoiseSettings.RidgedNoiseSettings settings;

    public RidgedNoiseFilter(NoiseSettings.RidgedNoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float sum = 0;
        float elevationWeight = 1; // makes layers more prominent that start at higher elevations

        for (int layer = 0; layer < settings.numLayers; layer++)
        {
            float strength = settings.strength * Mathf.Pow(settings.strengthPersistence, layer * .5f);
            float roughness = settings.baseRoughness * Mathf.Pow(settings.roughness, layer);
            float noiseValue = 1 - Mathf.Abs(noise.Evaluate(point * roughness + settings.center)); // range is 0 to 1
            noiseValue = Mathf.Pow(noiseValue,settings.sharpness);
            noiseValue *= elevationWeight;
            elevationWeight = Mathf.Clamp01(noiseValue * settings.elevationWeightMultiplier);

            sum += noiseValue * strength + (strength < 0 ? -strength : 0);
        }

        sum = Mathf.Max(sum + settings.elevation, settings.floor ? 0 : -1); // (this value represents an amount of deviation from the unit sphere's radius, so a value of -1 would be all the way to the center of the sphere)

        if (settings.ceiling) {
            sum = Mathf.Min(sum,settings.ceilingHeight * settings.strength * settings.strength);
        }

        return sum;
    }
}
