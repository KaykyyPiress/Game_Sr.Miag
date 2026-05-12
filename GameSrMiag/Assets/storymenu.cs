using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryMenu : MonoBehaviour
{
    public string menuSceneName = "Menu";

    public void BackToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}