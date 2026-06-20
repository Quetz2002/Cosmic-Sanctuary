using UnityEngine;

public class AnimacionAbsorcion : MonoBehaviour
{
    public float velocidadInicial = 5f;
    public float aceleracion = 20f;

    public float distanciaFinal = 0.5f;

    private Transform objetivo;
    private float velocidadActual;
    private bool absorbiendo = false;
    private System.Action<GameObject> alCompletar;

    private Vector3 escalaOriginal;  

    void Awake()
    {
        escalaOriginal = transform.localScale; 
    }

    public void Iniciar(Transform destino, System.Action<GameObject> callback)
    {
        objetivo = destino;
        alCompletar = callback;
        velocidadActual = velocidadInicial;
        absorbiendo = true;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
    }

    void Update()
    {
        if (!absorbiendo || objetivo == null)
            return;

        velocidadActual += aceleracion * Time.deltaTime;

        transform.position = Vector3.MoveTowards(
            transform.position,
            objetivo.position,
            velocidadActual * Time.deltaTime
        );

        transform.localScale = Vector3.MoveTowards(
            transform.localScale,
            Vector3.zero,
            Time.deltaTime * 2f
        );

        if (Vector3.Distance(transform.position, objetivo.position) <= distanciaFinal)
        {
            absorbiendo = false;
            alCompletar?.Invoke(gameObject);
        }
    }

    public void Resetear()
    {
        transform.localScale = escalaOriginal;
        absorbiendo = false;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;
    }
}