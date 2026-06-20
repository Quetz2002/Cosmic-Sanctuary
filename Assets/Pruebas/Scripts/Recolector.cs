using UnityEngine;
using UnityEngine.InputSystem;

public class Recolector : MonoBehaviour
{
    public float distanciaMaxima = 3f;
    public Camera camara;
    public Spawner spawner;
    public TextoFlotante textoEntrega;
    public int objetosCargando = 0;
    public int depositadosEstaSesion = 0;

    private Controles controles;
    private bool enZonaEntrega = false;
    
    public AudioSource audioSource;
    public AudioClip sonidoArrancar;       
    public AudioClip sonidoRecibir;
    public AudioClip sonidoDepositar;
    void Awake()
    {
        controles = new Controles();
        if (camara == null)
            camara = Camera.main;
    }

    private void OnRecolectar(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => IntentarRecolectar();
    private void OnDepositar(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => IntentarDepositar();

    void OnEnable()
    {
        controles.Jugador.Enable();
        controles.Jugador.Recolectar.performed += OnRecolectar;
        controles.Jugador.Depositar.performed += OnDepositar;
    }

    void OnDisable()
    {
        if (controles != null)
        {
            controles.Jugador.Recolectar.performed -= OnRecolectar;
            controles.Jugador.Depositar.performed -= OnDepositar;
            controles.Jugador.Disable();
        }
    }

    void IntentarRecolectar()
    {
        if (camara == null)
            camara = Camera.main;
        if (camara == null) return;

        Ray ray = new Ray(camara.transform.position, camara.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaMaxima))
        {
            if (hit.collider.CompareTag("Recolectable"))
            {
                ReproducirSonido(sonidoArrancar);
                AnimacionAbsorcion absorcion = hit.collider.GetComponent<AnimacionAbsorcion>();

                if (absorcion != null)
                    absorcion.Iniciar(camara.transform, RecolectarFinal);
                else
                    RecolectarFinal(hit.collider.gameObject);
            }
        }
    }

    void RecolectarFinal(GameObject obj)
    {
        objetosCargando++;

        if (DataManager.Instancia != null)
            DataManager.Instancia.SumarRecolectado(1);

        ReproducirSonido(sonidoRecibir);

        AnimacionAbsorcion absorcion = obj.GetComponent<AnimacionAbsorcion>();
        if (absorcion != null)
            absorcion.Resetear();

        obj.SetActive(false);

        if (spawner != null)
            spawner.Reaparecer(obj);
    }

    void IntentarDepositar()
    {
        if (enZonaEntrega && objetosCargando > 0)
        {
            if (DataManager.Instancia != null)
                DataManager.Instancia.SumarScore(objetosCargando);
            depositadosEstaSesion += objetosCargando;
            ReproducirSonido(sonidoDepositar);
            objetosCargando = 0;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ZonaEntrega"))
        {
            enZonaEntrega = true;
            if (textoEntrega != null)
                textoEntrega.Mostrar();
        }
    }

    void ReproducirSonido(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ZonaEntrega"))
        {
            enZonaEntrega = false;
            if (textoEntrega != null)
                textoEntrega.Ocultar();
        }
    }
}