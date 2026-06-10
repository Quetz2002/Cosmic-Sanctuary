using UnityEngine;
using System.Collections;

public class ShuttleShooter : MonoBehaviour
{
    [Header("Shooting Properties")]
    public float fireRange = 100f;
    public LayerMask meteorLayer;
    public float fireRate = 0.25f;

    [Header("Laser Visual FX")]
    public LineRenderer laserLine;
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
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            ShootLaser();
        }
    }

    private void ShootLaser()
    {
        Ray ray = shuttleCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 laserEndPoint = ray.origin + (ray.direction * fireRange);

        // I store the raycast hit status in a boolean to separate physics detection from game loop logic
        bool hitSomething = Physics.Raycast(ray, out RaycastHit hit, fireRange, meteorLayer);

        if (hitSomething)
        {
            laserEndPoint = hit.point;
        }

        // FIXED: I trigger the visual laser beam rendering BEFORE processing any target destruction.
        // This guarantees the laser always renders on screen immediately, even if a script crash happens later.
        if (laserLine != null)
        {
            StopAllCoroutines();
            StartCoroutine(RenderLaserBeam(ray.origin - new Vector3(0f, 0.2f, 0f), laserEndPoint));
        }

        // Now that the visual effect is safely fired, I process the gameplay mechanics of destroying the rock
        if (hitSomething)
        {
            MeteorBehavior meteor = hit.collider.GetComponentInParent<MeteorBehavior>();
            if (meteor != null)
            {
                meteor.DestroyMeteor();
            }
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