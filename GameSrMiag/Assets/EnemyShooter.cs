using UnityEngine;
using System.Collections; 

public class EnemyShooter : MonoBehaviour
{
    [Header("Identificação única")]
    public string uniqueId;

    [Header("Vida")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Movimento")]
    public bool canPatrol = true;
    public float speed = 2f;
    public LayerMask wallLayer;
    [Tooltip("1 para Direita, -1 para Esquerda")]
    public int startingDirection = 1;

    [Header("Detecção do player")]
    public Transform attackPoint;
    public float detectionRadius = 4f;
    public LayerMask playerLayer;

    [Header("Tiro")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootCooldown = 2f;

    [Header("Projetil opcional seguindo o player")]
    public bool projectileFollowsPlayer = false;
    public float projectileFollowStrength = 5f;

    [Header("Morte")]
    public float deathDestroyDelay = 0.4f;

    private Rigidbody2D rb;
    [Header("Piscar ao tomar dano")]
    public SpriteRenderer[] renderersToBlink;
    public float blinkDuration = 0.15f;
    public int blinkCount = 2;
    private Animator animator;
    private Collider2D col;

    private int direction;
    private bool facingRight;

    private float shootTimer = 0f;
    private bool isShooting = false;
    private bool isDead = false;

    private Transform playerTarget;

    void Start()
    {
        if (GameProgress.IsEnemyDead(uniqueId))
        {
            Destroy(gameObject);
            return;
        }

        if (renderersToBlink == null || renderersToBlink.Length == 0)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                renderersToBlink = new SpriteRenderer[] { sr };
            }
        }

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();

        currentHealth = maxHealth;

        // Configuração da direção inicial
        direction = startingDirection;

        // Ajusta o visual e a flag facingRight baseada na direção escolhida
        if (direction == -1)
        {
            facingRight = false;
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        else
        {
            facingRight = true;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    void Update()
    {
        if (isDead) return;

        shootTimer += Time.deltaTime;

        Collider2D detectedPlayer = Physics2D.OverlapCircle(
            attackPoint.position,
            detectionRadius,
            playerLayer
        );

        if (isShooting)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (detectedPlayer != null)
        {
            playerTarget = detectedPlayer.transform;

            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            LookAtPlayer();

            if (shootTimer >= shootCooldown)
            {
                StartShoot();
                shootTimer = 0f;
            }
        }
        else
        {
            playerTarget = null;

            if (canPatrol)
                rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
            else
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    void StartShoot()
    {
        if (isDead) return;

        isShooting = true;

        if (animator != null)
        {
            animator.ResetTrigger("Shoot");
            animator.SetTrigger("Shoot");
        }
    }

    public void ShootProjectile()
    {

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEnemyShoot();
        }
        if (isDead) return;
        if (projectilePrefab == null || firePoint == null) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Vector2 shotDirection;

        if (playerTarget != null)
            shotDirection = ((Vector2)playerTarget.position - (Vector2)firePoint.position).normalized;
        else
            shotDirection = facingRight ? Vector2.right : Vector2.left;

        EnemyProjectile projectile = proj.GetComponent<EnemyProjectile>();
        if (projectile != null)
        {
            projectile.SetDirection(
                shotDirection,
                col,
                playerTarget,
                projectileFollowsPlayer,
                projectileFollowStrength
            );
        }
    }

    public void EndShoot()
    {
        if (isDead) return;
        isShooting = false;
    }

    void LookAtPlayer()
    {
        if (playerTarget == null) return;

        if (playerTarget.position.x > transform.position.x && !facingRight)
            Flip();
        else if (playerTarget.position.x < transform.position.x && facingRight)
            Flip();
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEnemyDamage();
        }

        if (animator != null)
        {
            animator.ResetTrigger("Hurt");
            animator.SetTrigger("Hurt");
        }

        StopCoroutine(nameof(BlinkRoutine));
        StartCoroutine(nameof(BlinkRoutine));

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator BlinkRoutine()
    {
        if (renderersToBlink == null || renderersToBlink.Length == 0)
            yield break;

        for (int i = 0; i < blinkCount; i++)
        {
            SetRenderersVisible(false);
            yield return new WaitForSeconds(blinkDuration / 2f);

            SetRenderersVisible(true);
            yield return new WaitForSeconds(blinkDuration / 2f);
        }
    }

    void SetRenderersVisible(bool visible)
    {
        for (int i = 0; i < renderersToBlink.Length; i++)
        {
            if (renderersToBlink[i] != null)
            {
                renderersToBlink[i].enabled = visible;
            }
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        isShooting = false;

        GameProgress.MarkEnemyDead(uniqueId);

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (col != null)
        {
            col.enabled = false;
        }

        if (animator != null)
        {
            animator.ResetTrigger("Shoot");
            animator.ResetTrigger("Hurt");
            animator.SetTrigger("Die");
        }

        Destroy(gameObject, deathDestroyDelay);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        if (!canPatrol) return;

        if (((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            Flip();
        }
    }

    void Flip()
    {
        direction *= -1;
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(attackPoint.position, detectionRadius);
        }

        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(firePoint.position, 0.08f);
        }
    }
}