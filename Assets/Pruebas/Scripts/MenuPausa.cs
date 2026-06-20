using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MenuPausa : MonoBehaviour
{
    public GameObject panelPausa;
    public Slider sliderVolumen;

    public CamaraMouse camaraMouse;

    public string nombreEscenaTitulo = "Titulo";

    private Controles controles;
    private bool pausado = false;

    void Awake()
    {
        controles = new Controles();
    }

    void OnEnable()
    {
        controles.Jugador.Enable();
        controles.Jugador.Pausa.performed += ctx => AlternarPausa();
    }

    void OnDisable()
    {
        controles.Jugador.Disable();
    }

    void Start()
    {
        if (panelPausa != null)
            panelPausa.SetActive(false);

        if (sliderVolumen != null)
        {
            sliderVolumen.value = AudioListener.volume;
            sliderVolumen.onValueChanged.AddListener(CambiarVolumen);
        }
    }

    void AlternarPausa()
    {
        if (pausado)
            Reanudar();
        else
            Pausar();
    }

    void Pausar()
    {
        pausado = true;
        if (panelPausa != null)
            panelPausa.SetActive(true);

        Time.timeScale = 0f;

        if (camaraMouse != null)
            camaraMouse.controlActivo = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Reanudar()
    {
        pausado = false;
        if (panelPausa != null)
            panelPausa.SetActive(false);

        Time.timeScale = 1f;   

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void IrAlMenuPrincipal()
    {
        Time.timeScale = 1f;   
        SceneManager.LoadScene(nombreEscenaTitulo);
    }

    public void SalirDelJuego()
    {
        Application.Quit();
    }

    public void CambiarVolumen(float valor)
    {
        AudioListener.volume = valor;
    }
}