using UnityEngine;
using System.Collections;

public class MeteorBehavior : MonoBehaviour
{
    [Header("Movement Mechanics")]
    public float minSpeed = 15f;
    public float maxSpeed = 30f;
    public Vector3 travelDirection = new Vector3(0f, 0f, -1f);

    [Header("Destruction Effects")]
    public GameObject explosionParticlePrefab;

    private float finalSpeed;
    private Coroutine deactivateCoroutine;

    private void OnEnable()
    {
        finalSpeed = Random.Range(minSpeed, maxSpeed);
        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
        }
        deactivateCoroutine = StartCoroutine(DeactivateAfterDelay(8f));
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.Translate(travelDirection.normalized * finalSpeed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.up * Time.deltaTime * 30f, Space.Self);
    }

    public void DestroyMeteor()
    {
        if (explosionParticlePrefab != null)
        {
            GameObject fx = Instantiate(explosionParticlePrefab, transform.position, Quaternion.identity);
            Destroy(fx, 2f);
        }

        // FIXED: If we play this scene directly from the editor for fast testing, the GameManager won't exist.
        // I add a conditional check to safely add currency only if the Instance exists, avoiding the game-breaking NullReferenceException.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.cosmicMaterials += 1;
        }
        else
        {
            // I leave a warning in the console just to remind myself why materials aren't increasing during isolated tests
            Debug.LogWarning("GameManager instance not found. Materials were not added, but the meteor will still be destroyed safely.");
        }

        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
            deactivateCoroutine = null;
        }

        // I instantly deactivate the meteor instance from active processing loops
        gameObject.SetActive(false);
    }
}