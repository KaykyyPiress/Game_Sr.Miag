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
            if (!GameProgress.IsPastelCollected(uniqueId))
            {
                GameProgress.MarkPastelCollected(uniqueId);

                player.CollectPastelSupremo(
                    projectileSpeedMultiplier,
                    shootCooldownMultiplier,
                    buffDuration
                );
            }

            if(AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCollectPastel();
            }
            Destroy(gameObject);
        }
    }
}