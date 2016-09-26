
using UnityEngine;

namespace Generator.HeightMap
{
    class DiamondSquareHeightMap : IHeightMap
    {
        private float[,] map;
        private int size;
        private float height;
        private float scale;

        public DiamondSquareHeightMap(int level, float scale, float height)
        {
            this.height = height;
            this.scale = scale;
            GenerateHeightMap(level, 2f, 0.3f); 
        }


        private void GenerateHeightMap(int steps, float initialRandomLimit, float RandomLimitDecay)
        {
            float randomLimit = initialRandomLimit;
            size = (int) Mathf.Pow(2, steps) + 1;

            // init height map
            map = new float[size, size];
            map[0, 0] = 0.5f; // top left
            map[0, size-1] = 0.5f; // top right
            map[size-1, 0] = 0.5f; // bottom left
            map[size-1, size-1] = 0.5f; // bottom right

            // will perform steps (backwards since it does it rough first then more and more detailed)
            for (int step = steps; step > 0; step--)
            {
                PerformDiamondStep(step, randomLimit);
                randomLimit *= 1 - RandomLimitDecay;
                PerformSquareStep(step, randomLimit);
                randomLimit *= 1 - RandomLimitDecay;
            }
        }

        private void PerformDiamondStep(int step, float randomLimit)
        {
            int squareSize = (int)Mathf.Pow(2, step);

            // iterate through all squares in current step
            for(int x = 0; x < size - 1; x += squareSize)
            {
                for (int z = 0; z < size - 1; z += squareSize)
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
            for (int x = 0; x < size; x += diamondSize)
            {
                for (int z = 0; z < size; z += diamondSize)
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
                    if (z < size - 1)
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
                    if (x < size - 1)
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

        public float Sample(float x, float z)
        {
            // clamp values
            int x2 = Mathf.Clamp((int) (x * scale), 0, size - 2);
            int z2 = Mathf.Clamp((int) (z * scale), 0, size - 2);

            // get nearby grid values
            float topLeft = map[x2, z2];
            float topRight = map[x2 + 1, z2];
            float bottomLeft = map[x2, z2 + 1];
            float bottomRight = map[x2 + 1, z2 + 1];

            // bilinear interpolation to get value
            float v1 = Mathf.Lerp(topLeft, topRight, x % 1f);
            float v2 = Mathf.Lerp(bottomLeft, bottomRight, x % 1f);
            return height * Mathf.Lerp(v1, v2, z % 1f);
        }
    }
}
