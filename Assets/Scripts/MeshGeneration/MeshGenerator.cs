using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    // Size of each chunk in world space
    public const float Size = 20f;
    [Tooltip("Level of detail is used to control how detailed the terrain geometry will become.")]
    [Range(1, 7)]
    [SerializeField]
    private int lod;
    [SerializeField]
    private HeightMapArgs[] heightMaps;
    private NoiseHeightMap noiseMap;
    private DiamondSquareHeightMap diamondSquareMap;

    private void Reset()
    {
        lod = 4;
        heightMaps = new []
        {
            new HeightMapArgs(),
            new HeightMapArgs()
        };
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
            GetComponent<TerrainGenerator>().GenerateChunks();
    }

    public void GenerateMesh(GameObject chunk)
    {
        if (noiseMap == null)
        {
            noiseMap = new NoiseHeightMap(0);
            diamondSquareMap = new DiamondSquareHeightMap(0);
        }  

        var mesh = chunk.GetComponent<MeshFilter>().sharedMesh;
        mesh.Clear();
        mesh.vertices = GenerateVertices(chunk.transform.position);
        mesh.triangles = GenerateTriangles();
        mesh.RecalculateNormals();
    }

    private Vector3[] GenerateVertices(Vector3 chunkPos)
    {
        //calculate distance between vertices
        var vertices = (int)Mathf.Pow(2, lod) + 1;
        var dist = Size / (vertices - 1);

        // create vertices
        var vertexArray = new Vector3[vertices * vertices];
        // the coordinate from which the chunk will begin
        var startCoord = new Vector3(-Size / 2f, 0, -Size / 2f);

        for (var i = 0; i < vertices; i++)
        {
            var vertex = new Vector3
            {
                z = startCoord.z + dist * i
            };
            for (var j = 0; j < vertices; j++)
            {
                vertex.x = startCoord.x + dist * j;
                vertex.y = GetHeight(vertex + chunkPos);
                vertexArray[i * vertices + j] = vertex;
            }
        }

        return vertexArray;
    }

    private int[] GenerateTriangles()
    {
        var vertices = (int)Mathf.Pow(2, lod) + 1;

        // create the triangles
        var triangles = new List<int>();
        for (var i = 0; i < vertices - 1; i++)
        {
            for (var j = 0; j < vertices - 1; j++)
            {
                var index = i * vertices + j;
                // add first triangle
                triangles.Add(index);
                triangles.Add(index + vertices);
                triangles.Add(index + 1);
                // add second triangle
                triangles.Add(index + 1);
                triangles.Add(index + vertices);
                triangles.Add(index + vertices + 1);
            }
        }
        return triangles.ToArray();
    }

    private float GetHeight(Vector3 worldPos)
    {
        return heightMaps.Sum(hm =>
        {
            if (hm.type == HeightMapArgs.HeightMapType.Noise)
                return noiseMap.Sample(new Vector2(worldPos.x, worldPos.z), hm.amplitude, hm.frequence, hm.curve);
            else
                return diamondSquareMap.Sample(new Vector2(worldPos.x, worldPos.z), hm.amplitude, hm.frequence, hm.curve);
        });
    }

    [Serializable]
    private class HeightMapArgs
    {
        public enum HeightMapType
        {
            Noise, DiamondSquare
        }
        public HeightMapType type;
        public float amplitude;
        [Range(0.001f, 5f)]
        public float frequence;
        public AnimationCurve curve;

        public HeightMapArgs()
        {
            type = HeightMapType.Noise;
            amplitude = 1f;
            frequence = 1f;
            curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }
    }
}
