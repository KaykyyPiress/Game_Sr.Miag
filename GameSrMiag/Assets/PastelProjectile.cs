using UnityEngine;

public class PastelProjectile : MonoBehaviour
{
    [Header("Configurações")]
    public float speed = 10f;
    public float lifeTime = 3f;
    public int damage = 1;
    public LayerMask collisionLayers;

    [Header("Destruição na borda da câmera")]
    public float cameraMargin = 0.02f;

    private int direction = 1;
    private float speedMultiplier = 1f;
    private Collider2D projectileCollider;
    private Camera mainCamera;

    void Awake()
    {
        projectileCollider = GetComponent<Collider2D>();
        mainCamera = Camera.main;
    }

    public void SetDirection(int dir, Collider2D playerCollider)
    {
        direction = dir;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;

        if (playerCollider != null && projectileCollider != null)
        {
            Physics2D.IgnoreCollision(projectileCollider, playerCollider, true);
        }
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        MoveProjectile();
        DestroyIfOutsideCamera();
    }

    void MoveProjectile()
    {
        Vector3 currentPosition = transform.position;
        Vector3 nextPosition = currentPosition + Vector3.right * direction * speed * speedMultiplier * Time.deltaTime;

        RaycastHit2D hit = Physics2D.Linecast(currentPosition, nextPosition, collisionLayers);

        if (hit.collider != null)
        {
            EnemyPatrol patrolEnemy = hit.collider.GetComponentInParent<EnemyPatrol>();
            if (patrolEnemy != null)
            {
                patrolEnemy.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            EnemyShooter shooterEnemy = hit.collider.GetComponentInParent<EnemyShooter>();
            if (shooterEnemy != null)
            {
                shooterEnemy.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            BossController boss = hit.collider.GetComponentInParent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            Destroy(gameObject);
            return;
        }

        transform.position = nextPosition;
    }

    void DestroyIfOutsideCamera()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

        bool outside =
            viewportPos.z < 0f ||
            viewportPos.x < -cameraMargin ||
            viewportPos.x > 1f + cameraMargin ||
            viewportPos.y < -cameraMargin ||
            viewportPos.y > 1f + cameraMargin;

        if (outside)
        {
            Destroy(gameObject);
        }
    }
}