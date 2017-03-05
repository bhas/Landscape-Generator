using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshGenerator))]
public class TerrainGenerator : MonoBehaviour
{
    [SerializeField]
    private int seed;
    [SerializeField]
    private GameObject chunkPrefab;
    [SerializeField]
    private Transform parent;

    private MeshGenerator meshGen;

    private void Start()
    {
        GenerateChunks();
    }

    public void GenerateChunks()
    {
        meshGen = GetComponent<MeshGenerator>();
        foreach (Transform t in parent)
        {
            Destroy(t.gameObject);
        }

        for (var x = -5; x <= 5f; x++)
        {
            for (var z = -5; z <= 5; z++)
            {
                GenerateChunk(new Vector3(x, 0, z) * MeshGenerator.Size);
            }
        }
    }

    public GameObject GenerateChunk(Vector3 position)
    {
        var chunk = Instantiate(chunkPrefab, position, Quaternion.identity, parent);
        var mesh = new Mesh
        {
            name = "terrainMesh"
        };
        chunk.GetComponent<MeshFilter>().sharedMesh = mesh;
        meshGen.GenerateMesh(chunk);
        chunk.GetComponent<MeshCollider>().sharedMesh = mesh;
        return chunk;
    }
}

