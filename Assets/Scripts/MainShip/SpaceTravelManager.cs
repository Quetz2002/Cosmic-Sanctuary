using System.Collections;
using UnityEngine;

public class SpaceTravelManager : MonoBehaviour
{
    [Header("Scene Planets")]
    public GameObject[] scenePlanets; // The actual planet GameObjects currently in the scene

    [Header("Animation Anchors")]
    public Transform spawnPointFar;        // Empty object placed deep in space
    public Transform destinationPointClose; // Empty object placed right in front of the window

    [Header("Travel Settings")]
    public float travelDuration = 6f;
    public AnimationCurve travelCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private void Start()
    {
        // At the start, I make sure all planets (even if they are at Y=50) are hidden
        foreach (GameObject planet in scenePlanets)
        {
            if (planet != null) planet.SetActive(false);
        }
    }

    public void StartTravelAnimation(int planetIndex)
    {
        if (planetIndex >= 0 && planetIndex < scenePlanets.Length)
        {
            StopAllCoroutines();
            StartCoroutine(AnimatePlanetApproach(planetIndex));
        }
    }

    private IEnumerator AnimatePlanetApproach(int index)
    {
        // I double check that all planets are turned off first
        foreach (GameObject p in scenePlanets)
        {
            if (p != null) p.SetActive(false);
        }

        GameObject activePlanet = scenePlanets[index];

        if (activePlanet != null)
        {
            // I snap the planet from Y=50 directly to our far spawn point and turn it on
            activePlanet.transform.position = spawnPointFar.position;
            activePlanet.SetActive(true);

            float elapsed = 0f;

            // I smoothly animate its position towards the window
            while (elapsed < travelDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / travelDuration;
                float curveValue = travelCurve.Evaluate(t); // Adds smooth acceleration and deceleration

                activePlanet.transform.position = Vector3.Lerp(spawnPointFar.position, destinationPointClose.position, curveValue);

                // I add a very slow, majestic rotation to make the planet feel alive
                activePlanet.transform.Rotate(Vector3.up * Time.deltaTime * 2f, Space.World);

                yield return null;
            }

            // I snap it to the exact final position just to be perfectly precise
            activePlanet.transform.position = destinationPointClose.position;
        }
    }
}