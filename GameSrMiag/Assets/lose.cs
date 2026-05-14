using UnityEngine;

public class lose : MonoBehaviour
{
    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDefeat();
        }
    }
}