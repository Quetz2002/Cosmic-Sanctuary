using UnityEngine;

public class TormentaArena : MonoBehaviour
{
    public ParticleSystem particulas;
    public Spawner generador;

    [Range(0f, 1f)]
    public float intensidad = 1f;

    [Range(0f, 1f)]
    public float umbralInicioDisminucion = 0.5f;
    public float velocidadCambio = 1f;

    public float emisionMaxima = 400f;

    public float densidadMaxima = 0.03f;

    private ParticleSystem.EmissionModule emision;

    void Awake()
    {
        if (particulas == null)
            particulas = GetComponent<ParticleSystem>();

        emision = particulas.emission;

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
    }

    void Update()
    {
        if (generador != null)
        {
            float intensidadObjetivo = CalcularIntensidadPorProgreso();
            intensidad = Mathf.MoveTowards(intensidad, intensidadObjetivo, velocidadCambio * Time.deltaTime);
        }

        AplicarIntensidad();
    }

    float CalcularIntensidadPorProgreso()
    {
        float progreso = generador.ProgresoRecoleccion();

        if (progreso < umbralInicioDisminucion)
            return 1f;

        if (progreso >= 1f)
            return 0f;

        float t = (progreso - umbralInicioDisminucion) / (1f - umbralInicioDisminucion);
        return 1f - t;
    }

    void AplicarIntensidad()
    {
        emision.rateOverTime = emisionMaxima * intensidad;
        RenderSettings.fogDensity = densidadMaxima * intensidad;
        RenderSettings.fog = intensidad > 0.001f;
    }
}