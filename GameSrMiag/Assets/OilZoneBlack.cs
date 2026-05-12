using UnityEngine;

public class OilZoneBlack : MonoBehaviour
{
    [Header("Efeito do óleo")]
    [Range(0.05f, 1f)]
    public float slowMultiplier = 0.2f;

    [Range(0.01f, 1f)]
    public float movementResponsiveness = 0.03f;

    [Range(0.1f, 1f)]
    public float jumpMultiplier = 0.5f;

    public float damagePerSecond = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null)
        {
            player.ApplyOilEffect(
                slowMultiplier,
                movementResponsiveness,
                jumpMultiplier,
                damagePerSecond
            );
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null)
        {
            player.ResetOilEffect();
        }
    }
}