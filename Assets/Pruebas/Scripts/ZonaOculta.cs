using UnityEngine;

public class ZonaOculta : MonoBehaviour
{
    public Spawner spawner;                  
    public Collider barrera;                 
    public ParticleSystem efectoCerrado;

    void OnEnable()
    {
        if (spawner != null)
            spawner.AlRecolectarTodo += Abrir;   
    }
    void OnDisable()
    {
        if (spawner != null)
            spawner.AlRecolectarTodo -= Abrir;   
    }
    void Abrir()
    {
        if (barrera != null)
            barrera.enabled = false;

        if (efectoCerrado != null)
            efectoCerrado.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}
