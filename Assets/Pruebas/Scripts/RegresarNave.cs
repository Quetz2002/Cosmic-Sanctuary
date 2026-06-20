using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PortalEscena : MonoBehaviour
{
    public GameObject textoFlotante;     
    public Camera camara;               

    public string nombreEscena;   

    public string tagJugador = "Player";    

    private Controles controles;
    private bool jugadorDentro = false;

    void Awake()
    {
        controles = new Controles();
        if (camara == null)
            camara = Camera.main;
    }

    void OnEnable()
    {
        controles.Jugador.Enable();
        controles.Jugador.Depositar.performed += ctx => IntentarEntrar();
    }

    void OnDisable()
    {
        controles.Jugador.Disable();
    }

    void Start()
    {
        if (textoFlotante != null)
            textoFlotante.SetActive(false);
    }

    void LateUpdate()
    {
        if (textoFlotante != null && textoFlotante.activeSelf && camara != null)
        {
            textoFlotante.transform.rotation = Quaternion.LookRotation(
                textoFlotante.transform.position - camara.transform.position
            );
        }
    }

    void IntentarEntrar()
    {
        if (jugadorDentro)
        {
            CambiarEscena();
        }
    }

    void CambiarEscena()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscena);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagJugador))
        {
            jugadorDentro = true;

            if (textoFlotante != null)
                textoFlotante.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagJugador))
        {
            jugadorDentro = false;

            if (textoFlotante != null)
                textoFlotante.SetActive(false);
        }
    }
}