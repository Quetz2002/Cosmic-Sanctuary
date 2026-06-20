using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 2f;

    private CharacterController controller;
    private Transform cameraTransform;
    private float verticalRotation = 0f;
    private Vector3 velocity;
    private float gravity = -9.81f;

    [Header("Ground Check Settings")]
    public Transform groundCheck;      // I will link an empty object at the player's feet here
    public float groundDistance = 0.3f; // Radius of the detection sphere
    public LayerMask groundMask;       // Layers that the player can step on (Placeable and NotPlaceable)
    private bool isGrounded;

    public bool isEditingUI = false;
    
    [Header("Movement Control")]
    public bool canMove = true;

    private float startYaw = 0f;
    private float horizontalRotation = 0f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning("[PlayerController] Camera.main not found during Start. Attempting to search children.");
            cameraTransform = GetComponentInChildren<Camera>()?.transform;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        startYaw = transform.localEulerAngles.y;
        horizontalRotation = 0f;

        // Auto-disable movement in the Shuttle scene so the player can only aim and shoot
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Shuttle")
        {
            canMove = false;
        }
    }

    private void Update()
    {
        if (isEditingUI) return;

        HandleLook();

        if (canMove)
        {
            HandleMovement();
        }
    }

    private void HandleLook()
    {
        if (cameraTransform == null)
        {
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
            else
            {
                return;
            }
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        bool inShuttle = (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Shuttle");

        // Pitch clamp (Up/Down)
        verticalRotation -= mouseY;
        float pitchLimit = inShuttle ? 80f : 90f;
        verticalRotation = Mathf.Clamp(verticalRotation, -pitchLimit, pitchLimit);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // Yaw clamp (Left/Right)
        if (inShuttle)
        {
            horizontalRotation += mouseX;
            horizontalRotation = Mathf.Clamp(horizontalRotation, -80f, 80f);
            transform.localRotation = Quaternion.Euler(0f, startYaw + horizontalRotation, 0f);
        }
        else
        {
            transform.Rotate(Vector3.up * mouseX);
        }
    }

    private void HandleMovement()
    {
        // I calculate custom ground detection using a physics sphere check at the feet
        isGrounded = groundCheck != null ? Physics.CheckSphere(groundCheck.position, groundDistance, groundMask) : controller.isGrounded;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // I apply stable downward force when grounded to prevent sliding
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // I allow jumping only when my custom ground check confirms we are touching the floor
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}