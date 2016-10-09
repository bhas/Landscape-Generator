using Generator.HeightMap;
using System.Collections.Generic;
using UnityEngine;

namespace Generation.Terrain
{
    public class Chunk : MonoBehaviour
    {
        [Tooltip("The size of each chunk in world space.")]
        public const float size = 16;
        // the number of vertices along each side of a chunk
        private const int vertices = 17;
        // the triangle information used by the mesh. 
        private static int[] triangles00;
        private static int[] triangles01;
        private static int[] triangles02;
        // level of detail (for each increase the number of triangles halfs)
        private int lod = -1;
        private Mesh mesh;
        private MeshCollider meshCollider;

        void Awake()
        {
            if (triangles01 == null)
            {
                // initialise triangle arrays
                triangles00 = GenerateTriangles(0);
                triangles01 = GenerateTriangles(1);
                triangles02 = GenerateTriangles(2);
            }

            // creates a new mesh
            mesh = new Mesh();
            mesh.name = "ChunkMesh";
            GetComponent<MeshFilter>().sharedMesh = mesh;
            meshCollider = GetComponent<MeshCollider>();
        }

        public void SetPosition(Vector3 pos, IHeightMap heightMap)
        {
            transform.position = pos;
            // regenerate all vertices to match new position
            GenerateVertices(heightMap);
            SetLevelOfDetail(0);
        }

        public void SetLevelOfDetail(int level)
        {
            // update the level of detail and the mesh
            if (lod != level)
            {
                switch(level)
                {
                    case 0:
                        mesh.triangles = triangles00;
                        break;
                    case 1:
                        mesh.triangles = triangles01;
                        break;
                    case 2:
                        mesh.triangles = triangles02;
                        break;
                    default:
                        throw new UnityException("Unknown detail level: " + level);
                }
                lod = level;
            }
        }

        private void GenerateVertices(IHeightMap heightMap)
        {
            // calculate distance between vertices
            float dist = size / (vertices - 1);

            // create vertices
            Vector3[] vertexArray = new Vector3[vertices * vertices];
            // the coordinate from which the chunk will begin
            Vector3 startCoord = new Vector3(-size / 2, 0, -size / 2);
            Vector3 vertex;
            for (int i = 0; i < vertices; i++)
            {
                vertex.z = startCoord.z + dist * i;
                for (int j = 0; j < vertices; j++)
                {
                    vertex.x = startCoord.x + dist * j;
                    vertex.y = 0;
                    vertex.y = heightMap.Sample(transform.position.x + vertex.x, transform.position.z + vertex.z);
                    vertexArray[i * vertices + j] = vertex;
                }
            }
            mesh.Clear();
            mesh.vertices = vertexArray;
            mesh.triangles = triangles00;

            // update mesh collider to fit new mesh
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
        }

        private int[] GenerateTriangles(int detailLevel)
        {
            // the number of vertices in each step (e.g. step=2 means it only uses every second vertex)
            int step = (int)Mathf.Pow(2, detailLevel);

            // create the triangles
            List<int> triangles = new List<int>();
            int index;
            for (int i = 0; i < vertices - 1; i += step)
            {
                for (int j = 0; j < vertices - 1; j += step)
                {
                    index = i * vertices + j;
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
    }
}
