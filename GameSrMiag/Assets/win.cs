using UnityEngine;

public class win : MonoBehaviour
{
    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayVictory();
        }
    }
}