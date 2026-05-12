using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string firstSceneName = "Recep";
    public string storySceneName = "Historia";
    public string howToPlaySceneName = "ComoJogar";
    public string creditosSceneName = "Creditos";

    public GameObject menuRoot;

    public void StartGame()
    {
        if (menuRoot != null)
        {
            Destroy(menuRoot);
        }

        SceneManager.LoadScene(firstSceneName);
    }

    public void GoToStory()
    {
        SceneManager.LoadScene(storySceneName);
    }

    public void GoToHowToPlay()
    {
        SceneManager.LoadScene(howToPlaySceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Creditos()
    {
        SceneManager.LoadScene(creditosSceneName);
    }
}