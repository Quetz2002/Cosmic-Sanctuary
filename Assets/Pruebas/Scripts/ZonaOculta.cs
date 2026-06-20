using UnityEngine;

public class ZonaOculta : MonoBehaviour
{               
    public Collider barrera;                 
    public ParticleSystem efectoCerrado;

    public int scoreDesbloqueo;
    private bool desbloqueada = false;

    void Start()
    {
        if (DataManager.Instancia != null && DataManager.Instancia.EstaZonaDesbloqueada())
        {
            AbrirInmediato();
        }
    }
    void Update()
    {
        if (desbloqueada)
            return;

        if (DataManager.Instancia != null && DataManager.Instancia.ObtenerTotalRecolectado() >= scoreDesbloqueo)   
            Abrir();
    }

    void Abrir()
    {
        desbloqueada = true;

        if (barrera != null)
            barrera.enabled = false;

        if (efectoCerrado != null)
            efectoCerrado.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        if (DataManager.Instancia != null)
            DataManager.Instancia.MarcarZonaDesbloqueada();
    }

    void AbrirInmediato()
    {
        desbloqueada = true;

        if (barrera != null)
            barrera.enabled = false;

        if (efectoCerrado != null)
            efectoCerrado.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
