
using Generation.Terrain;
using UnityEngine;

namespace Generator.HeightMap
{
    [RequireComponent(typeof(TerrainGenerator))]
    public class NoiseHeightMap : MonoBehaviour, IHeightMap
    {
        [Range(1, 5)]
        public int octaves = 2;
        [Range(1f, 50f)]
        public float smoothness = 20;
        public AnimationCurve heightCurve = AnimationCurve.Linear(0, 0, 2, 20);

        private NoiseFunction[] noiseFunctions;

        void Awake()
        {
            GenerateHeightMap();
            GetComponent<TerrainGenerator>().AddHeightMap(this);
        }

        void OnValidate()
        {
            GenerateHeightMap();
            GetComponent<TerrainGenerator>().UpdateTerrainGeometry();
        }

        public float Sample(float x, float z)
        {
            // find new height from perlin noise
            float acc = 0;
            foreach(NoiseFunction nf in noiseFunctions)
            {
                acc += nf.Sample(x, z);
            }
            return heightCurve.Evaluate(acc);
        }

        public void GenerateHeightMap()
        {
            noiseFunctions = new NoiseFunction[octaves];
            // create the noise functions
            float freq = 1 / smoothness;
            float amp = 1;
            for (int i = 0; i < octaves; i++)
            {
                noiseFunctions[i] = new NoiseFunction()
                {
                    offsetX = Random.value * 10000,
                    offsetZ = Random.value * 10000,
                    frequency = freq,
                    amplitude = amp
                };
                freq *= 2;
                amp *= 0.5f;
            }
        }

        private struct NoiseFunction
        {
            public float offsetX;
            public float offsetZ;
            public float frequency;
            public float amplitude;

            public float Sample(float x, float z)
            {
                return amplitude * Mathf.PerlinNoise(offsetX + x * frequency, offsetZ + z * frequency);
            }
        }
    }
}
