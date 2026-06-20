using UnityEngine;
using UnityEngine.InputSystem;


public class Movimiento : MonoBehaviour
{
    public float velocidad;
    public float velocidadSprint;
    public float multiplicadorAire;

    public float fuerzaSalto;
    public float gravedadExtra;

    public Transform checkSuelo;
    public float radioCheck = 0.3f;
    public LayerMask capaSuelo;
                        
    public float estaminaMaxima = 100f;           
    public float consumoPorSegundo = 25f;         
    public float recargaPorSegundo = 15f;         
    public float retrasoRecarga = 1f;             
    public float estaminaMinimaParaCorrer = 10f;  

    private Controles controles;
    private Vector2 entradaMovimiento;
    private Rigidbody rb;
    private bool enSuelo;
    private bool sprintando;

    private float estaminaActual;                 
    private float tiempoSinCorrer;                
    private bool agotado;                          

    void Awake()
    {
        controles = new Controles();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        estaminaActual = estaminaMaxima;          
    }

    private void OnMover(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => entradaMovimiento = ctx.ReadValue<Vector2>();
    private void OnMoverCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => entradaMovimiento = Vector2.zero;
    private void OnSaltar(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => Saltar();
    private void OnSprint(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => sprintando = true;
    private void OnSprintCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => sprintando = false;

    void OnEnable()
    {
        controles.Jugador.Enable();
        controles.Jugador.Mover.performed += OnMover;
        controles.Jugador.Mover.canceled += OnMoverCanceled;
        controles.Jugador.Saltar.performed += OnSaltar;
        controles.Jugador.Sprint.performed += OnSprint;
        controles.Jugador.Sprint.canceled += OnSprintCanceled;
    }

    void OnDisable()
    {
        if (controles != null)
        {
            controles.Jugador.Mover.performed -= OnMover;
            controles.Jugador.Mover.canceled -= OnMoverCanceled;
            controles.Jugador.Saltar.performed -= OnSaltar;
            controles.Jugador.Sprint.performed -= OnSprint;
            controles.Jugador.Sprint.canceled -= OnSprintCanceled;
            controles.Jugador.Disable();
        }
    }

    void Update()
    {
        enSuelo = checkSuelo != null ? Physics.CheckSphere(checkSuelo.position, radioCheck, capaSuelo) : false;
        GestionarEstamina();                      
    }

    void FixedUpdate()
    {
        Mover();
        AplicarGravedadExtra();
    }

    void GestionarEstamina()
    {
        bool corriendoDeVerdad = EstaCorriendo();

        if (corriendoDeVerdad)
        {
            estaminaActual -= consumoPorSegundo * Time.deltaTime;
            tiempoSinCorrer = 0f;

            if (estaminaActual <= 0f)
            {
                estaminaActual = 0f;
                agotado = true; 
            }
        }
        else
        {
            tiempoSinCorrer += Time.deltaTime;

            if (tiempoSinCorrer >= retrasoRecarga)
            {
                estaminaActual += recargaPorSegundo * Time.deltaTime;
                estaminaActual = Mathf.Min(estaminaActual, estaminaMaxima);
            }

            if (agotado && estaminaActual >= estaminaMinimaParaCorrer)
                agotado = false;
        }
    }

    bool EstaCorriendo()
    {
        bool hayMovimiento = entradaMovimiento.sqrMagnitude > 0.01f;
        return sprintando && enSuelo && hayMovimiento && !agotado && estaminaActual > 0f;
    }

    void Mover()
    {
        Vector3 direccion = transform.right * entradaMovimiento.x + transform.forward * entradaMovimiento.y;
        direccion.Normalize();

        float control = enSuelo ? 1f : multiplicadorAire;

        float velocidadActualMax = EstaCorriendo() ? velocidadSprint : velocidad;

        Vector3 velocidadObjetivo = direccion * velocidadActualMax;
        Vector3 velocidadActual = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        Vector3 cambioVelocidad = (velocidadObjetivo - velocidadActual) * control;

        rb.AddForce(cambioVelocidad, ForceMode.VelocityChange);
    }

    void AplicarGravedadExtra()
    {
        if (!enSuelo)
            rb.AddForce(Vector3.down * gravedadExtra, ForceMode.Acceleration);
    }

    void Saltar()
    {
        if (enSuelo)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
        }
    }

    public float ObtenerEstaminaNormalizada()
    {
        return estaminaActual / estaminaMaxima;
    }

    void OnDrawGizmosSelected()
    {
        if (checkSuelo != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(checkSuelo.position, radioCheck);
        }
    }
}