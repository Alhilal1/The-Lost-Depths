using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Referensi panel untuk drag-and-drop
    [Header("Panel References")]
    [Tooltip("Referensi ke Grid Panel (panel inventory utama)")]
    [SerializeField] private Transform _gridPanel;
    
    [Tooltip("Referensi ke HandIconPanel (panel di tangan kiri)")]
    [SerializeField] private Transform _handIconPanel;

    // Komponen UI dan posisi awal
    [Header("UI Components")]
    [Tooltip("Transform asal item sebelum drag")]
    private Transform originalParent;
    
    [Tooltip("Panel asal item (GridPanel atau HandIconPanel)")]
    private Transform originalPanel;
    
    [Tooltip("Posisi awal item sebelum drag")]
    private Vector2 originalPosition;
    
    [Tooltip("Canvas tempat item berada")]
    private Canvas canvas;
    
    [Tooltip("RectTransform untuk mengatur posisi item")]
    private RectTransform rectTransform;
    
    [Tooltip("Komponen Image untuk menampilkan ikon item")]
    private Image image;

    // Properti untuk item yang terkait
    [Header("Item Data")]
    [Tooltip("Item yang terkait dengan slot ini")]
    public PickableItem AssociatedItem { get; private set; }

    private void Start()
    {
        // Inisialisasi komponen saat game dimulai
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        // Pengecekan jika komponen atau referensi tidak diatur
        if (canvas == null)
        {
            Debug.LogWarning("Canvas tidak ditemukan di parent!");
        }
        if (_gridPanel == null)
        {
            Debug.LogWarning("Grid Panel tidak diatur di Inspector!");
        }
        if (_handIconPanel == null)
        {
            Debug.LogWarning("HandIconPanel tidak diatur di Inspector!");
        }
        if (image == null)
        {
            Debug.LogError("Komponen Image tidak ditemukan di objek ini!");
        }
    }

    // Mengatur item yang terkait dengan slot ini
    public void SetAssociatedItem(PickableItem item)
    {
        // Simpan item yang terkait
        AssociatedItem = item;
    }

    // Dipanggil saat mulai drag
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Pastikan ada image dan sprite sebelum drag
        if (image == null || image.sprite == null)
        {
            Debug.Log("OnBeginDrag: Tidak ada image atau sprite di " + gameObject.name);
            return;
        }

        Debug.Log("Mulai drag pada: " + gameObject.name + " | Position: " + eventData.position);

        // Simpan posisi dan parent awal
        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;
        originalPanel = originalParent;

        // Pindahkan ke root agar bisa digerakkan bebas
        transform.SetParent(transform.root);
        image.raycastTarget = false; // Nonaktifkan raycast agar tidak mengganggu drag
    }

    // Dipanggil saat sedang drag
    public void OnDrag(PointerEventData eventData)
    {
        // Pastikan ada image dan sprite selama drag
        if (image == null || image.sprite == null)
        {
            Debug.Log("OnDrag: Tidak ada image atau sprite di " + gameObject.name);
            return;
        }


        // Ubah posisi item mengikuti kursor
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
        }
    }

    // Dipanggil saat selesai drag
    public void OnEndDrag(PointerEventData eventData)
    {
        // Pastikan ada image dan sprite setelah drag
        if (image == null || image.sprite == null)
        {
            Debug.Log("OnEndDrag: Tidak ada image atau sprite di " + gameObject.name);
            return;
        }

        Debug.Log("Selesai drag pada: " + gameObject.name + " | Position: " + eventData.position);

        // Cek apakah di-drop di HandIconPanel
        if (_handIconPanel != null && RectTransformUtility.RectangleContainsScreenPoint(_handIconPanel as RectTransform,eventData.position,canvas.worldCamera))
        {
            // Cek apakah HandIconPanel sudah berisi item
            if (_handIconPanel.childCount == 0) // Hanya izinkan jika HandIconPanel kosong
            {
                transform.SetParent(_handIconPanel);
                rectTransform.anchoredPosition = Vector2.zero;
                Debug.Log("Item berhasil dipindahkan ke HandIconPanel: " + gameObject.name);
            }
            else
            {
                // Jika HandIconPanel sudah berisi item, kembalikan ke panel asal
                Debug.LogWarning("HandIconPanel sudah berisi item! Mengembalikan " + gameObject.name + " ke panel asal.");
                transform.SetParent(originalPanel);
                rectTransform.anchoredPosition = originalPosition;
            }
        }
        // Cek apakah di-drop di Grid Panel
        else if (_gridPanel != null)
        {
            transform.SetParent(_gridPanel);
            rectTransform.anchoredPosition = Vector2.zero;
        }
        // Jika di-drop di luar kedua panel, kembali ke panel asal
        else
        {
            transform.SetParent(originalPanel);
            rectTransform.anchoredPosition = originalPosition;
        }

        // Aktifkan kembali raycast setelah selesai drag
        image.raycastTarget = true;
    }
}