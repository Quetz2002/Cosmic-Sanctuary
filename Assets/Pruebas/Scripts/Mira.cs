using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mira : MonoBehaviour
{
    public Camera camara;
    public Image imagenMira;

    public float distanciaMaxima = 3f;

    public Sprite spriteNormal;   
    public Sprite spriteResaltado;

    private bool estabaResaltado = false;

    void Awake()
    {
        if (camara == null)
            camara = Camera.main;

        if (imagenMira == null)
            imagenMira = GetComponent<Image>();

        if (imagenMira != null)
            imagenMira.sprite = spriteNormal;
    }

    void Update()
    {
        bool apuntando = ApuntandoARecolectable();

        if (apuntando != estabaResaltado)
        {
            if (imagenMira != null)
                imagenMira.sprite = apuntando ? spriteResaltado : spriteNormal;
            estabaResaltado = apuntando;
        }
    }

    bool ApuntandoARecolectable()
    {
        if (camara == null)
            camara = Camera.main;
        if (camara == null) return false;

        Ray ray = new Ray(camara.transform.position, camara.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaMaxima))
        {
            return hit.collider.CompareTag("Recolectable");
        }

        return false;
    }
}