using UnityEngine;
using UnityEngine.InputSystem;

public class CamaraMouse : MonoBehaviour
{
    public float sensibilidad = 0.1f;
    public Transform cuerpo;

    private Controles controles;
    private Vector2 entradaMirar;
    private float rotacionX = 0f;

    public bool controlActivo = false;

    void Awake()
    {
        controles = new Controles();
    }

    private void OnMirar(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => entradaMirar = ctx.ReadValue<Vector2>();
    private void OnMirarCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => entradaMirar = Vector2.zero;

    void OnEnable()
    {
        controles.Jugador.Enable();
        controles.Jugador.Mirar.performed += OnMirar;
        controles.Jugador.Mirar.canceled += OnMirarCanceled;
    }

    void OnDisable()
    {
        if (controles != null)
        {
            controles.Jugador.Mirar.performed -= OnMirar;
            controles.Jugador.Mirar.canceled -= OnMirarCanceled;
            controles.Jugador.Disable();
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!controlActivo)
            return;

            float mouseX = entradaMirar.x * sensibilidad;
        float mouseY = entradaMirar.y * sensibilidad;

        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f);
        transform.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);

        if (cuerpo != null)
            cuerpo.Rotate(Vector3.up * mouseX);
        else
            transform.Rotate(Vector3.up * mouseX, Space.World);
    }
}