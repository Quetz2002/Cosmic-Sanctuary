using UnityEngine;
using System.Collections;

public class ShuttleShooter : MonoBehaviour
{
    [Header("Shooting Properties")]
    public float fireRange = 100f;
    public LayerMask meteorLayer; // Layer exclusively for shootable rocks
    public float fireRate = 0.25f;

    [Header("Laser Visual FX")]
    public LineRenderer laserLine; // Visual representation of the energy beam
    public float laserDuration = 0.05f;

    private Camera shuttleCamera;
    private float nextFireTime = 0f;

    private void Start()
    {
        shuttleCamera = Camera.main;
        if (laserLine != null) laserLine.enabled = false;
    }

    private void Update()
    {
        // I check if the player pulls the trigger (LMB) and the weapon is cooled down
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            ShootLaser();
        }
    }

    private void ShootLaser()
    {
        // I cast a ray directly forward from the center of the screen
        Ray ray = shuttleCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 laserEndPoint = ray.origin + (ray.direction * fireRange);

        if (Physics.Raycast(ray, out RaycastHit hit, fireRange, meteorLayer))
        {
            laserEndPoint = hit.point;

            // I find the meteor behavior component on the object we hit and obliterate it
            MeteorBehavior meteor = hit.collider.GetComponentInParent<MeteorBehavior>();
            if (meteor != null)
            {
                meteor.DestroyMeteor();
            }
        }

        // I trigger the visual laser effect sequence
        if (laserLine != null)
        {
            StopAllCoroutines();
            StartCoroutine(RenderLaserBeam(ray.origin - new Vector3(0f, 0.2f, 0f), laserEndPoint));
        }
    }

    private IEnumerator RenderLaserBeam(Vector3 start, Vector3 end)
    {
        laserLine.enabled = true;
        laserLine.SetPosition(0, start);
        laserLine.SetPosition(1, end);
        yield return new WaitForSeconds(laserDuration);
        laserLine.enabled = false;
    }
}