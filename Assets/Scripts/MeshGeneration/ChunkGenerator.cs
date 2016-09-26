using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace Generation.Terrain
{
    public class ChunkGenerator
    {
        // the maximum number of vertices per chunk
        private const byte maxVertices = 32;

        public static GameObject GenerateChunk(byte detailLevel, float size)
        {
            Mesh mesh = GenerateMesh(detailLevel, size);
            // creates the chunk
            GameObject chunk = new GameObject("Chunk");
            var renderer = chunk.AddComponent<MeshRenderer>();
            renderer.material = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
            chunk.AddComponent<MeshFilter>().mesh = mesh;
            chunk.AddComponent<MeshCollider>().sharedMesh = mesh;
            return chunk;
        }

        private static Mesh GenerateMesh(int detailLevel, float size)
        {
            // create mesh
            Mesh mesh = new Mesh();
            mesh.name = "ChunkMesh";

            int vertexCount = (int)Mathf.Pow(0.5f, detailLevel);
            // calculate distance between vertices
            float dist = size / (vertexCount - 1);

            // create vertices
            Vector3[] vertices = new Vector3[vertexCount * vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = 0; j < vertexCount; j++)
                {
                    vertices[i * vertexCount + j] = new Vector3(dist * j, 0, dist * i);
                }
            }
            mesh.vertices = vertices;

            // create triangles
            List<int> triangles = new List<int>();
            int index;
            for (int i = 0; i < vertexCount - 1; i++)
            {
                for (int j = 0; j < vertexCount - 1; j++)
                {
                    index = i * vertexCount + j;
                    triangles.AddRange(new int[] { index, index + vertexCount, index + 1 });
                    triangles.AddRange(new int[] { index + 1, index + vertexCount, index + vertexCount + 1 });
                }
            }
            mesh.triangles = triangles.ToArray();
            return mesh;
        }
    }
}

