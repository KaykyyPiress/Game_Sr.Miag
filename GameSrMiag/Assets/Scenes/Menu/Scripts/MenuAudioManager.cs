using UnityEngine;

public class MenuAudioManager : MonoBehaviour
{
    private static MenuAudioManager instance;

    private AudioSource audioSource;

    private void Awake()
    {
        // Se já existe um MenuAudioManager, destrói este novo
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Define este como o único
        instance = this;

        // Faz este objeto não ser destruído ao trocar de cena
        DontDestroyOnLoad(gameObject);

        // Pega o AudioSource
        audioSource = GetComponent<AudioSource>();

        // Se não estiver tocando, toca
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void RestartMusic()
    {
        if (audioSource == null) return;

        audioSource.Stop();
        audioSource.Play();
    }

    public void StopAndDestroy()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (instance == this)
        {
            instance = null;
        }

        Destroy(gameObject);
    }
}