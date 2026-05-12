using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [Header("Vidas")]
    public Image[] lifeImages;
    public Color fullLifeColor = Color.white;
    public Color emptyLifeColor = new Color(1f, 1f, 1f, 0.25f);

    [Header("Pastel Supremo")]
    public TMP_Text pastelCountText;
    public TMP_Text buffTimerText;

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

    public void UpdateLives(int currentLives, int maxLives)
    {
        if (lifeImages == null) return;

        for (int i = 0; i < lifeImages.Length; i++)
        {
            if (lifeImages[i] == null) continue;

            lifeImages[i].color = i < currentLives ? fullLifeColor : emptyLifeColor;
        }
    }

    public void UpdatePastelCount(int collected, int total)
    {
        if (pastelCountText != null)
        {
            pastelCountText.text = collected + "/" + total;
        }
    }

    public void UpdateBuffTimer(float timeRemaining)
    {
        if (buffTimerText == null) return;

        if (timeRemaining > 0f)
        {
            buffTimerText.gameObject.SetActive(true);
            buffTimerText.text = "Buff: " + timeRemaining.ToString("F1") + "s";
        }
        else
        {
            buffTimerText.gameObject.SetActive(false);
        }
    }
}