using UnityEngine;
using UnityEngine.SceneManagement;

public class Elevator : MonoBehaviour
{
    [Header("Cenas")]
    public string upperFloorScene = "K1";
    public string lowerFloorScene = "K0";

    [Header("Permissões")]
    public bool canGoUp = true;
    public bool canGoDown = true;

    [Header("Ícones no mundo")]
    public GameObject upIcon;
    public GameObject downIcon;

    private bool playerInside = false;

    void Update()
    {
        if (!playerInside) return;

        float vertical = Input.GetAxisRaw("Vertical");

        if (vertical > 0 && canGoUp)
        {
            SceneManager.LoadScene(upperFloorScene);
        }
        else if (vertical < 0 && canGoDown)
        {
            SceneManager.LoadScene(lowerFloorScene);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            if (upIcon != null)
                upIcon.SetActive(canGoUp);

            if (downIcon != null)
                downIcon.SetActive(canGoDown);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            if (upIcon != null)
                upIcon.SetActive(false);

            if (downIcon != null)
                downIcon.SetActive(false);
        }
    }
}