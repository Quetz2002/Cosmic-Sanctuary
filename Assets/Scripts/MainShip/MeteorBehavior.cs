using UnityEngine;

public class MeteorBehavior : MonoBehaviour
{
    [Header("Movement Mechanics")]
    public float minSpeed = 15f;
    public float maxSpeed = 30f;
    public Vector3 travelDirection = new Vector3(0f, 0f, -1f); // Flying towards the ship window

    [Header("Destruction Effects")]
    public GameObject explosionParticlePrefab; // Optional clean particle system prefab

    private float finalSpeed;

    private void Start()
    {
        // I randomize the individual meteor's flight speed for variety
        finalSpeed = Random.Range(minSpeed, maxSpeed);

        // I clean up the object automatically after 8 seconds if the player misses it
        Destroy(gameObject, 8f);
    }

    private void Update()
    {
        // I slide the meteor along the travel trajectory matrix
        transform.Translate(travelDirection.normalized * finalSpeed * Time.deltaTime, Space.World);

        // I tumble the rock constantly to make the space environment look more cinematic
        transform.Rotate(Vector3.up * Time.deltaTime * 30f, Space.Self);
    }

    public void DestroyMeteor()
    {
        // I instantiate particle dynamics at the impact site if an asset is assigned
        if (explosionParticlePrefab != null)
        {
            GameObject fx = Instantiate(explosionParticlePrefab, transform.position, Quaternion.identity);
            Destroy(fx, 2f); // Clear FX from memory
        }

        // Add currency reward directly to our GameManager data layers upon clean shot destruction
        GameManager.Instance.cosmicMaterials += 1;

        // I instantly wipe the meteor instance from active processing loops
        Destroy(gameObject);
    }
}