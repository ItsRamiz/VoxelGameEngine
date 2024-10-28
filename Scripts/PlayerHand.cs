using TreeEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHand : MonoBehaviour
{

    public Camera playerCamera;
    public float maxRayDistance = 100f;
    public Material redMaterial;
    public Material greenMaterial;


    void Start()
    {
        
    }

    void Update()
    {
        HandleBlockSelection();
       
    }
    void HandleBlockSelection()
    {
        // MOUSE LEFT CLICK
        int xOffset = 0;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxRayDistance))
            {
                // Round the hit point to a block cords (nearest)

                int faceIndex = GetClickedFace(hit);
                Debug.Log("Face clicked: " + faceIndex);

                Vector3 blockPosition = new Vector3(
                    Mathf.FloorToInt(hit.point.x + hit.normal.x * 0.5f),
                    Mathf.FloorToInt(hit.point.y + hit.normal.y * 0.5f),
                    Mathf.FloorToInt(hit.point.z + hit.normal.z * 0.5f)
                );

                if (World.Instance.IsVoxelActiveAtPosition((int)blockPosition.x - 1, (int)blockPosition.y, (int)blockPosition.z) == true ||
                    World.Instance.IsVoxelActiveAtPosition((int)blockPosition.x - 3, (int)blockPosition.y, (int)blockPosition.z) == true)
                {
                    xOffset += 1;
                }

                if (World.Instance.IsVoxelActiveAtPosition((int)blockPosition.x + 1, (int)blockPosition.y, (int)blockPosition.z) == true ||
                    World.Instance.IsVoxelActiveAtPosition((int)blockPosition.x + 3, (int)blockPosition.y, (int)blockPosition.z) == true)
                {
                    xOffset += -1;
                }

                World.Instance.AddBoxAtPosition((int)blockPosition.x + xOffset, (int)blockPosition.y, (int)blockPosition.z);
            }
        }
        // MOUSE RIGHT CLICK
        else if (Input.GetMouseButtonDown(1))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxRayDistance))
            {
                Chunk chunk = hit.collider.GetComponent<Chunk>();
                if (chunk != null)
                {
                    int triangleIndex = hit.triangleIndex;
                    Vector3 blockPosition = chunk.GetVoxelPositionFromTriangleIndex(triangleIndex);
                    World.Instance.RemoveBoxAtPosition((int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z);
                }
            }
        }
    }
    int GetClickedFace(RaycastHit hit)
    {
        Vector3 normal = hit.normal.normalized;

        // Define the custom face normals
        Vector3 leftFaceNormal = new Vector3(0, 0, 1);           // Left Face
        Vector3 rightFaceNormal = new Vector3(0, 0, -1);         // Right Face
        Vector3 topFaceNormal = new Vector3(0, 1, 0);            // Top Face
        Vector3 backFaceNormal = new Vector3(-0.71f, 0.71f, 0);  // Back Face
        Vector3 frontFaceNormal = new Vector3(0.71f, -0.71f, 0); // Front Face

        // Set a threshold for similarity to account for potential floating-point variations
        float threshold = 0.9f;

        // Determine which face was clicked by comparing the normal with each face's normal
        if (Vector3.Dot(normal, leftFaceNormal) > threshold)
        {
            return 2; // Left Face
        }
        if (Vector3.Dot(normal, rightFaceNormal) > threshold)
        {
            return 3; // Right Face
        }
        if (Vector3.Dot(normal, topFaceNormal) > threshold)
        {
            return 0; // Top Face
        }
        if (Vector3.Dot(normal, backFaceNormal) > threshold)
        {
            return 5; // Back Face
        }
        if (Vector3.Dot(normal, frontFaceNormal) > threshold)
        {
            return 4; // Front Face
        }

        // Return -1 if no face matches
        return -1;
    }

    IEnumerator ChangeBlockMaterial(Renderer blockRenderer)
    {
        blockRenderer.material = redMaterial;
        yield return new WaitForSeconds(2);
        blockRenderer.material = greenMaterial;
    }
}
