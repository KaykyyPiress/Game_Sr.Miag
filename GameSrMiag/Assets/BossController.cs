using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 15;
    private int currentHealth;
    public string uniqueId;

    [Header("Movimento")]
    public float moveSpeed = 2f;
    public float stopDistance = 1.5f;

    [Header("Detecção")]
    public Transform detectionPoint;
    public float detectionRadius = 6f;
    public LayerMask playerLayer;
    public float chaseMemoryTime = 3f;

    [Header("Ataque")]
    public Transform attackPoint;
    public float attackRadius = 1.2f;
    public int damage = 1;
    public float attackCooldown = 1.8f;

    [Header("Knockback no Player")]
    public float playerKnockbackForceX = 10f;
    public float playerKnockbackForceY = 4f;
    public float playerKnockbackDuration = 0.25f;

    [Header("Knockback no Boss")]
    public float bossKnockbackForce = 4f;
    public float bossKnockbackDuration = 0.15f;

    [Header("Fase 2 - Spawn de inimigos")]
    public int phase2HealthThreshold = 10;
    public GameObject swordEnemyPrefab;
    public GameObject shooterEnemyPrefab;
    public Transform[] spawnPoints;
    public int phase2EnemiesToSpawn = 4;
    private bool phase2Started = false;

    [Header("Fase 3 - Boss agressivo")]
    public int phase3HealthThreshold = 5;
    public float phase3MoveSpeed = 3.2f;
    public float phase3AttackCooldown = 1.1f;
    public int phase3EnemiesToSpawn = 3;
    private bool phase3Started = false;

    [Header("Drop")]
    public GameObject pastelSupremoPrefab;
    public Transform dropPoint;

    [Header("Piscar ao tomar dano")]
    public SpriteRenderer[] renderersToBlink;
    public float blinkDuration = 0.15f;
    public int blinkCount = 2;

    [Header("Morte")]
    public float deathDestroyDelay = 1.2f;

    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D col;
    private Transform playerTarget;

    private float attackTimer = 0f;
    private float timeSincePlayerSeen = 0f;

    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isDead = false;
    private bool isKnockedBack = false;
    private bool facingRight = true;

    private float originalMoveSpeed;
    private float originalAttackCooldown;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();

        currentHealth = maxHealth;

        originalMoveSpeed = moveSpeed;
        originalAttackCooldown = attackCooldown;

        if (renderersToBlink == null || renderersToBlink.Length == 0)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                renderersToBlink = new SpriteRenderer[] { sr };
            }
        }

        if (GameProgress.IsBossDead(uniqueId))
        {
            gameObject.SetActive(false);
            return;
        }

        FindPlayer();
    }

    void Update()
    {
        if (isDead) return;

        if (playerTarget == null)
        {
            FindPlayer();
            return;
        }

        attackTimer += Time.deltaTime;
        CheckPlayerDetection();

        if (isAttacking || isKnockedBack)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            SetWalking(false);
            return;
        }

        if (!isChasing)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            SetWalking(false);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        LookAtPlayer();

        if (distanceToPlayer > stopDistance)
        {
            float dir = playerTarget.position.x > transform.position.x ? 1f : -1f;
            rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
            SetWalking(true);
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            SetWalking(false);

            if (attackTimer >= attackCooldown)
            {
                StartAttack();
                attackTimer = 0f;
            }
        }
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerTarget = player.transform;
        }
    }

    void CheckPlayerDetection()
    {
        Transform point = detectionPoint != null ? detectionPoint : transform;

        Collider2D detectedPlayer = Physics2D.OverlapCircle(
            point.position,
            detectionRadius,
            playerLayer
        );

        if (detectedPlayer != null)
        {
            isChasing = true;
            timeSincePlayerSeen = 0f;
        }
        else if (isChasing)
        {
            timeSincePlayerSeen += Time.deltaTime;

            if (timeSincePlayerSeen >= chaseMemoryTime)
            {
                isChasing = false;
                timeSincePlayerSeen = 0f;
            }
        }
    }

    void SetWalking(bool walking)
    {
        if (animator != null)
        {
            animator.SetBool("IsWalking", walking);
        }
    }

    void LookAtPlayer()
    {
        if (playerTarget == null) return;

        if (playerTarget.position.x > transform.position.x && !facingRight)
        {
            Flip();
        }
        else if (playerTarget.position.x < transform.position.x && facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    void StartAttack()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySwordAttack();
        }
        
        if (isDead) return;


        isAttacking = true;
        rb.linearVelocity = Vector2.zero;
        SetWalking(false);

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }
    }

    public void PlayBossFootstep()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBossFootstep();
        }
    }

    // Animation Event no frame do golpe
    public void DealDamage()
    {
        if (isDead) return;

        Collider2D hit = Physics2D.OverlapCircle(
            attackPoint.position,
            attackRadius,
            playerLayer
        );

        if (hit != null)
        {
            PlayerMovement player = hit.GetComponentInParent<PlayerMovement>();

            if (player != null)
            {
                float direction = player.transform.position.x > transform.position.x ? 1f : -1f;
                Vector2 knockbackForce = new Vector2(direction * playerKnockbackForceX, playerKnockbackForceY);

                player.TakeDamageFromEnemy(damage);
                player.ApplyKnockback(knockbackForce, playerKnockbackDuration);
            }
        }
    }

    // Animation Event no último frame do ataque
    public void EndAttack()
    {
        if (isDead) return;
        isAttacking = false;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBossDamage();
        }

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        ForceChasePlayer();

        StopCoroutine(nameof(BlinkRoutine));
        StartCoroutine(nameof(BlinkRoutine));

        StopCoroutine(nameof(BossKnockbackRoutine));
        StartCoroutine(nameof(BossKnockbackRoutine));

        CheckBossPhases();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void CheckBossPhases()
    {
        if (!phase2Started && currentHealth <= phase2HealthThreshold)
        {
            phase2Started = true;
            SpawnEnemies(phase2EnemiesToSpawn);
        }

        if (!phase3Started && currentHealth <= phase3HealthThreshold)
        {
            phase3Started = true;
            StartPhase3();
        }
    }

    void StartPhase3()
    {
        moveSpeed = phase3MoveSpeed;
        attackCooldown = phase3AttackCooldown;

        ForceChasePlayer();

        SpawnEnemies(phase3EnemiesToSpawn);
    }

    void SpawnEnemies(int amount)
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        for (int i = 0; i < amount; i++)
        {
            Transform spawnPoint = spawnPoints[i % spawnPoints.Length];

            GameObject prefabToSpawn = Random.value < 0.5f ? swordEnemyPrefab : shooterEnemyPrefab;

            if (prefabToSpawn != null && spawnPoint != null)
            {
                Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);
            }
        }
    }

    void ForceChasePlayer()
    {
        isChasing = true;
        timeSincePlayerSeen = 0f;

        if (playerTarget == null)
        {
            FindPlayer();
        }
    }

    IEnumerator BossKnockbackRoutine()
    {
        if (playerTarget == null) yield break;

        isKnockedBack = true;

        float direction = playerTarget.position.x > transform.position.x ? -1f : 1f;
        rb.linearVelocity = new Vector2(direction * bossKnockbackForce, rb.linearVelocity.y);

        yield return new WaitForSeconds(bossKnockbackDuration);

        isKnockedBack = false;
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
        isAttacking = false;
        isChasing = false;
        isKnockedBack = false;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (col != null)
        {
            col.enabled = false;
        }

        SetWalking(false);

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Die");
        }

        GameProgress.MarkBossDead(uniqueId);

        DropPastelSupremo();
    }

    void DropPastelSupremo()
    {
        if (pastelSupremoPrefab == null) return;

        Vector3 spawnPos = dropPoint != null ? dropPoint.position : transform.position;
        Instantiate(pastelSupremoPrefab, spawnPos, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
    {
        Transform point = detectionPoint != null ? detectionPoint : transform;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(point.position, detectionRadius);

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }

        if (spawnPoints != null)
        {
            Gizmos.color = Color.yellow;

            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (spawnPoints[i] != null)
                {
                    Gizmos.DrawSphere(spawnPoints[i].position, 0.2f);
                }
            }
        }
    }
}