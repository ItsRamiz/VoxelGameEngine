using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    private Voxel[,,] voxels;
    private int chunkSize = 16;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    // Dictionary to map each triangle's index to its voxel position
    private Dictionary<int, Vector3> triangleToVoxelMap = new Dictionary<int, Vector3>();

    public bool oneLayer = true;

    void Start()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        GenerateMesh();
    }

    void Update()
    {
        // Update logic if needed
    }

    private void PrintActiveVoxels()
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    if (voxels[x, y, z].isActive)
                    {
                        Debug.Log($"Active Voxel at Coordinates: ({x}, {y}, {z})");
                    }
                }
            }
        }
    }

    public void OneLayerChunk()
    {
        oneLayer = false;
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    if (y > 0)
                    {
                        voxels[x, y, z].isActive = false;
                    }
                }
            }
        }
    }

    public void GenerateMesh()
    {
        if (oneLayer)
        {
            Debug.Log("One Layer Processed");
            OneLayerChunk();
        }

        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
        triangleToVoxelMap.Clear(); // Clear mapping for regenerating

        ProcessVoxels();

        Mesh mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uvs.ToArray()
        };

        mesh.RecalculateNormals(); // for lighting

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        meshRenderer.material = World.Instance.VoxelMaterial;
    }

    public void ProcessVoxels()
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    if (voxels == null || x < 0 || x >= voxels.GetLength(0) ||
                        y < 0 || y >= voxels.GetLength(1) || z < 0 || z >= voxels.GetLength(2))
                    {
                        return;
                    }
                    Voxel voxel = voxels[x, y, z];
                    if (voxel.isActive)
                    {
                        bool[] facesVisible = new bool[6];

                        facesVisible[0] = IsFaceVisible(x, y + 1, z); // Top
                        facesVisible[1] = IsFaceVisible(x, y - 1, z); // Bottom
                        facesVisible[2] = IsFaceVisible(x - 1, y, z); // Left
                        facesVisible[3] = IsFaceVisible(x + 1, y, z); // Right
                        facesVisible[4] = IsFaceVisible(x, y, z + 1); // Front
                        facesVisible[5] = IsFaceVisible(x, y, z - 1); // Back

                        for (int i = 0; i < facesVisible.Length; i++)
                        {
                            if (facesVisible[i])
                                AddFaceData(x, y, z, i);
                        }
                    }
                }
            }
        }
    }

    private void AddFaceData(int x, int y, int z, int faceIndex)
    {
        Vector3 voxelPosition = new Vector3(x, y, z); // Store the voxel's position

        if (faceIndex == 0) // Top Face
        {
            vertices.Add(new Vector3(x, y + 1, z));
            vertices.Add(new Vector3(x, y + 1, z + 1));
            vertices.Add(new Vector3(x + 2, y + 1, z + 1));
            vertices.Add(new Vector3(x + 2, y + 1, z));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(0, 1));
            AddTriangleIndices(true, voxelPosition);
        }
        else if (faceIndex == 1) // Bottom Face
        {
            vertices.Add(new Vector3(x - 1, y, z));
            vertices.Add(new Vector3(x + 1, y, z));
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x - 1, y, z + 1));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));
            AddTriangleIndices(true, voxelPosition);
        }
        else if (faceIndex == 2) // Left Face
        {
            vertices.Add(new Vector3(x - 1, y, z));
            vertices.Add(new Vector3(x - 1, y, z + 1));
            vertices.Add(new Vector3(x, y + 1, z + 1));
            vertices.Add(new Vector3(x, y + 1, z));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(0, 1));
            AddTriangleIndices(true, voxelPosition);
        }
        else if (faceIndex == 3) // Right Face
        {
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x + 1, y, z));
            vertices.Add(new Vector3(x + 2, y + 1, z));
            vertices.Add(new Vector3(x + 2, y + 1, z + 1));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));
            AddTriangleIndices(true, voxelPosition);
        }
        else if (faceIndex == 4) // Front Face (with additional triangle for parallelogram shape)
        {
            vertices.Add(new Vector3(x, y, z + 1));
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            vertices.Add(new Vector3(x, y + 1, z + 1));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 1));
            AddTriangleIndices(true, voxelPosition);

            // Additional triangle for parallelogram
            vertices.Add(new Vector3(x - 1, y, z + 1));
            vertices.Add(new Vector3(x, y, z + 1));
            vertices.Add(new Vector3(x, y + 1, z + 1));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0.5f, 1));
            AddTriangleIndices(false, voxelPosition);

            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x + 2, y + 1, z + 1));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(0, 1));
            AddTriangleIndices(false, voxelPosition);
        }
        else if (faceIndex == 5) // Back Face
        {
            vertices.Add(new Vector3(x + 1, y, z));
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x, y + 1, z));
            vertices.Add(new Vector3(x + 1, y + 1, z));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            AddTriangleIndices(true, voxelPosition);

            // Additional triangles for parallelogram on the back face
            vertices.Add(new Vector3(x - 1, y, z));
            vertices.Add(new Vector3(x, y + 1, z));
            vertices.Add(new Vector3(x, y, z));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0.5f, 1));
            uvs.Add(new Vector2(0, 0.5f));
            AddTriangleIndices(false, voxelPosition);

            vertices.Add(new Vector3(x + 1, y, z));
            vertices.Add(new Vector3(x + 1, y + 1, z));
            vertices.Add(new Vector3(x + 2, y + 1, z));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            AddTriangleIndices(false, voxelPosition);
        }
    }

    private void AddTriangleIndices(bool isQuad, Vector3 voxelPosition)
    {
        int vertCount = vertices.Count;

        if (isQuad)
        {
            triangles.Add(vertCount - 4);
            triangles.Add(vertCount - 3);
            triangles.Add(vertCount - 2);
            triangles.Add(vertCount - 4);
            triangles.Add(vertCount - 2);
            triangles.Add(vertCount - 1);

            int triangleIndex = (triangles.Count - 6) / 3;
            triangleToVoxelMap[triangleIndex] = voxelPosition;
            triangleToVoxelMap[triangleIndex + 1] = voxelPosition;
        }
        else
        {
            triangles.Add(vertCount - 3);
            triangles.Add(vertCount - 2);
            triangles.Add(vertCount - 1);

            int triangleIndex = (triangles.Count - 3) / 3;
            triangleToVoxelMap[triangleIndex] = voxelPosition;
        }
    }

    private bool IsFaceVisible(int x, int y, int z)
    {
        Vector3 globalPos = transform.position + new Vector3(x, y, z);
        return IsVoxelHiddenInChunk(x, y, z) && IsVoxelHiddenInWorld(globalPos);
    }

    private bool IsVoxelHiddenInWorld(Vector3 globalPos)
    {
        Chunk neighborChunk = World.Instance.GetChunkAt(globalPos);
        if (neighborChunk == null)
        {
            return true;
        }

        Vector3 localPos = neighborChunk.transform.InverseTransformPoint(globalPos);
        return !neighborChunk.IsVoxelActiveAt(localPos);
    }

    public bool IsVoxelActiveAt(Vector3 localPosition)
    {
        int x = Mathf.RoundToInt(localPosition.x);
        int y = Mathf.RoundToInt(localPosition.y);
        int z = Mathf.RoundToInt(localPosition.z);

        if (x >= 0 && x < chunkSize && y >= 0 && y < chunkSize && z >= 0 && z < chunkSize)
        {
            return voxels[x, y, z].isActive;
        }

        return false;
    }

    private bool IsVoxelHiddenInChunk(int x, int y, int z)
    {
        if (x < 0 || x >= chunkSize || y < 0 || y >= chunkSize || z < 0 || z >= chunkSize)
            return true;
        return !voxels[x, y, z].isActive;
    }

    public Vector3 GetVoxelPositionFromTriangleIndex(int triangleIndex)
    {
        return triangleToVoxelMap.ContainsKey(triangleIndex) ? triangleToVoxelMap[triangleIndex] : Vector3.zero;
    }

    public void SetVoxel(int x, int y, int z, bool isActive)
    {
        if (x >= 0 && x < chunkSize && y >= 0 && y < chunkSize && z >= 0 && z < chunkSize)
        {
            voxels[x, y, z].isActive = isActive;
            vertices.Clear();
            triangles.Clear();
            uvs.Clear();
            GenerateMesh();
        }
    }

    public void Initialize(int size)
    {
        chunkSize = size;
        voxels = new Voxel[size, size, size];
        for (int x = 0; x < chunkSize; x++)
            for (int y = 0; y < chunkSize; y++)
                for (int z = 0; z < chunkSize; z++)
                    voxels[x, y, z] = new Voxel(transform.position + new Vector3(x, y, z), Color.white);
    }
}
