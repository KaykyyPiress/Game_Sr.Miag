using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FixedResolutionCamera : MonoBehaviour
{
    [Header("Resolução base")]
    public float targetWidth = 1101f;
    public float targetHeight = 534f;

    void Start()
    {
        ApplyAspect();
    }

    void ApplyAspect()
    {
        Camera cam = GetComponent<Camera>();

        float targetAspect = targetWidth / targetHeight;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            Rect rect = cam.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0f;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            cam.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = cam.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0f;

            cam.rect = rect;
        }
    }
}