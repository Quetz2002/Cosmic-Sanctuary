using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public GameObject door;
    public float riseAmount = 3f;
    public float speed = 2f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool opening = false;

    void Start()
    {
        closedPosition = door.transform.position;
        openPosition = closedPosition + Vector3.up * riseAmount;
    }

    void Update()
    {
        if (opening)
        {
            door.transform.position = Vector3.MoveTowards(
                door.transform.position,
                openPosition,
                speed * Time.deltaTime
            );

            if (door.transform.position == openPosition)
            {
                opening = false;
            }
        }
    }

    public void Open()
    {
        opening = true;
    }

    public void Close()
    {
        opening = false;
        door.transform.position = closedPosition;
    }
}
