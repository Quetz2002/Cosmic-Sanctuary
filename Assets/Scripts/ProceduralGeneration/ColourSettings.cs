using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu()]
public class ColourSettings : ScriptableObject
{
    //public Gradient gradient;
    public Material planetMaterial;
    public BiomeColourSettings biomeColourSettings;

    [System.Serializable]
    public class BiomeColourSettings
    {
        public Biome[] biomes;
        public NoiseSettings noise;
        public float noiseOffset;
        public float noiseStrength;
        [Range(0.0f, 1.0f)]
        public float blendAmount;
        [System.Serializable]
        public class Biome
        {
            public Gradient gradient;
            public Color tint;
            [Range(0f, 1f)]
            public float startHeight;
            [Range (0f, 1f)]
            public float tintPercent;
        }
    }
}
