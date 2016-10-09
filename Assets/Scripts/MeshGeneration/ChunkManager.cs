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

        private Vector2 currentChunk; // the chunk the render controller is in
        private List<GameObject> chunks = new List<GameObject>();
        private List<GameObject> unusedChunks = new List<GameObject>();
        private TerrainGenerator generator;

        void Start()
        {
            generator = GetComponent<TerrainGenerator>();
            // find the current chunk
            currentChunk = new Vector2(SnapToChunk(renderController.position.x), SnapToChunk(renderController.position.z));
            AddNewChunks();
        }

        void Update()
        {
            // check if the controller is in a new chunk
            if (CurrentChunkChanged())
            {
                UpdateExistingChunks();
                AddNewChunks();
            }
        }

        /// <summary>
        /// Finds the current chunk (the chunk that is occupied by the render controller)
        /// </summary>
        /// <returns>true if the current chunk was changed since last frame</returns>
        private bool CurrentChunkChanged()
        {
            var old = currentChunk;
            currentChunk = new Vector2(SnapToChunk(renderController.position.x), SnapToChunk(renderController.position.z));
            return old != currentChunk;
        }

        // snaps a float to the nearest chunk position
        private float SnapToChunk(float f)
        {
            return Mathf.Round(f / Chunk.size) * Chunk.size;
        }

        private void AddNewChunks()
        {
            // then number of chunks to check
            int count = Mathf.CeilToInt(viewDistance / Chunk.size);
            Vector2 position;
            for (int i = -count; i <= count; i++)
            {
                for (int j = -count; j <= count; j++)
                {
                    // position of the chunk in world space
                    position = new Vector2(i, j) * Chunk.size + currentChunk;

                    // check if chunk is within view distance
                    if (DistanceToChunk(position) < viewDistance)
                    {
                        // Check if chunk exists already
                        if (!HasChunk(position.x, position.y))
                        {
                            if (unusedChunks.Count == 0)
                            {
                                // Create new chunk
                                chunks.Add(generator.GenerateChunk(new Vector3(position.x, 0, position.y)));
                            }
                            else
                            {
                                // Reuse old chunk
                                chunks.Add(generator.GenerateChunk(new Vector3(position.x, 0, position.y), unusedChunks[0]));
                                unusedChunks.RemoveAt(0);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateExistingChunks()
        {
            // copy all the chunks to a temporary list in order to edit the original while iterating
            List<GameObject> chunksCopy = new List<GameObject>(chunks);
            float distance;
            foreach (GameObject chunk in chunksCopy)
            {
                // find the distance to the chunk
                Vector3 pos = chunk.transform.position;
                distance = DistanceToChunk(new Vector2(pos.x, pos.z));

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

        private float DistanceToChunk(Vector2 chunkPos)
        {
            return (chunkPos - currentChunk).magnitude;
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
