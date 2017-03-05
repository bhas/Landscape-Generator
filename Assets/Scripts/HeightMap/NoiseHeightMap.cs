using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NoiseHeightMap : IHeightMap
{
    private Vector2 offset;

    public NoiseHeightMap(int seed)
    {
        Random.InitState(seed);
        offset = Random.insideUnitCircle * 10000f;
    }

    public float Sample(Vector2 point, float amplitude, float frequency)
    {
        return amplitude * Mathf.PerlinNoise(frequency * point.x, frequency * point.y);
    }

    public float Sample(Vector2 point, float amplitude, float frequency, AnimationCurve curve)
    {
        return amplitude * curve.Evaluate(Mathf.PerlinNoise(offset.x + frequency * point.x, offset.y + frequency * point.y));
    }
}
