using UnityEngine;
using System.Collections.Generic;
using Generator.HeightMap;

namespace Generation.Terrain
{
    public class TerrainGenerator : MonoBehaviour
    {
        public int seed = 0;
        private CompositeHeightMap heightMap;
        public GameObject chunkPrefab;


        void Awake()
        {
            Random.InitState(seed);
            heightMap = new CompositeHeightMap();
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
                chunk.transform.SetParent(transform);
            }

            chunk.GetComponent<Chunk>().SetPosition(position, heightMap);
            return chunk.gameObject;
        }
    }

    
}

