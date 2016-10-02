using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Generator.HeightMap;

namespace Generation.Terrain
{
    [RequireComponent(typeof(CompositeHeightMap))]
    public class TerrainGenerator : MonoBehaviour
    {
        public GameObject chunkPrefab;
        public int seed = 0;
        private CompositeHeightMap hm;

        void Awake()
        {
            Random.InitState(seed);
        }

        void Start()
        {
            hm = GetComponent<CompositeHeightMap>();
        }

        public GameObject GenerateChunk(Vector3 position, GameObject chunk = null)
        {
            if (chunk == null)
            {
                chunk = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }

            chunk.GetComponent<Chunk>().SetPosition(position, hm);
            return chunk.gameObject;
        }
    }
}

