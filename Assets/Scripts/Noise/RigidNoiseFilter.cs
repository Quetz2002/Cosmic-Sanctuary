using UnityEngine;

public class RigidNoiseFilter : INoiseFilter
{
    NoiseSettings.RigidNoiseSettings settings;
    Noise noise = new Noise();

    public RigidNoiseFilter(NoiseSettings.RigidNoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float amplitude = 1;
        float frequency = settings.baseRoughness;
        float noiseValue = 0;
        float weight = 1;

        for (int i = 0; i < settings.numLayers; i++)
        {
            float perlinValue = 1-Mathf.Abs(noise.Evaluate(point * frequency + settings.centre));
            perlinValue *= perlinValue;
            perlinValue *= weight;
            weight = Mathf.Clamp01(perlinValue * settings.weightMultiplier);
            noiseValue += perlinValue * amplitude;
            amplitude *= settings.persistence;
            frequency *= settings.roughness;
        }

        noiseValue = Mathf.Max(noiseValue - settings.minValue, 0);
        return noiseValue * settings.strength;
    }
}
