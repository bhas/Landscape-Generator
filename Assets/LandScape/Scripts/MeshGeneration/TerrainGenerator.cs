using UnityEngine;
using System.Collections.Generic;
using Generator.HeightMap;

namespace Generation.Terrain
{
    public class TerrainGenerator : MonoBehaviour
    {
        public int seed = 0;
        private CompositeHeightMap heightMap = new CompositeHeightMap();
        public GameObject chunkPrefab;
        private List<Chunk> chunks = new List<Chunk>();

        void OnValidate()
        {
            UpdateTerrainGeometry();
        }

        public void UpdateTerrainGeometry()
        {
            Random.InitState(seed);
            foreach (Chunk chunk in chunks)
            {
                chunk.SetPosition(chunk.transform.position, heightMap);
            }
        }

        public void AddHeightMap(IHeightMap map)
        {
            heightMap.AddHeightMap(map);
        }

        public GameObject GenerateChunk(Vector3 position, GameObject chunk = null)
        {
            if (chunk == null)
            {
                chunk = Instantiate(chunkPrefab);
                chunks.Add(chunk.GetComponent<Chunk>());
                chunk.transform.SetParent(transform);
            }

            chunk.GetComponent<Chunk>().SetPosition(position, heightMap);
            return chunk.gameObject;
        }
    }
}

