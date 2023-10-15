using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNoiseFilter : INoiseFilter
{
    Noise noise = new Noise();
    NoiseSettings.SimpleNoiseSettings settings;

    public SimpleNoiseFilter(NoiseSettings.SimpleNoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point) {
        float sum = 0;
        // float maxPossibleValue = 0;

        for (int layer = 0; layer < settings.numLayers; layer++)
        {
            float strength = settings.strength * Mathf.Pow(settings.strengthPersistence,layer*.5f);
            float roughness = settings.baseRoughness * Mathf.Pow(settings.roughness,layer);
            float noiseValue = noise.Evaluate(point * roughness + settings.center) * 0.5f + 0.5f; // range is 0 to 1
            noiseValue = Mathf.Pow(noiseValue, settings.sharpness);
            sum += noiseValue * strength + (strength < 0 ? -strength : 0);
            // maxPossibleValue += strength + (strength < 0 ? -strength : 0);
        }
        // sum = Mathf.Pow(sum / maxPossibleValue, settings.sharpness) * maxPossibleValue;
        sum = Mathf.Max(sum + settings.elevation, settings.floor ? 0 : -1); // (this value represents an amount of deviation from the unit sphere's radius, so a value of -1 would be all the way to the center of the sphere)

        if (settings.ceiling)
        {
            sum = Mathf.Min(sum, settings.ceilingHeight * settings.strength * settings.strength);
        }

        return sum;
    }
}
