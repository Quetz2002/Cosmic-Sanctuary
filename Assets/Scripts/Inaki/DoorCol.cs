using UnityEngine;

public class DoorCol : MonoBehaviour
{
    public OpenDoor door1;
    public OpenDoor door2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door1.Open();
            door2.Open();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door1.Close();
            door2.Close();
        }
    }
}
