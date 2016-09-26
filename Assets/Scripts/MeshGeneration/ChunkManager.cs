using System.Collections.Generic;
using UnityEngine;

namespace Generation.Terrain
{
    [RequireComponent(typeof(TerrainGenerator))]
    class ChunkManager : MonoBehaviour
    {
        public Transform renderController;
        public float viewDistance;
        public float bufferDistance;

        private Vector2 chunkIndex;
        private List<GameObject> chunks;
        private List<GameObject> unusedChunks;
        private TerrainGenerator generator;
        
        void Awake()
        {
            generator = GetComponent<TerrainGenerator>();
        }

        void Update()
        {
            // check if the controller is in a new chunks
            var currentChunkIndex = GetChunkIndex();
            if (currentChunkIndex != chunkIndex)
            {
                chunkIndex = currentChunkIndex;
                UpdateExistingChunks();
                AddNewChunks();
            }
        }

        private Vector2 GetChunkIndex()
        {
            int x = (int) (renderController.position.x / generator.chunkSize);
            int y = (int) (renderController.position.z / generator.chunkSize);
            return new Vector2(x, y);
        }

        private void AddNewChunks()
        {
            int count = (int)(viewDistance / generator.chunkSize) + 1;
            for (int i = -count; i <= count; i++)
            {
                for (int j = -count; j <= count; j++)
                {
                    var index = new Vector3(i + chunkIndex.x, 0, j + chunkIndex.y);
                    var position = index * generator.chunkSize;
                    if (DistanceToChunk(position) < viewDistance)
                    {
                        if (!HasChunk(position.x, position.z))
                        {
                            // Create new chunk
                        }
                    }
                }
            }
        }

        private void UpdateExistingChunks()
        {
            float distance;
            foreach(GameObject chunk in chunks)
            {
                distance = DistanceToChunk(chunk.transform.position);

                if (distance < viewDistance)
                {
                    chunk.SetActive(true);
                }
                else
                {
                    chunk.SetActive(false);
                    if (distance > viewDistance + bufferDistance)
                    {
                        // psedo delete chunk if too far away (put it back in the spawning pool)
                        chunks.Remove(chunk);
                        unusedChunks.Add(chunk);
                    }
                }

            }
        }

        private float DistanceToChunk(Vector3 chunkPos)
        {
            Vector3 diff = chunkPos - renderController.position;
            diff.y = 0;
            return diff.magnitude;
        }

        private bool HasChunk(float x, float z)
        {
            Vector3 pos;
            foreach(GameObject chunk in chunks)
            {
                pos = chunk.transform.position;
                if (pos.x == x && pos.z == z)
                    return true;
            }
            return false;
        }
    }
}
