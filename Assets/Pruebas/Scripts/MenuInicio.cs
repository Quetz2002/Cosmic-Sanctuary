using UnityEngine;
using UnityEngine.InputSystem;

public class MenuInicio : MonoBehaviour
{
    public GameObject canvasMenu;     
    public GameObject canvasHUD;     

    public Movimiento movimiento;
    public Recolector recolector;
    public CamaraMouse camaraMouse;  

    private Controles controles;
    private bool juegoIniciado = false;

    void Awake()
    {
        controles = new Controles();
    }

    private void OnComenzar(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => Comenzar();

    void OnEnable()
    {
        controles.Jugador.Enable();
        controles.Jugador.Comenzar.performed += OnComenzar;
    }

    void OnDisable()
    {
        if (controles != null)
        {
            controles.Jugador.Comenzar.performed -= OnComenzar;
            controles.Jugador.Disable();
        }
    }

    void Start()
    {
        MostrarMenu();
    }

    void MostrarMenu()
    {
        if (canvasMenu != null) canvasMenu.SetActive(true);
        if (canvasHUD != null) canvasHUD.SetActive(false);

        if (camaraMouse != null) camaraMouse.controlActivo = false;

        if (movimiento != null) movimiento.enabled = false;
        if (recolector != null) recolector.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Comenzar()
    {
        if (juegoIniciado)
            return;   

        juegoIniciado = true;

        if (canvasMenu != null) canvasMenu.SetActive(false);
        if (canvasHUD != null) canvasHUD.SetActive(true);
        
        if (camaraMouse != null) camaraMouse.controlActivo = true;
        if (movimiento != null) movimiento.enabled = true;
        if (recolector != null) recolector.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}