using UnityEngine;
using UnityEngine.InputSystem;

public class Movimiento : MonoBehaviour
{
    public float velocidad = 5f;
    private Controles controles;
    private Vector2 entradaMovimiento;
    private Rigidbody rb;

    void Awake()
    {
        controles = new Controles();
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        controles.Jugador.Enable();
        controles.Jugador.Mover.performed += ctx => entradaMovimiento = ctx.ReadValue<Vector2>();
        controles.Jugador.Mover.canceled += ctx => entradaMovimiento = Vector2.zero;
    }

    void OnDisable()
    {
        controles.Jugador.Disable();
    }

    void FixedUpdate()
    {
        Vector3 movimiento = transform.right * entradaMovimiento.x + transform.forward * entradaMovimiento.y;
        rb.MovePosition(rb.position + movimiento * velocidad * Time.fixedDeltaTime);
    }
}