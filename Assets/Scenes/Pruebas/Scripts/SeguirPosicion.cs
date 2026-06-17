using UnityEngine;

public class SeguirPosicion : MonoBehaviour
{
    public Transform objetivo;         
    public Vector3 desfase = new Vector3(0f, 5f, 0f); 

    void LateUpdate()
    {
        if (objetivo != null)
        {
            transform.position = objetivo.position + desfase;
        }
    }
}
