using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int worldSize = 1; 
    private int chunkSize = 16; 
    public static World Instance { get; private set; }
    public Material VoxelMaterial;
    private Dictionary<Vector3, Chunk> chunks;

    public Dictionary<TriangleData, Vector3> triangleToBlockMap = new Dictionary<TriangleData, Vector3>();

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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {

    }
    public void AddTriangle(List<Vector3> corners, Vector3 blockPosition)
    {
        // Create a new TriangleData instance
        TriangleData triangleData = new TriangleData(corners);

        // Map the triangle to its block position
        triangleToBlockMap[triangleData] = blockPosition;
    }
    public void AddBoxAtPosition(int x, int y, int z)
    {
        Vector3 globalPosition = new Vector3(x, y, z);
        Chunk targetChunk = GetChunkAt(globalPosition);

        if (targetChunk != null)
        {
            Debug.Log("Add: " + globalPosition);

            int localX = Mathf.FloorToInt(globalPosition.x % chunkSize);
            int localY = Mathf.FloorToInt(globalPosition.y % chunkSize);
            int localZ = Mathf.FloorToInt(globalPosition.z % chunkSize);

            targetChunk.SetVoxel(localX, localY, localZ, true);
            targetChunk.GenerateMesh();
        }
        else
        {
            Debug.LogWarning("No chunk found for position: " + globalPosition);
        }
    }
    public void RemoveBoxAtPosition(int x, int y, int z)
    {
        Vector3 globalPosition = new Vector3(x, y, z);
        Chunk targetChunk = GetChunkAt(globalPosition);

        if (targetChunk != null)
        {
            Debug.Log("Remove: " + globalPosition);

            int localX = Mathf.FloorToInt(globalPosition.x % chunkSize);
            int localY = Mathf.FloorToInt(globalPosition.y % chunkSize);
            int localZ = Mathf.FloorToInt(globalPosition.z % chunkSize);

            targetChunk.SetVoxel(localX, localY, localZ, false); 
            targetChunk.GenerateMesh(); 
        }
        else
        {
            //Debug.LogWarning("No chunk found for position: " + globalPosition);
        }
    }

    public Chunk GetChunkAt(Vector3 globalPosition)
    {
        Vector3Int chunkCoordinates = new Vector3Int(
            Mathf.FloorToInt(globalPosition.x / chunkSize) * chunkSize,
            Mathf.FloorToInt(globalPosition.y / chunkSize) * chunkSize,
            Mathf.FloorToInt(globalPosition.z / chunkSize) * chunkSize
        );

        if (chunks.TryGetValue(chunkCoordinates, out Chunk chunk))
        {
            return chunk;
        }

        return null;
    }

    private void GenerateWorld()
    {
        Vector3 chunkPosition = new Vector3(0,0,0);
        GameObject newChunkObject = new GameObject();
        newChunkObject.transform.position = chunkPosition;
        newChunkObject.transform.parent = this.transform;
        Chunk newChunk = newChunkObject.AddComponent<Chunk>();
        newChunk.Initialize(chunkSize);
        chunks.Add(chunkPosition, newChunk);
    }
    public bool IsVoxelActiveAtPosition(int x, int y, int z)
    {
        // Get the global position vector
        Vector3 globalPosition = new Vector3(x, y, z);

        // Find the chunk that contains this global position
        Chunk targetChunk = GetChunkAt(globalPosition);

        // If there's no chunk at this position, return false
        if (targetChunk == null)
        {
            Debug.LogWarning("No chunk found for position: " + globalPosition);
            return false;
        }

        // Calculate the local position within the chunk
        int localX = Mathf.FloorToInt(globalPosition.x % chunkSize);
        int localY = Mathf.FloorToInt(globalPosition.y % chunkSize);
        int localZ = Mathf.FloorToInt(globalPosition.z % chunkSize);

        // Use the chunk's method to check if the voxel is active
        return targetChunk.IsVoxelActiveAt(new Vector3(localX, localY, localZ));
    }



}