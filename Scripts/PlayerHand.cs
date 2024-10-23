using TreeEditor;
using UnityEngine;
using System.Collections;

public class PlayerHand : MonoBehaviour
{

    public Camera playerCamera; // Reference to the player's camera
    public float maxRayDistance = 100f; // Max distance the ray can travel
    public Material redMaterial;  // The red material
    public Material greenMaterial; // The green material

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        HandleBlockSelection();
       
    }
    void HandleBlockSelection()
    {
        // Check for LMB click
        if (Input.GetMouseButtonDown(0))
        {
            // Cast a ray from the center of the screen (camera)
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            // Check if the ray hits something
            if (Physics.Raycast(ray, out hit, maxRayDistance))
            {
                // Round the hit point to the nearest block center
                Vector3 blockPosition = new Vector3(
                    Mathf.FloorToInt(hit.point.x + hit.normal.x * 0.5f),
                    Mathf.FloorToInt(hit.point.y + hit.normal.y * 0.5f),
                    Mathf.FloorToInt(hit.point.z + hit.normal.z * 0.5f)
                );

                // Call World.AddBoxAtPosition with the rounded block coordinates
                World.Instance.AddBoxAtPosition((int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            // Cast a ray from the center of the screen (camera)
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            // Check if the ray hits something
            if (Physics.Raycast(ray, out hit, maxRayDistance))
            {
                // Call the function to identify which face of the cube was clicked
                int faceIndex = GetClickedFace(hit);
                Debug.Log("Face clicked: " + faceIndex);

                // Round the hit point to the nearest block center
                Vector3 blockPosition = new Vector3(
                    Mathf.FloorToInt(hit.point.x + hit.normal.x * 0.5f),
                    Mathf.FloorToInt(hit.point.y + hit.normal.y * 0.5f),
                    Mathf.FloorToInt(hit.point.z + hit.normal.z * 0.5f)
                );

                if (faceIndex == 0)
                {
                    // Call World.RemoveBoxAtPosition with the rounded block coordinates
                    World.Instance.RemoveBoxAtPosition((int)blockPosition.x, (int)blockPosition.y - 1, (int)blockPosition.z);
                }
                else if (faceIndex == 2)
                {
                    World.Instance.RemoveBoxAtPosition((int)blockPosition.x + 1, (int)blockPosition.y, (int)blockPosition.z);
                }
                else if (faceIndex == 3)
                {
                    World.Instance.RemoveBoxAtPosition((int)blockPosition.x - 1, (int)blockPosition.y, (int)blockPosition.z);
                }
                else if (faceIndex == 4)
                {
                    World.Instance.RemoveBoxAtPosition((int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z - 1);
                }
                else if(faceIndex == 5)
                {
                    World.Instance.RemoveBoxAtPosition((int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z + 1);
                }
                else
                {
                    Debug.Log("No implementation for face 1");
                }
            }
        }
    }
    int GetClickedFace(RaycastHit hit)
    {
        // hit.normal will tell us the face that was hit
        Vector3 normal = hit.normal;

        // Check which axis is most aligned with the normal
        if (normal == Vector3.up)
        {
            return 0; // Top face
        }
        else if (normal == Vector3.down)
        {
            return 1; // Bottom face
        }
        else if (normal == Vector3.left)
        {
            return 2; // Left face
        }
        else if (normal == Vector3.right)
        {
            return 3; // Right face
        }
        else if (normal == Vector3.forward)
        {
            return 4; // Front face
        }
        else if (normal == Vector3.back)
        {
            return 5; // Back face
        }

        // Default case (shouldn't happen, but we return -1 in case it does)
        return -1;
    }

    IEnumerator ChangeBlockMaterial(Renderer blockRenderer)
    {
        // Change the material to red
        blockRenderer.material = redMaterial;

        // Wait for 2 seconds
        yield return new WaitForSeconds(2);

        // Change the material back to green
        blockRenderer.material = greenMaterial;
    }
}
