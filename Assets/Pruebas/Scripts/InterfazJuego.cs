using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InterfazJuego : MonoBehaviour
{
    public Movimiento movimiento;
    public Recolector recolector;

    public Image rellenoEstamina;
    public CanvasGroup grupoEstamina;
    public float velocidadFundido = 5f;

    public TMP_Text textoCargando;
    public TMP_Text textoDepositado;    
    public TMP_Text textoScoreTotal;     

    void Update()
    {
        ActualizarEstamina();
        ActualizarTextos();
    }

    void ActualizarEstamina()
    {
        float estamina = movimiento.ObtenerEstaminaNormalizada();
        rellenoEstamina.fillAmount = estamina;

        if (grupoEstamina != null)
        {
            float alphaObjetivo = (estamina < 1f) ? 1f : 0f;
            grupoEstamina.alpha = Mathf.MoveTowards(
                grupoEstamina.alpha,
                alphaObjetivo,
                velocidadFundido * Time.deltaTime
            );
        }
    }

    void ActualizarTextos()
    {
        if (recolector != null)
        {
            textoCargando.text = "Recolectados: " + recolector.objetosCargando;
            textoDepositado.text = "Entregados: " + recolector.depositadosEstaSesion;
        }

        if (DataManager.Instancia != null)
            textoScoreTotal.text = "Total: " + DataManager.Instancia.ObtenerScoreTotal();
    }
}