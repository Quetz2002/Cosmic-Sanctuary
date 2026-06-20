using UnityEngine;
using UnityEngine.InputSystem;

public class Recolectar : MonoBehaviour
{
    public float distanciaMaxima = 3f;
    public Camera camara;
    public Spawner generador;
    public TextoFlotante textoEntrega;
    public int objetosCargando = 0;
    public int puntuacionTotal = 0;

    private Controles controles;
    private bool enZonaEntrega = false;

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
                Absorcion absorcion = hit.collider.GetComponent<Absorcion>();

                if (absorcion != null)
                {
                    absorcion.Iniciar(camara.transform, RecolectarFinal);
                }
            }
        }
    }

    void RecolectarFinal(GameObject obj)
    {
        generador.RetirarDePool(obj);
        objetosCargando++;
    }

    void IntentarDepositar()
    {
        if (enZonaEntrega && objetosCargando > 0)
        {
            puntuacionTotal += objetosCargando;
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