using UnityEngine;

public class TextoFlotante : MonoBehaviour
{
    private Camera camara;

    void Start()
    {
        camara = Camera.main;
        Ocultar(); 
    }

    void LateUpdate()
    {
        if (camara != null)
        {
            transform.rotation = Quaternion.LookRotation(
                transform.position - camara.transform.position
            );
        }
    }

    public void Mostrar()
    {
        gameObject.SetActive(true);
    }

    public void Ocultar()
    {
        gameObject.SetActive(false);
    }
}
