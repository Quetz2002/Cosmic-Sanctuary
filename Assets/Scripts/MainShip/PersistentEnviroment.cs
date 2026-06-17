using UnityEngine;

public class PersistentEnvironment : MonoBehaviour
{
    private static PersistentEnvironment instance;

    private void Awake()
    {
        // I protect Giovanni's procedural planets from being destroyed when loading the Shuttle
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}