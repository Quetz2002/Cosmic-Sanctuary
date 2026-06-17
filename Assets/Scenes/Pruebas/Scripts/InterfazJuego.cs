using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InterfazJuego : MonoBehaviour
{
    public Movimiento movimiento;      
    public Recolectar recolector;       
    public Spawner generador; 

    public Image rellenoEstamina;       

    public TMP_Text textoCargando;
    public TMP_Text textoEntregado;
    public TMP_Text textoRestantes;

    void Update()
    {
        ActualizarEstamina();
        ActualizarTextos();
    }

    void ActualizarEstamina()
    {
        if (movimiento != null && rellenoEstamina != null)
            rellenoEstamina.fillAmount = movimiento.ObtenerEstaminaNormalizada();
    }

    void ActualizarTextos()
    {
        if (recolector != null)
        {
            textoCargando.text = "Cargando: " + recolector.objetosCargando;
            textoEntregado.text = "Entregados: " + recolector.puntuacionTotal;
        }

        if (generador != null)
            textoRestantes.text = "Restantes: " + generador.ObjetosRestantes();
    }
}