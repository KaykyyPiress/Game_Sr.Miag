using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    [Header("Movimento vertical")]
    public float floatAmplitude = 0.2f;   // altura do sobe/desce
    public float floatSpeed = 2f;         // velocidade

    [Header("Escala (pulso)")]
    public float scaleAmplitude = 0.1f;   // quanto cresce/diminui
    public float scaleSpeed = 3f;

    private Vector3 startPos;
    private Vector3 startScale;

    void Start()
    {
        startPos = transform.localPosition;
        startScale = transform.localScale;
    }

    void Update()
    {
        // Movimento vertical (flutuação)
        float newY = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

        transform.localPosition = new Vector3(
            startPos.x,
            startPos.y + newY,
            startPos.z
        );

        // Pulso de escala
        float scaleOffset = Mathf.Sin(Time.time * scaleSpeed) * scaleAmplitude;

        transform.localScale = new Vector3(
            startScale.x + scaleOffset,
            startScale.y + scaleOffset,
            startScale.z
        );
    }
}