using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Vector3 escalaNormal = Vector3.one;
    [SerializeField] private Vector3 escalaHover = new Vector3(1.1f, 1.1f, 1f);
    [SerializeField] private float velocidade = 10f;

    private Vector3 escalaAlvo;

    private void Start()
    {
        escalaAlvo = escalaNormal;
        transform.localScale = escalaNormal;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, escalaAlvo, Time.deltaTime * velocidade);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        escalaAlvo = escalaHover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        escalaAlvo = escalaNormal;
    }
}