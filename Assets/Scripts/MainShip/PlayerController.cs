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

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (isEditingUI) return;

        HandleLook();
        HandleMovement();
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
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