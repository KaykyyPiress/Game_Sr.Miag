using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource loopSfxSource;

    [Header("Músicas")]
    public AudioClip menuMusic;
    public AudioClip defaultMusic;
    public AudioClip bossMusic;

    [Header("Dano")]
    public AudioClip enemyDamageSfx;
    public AudioClip bossDamageSfx;

    public string menuSceneName = "Menu";
    public string bossSceneName = "Boss";

    [Header("Efeitos Sonoros")]
    public AudioClip playerShootSfx;
    public AudioClip enemyShootSfx;
    public AudioClip playerDamageSfx;
    public AudioClip jumpSfx;
    public AudioClip footstepSfx;
    public AudioClip collectPastelSfx;
    public AudioClip swordAttackSfx;
    public AudioClip bossFootstepSfx;
    public AudioClip victorySfx;
    public AudioClip defeatSfx;

    private bool isFootstepPlaying = false;

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
        PreloadSFX();
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
        StopFootstep();
    }

    void PreloadSFX()
    {
        AudioClip[] clips =
        {
            enemyDamageSfx,
            bossDamageSfx,
            playerShootSfx,
            enemyShootSfx,
            playerDamageSfx,
            jumpSfx,
            footstepSfx,
            collectPastelSfx,
            swordAttackSfx,
            bossFootstepSfx,
            victorySfx,
            defeatSfx
        };

        foreach (AudioClip clip in clips)
        {
            if (clip != null)
            {
                clip.LoadAudioData();
            }
        }
    }

    void UpdateMusicForScene(string sceneName)
    {
        if (musicSource == null) return;

        AudioClip targetClip = defaultMusic;

        if (sceneName == menuSceneName)
        {
            targetClip = menuMusic;
        }
        else if (sceneName == bossSceneName)
        {
            targetClip = bossMusic;
        }

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

        if (clip.loadState != AudioDataLoadState.Loaded)
        {
            clip.LoadAudioData();
        }

        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayPlayerShoot()
    {
        PlaySFX(playerShootSfx, 1f);
    }

    public void PlayEnemyShoot()
    {
        PlaySFX(enemyShootSfx, 1f);
    }

    public void PlayPlayerDamage()
    {
        PlaySFX(playerDamageSfx, 1f);
    }

    public void PlayJump()
    {
        PlaySFX(jumpSfx, 1f);
    }

    public void PlaySwordAttack()
    {
        PlaySFX(swordAttackSfx, 1f);
    }

    public void PlayCollectPastel()
    {
        PlaySFX(collectPastelSfx, 1f);
    }

    public void StartFootstep()
    {
        if (loopSfxSource == null || footstepSfx == null) return;
        if (isFootstepPlaying) return;

        if (footstepSfx.loadState != AudioDataLoadState.Loaded)
        {
            footstepSfx.LoadAudioData();
        }

        loopSfxSource.clip = footstepSfx;
        loopSfxSource.loop = true;
        loopSfxSource.Play();
        isFootstepPlaying = true;
    }

    public void StopFootstep()
    {
        if (loopSfxSource == null) return;

        loopSfxSource.Stop();
        isFootstepPlaying = false;
    }

    public void PlayBossFootstep()
    {
        PlaySFX(bossFootstepSfx, 1f);
    }

    public void PlayVictory()
    {
        PlaySFX(victorySfx, 1f);
    }

    public void PlayDefeat()
    {
        PlaySFX(defeatSfx, 1f);
    }

    public void PlayEnemyDamage()
    {
        PlaySFX(enemyDamageSfx, 1f);
    }

    public void PlayBossDamage()
    {
        PlaySFX(bossDamageSfx, 1f);
    }
}