
using System;
using System.Runtime.InteropServices;
using Generation.Terrain;
using UnityEditor;
using System.Linq;
using UnityEngine;

class DiamondSquareHeightMap : IHeightMap
{
    private const int DetailLevel = 10;
    private const float randomLimit = 0.4f;
    private const float randomLimitDecay = 0.85f;

    private float[,] map;
    private int samples;
    private Vector2 offset;
    private float min = float.MaxValue;
    private float max = float.MinValue;

    public DiamondSquareHeightMap(int seed)
    {
        UnityEngine.Random.InitState(seed);
        offset = UnityEngine.Random.insideUnitCircle * 10000f;
        samples = (int)Mathf.Pow(2, DetailLevel) + 1;
        GenerateHeightMap();
    }

    private void GenerateHeightMap()
    {
        var tmpRandomLimit = randomLimit;

        // init height map
        map = new float[samples, samples];
        map[0, 0] = 0.5f; // top left
        map[0, samples - 1] = 0.5f; // top right
        map[samples - 1, 0] = 0.5f; // bottom left
        map[samples - 1, samples - 1] = 0.5f; // bottom right

        // will perform steps (backwards since it does it rough first then more and more detailed)
        for (var step = DetailLevel; step > 0; step--)
        {
            PerformDiamondStep(step, tmpRandomLimit);
            tmpRandomLimit *= randomLimitDecay;
            PerformSquareStep(step, tmpRandomLimit);
            tmpRandomLimit *= randomLimitDecay;
        }
        
        NormalizeMap();
    }

    /// <summary>
    /// Normalize all values in the map
    /// </summary>
    private void NormalizeMap()
    {
        var diff = max - min;
        for (var i = 0; i < samples; i++)
        {
            for (var j = 0; j < samples; j++)
            {
                map[i, j] = (map[i, j] - min) / diff;
            }
        }
    }

    private void PerformDiamondStep(int step, float randomLimit)
    {
        var squareSize = (int)Mathf.Pow(2, step);

        // iterate through all squares in current step
        for (var x = 0; x < samples - 1; x += squareSize)
        {
            for (var z = 0; z < samples - 1; z += squareSize)
            {
                // get corner values
                var topLeft = map[x, z];
                var topRight = map[x + squareSize, z];
                var bottomLeft = map[x, z + squareSize];
                var bottomRight = map[x + squareSize, z + squareSize];

                // take average of corners
                var value = (topLeft + topRight + bottomLeft + bottomRight) / 4f;
                // add random value in range [-randomLimit, randomLimit]
                value += UnityEngine.Random.Range(-1f, 1f) * randomLimit;
                min = Mathf.Min(value, min);
                max = Mathf.Max(value, max);
                // set center value
                map[x + squareSize / 2, z + squareSize / 2] = value;
            }
        }
    }

    private void PerformSquareStep(int step, float randomLimit)
    {
        var diamondSize = (int)Mathf.Pow(2, step - 1);

        // iterate through all squares in current step
        for (var x = 0; x < samples; x += diamondSize)
        {
            for (var z = 0; z < samples; z += diamondSize)
            {
                // skip irrelevant points
                if ((x + z) % (diamondSize * 2) == 0)
                    continue;

                // get the corners of the diamond (if possible)
                var corners = 0;
                var value = 0f;
                if (z > 0)
                {
                    // get top corner
                    corners++;
                    value += map[x, z - diamondSize];
                }
                if (z < samples - 1)
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
                if (x < samples - 1)
                {
                    // get right corner
                    corners++;
                    value += map[x + diamondSize, z];
                }
                // take average of corners
                value /= corners;
                // add random value in range [-randomLimit, randomLimit]
                value += UnityEngine.Random.Range(-1f, 1f) * randomLimit;
                min = Mathf.Min(value, min);
                max = Mathf.Max(value, max);
                map[x, z] = value;
            }
        }
    }

    private float RawSample(Vector2 point, float frequency)
    {
        // repeat coordinates to ensure that they are within the grid
        var x = Mathf.Repeat(offset.x + frequency * point.x, samples - 1.0001f);
        var z = Mathf.Repeat(offset.y + frequency * point.y, samples - 1.0001f);
        
        // get nearby samples
        var ix = (int)x;
        var iz = (int)z;
        var topLeft = map[ix, iz];
        var topRight = map[ix + 1, iz];
        var bottomLeft = map[ix, iz + 1];
        var bottomRight = map[ix + 1, iz + 1];

        // bilinear interpolation to get value
        var v1 = Mathf.Lerp(topLeft, topRight, x%1f);
        var v2 = Mathf.Lerp(bottomLeft, bottomRight, x%1f);
        return Mathf.Lerp(v1, v2, z%1f);
    }

    public float Sample(Vector2 point, float amplitude, float frequency)
    {
        return amplitude * RawSample(point, frequency);
    }

    public float Sample(Vector2 point, float amplitude, float frequency, AnimationCurve curve)
    {
        return amplitude * curve.Evaluate(RawSample(point, frequency));
    }
}

