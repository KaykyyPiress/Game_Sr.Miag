using UnityEngine;

public class PersistentCamera : MonoBehaviour
{
    private static PersistentCamera instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}