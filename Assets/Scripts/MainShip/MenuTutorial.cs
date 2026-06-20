using UnityEngine;
using UnityEngine.InputSystem;

public class MenuTutorial : MonoBehaviour
{
    public GameObject canvasMenu;

    private Controles controles;
    private bool juegoIniciado = false;

    void Awake()
    {
        controles = new Controles();
    }

    void OnEnable()
    {
        controles.Jugador.Enable();
        controles.Jugador.Comenzar.performed += ctx => Comenzar();
    }

    void OnDisable()
    {
        controles.Jugador.Disable();
        Time.timeScale = 1f;
    }

    void Start()
    {
        MostrarMenu();
    }

    void MostrarMenu()
    {
        if (canvasMenu != null) canvasMenu.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Comenzar()
    {
        if (juegoIniciado)
            return;

        juegoIniciado = true;

        Time.timeScale = 1f;

        if (canvasMenu != null) canvasMenu.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}