using UnityEngine;

public class MainMenuAudioController : MonoBehaviour
{
    [SerializeField] private AudioSource menuMusicSource;

    private void Start()
    {
        // Para TUDO que estiver tocando
        AudioSource[] allSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource src in allSources)
        {
            // Não para a própria música do menu (se for o mesmo AudioSource)
            if (menuMusicSource != null && src == menuMusicSource)
                continue;

            src.Stop();
        }

        // Garante que a música do menu toque
        if (menuMusicSource != null)
        {
            if (menuMusicSource.isPlaying)
            {
                menuMusicSource.Stop();
            }

            menuMusicSource.loop = true;
            menuMusicSource.Play();
        }
    }
}