using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Configurações")]
    public float speed = 8f;
    public float lifeTime = 2f;
    public int damage = 1;
    public LayerMask collisionLayers;

    [Header("Seguir player")]
    public bool followPlayer = false;
    public float homingStrength = 5f;

    private Vector2 direction;
    private Transform target;
    private Collider2D projectileCollider;

    void Awake()
    {
        projectileCollider = GetComponent<Collider2D>();
    }

    public void SetDirection(Vector2 dir, Collider2D enemyCollider, Transform playerTarget, bool shouldFollow, float followStrength)
    {
        direction = dir.normalized;
        target = playerTarget;
        followPlayer = shouldFollow;
        homingStrength = followStrength;

        if (enemyCollider != null && projectileCollider != null)
        {
            Physics2D.IgnoreCollision(projectileCollider, enemyCollider, true);
        }

        if (direction.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction.x);
            transform.localScale = scale;
        }
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        UpdateDirection();
        MoveProjectile();
    }

    void UpdateDirection()
    {
        if (!followPlayer) return;
        if (target == null) return;

        Vector2 desiredDirection = ((Vector2)target.position - (Vector2)transform.position).normalized;
        direction = Vector2.Lerp(direction, desiredDirection, homingStrength * Time.deltaTime).normalized;

        if (direction.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction.x);
            transform.localScale = scale;
        }
    }

    void MoveProjectile()
    {
        Vector3 currentPosition = transform.position;
        Vector3 nextPosition = currentPosition + (Vector3)(direction * speed * Time.deltaTime);

        RaycastHit2D hit = Physics2D.Linecast(currentPosition, nextPosition, collisionLayers);

        if (hit.collider != null)
        {
            PlayerMovement player = hit.collider.GetComponentInParent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamageFromEnemy(damage);
                Destroy(gameObject);
                return;
            }

            Destroy(gameObject);
            return;
        }

        transform.position = nextPosition;
    }
}