using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;

    [Header("Gravidade")]
    public float fallMultiplier = 2f;
    public float lowJumpMultiplier = 2f;

    [Header("Detecção de chão")]
    public LayerMask groundLayers;
    public float groundCheckDistance = 0.1f;

    [Header("Pulo")]
    public float jumpCooldown = 0.15f;
    private float nextJumpTime = 0f;

    [Header("Status")]
    public int maxLives = 4;
    public int lives = 4;

    [Header("Cenas")]
    public string victorySceneName = "Vitoria";
    public string defeatSceneName = "Derrota";

    [Header("Dano")]
    public float invincibleTime = 0.8f;

    [Header("Ataque")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootCooldown = 0.5f;
    public float projectileSpawnOffset = 0.4f;
    public float projectileSpeedMultiplier = 1f;
    private float nextShootTime = 0f;
    private bool isShooting = false;

    [Header("Pastel Supremo")]
    public int totalPasteisSupremos = 6;

    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D col;

    private float horizontalInput;
    private bool isGrounded;
    private bool facingRight = true;

    private bool isInvincible = false;
    private bool isKnockedBack = false;

    private float originalSpeed;
    private float originalJumpForce;

    private bool inOil = false;
    private float oilDamagePerSecond = 0f;
    private float oilTimer = 0f;
    private float oilMovementResponsiveness = 0.1f;

    private float originalShootCooldown;
    private float originalProjectileSpeedMultiplier;

    private Coroutine supremeRoutine;
    private float supremeTimeRemaining = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();

        originalSpeed = moveSpeed;
        originalJumpForce = jumpForce;
        originalShootCooldown = shootCooldown;
        originalProjectileSpeedMultiplier = projectileSpeedMultiplier;

        LoadOrInitProgress();
        UpdateUI();
    }

    void LoadOrInitProgress()
    {
        if (!GameProgress.initialized)
        {
            GameProgress.ResetProgress(maxLives, totalPasteisSupremos);
        }

        lives = Mathf.Clamp(GameProgress.currentLives, 0, maxLives);
    }

    void SaveLives()
    {
        GameProgress.currentLives = lives;
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        CheckGround();
        HandleJump();
        BetterJump();
        HandleShootInput();
        FlipPlayer();
        UpdateAnimator();
        HandleOilDamage();
        UpdateBuffUI();
    }

    void FixedUpdate()
    {
        if (isKnockedBack)
            return;

        float currentSpeed = isShooting ? 0f : moveSpeed;
        float targetSpeed = horizontalInput * currentSpeed;

        if (inOil)
        {
            rb.linearVelocity = Vector2.Lerp(
                rb.linearVelocity,
                new Vector2(targetSpeed, rb.linearVelocity.y),
                oilMovementResponsiveness
            );
        }
        else
        {
            rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
        }
    }

    void CheckGround()
    {
        Bounds bounds = col.bounds;

        RaycastHit2D hit = Physics2D.BoxCast(
            bounds.center,
            bounds.size,
            0f,
            Vector2.down,
            groundCheckDistance,
            groundLayers
        );

        isGrounded = hit.collider != null;

        Debug.Log($"[Ground] groundLayers value = {groundLayers.value} ({groundLayers}) | isGrounded = {isGrounded}");
    }

    void HandleJump()
    {
        if (isKnockedBack || isShooting)
        {
            return;
        }

        if (Input.GetButtonDown("Jump") && isGrounded && Time.time >= nextJumpTime)
        {
            Jump();
            nextJumpTime = Time.time + jumpCooldown;

            if (animator != null)
            {
                animator.ResetTrigger("JumpTrigger");
                animator.SetTrigger("JumpTrigger");
                animator.Play("Jump", 0, 0f);
            }
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void BetterJump()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.deltaTime;
        }
    }

    void HandleShootInput()
    {
        if (projectilePrefab == null || firePoint == null || animator == null) return;
        if (isKnockedBack) return;

        if (Input.GetKeyDown(KeyCode.F) && Time.time >= nextShootTime && !isShooting)
        {
            nextShootTime = Time.time + shootCooldown;
            StartShoot();
        }
    }

    void StartShoot()
    {
        isShooting = true;
        if (animator != null)
        {
            animator.ResetTrigger("Shoot");
            animator.SetTrigger("Shoot");
        }
    }

    public void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;

        int dir = facingRight ? 1 : -1;
        Vector3 spawnPosition = firePoint.position + new Vector3(projectileSpawnOffset * dir, 0f, 0f);

        GameObject proj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        PastelProjectile pastel = proj.GetComponent<PastelProjectile>();
        if (pastel != null)
        {
            pastel.SetDirection(dir, col);
            pastel.SetSpeedMultiplier(projectileSpeedMultiplier);
        }
        {
            AudioManager.Instance.PlayPlayerShoot();
        }
    }

    public void EndShoot()
    {
        isShooting = false;
    }

    void FlipPlayer()
    {
        if (isKnockedBack || isShooting) return;

        if (horizontalInput > 0 && !facingRight)
            Flip();
        else if (horizontalInput < 0 && facingRight)
            Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    void UpdateAnimator()
    {
        if (animator == null) return;

        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        animator.SetBool("IsGrounded", isGrounded);
    }

    public void TakeDamageFromEnemy(int damage)
    {
        if (isInvincible) return;

        lives -= damage;
        lives = Mathf.Clamp(lives, 0, maxLives);

        {
            AudioManager.Instance.PlayPlayerHurt();
        }
        
        if (animator != null)
        {
            animator.ResetTrigger("Hurt");
            animator.SetTrigger("Hurt");
            Debug.Log("Chamou Hurt");
        }

        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.UpdateLives(lives, maxLives);
        }

        StopCoroutine(nameof(InvincibilityRoutine));
        StartCoroutine(nameof(InvincibilityRoutine));

        if (lives <= 0)
        {
            Die();
        }
    }


    IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }

    public void ApplyKnockback(Vector2 force, float duration)
    {
        StopCoroutine(nameof(KnockbackRoutine));
        StartCoroutine(KnockbackRoutine(force, duration));
    }

    IEnumerator KnockbackRoutine(Vector2 force, float duration)
    {
        isKnockedBack = true;

        // knockback previsível, sem depender de massa/drag
        rb.linearVelocity = force;

        yield return new WaitForSeconds(duration);

        isKnockedBack = false;
    }

    void Die()
    {
        SceneManager.LoadScene(defeatSceneName);
    }

    public void ApplyOilEffect(float slowMultiplier, float movementResponsiveness, float jumpMultiplier, float damagePerSecond)
    {
        inOil = true;

        moveSpeed = originalSpeed * slowMultiplier;
        jumpForce = originalJumpForce * jumpMultiplier;
        oilMovementResponsiveness = movementResponsiveness;
        oilDamagePerSecond = damagePerSecond;
    }

    public void ResetOilEffect()
    {
        inOil = false;

        moveSpeed = originalSpeed;
        jumpForce = originalJumpForce;
        oilMovementResponsiveness = 0.1f;
        oilDamagePerSecond = 0f;
        oilTimer = 0f;
    }

    void HandleOilDamage()
    {
        if (!inOil) return;

        oilTimer += Time.deltaTime;

        if (oilTimer >= 1f)
        {
            oilTimer = 0f;
            TakeDamageFromEnemy(Mathf.RoundToInt(oilDamagePerSecond));
        }
    }

    void UpdateUI()
    {
        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.UpdateLives(GameProgress.currentLives, GameProgress.maxLives);
            GameUIManager.Instance.UpdatePastelCount(GameProgress.collectedPasteisSupremos, GameProgress.totalPasteisSupremos);
        }
    }

    public void CollectPastelSupremo(float speedMultiplier, float cooldownMultiplier, float duration)
    {
        lives = maxLives;
        SaveLives();

        ActivateSupremeBuff(speedMultiplier, cooldownMultiplier, duration);
        UpdateUI();

        if (GameProgress.collectedPasteisSupremos >= GameProgress.totalPasteisSupremos)
        {
            WinGame();
        }
    }

    public void ActivateSupremeBuff(float speedMultiplier, float cooldownMultiplier, float duration)
    {
        projectileSpeedMultiplier = originalProjectileSpeedMultiplier * speedMultiplier;
        shootCooldown = originalShootCooldown * cooldownMultiplier;

        if (supremeRoutine != null)
            StopCoroutine(supremeRoutine);

        supremeRoutine = StartCoroutine(SupremeBuffRoutine(duration));
    }

    IEnumerator SupremeBuffRoutine(float duration)
    {
        supremeTimeRemaining = duration;

        while (supremeTimeRemaining > 0f)
        {
            supremeTimeRemaining -= Time.deltaTime;
            yield return null;
        }

        projectileSpeedMultiplier = originalProjectileSpeedMultiplier;
        shootCooldown = originalShootCooldown;
        supremeTimeRemaining = 0f;
    }

    void UpdateBuffUI()
    {
        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.UpdateBuffTimer(supremeTimeRemaining);
        }
    }

    void WinGame()
    {
        SceneManager.LoadScene(victorySceneName);
    }

    void OnDrawGizmosSelected()
    {
        Collider2D currentCol = GetComponent<Collider2D>();
        if (currentCol == null) return;

        Gizmos.color = Color.yellow;
        Bounds bounds = currentCol.bounds;

        Gizmos.DrawWireCube(
            bounds.center + Vector3.down * groundCheckDistance,
            bounds.size
        );

        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(firePoint.position, 0.08f);
        }
    }
}