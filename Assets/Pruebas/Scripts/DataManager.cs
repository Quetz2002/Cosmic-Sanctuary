using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instancia { get; private set; }

    public int scoreTotal = 0;
    public int totalRecolectado = 0;
    public bool zonaDesbloqueada = false;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SumarScore(int cantidad)
    {
        scoreTotal += cantidad;
    }

    public int ObtenerScoreTotal()
    {
        return scoreTotal;
    }

    public void SumarRecolectado(int cantidad)
    {
        totalRecolectado += cantidad;
    }

    public int ObtenerTotalRecolectado()
    {
        return totalRecolectado;
    }

    public void MarcarZonaDesbloqueada()
    {
        zonaDesbloqueada = true;
    }

    public bool EstaZonaDesbloqueada()
    {
        return zonaDesbloqueada;
    }
}
