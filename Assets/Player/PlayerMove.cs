using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5f;
    private float normalSpeed = 5f;
    private float sprintSpeed = 8f;
    bool hasStamina = false;
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(x, 0f, z).normalized;
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
        Sprinting();
    }

    void Sprinting()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !hasStamina)
            {
               
                speed = sprintSpeed; // Increase speed when sprinting
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
            speed = normalSpeed; // Reset speed when not sprinting
        }
    }
}
