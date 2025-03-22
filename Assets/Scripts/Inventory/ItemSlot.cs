using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    // Dipanggil saat item di-drop ke slot ini
    public void OnDrop(PointerEventData eventData)
    {
        // Ambil objek yang sedang di-drag
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject != null)
        {
            // Ambil komponen DragAndDrop dari objek yang di-drag
            DragAndDrop draggedItem = draggedObject.GetComponent<DragAndDrop>();
            if (draggedItem != null)
            {
                // Panggil OnEndDrag untuk menangani drop
                draggedItem.OnEndDrag(eventData);
            }
        }
    }
}