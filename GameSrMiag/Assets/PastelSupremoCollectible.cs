using UnityEngine;

public class PastelSupremoCollectible : MonoBehaviour
{
    [Header("Identificação única")]
    public string uniqueId;

    [Header("Buff")]
    public float projectileSpeedMultiplier = 1.5f;
    public float shootCooldownMultiplier = 0.7f;
    public float buffDuration = 5f;

    void Start()
    {
        if (GameProgress.IsPastelCollected(uniqueId))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null)
        {
            Debug.Log("Pastel: Player entrou no trigger");

            if (!GameProgress.IsPastelCollected(uniqueId))
            {
                Debug.Log("Pastel: ainda não coletado, aplicando buff e marcando progresso");

                GameProgress.MarkPastelCollected(uniqueId);

                player.CollectPastelSupremo(
                    projectileSpeedMultiplier,
                    shootCooldownMultiplier,
                    buffDuration
                );
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCollectPastel();
            }

            Destroy(gameObject);
        }
    }
}