
using Generation.Terrain;
using UnityEngine;

namespace Generator.HeightMap
{
    [RequireComponent(typeof(TerrainGenerator))]
    class DiamondSquareHeightMap : MonoBehaviour, IHeightMap
    {
        [Range(1, 20)]
        public int detailLevel = 8;
        [Range(10, 10000)]
        public float size = 1000;
        [Range(0, 1)]
        public float randomLimit = 0.3f;
        [Range(0, 0.5f)]
        public float randomLimitDecay = 0.1f;
        public AnimationCurve heightCurve;

        private float[,] map;
        private int vertices;

        void Start()
        {
            GenerateHeightMap();
            GetComponent<TerrainGenerator>().AddHeightMap(this);
        }

        private void GenerateHeightMap()
        {
            float tmpRandomLimit = randomLimit;
            vertices = (int) Mathf.Pow(2, detailLevel) + 1;

            // init height map
            map = new float[vertices, vertices];
            map[0, 0] = 0.5f; // top left
            map[0, vertices-1] = 0.5f; // top right
            map[vertices-1, 0] = 0.5f; // bottom left
            map[vertices-1, vertices-1] = 0.5f; // bottom right

            // will perform steps (backwards since it does it rough first then more and more detailed)
            for (int step = detailLevel; step > 0; step--)
            {
                PerformDiamondStep(step, tmpRandomLimit);
                tmpRandomLimit *= 1 - randomLimitDecay;
                PerformSquareStep(step, tmpRandomLimit);
                tmpRandomLimit *= 1 - randomLimitDecay;
            }
        }

        private void PerformDiamondStep(int step, float randomLimit)
        {
            int squareSize = (int)Mathf.Pow(2, step);

            // iterate through all squares in current step
            for(int x = 0; x < vertices - 1; x += squareSize)
            {
                for (int z = 0; z < vertices - 1; z += squareSize)
                {
                    // get corner values
                    float topLeft = map[x, z];
                    float topRight = map[x + squareSize, z];
                    float bottomLeft = map[x, z + squareSize];
                    float bottomRight = map[x + squareSize, z + squareSize];

                    // take average of corners
                    float value = (topLeft + topRight + bottomLeft + bottomRight) / 4f;
                    // add random value in range [-randomLimit, randomLimit]
                    value += (Random.value * 2 - 1) * randomLimit;
                    map[x + squareSize / 2, z + squareSize / 2] = value;
                }
            }
        }

        private void PerformSquareStep(int step, float randomLimit)
        {
            int diamondSize = (int)Mathf.Pow(2, step - 1);

            // iterate through all squares in current step
            for (int x = 0; x < vertices; x += diamondSize)
            {
                for (int z = 0; z < vertices; z += diamondSize)
                {
                    // skip irrelevant points
                    if ((x + z) % (diamondSize * 2) == 0)
                        continue;

                    // get the corners of the diamond (if possible)
                    int corners = 0;
                    float value = 0;
                    if (z > 0)
                    {
                        // get top corner
                        corners++;
                        value += map[x, z - diamondSize];
                    }
                    if (z < vertices - 1)
                    {
                        // get bottom corner
                        corners++;
                        value += map[x, z + diamondSize];
                    }
                    if (x > 0)
                    {
                        // get left corner
                        corners++;
                        value += map[x - diamondSize, z];
                    }
                    if (x < vertices - 1)
                    {
                        // get right corner
                        corners++;
                        value += map[x + diamondSize, z];
                    }
                    // take average of corners
                    value /= corners;
                    // add random value in range [-randomLimit, randomLimit]
                    value += (Random.value * 2 - 1) * randomLimit;

                    map[x, z] = value;
                }
            }
        }

        private float ClampCoordinate(float f)
        {
            float result = Mathf.Abs(f) % (size * 2);
            if (result >= size)
                result = size * 2 - result;
            return result;
        }

        public float Sample(float x, float z)
        {
            // clamp values to ensure that they are within the grid
            float x2 = ClampCoordinate(x);
            float z2 = ClampCoordinate(z);

            // get map grid indices
            float vPerSize = vertices / size;
            int ix = (int)(x2 * vPerSize);
            int iz = (int)(z2 * vPerSize);

            // get nearby grid values
            float topLeft = map[ix, iz];
            float topRight = map[ix + 1, iz];
            float bottomLeft = map[ix, iz + 1];
            float bottomRight = map[ix + 1, iz + 1];

            // bilinear interpolation to get value
            float v1 = Mathf.Lerp(topLeft, topRight, x % vPerSize);
            float v2 = Mathf.Lerp(bottomLeft, bottomRight, x % vPerSize);
            float v3 = Mathf.Lerp(v1, v2, z % vPerSize);
            return heightCurve.Evaluate(v3);
        }
    }
}
