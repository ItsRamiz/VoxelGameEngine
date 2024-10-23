using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int worldSize = 1; // Size of the world in number of chunks
    private int chunkSize = 16; // Assuming chunk size is 16x16x16
    public static World Instance { get; private set; }
    public Material VoxelMaterial;
    private Dictionary<Vector3, Chunk> chunks;

    void Start()
    {
        chunks = new Dictionary<Vector3, Chunk>();

        GenerateWorld();
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: if you want this to persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("Pressed K");
            AddBoxAtPosition( 5, 1, 2);
        }
    }
    public void AddBoxAtPosition(int x, int y, int z)
    {
        // Find the chunk that contains the voxel at the given global coordinates (x, y, z)
        Vector3 globalPosition = new Vector3(x, y, z);
        Chunk targetChunk = GetChunkAt(globalPosition);

        if (targetChunk != null)
        {
            Debug.Log("Add: " + globalPosition);

            // Calculate the local position of the voxel within the chunk
            int localX = Mathf.FloorToInt(globalPosition.x % chunkSize);
            int localY = Mathf.FloorToInt(globalPosition.y % chunkSize);
            int localZ = Mathf.FloorToInt(globalPosition.z % chunkSize);

            // Add the voxel at the local chunk coordinates and regenerate the chunk's mesh
            targetChunk.SetVoxel(localX, localY, localZ, true);
            targetChunk.GenerateMesh(); // Regenerate the mesh to reflect the changes
        }
        else
        {
            Debug.LogWarning("No chunk found for position: " + globalPosition);
        }
    }
    public void RemoveBoxAtPosition(int x, int y, int z)
    {
        // Find the chunk that contains the voxel at the given global coordinates (x, y, z)
        Vector3 globalPosition = new Vector3(x, y, z);
        Chunk targetChunk = GetChunkAt(globalPosition);

        if (targetChunk != null)
        {
            Debug.Log("Remove: " + globalPosition);

            // Calculate the local position of the voxel within the chunk
            int localX = Mathf.FloorToInt(globalPosition.x % chunkSize);
            int localY = Mathf.FloorToInt(globalPosition.y % chunkSize);
            int localZ = Mathf.FloorToInt(globalPosition.z % chunkSize);

            // Remove (deactivate) the voxel at the local chunk coordinates and regenerate the chunk's mesh
            targetChunk.SetVoxel(localX, localY, localZ, false); // Set the voxel to inactive
            targetChunk.GenerateMesh(); // Regenerate the mesh to reflect the removal
        }
        else
        {
            Debug.LogWarning("No chunk found for position: " + globalPosition);
        }
    }

    public Chunk GetChunkAt(Vector3 globalPosition)
    {
        // Calculate the chunk's starting position based on the global position
        Vector3Int chunkCoordinates = new Vector3Int(
            Mathf.FloorToInt(globalPosition.x / chunkSize) * chunkSize,
            Mathf.FloorToInt(globalPosition.y / chunkSize) * chunkSize,
            Mathf.FloorToInt(globalPosition.z / chunkSize) * chunkSize
        );

        // Retrieve and return the chunk at the calculated position
        if (chunks.TryGetValue(chunkCoordinates, out Chunk chunk))
        {
            return chunk;
        }

        // Return null if no chunk exists at the position
        return null;
    }

    private void GenerateWorld()
    {
        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                for (int z = 0; z < worldSize; z++)
                {
                    Vector3 chunkPosition = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);
                    GameObject newChunkObject = new GameObject($"Chunk_{x}_{y}_{z}");
                    newChunkObject.transform.position = chunkPosition;
                    newChunkObject.transform.parent = this.transform;

                    Chunk newChunk = newChunkObject.AddComponent<Chunk>();
                    newChunk.Initialize(chunkSize);
                    chunks.Add(chunkPosition, newChunk);
                }
            }
        }
    }

    // Additional methods for managing chunks, like loading and unloading, can be added here
}