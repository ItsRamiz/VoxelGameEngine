using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;
    public Transform cameraTransform;

    public float speed = 10.0f;
    public float gravity = -15.81f;
    public float jumpHeight = 3.0f;

    public float mouseSensitivity = 300.0f; // Mouse sensitivity for looking around

    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float xRotation = 0f; // Rotation around the x-axis for camera (pitch)

    private Transform playerTransform;

    void Start()
    {
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        playerTransform = transform;
    }

    void Update()
    {
        PlayerMove();
        HandleCameraRotation(); // Handle looking around
    }

    public Vector3 getPlayerPosition()
    {
        return playerTransform.position; // Return the current position of the player.
    }

    void PlayerMove()
    {
        // Check if the player is grounded
        groundedPlayer = characterController.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // Get the input for movement
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0; // We do not want to move up/down by the camera's forward vector

        // Move the player
        characterController.Move(move * Time.deltaTime * speed);

        // Jumping
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        }

        // Apply gravity
        playerVelocity.y += gravity * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
    }

    void HandleCameraRotation()
    {
        // Get mouse input for looking around
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the player around the y-axis (yaw) using mouseX
        playerTransform.Rotate(Vector3.up * mouseX);

        // Control camera pitch (up/down rotation), clamping to prevent flipping
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Restrict vertical rotation

        // Apply the rotation to the camera
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
