using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Configuração do jogo")]
    public int maxLives = 4;
    public int currentLives = 4;

    public int totalPasteisSupremos = 6;
    public int collectedPasteisSupremos = 0;

    public string victorySceneName = "Vitoria";
    public string defeatSceneName = "Derrota";
    public string firstGameplaySceneName = "K1";

    private bool gameEnded = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ResetRun();
    }

    public void ResetRun()
    {
        currentLives = maxLives;
        collectedPasteisSupremos = 0;
        gameEnded = false;
        UpdateUI();
    }

    public void RestoreFullLives()
    {
        currentLives = maxLives;
        UpdateUI();
    }

    public void DamagePlayer(int damage)
    {
        if (gameEnded) return;

        currentLives -= damage;
        currentLives = Mathf.Clamp(currentLives, 0, maxLives);

        UpdateUI();

        if (currentLives <= 0)
        {
            LoseGame();
        }
    }

    public void CollectPastelSupremo()
    {
        if (gameEnded) return;

        collectedPasteisSupremos++;
        collectedPasteisSupremos = Mathf.Clamp(collectedPasteisSupremos, 0, totalPasteisSupremos);

        UpdateUI();

        if (collectedPasteisSupremos >= totalPasteisSupremos)
        {
            WinGame();
        }
    }

    public void WinGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        SceneManager.LoadScene(victorySceneName);
    }

    public void LoseGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        SceneManager.LoadScene(defeatSceneName);
    }

    public void RestartFromBeginning()
    {
        ResetRun();
        SceneManager.LoadScene(firstGameplaySceneName);
    }

    public void UpdateUI()
    {
        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.UpdateLives(currentLives, maxLives);
            GameUIManager.Instance.UpdatePastelCount(collectedPasteisSupremos, totalPasteisSupremos);
        }
    }
}