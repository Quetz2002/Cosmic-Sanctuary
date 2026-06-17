using UnityEngine;
using System.Collections;

// I create a custom class to easily manage multiple turrets in the Unity Inspector
[System.Serializable]
public class ShipTurret
{
    public Transform turretHead;
    public Transform muzzlePoint;
    public LineRenderer laserLine;
}

public class ShuttleShooter : MonoBehaviour
{
    [Header("Turret Setup")]
    public ShipTurret[] turrets; // An array to hold as many turrets as we want
    public float turretTurnSpeed = 15f;

    [Header("Shooting Properties")]
    public float fireRange = 200f;
    public LayerMask meteorLayer;
    public float fireRate = 0.2f; // Slightly faster to make alternating fire feel good

    [Header("Laser Visual FX")]
    public float laserDuration = 0.05f;

    private Camera shuttleCamera;
    private float nextFireTime = 0f;
    private Vector3 currentAimPoint;

    // I use this to track which turret should fire next (Left or Right)
    private int currentTurretIndex = 0;

    private void Start()
    {
        shuttleCamera = Camera.main;

        // Disable all lasers at the start
        foreach (ShipTurret turret in turrets)
        {
            if (turret.laserLine != null) turret.laserLine.enabled = false;
        }
    }

    private void Update()
    {
        AimTurrets();

        // FIXED: I changed GetMouseButtonDown to GetMouseButton(0) so the player can hold the click to autofire
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            ShootLaser();
        }
    }

    private void AimTurrets()
    {
        // 1. Find the target point from the center of the camera
        Ray screenRay = shuttleCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(screenRay, out RaycastHit hit, fireRange, meteorLayer))
        {
            currentAimPoint = hit.point;
        }
        else
        {
            currentAimPoint = screenRay.GetPoint(fireRange);
        }

        // 2. Make EVERY turret look at that exact target point
        foreach (ShipTurret turret in turrets)
        {
            if (turret.turretHead != null)
            {
                Vector3 aimDirection = currentAimPoint - turret.turretHead.position;

                if (aimDirection.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(aimDirection);
                    turret.turretHead.rotation = Quaternion.Slerp(turret.turretHead.rotation, targetRotation, Time.deltaTime * turretTurnSpeed);
                }
            }
        }
    }

    private void ShootLaser()
    {
        if (turrets.Length == 0) return;

        // I get the turret that is supposed to fire this turn
        ShipTurret activeTurret = turrets[currentTurretIndex];

        // I advance the index, looping back to 0 if it reaches the end of the array (Alternating fire)
        currentTurretIndex = (currentTurretIndex + 1) % turrets.Length;

        if (activeTurret.muzzlePoint == null || activeTurret.laserLine == null) return;

        // Calculate trajectory
        Vector3 shootDirection = (currentAimPoint - activeTurret.muzzlePoint.position).normalized;
        Vector3 laserEndPoint = activeTurret.muzzlePoint.position + (shootDirection * fireRange);

        bool hitSomething = Physics.Raycast(activeTurret.muzzlePoint.position, shootDirection, out RaycastHit hit, fireRange, meteorLayer);

        if (hitSomething)
        {
            laserEndPoint = hit.point;
        }

        // I trigger the visual effect exclusively on the active turret's line renderer
        StartCoroutine(RenderLaserBeam(activeTurret.laserLine, activeTurret.muzzlePoint.position, laserEndPoint));

        // Handle impact
        if (hitSomething)
        {
            MeteorBehavior meteor = hit.collider.GetComponentInParent<MeteorBehavior>();
            if (meteor != null)
            {
                meteor.DestroyMeteor();
            }
        }
    }

    private IEnumerator RenderLaserBeam(LineRenderer line, Vector3 start, Vector3 end)
    {
        line.enabled = true;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        yield return new WaitForSeconds(laserDuration);
        line.enabled = false;
    }
}