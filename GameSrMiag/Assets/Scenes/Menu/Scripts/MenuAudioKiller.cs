using UnityEngine;

public class MenuAudioKiller : MonoBehaviour
{
    public void KillMenuMusic()
    {
        MenuAudioManager manager = FindObjectOfType<MenuAudioManager>();
        if (manager != null)
        {
            manager.StopAndDestroy();
        }
    }
}