using UnityEngine;

namespace Generation.Terrain
{
    class TerrainGenerator : MonoBehaviour
    {

        public float chunkSize;

        public GameObject GenerateChunk(GameObject chunk = null)
        {
            if (chunk == null)
            {
                chunk = ChunkGenerator.GenerateChunk(0, chunkSize);
            }

            
        }
    }
}
