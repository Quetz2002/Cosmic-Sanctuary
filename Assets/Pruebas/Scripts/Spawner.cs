using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public GameObject[] prefabs;

    public int cantidad;

    public BoxCollider zonaSpawn;

    public float alturaRaycast;
    public float distanciaRaycast;
    public LayerMask capaSuelo;
    public float offsetAltura = 0.05f;

    public float distanciaMinima = 3f;

    private List<GameObject> pool = new List<GameObject>();

    public float retrasoReaparicion = 2f;

    void Start()
    {
        GenerarIniciales();
    }

    void GenerarIniciales()
    {
        int generados = 0;
        int intentos = 0;
        int maxIntentos = cantidad * 15;

        while (generados < cantidad && intentos < maxIntentos)
        {
            intentos++;

            Vector3 posicion;
            if (ObtenerPosicionLibre(out posicion))
            {
                GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
                GameObject obj = Instantiate(prefab, posicion, Quaternion.identity);

                pool.Add(obj);
                generados++;
            }
        }
    }

    public void Reaparecer(GameObject obj)
    {
        StartCoroutine(ReaparecerConRetraso(obj));
    }

    IEnumerator ReaparecerConRetraso(GameObject obj)
    {
        yield return new WaitForSeconds(retrasoReaparicion);

        Vector3 posicion;
        if (ObtenerPosicionLibre(out posicion))
        {
            obj.transform.position = posicion;
            obj.SetActive(true);
        }
        else
        {
            obj.SetActive(true);
        }
    }

    bool ObtenerPosicionLibre(out Vector3 resultado)
    {
        resultado = Vector3.zero;
        int intentos = 0;

        while (intentos < 20)
        {
            intentos++;

            if (ObtenerPuntoEnSuelo(out resultado))
            {
                if (!MuyCerca(resultado))
                    return true;
            }
        }

        return false; 
    }

    bool ObtenerPuntoEnSuelo(out Vector3 resultado)
    {
        resultado = Vector3.zero;

        Bounds limites = zonaSpawn.bounds;
        float x = Random.Range(limites.min.x, limites.max.x);
        float z = Random.Range(limites.min.z, limites.max.z);

        Vector3 origen = new Vector3(x, limites.max.y + alturaRaycast, z);
        Ray ray = new Ray(origen, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaRaycast + alturaRaycast, capaSuelo))
        {
            resultado = hit.point + Vector3.up* offsetAltura; ;
            return true;
        }

        return false;
    }

    bool MuyCerca(Vector3 punto)
    {
        foreach (GameObject obj in pool)
        {
            if (obj != null && obj.activeSelf)
            {
                if (Vector3.Distance(punto, obj.transform.position) < distanciaMinima)
                    return true;
            }
        }
        return false;
    }
}