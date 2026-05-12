using UnityEngine;

public class EndScreenMusicController : MonoBehaviour
{
    [SerializeField] private AudioClip endScreenMusic;
    [SerializeField] private bool loopMusic = true; 

    private AudioSource audioSource;

    private void Start()
    {
        // Para todos os sons que estiverem tocando
        AudioSource[] allSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource src in allSources)
        {
            src.Stop();
        }

        // Cria ou reutiliza um AudioSource neste objeto
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = endScreenMusic;
        audioSource.loop = loopMusic;      
        audioSource.playOnAwake = false;

        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
    }
}