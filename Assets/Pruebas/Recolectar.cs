using UnityEngine;

public class Recolectar : MonoBehaviour
{
    [SerializeField]
    private float distanciaMax;
    public Camera camara;
    public int objRecogidos;
    public int puntuacion;

    private Controles controles;
    private bool enZonaEnt = false;
    
    void Awake()
    {
        controles = new Controles();
        if (camara == null)
        {
            camara = Camera.main;
        }
    }

    void OnEnable()
    {
        controles.Jugador.Enable();
        controles.Jugador.Recolectar.performed += ctx => IntentarRecolectar();
        controles.Jugador.Depositar.performed += ctx => IntentarDepositar();
    }

    void OnDisable()
    {

    }

    void IntentarRecolectar()
    {
        Ray ray = new Ray(camara.transform.position, camara.transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, distanciaMax))
        {
            if (hit.collider.CompareTag("Recolectable"))
            {
                Destroy(hit.collider.gameObject, .5f);
                objRecogidos++;
                Debug.Log("Objetos cargando" + objRecogidos);
            }
        }
    }

    void IntentarDepositar()
    {
        if (enZonaEnt && objRecogidos > 0)
        {
            puntuacion += objRecogidos;
            objRecogidos = 0;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ZonaEntrega"))
        {
            enZonaEnt = true;
            Debug.Log("Entraste a la zona de entrega.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ZonaEntrega"))
        {
            enZonaEnt = false;
        }
    }
}
