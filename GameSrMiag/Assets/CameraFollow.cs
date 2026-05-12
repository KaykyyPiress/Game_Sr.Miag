using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;

    [Header("Limites")]
    public BoxCollider2D bounds;

    private float minX, maxX;

 void Start()
    {
        if (bounds != null)
        {
            float camHalfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;

            minX = bounds.bounds.min.x + camHalfWidth;
            maxX = bounds.bounds.max.x - camHalfWidth;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        float targetX = target.position.x;

        // Limitar movimento da câmera
        float clampedX = Mathf.Clamp(targetX, minX, maxX);

        Vector3 targetPosition = new Vector3(
            clampedX,
            transform.position.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}