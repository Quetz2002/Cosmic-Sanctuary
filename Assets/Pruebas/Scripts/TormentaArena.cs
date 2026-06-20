using UnityEngine;

public class TormentaArena : MonoBehaviour
{
    public ParticleSystem particulas;

    public int recolectadoInicioDisminucion;
    public int recolectadoEliminacion;         

    public float velocidadCambio = 1f;

    public float emisionMaxima;

    public float densidadMaxima = 0.03f;

    private ParticleSystem.EmissionModule emision;
    private float intensidad = 1f;

    void Awake()
    {
        if (particulas == null)
            particulas = GetComponent<ParticleSystem>();

        if (particulas != null)
        {
            emision = particulas.emission;
        }

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;

        intensidad = CalcularIntensidadObjetivo();
        AplicarIntensidad();
    }

    void Update()
    {
        float objetivo = CalcularIntensidadObjetivo();

        intensidad = Mathf.MoveTowards(intensidad, objetivo, velocidadCambio * Time.deltaTime);

        AplicarIntensidad();
    }

    float CalcularIntensidadObjetivo()
    {
        if (DataManager.Instancia == null)
            return 1f;

        int recolectado = DataManager.Instancia.ObtenerTotalRecolectado();

        if (recolectado < recolectadoInicioDisminucion)
            return 1f;

        if (recolectado >= recolectadoEliminacion)
            return 0f;

        float divisor = recolectadoEliminacion - recolectadoInicioDisminucion;
        if (divisor <= 0f)
            return 0f;

        float t = (float)(recolectado - recolectadoInicioDisminucion) / divisor;
        return 1f - t;
    }

    void AplicarIntensidad()
    {
        if (particulas != null)
        {
            emision.rateOverTime = emisionMaxima * intensidad;
        }
        RenderSettings.fogDensity = densidadMaxima * intensidad;
        RenderSettings.fog = intensidad > 0.001f;
    }
}