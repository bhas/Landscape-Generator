using UnityEngine;

namespace Generator.HeightMap
{
    [ExecuteInEditMode]
    public class NoiseTerrainGenerator : MonoBehaviour
    {
        public int seed;
        public NoiseFunction[] noiseFunctions;
        private Mesh mesh;
        private Vector3 vertex;
        private float y;

        void Awake()
        {
            mesh = GetComponent<MeshFilter>().sharedMesh;
        }

        void Update()
        {
            if (GUI.changed)
            {
                GenerateNoiseTerrain();
            }
        }

        private void GenerateNoiseTerrain()
        {
            Random.InitState(seed);
            // create the height map based on the noise functions
            foreach (var noiseFunction in noiseFunctions)
            {
                noiseFunction.Init();
            }
            var map = new CompositeHeightMap(noiseFunctions);

            // update the y position of each vertex
            Vector3[] vertices = new Vector3[mesh.vertices.Length];
            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                vertex = mesh.vertices[i];
                y = map.Sample(vertex.x, vertex.z);
                vertices[i] =  new Vector3(vertex.x, y, vertex.z);
            }
            mesh.vertices = vertices;
        }

    }

    [System.Serializable]
    public class NoiseFunction : IHeightMap
    {
        [Range(1, 1000)]
        public float height;
        [Range(0.001f, 1000)]
        public float smoothness;
        private float offsetX;
        private float offsetZ;
        private float scale;

        public void Init()
        {
            scale = 1 / smoothness;
            // set random offset
            offsetX = Random.value * 10000;
            offsetZ = Random.value * 10000;
        }

        public float Sample(float x, float z)
        {
            // find new height from perlin noise
            return height * Mathf.PerlinNoise(offsetX + x * scale, offsetZ + z * scale);
        }
    }
}
