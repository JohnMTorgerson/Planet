using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    public enum FilterType { Simple, Ridged };
    public FilterType filterType;

    [ConditionalHide("filterType", 0)]
    public SimpleNoiseSettings simpleNoiseSettings;
    [ConditionalHide("filterType", 1)]
    public RidgedNoiseSettings ridgedNoiseSettings;

    [System.Serializable]
    public class SimpleNoiseSettings {
        [Range(0, 10)]
        public int numLayers = 1;

        public bool floor = true;
        public bool ceiling;
        public float ceilingHeight = 1;

        [Range(-1,1)]
        public float strength = 1;
        [Range(-1, 5)]
        public float sharpness = 1; // a power to which the noise value will be raised (0-1 will be blunter, 1+ will be sharper)
        [Range(-1, 0)]
        public float elevation = 0;
        [Range(0, 10)]
        public float baseRoughness = 1;
        public Vector3 center;
        [Range(0, 1)]
        public float strengthPersistence = .5f; // how much the amplitude of each noise layer changes from the previous layer
        [Range(1, 5)]
        public float roughness = 2f; // how much the roughness of each noise layer changes from the previous layer
    }

    [System.Serializable]
    public class RidgedNoiseSettings : SimpleNoiseSettings {
        public float elevationWeightMultiplier = 1;
    }

}
