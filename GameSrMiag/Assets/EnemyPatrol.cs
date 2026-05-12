using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Identificação única")]
    public string uniqueId;

    [Header("Vida")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Movimento")]
    public float speed = 2f;
    public LayerMask wallLayer;
    [Tooltip("Use 1 para Direita e -1 para Esquerda")]
    public int startingDirection = 1; 

    [Header("Ataque")]
    public Transform attackPoint;
    public float attackRadius = 0.6f;
    public LayerMask playerLayer;
    public float attackCooldown = 1.5f;
    public int damage = 1;

    [Header("Morte")]
    public float deathDestroyDelay = 0.4f;

    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D col;

    private int direction;
    private bool facingRight;

    private float attackTimer = 0f;
    private bool isAttacking = false;
    private bool isDead = false;

    void Start()
    {
        // Verifica se o inimigo já foi morto anteriormente
        if (GameProgress.IsEnemyDead(uniqueId))
        {
            Destroy(gameObject);
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();

        currentHealth = maxHealth;

        // Configura a direção inicial
        direction = startingDirection;
        
        // Ajusta o visual inicial (Flip) baseado na direção escolhida
        if (direction == -1)
        {
            facingRight = false;
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x); // Garante que a escala X seja negativa
            transform.localScale = scale;
        }
        else
        {
            facingRight = true;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x); // Garante que a escala X seja positiva
            transform.localScale = scale;
        }
    }

    void Update()
    {
        if (isDead) return;

        attackTimer += Time.deltaTime;

        if (isAttacking)
        {
            // Para o movimento horizontal durante o ataque
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        // Aplica o movimento
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        // Checa se o jogador está no alcance
        Collider2D hit = Physics2D.OverlapCircle(
            attackPoint.position,
            attackRadius,
            playerLayer
        );

        if (hit != null && attackTimer >= attackCooldown)
        {
            StartAttack();
            attackTimer = 0f;
        }
    }

    void StartAttack()
    {
        if (isDead) return;

        isAttacking = true;

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }
    }

    // Chamado via Evento de Animação
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
            PlayerMovement player = hit.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamageFromEnemy(damage);
            }
        }
    }

    // Chamado via Evento de Animação
    public void EndAttack()
    {
        if (isDead) return;
        isAttacking = false;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEnemyHurt();
        }
        
        if (animator != null)
        {
            animator.ResetTrigger("Hurt");
            animator.SetTrigger("Hurt");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        isAttacking = false;

        // Salva o progresso para o inimigo não spawnar de novo
        GameProgress.MarkEnemyDead(uniqueId);

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (col != null)
        {
            col.enabled = false;
        }

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Hurt");
            animator.SetTrigger("Die");
        }

        Destroy(gameObject, deathDestroyDelay);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        // Inverte a direção ao colidir com algo na wallLayer
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
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}