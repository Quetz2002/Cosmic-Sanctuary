using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour
{
    public float targetY = 195.5f;
    public float speed = 4f;
    public float delayBeforeGoingUp = 4f;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private bool playerInside = false;
    private bool movingUp = false;
    private bool movingDown = false;
    private Coroutine moveRoutine;

    void Start()
    {
        startPosition = transform.position;
        targetPosition = new Vector3(transform.position.x, targetY, transform.position.z);
    }

    void Update()
    {
        if (movingUp)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime
            );

            if (transform.position == targetPosition)
            {
                movingUp = false;
            }
        }

        if (movingDown)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                startPosition,
                speed * Time.deltaTime
            );

            if (transform.position == startPosition)
            {
                movingDown = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            moveRoutine = StartCoroutine(WaitAndGoUp());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            movingUp = false;
            movingDown = true;
        }
    }

    IEnumerator WaitAndGoUp()
    {
        yield return new WaitForSeconds(delayBeforeGoingUp);

        if (playerInside)
        {
            movingDown = false;
            movingUp = true;
        }
    }
}
