using UnityEngine;

public class BurnTrash : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("canPickUP"))
        {
            //add points to the player
            Destroy(collision.gameObject);
        }
    }
}
