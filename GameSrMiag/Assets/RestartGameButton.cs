using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGameButton : MonoBehaviour
{
    public string firstSceneName = "Fase1";
    public int maxLives = 4;
    public int totalPasteisSupremos = 6;

    public void RestartGame()
    {
        GameProgress.ResetProgress(maxLives, totalPasteisSupremos);
        SceneManager.LoadScene(firstSceneName);
    }
}