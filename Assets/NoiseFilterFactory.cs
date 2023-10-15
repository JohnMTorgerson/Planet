using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseFilterFactory
{
    public static INoiseFilter CreateNoiseFilter(NoiseSettings settings) {
        switch (settings.filterType) {
            case NoiseSettings.FilterType.Simple :
                return new SimpleNoiseFilter(settings.simpleNoiseSettings);
            case NoiseSettings.FilterType.Ridged:
                return new RidgedNoiseFilter(settings.ridgedNoiseSettings);
        }
        throw new NoiseFilterFactoryException("settings.filterType '" + settings.filterType + "' not recognized");
    }
}

[System.Serializable]
public class NoiseFilterFactoryException : System.Exception
{
    public NoiseFilterFactoryException() { }
    public NoiseFilterFactoryException(string message) : base(message) { }
    public NoiseFilterFactoryException(string message, System.Exception inner) : base(message, inner) { }
    protected NoiseFilterFactoryException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}