using Generation.Terrain;
using UnityEngine;

namespace Generator.HeightMap
{
    [RequireComponent(typeof(TerrainGenerator))]
    public class NoiseHeightMap : MonoBehaviour, IHeightMap
    {
        [Range(1, 5)]
        public int octaves = 3;
        [Range(0, 1)]
        public float persistance = 0.5f;
        [Range(1, 40)]
        public float lacunarity = 2f;
        [Range(1f, 50f)]
        public float smoothness = 0.5f;
        public AnimationCurve heightCurve;

        private NoiseFunction[] noiseFunctions;

        void Start()
        {
            noiseFunctions = new NoiseFunction[octaves];
            // create the noise functions
            for(int i = 0; i < octaves; i++)
            {
                noiseFunctions[i] = new NoiseFunction()
                {
                    offsetX = Random.value * 10000,
                    offsetZ = Random.value * 10000,
                    frequency = 1 / (smoothness * Mathf.Pow(lacunarity, i)),
                    amplitude = Mathf.Pow(persistance, i)
                };
            }
            GetComponent<TerrainGenerator>().AddHeightMap(this);
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
