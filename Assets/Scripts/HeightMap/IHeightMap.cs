using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract super class for all heightmaps
/// Keep this class abstract rather than an interface to make it usable in the unity editor
/// </summary>
public interface IHeightMap
{
    float Sample(Vector2 point, float amplitude, float frequency);
    float Sample(Vector2 point, float amplitude, float frequency, AnimationCurve curve);
}
