using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] prefabs;

    public int cantidad;
    private int totalInicial = 0;

    public BoxCollider zonaSpawn;

    public float alturaRaycast;
    public float distanciaRaycast;
    public LayerMask capaSuelo;

    public float distanciaMinima = 2f;

    private List<GameObject> pool = new List<GameObject>();
    private List<Vector3> posicionesUsadas = new List<Vector3>();

    void Start()
    {
        GenerarTodos();
    }

    void GenerarTodos()
    {
        int generados = 0;
        int intentos = 0;
        int maxIntentos = cantidad * 15;

        while (generados < cantidad && intentos < maxIntentos)
        {
            intentos++;

            Vector3 posicion;
            if (ObtenerPuntoEnSuelo(out posicion))
            {
                if (!MuyCerca(posicion))
                {
                    GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
                    GameObject obj = Instantiate(prefab, posicion, Quaternion.identity);

                    pool.Add(obj);
                    posicionesUsadas.Add(posicion);
                    generados++;
                }
            }
        }

        if (generados < cantidad)
        {
            totalInicial = pool.Count;
        }
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
            resultado = hit.point;
            return true;
        }

        return false;
    }
    bool MuyCerca(Vector3 punto)
    {
        foreach (Vector3 usada in posicionesUsadas)
        {
            if (Vector3.Distance(punto, usada) < distanciaMinima)
                return true;   
        }
        return false;         
    }

    public void RetirarDePool(GameObject obj)
    {
        pool.Remove(obj);
        obj.SetActive(false);


        if (pool.Count == 0)
        {

        }
    }

    public int ObjetosRestantes()
    {
        return pool.Count;
    }
    public int TotalInicial()
    {
        return totalInicial;
    }
    public float ProgresoRecoleccion()
    {
        if (totalInicial == 0)
            return 0f;

        int recolectados = totalInicial - pool.Count;
        return (float)recolectados / totalInicial;
    }
}
