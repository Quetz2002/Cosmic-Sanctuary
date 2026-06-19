using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    public GameObject trashBlack;
    public GameObject trashGreen;
    public GameObject trashYellow;
    public void SpawnTrash(int black, int green, int yellow)
    {
        for (int i = 0; i < black; i++)
        {
            Instantiate(trashBlack, transform.position, Quaternion.identity);
        }
        for (int i = 0; i < green; i++)
        {
            Instantiate(trashGreen, transform.position, Quaternion.identity);
        }
        for (int i = 0; i < yellow; i++)
        {
            Instantiate(trashYellow, transform.position, Quaternion.identity);
        }
    }

    void Start()
    {
        SpawnTrash(5, 3, 2); // Example: Spawns 5 black, 3 green, and 2 yellow trash items
    }
}
