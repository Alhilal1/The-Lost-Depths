using UnityEngine;
using UnityEngine.EventSystems;

public class TESTING : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas tidak ditemukan di parent dari: " + gameObject.name);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Mulai drag pada: " + gameObject.name + " | Position: " + eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Sedang drag pada: " + gameObject.name + " | Position: " + eventData.position);

        // Konversi posisi mouse ke koordinat lokal Canvas
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Selesai drag pada: " + gameObject.name + " | Position: " + eventData.position);
    }
}