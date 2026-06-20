using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarraProgreso : MonoBehaviour
{
    [Header("Configuración")]
    public int maximoPorVuelta = 100;
    public int totalVueltas = 5;             

    [Header("Colores que cicla")]
    public Color[] colores;

    [Header("Referencias UI")]
    public Image rellenoProgreso;
    public TMP_Text textoNivel;

    [Header("Animación")]
    public float velocidadLlenado = 2f;

    private float rellenoVisual = 0f;
    private int nivelAnterior = 0;

    void Start()
    {
        if (DataManager.Instancia != null)
        {
            int score = DataManager.Instancia.ObtenerScoreTotal();
            int nivel = Mathf.Min(score / maximoPorVuelta, totalVueltas);

            nivelAnterior = nivel;

            if (nivel >= totalVueltas)
                rellenoVisual = 1f;
            else
                rellenoVisual = (float)(score % maximoPorVuelta) / maximoPorVuelta;
        }
    }

    void Update()
    {
        if (DataManager.Instancia == null || rellenoProgreso == null)
            return;

        int score = DataManager.Instancia.ObtenerScoreTotal();

        int nivel = Mathf.Min(score / maximoPorVuelta, totalVueltas);
        int progresoEnVuelta = score % maximoPorVuelta;
        float objetivo = (float)progresoEnVuelta / maximoPorVuelta;

        if (nivel >= totalVueltas)
        {
            rellenoVisual = Mathf.MoveTowards(rellenoVisual, 1f, velocidadLlenado * Time.deltaTime);
            nivelAnterior = totalVueltas;
        }
        else if (nivel > nivelAnterior)
        {
            rellenoVisual = Mathf.MoveTowards(rellenoVisual, 1f, velocidadLlenado * Time.deltaTime);

            if (rellenoVisual >= 0.999f)
            {
                rellenoVisual = 0f;
                nivelAnterior = nivel;
            }
        }
        else
        {
            rellenoVisual = Mathf.MoveTowards(rellenoVisual, objetivo, velocidadLlenado * Time.deltaTime);
        }

        rellenoProgreso.fillAmount = rellenoVisual;
        ActualizarColor(nivelAnterior);
        ActualizarTexto(nivelAnterior);
    }

    void ActualizarColor(int nivel)
    {
        if (colores == null || colores.Length == 0)
            return;

        int indiceColor = (nivel >= totalVueltas) ? (totalVueltas - 1) : nivel;
        Color color = colores[indiceColor % colores.Length];
        rellenoProgreso.color = color;
    }

    void ActualizarTexto(int nivel)
    {
        if (textoNivel == null)
            return;

        if (nivel >= totalVueltas)
            textoNivel.text = "Progreso de recompensa";
        else
            textoNivel.text = "Progreso a la siguiente recompensa";
    }
}