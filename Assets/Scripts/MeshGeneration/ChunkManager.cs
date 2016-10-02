using Generator;
using System.Collections.Generic;
using UnityEngine;

namespace Generation.Terrain
{
    [RequireComponent(typeof(TerrainGenerator))]
    class ChunkManager : MonoBehaviour
    {
        [Tooltip("The transform that will control which chunks to display.")]
        public Transform renderController;
        [Tooltip("Chunks within this distance will be loaded")]
        public float viewDistance = 30;
        [Tooltip("Chunks outside this distance will be destroyed")]
        public float bufferDistance = 10;

        private Vector2 chunkIndex = new Vector2(0.5f, 0.5f);
        private Vector2 currentChunkIndex;
        private Vector3 chunkCenter;
        private List<GameObject> chunks;
        private List<GameObject> unusedChunks;
        private TerrainGenerator generator;

        void Awake()
        {
            generator = GetComponent<TerrainGenerator>();
            chunks = new List<GameObject>();
            unusedChunks = new List<GameObject>();

            float halfSize = Chunk.size / 2;
            chunkCenter = new Vector3(halfSize, 0, halfSize);


        }

        void Update()
        {
            // check if the controller is in a new chunks
            currentChunkIndex = GetChunkIndex();
            if (currentChunkIndex != chunkIndex)
            {
                chunkIndex = currentChunkIndex;
                UpdateExistingChunks();
                AddNewChunks();
            }
        }

        private Vector2 GetChunkIndex()
        {
            int x = (int) (renderController.position.x / Chunk.size);
            int y = (int) (renderController.position.z / Chunk.size);
            return new Vector2(x, y);
        }

        private void AddNewChunks()
        {
            int count = (int)(viewDistance / Chunk.size) + 1;
            for (int i = -count; i <= count; i++)
            {
                for (int j = -count; j <= count; j++)
                {
                    var index = new Vector3(i + chunkIndex.x, 0, j + chunkIndex.y);
                    var position = index * Chunk.size;
                    if (DistanceToChunk(position) < viewDistance)
                    {
                        if (!HasChunk(position.x, position.z))
                        {
                            if (unusedChunks.Count == 0)
                            {
                                // Create new chunk
                                chunks.Add(generator.GenerateChunk(position));
                            }
                            else
                            {
                                // Reuse old chunk
                                chunks.Add(generator.GenerateChunk(position, unusedChunks[0]));
                                unusedChunks.RemoveAt(0);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateExistingChunks()
        {
            float distance;
            List<GameObject> chunkCopy = new List<GameObject>(chunks);
            foreach(GameObject chunk in chunkCopy)
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
            Vector3 diff = chunkPos + chunkCenter - renderController.position;
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
