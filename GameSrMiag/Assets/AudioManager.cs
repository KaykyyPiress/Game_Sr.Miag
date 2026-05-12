using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Músicas")]
    public AudioClip defaultMusic;
    public AudioClip bossMusic;
    public string bossSceneName = "Boss";

    [Header("Efeitos")]
    public AudioClip playerShootSfx;
    public AudioClip bossAttackSfx;
    public AudioClip collectPastelSfx;
    public AudioClip bossHurtSfx;
    public AudioClip bossSpawnEnemySfx;
    public AudioClip bossDeathSfx;
    public AudioClip playerHurtSfx;
    public AudioClip enemyHurtSfx;


    [Header("Config")]
    public string[] menuScenes = { "Menu", "Historia", "Creditos", "Como Jogar" }; 

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        UpdateMusicForScene(SceneManager.GetActiveScene().name);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateMusicForScene(scene.name);
    }

    void UpdateMusicForScene(string sceneName)
    {
        if (musicSource == null) return;

        if (IsMenuScene(sceneName))
            return;

        AudioClip targetClip = sceneName == bossSceneName ? bossMusic : defaultMusic;
        if (targetClip == null) return;

        if (musicSource.clip == targetClip && musicSource.isPlaying)
            return;

        musicSource.Stop();
        musicSource.clip = targetClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayPlayerShoot()
    {
        PlaySFX(playerShootSfx, 1f);
    }

    public void PlayBossAttack()
    {
        PlaySFX(bossAttackSfx, 1f);
    }

    public void PlayCollectPastel()
    {
        PlaySFX(collectPastelSfx, 1f);
    }

    public void PlayBossHurt()
    {
        PlaySFX(bossHurtSfx, 1f);
    }

    public void PlayBossSpawnEnemy()
    {
        PlaySFX(bossSpawnEnemySfx, 1f);
    }

    public void PlayBossDeath()
    {
        PlaySFX(bossDeathSfx, 1f);
    }

    public void PlayPlayerHurt()
    {
        PlaySFX(playerHurtSfx, 1f);
    }

    public void PlayEnemyHurt()
    {
        PlaySFX(enemyHurtSfx, 1f);
    }

    bool IsMenuScene(string sceneName)
    {
        if (menuScenes == null) return false;

        foreach (var menuScene in menuScenes)
        {
            if (!string.IsNullOrEmpty(menuScene) && sceneName == menuScene)
                return true;
        }

        return false;
    }
}