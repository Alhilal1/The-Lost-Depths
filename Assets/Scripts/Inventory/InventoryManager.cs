using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    // Referensi UI untuk inventory
    [Header("UI References")]
    [Tooltip("Panel interaksi saat mendekati item")]
    public GameObject InteractPanel;
    
    [Tooltip("Panel grid untuk slot inventory")]
    public GridLayoutGroup GridPanel;
    
    [Tooltip("Teks untuk menampilkan pesan inventory penuh")]
    public TextMeshProUGUI InventoryFullText;
    
    [Tooltip("Panel untuk ikon di tangan")]
    [SerializeField] private Transform _handIconPanel;

    // Pengaturan inventory
    [Header("Inventory Settings")]
    [Tooltip("Durasi tampilan pesan inventory penuh (dalam detik)")]
    [SerializeField] private float _displayDuration = 2f;
    
    [Tooltip("Jumlah maksimum slot di inventory")]
    private const int maxInventorySize = 16;

    // Data inventory
    [Header("Inventory Data")]
    [Tooltip("Item yang sedang dipilih untuk diambil")]
    private PickableItem currentPickableItem;
    
    [Tooltip("Array slot inventory (berisi Image)")]
    private Image[] inventorySlots;

    private void Start()
    {
        // Ambil semua slot dari GridPanel
        var children = GridPanel.transform;
        List<Transform> slotList = new List<Transform>();
        foreach (Transform child in children)
        {
            if (child.gameObject.name.Contains("Item") && child.parent == GridPanel.transform)
            {
                slotList.Add(child);
            }
        }
        var slots = slotList.ToArray();

        // Inisialisasi array slot inventory
        inventorySlots = new Image[slots.Length];
        for (int i = 0; i < slots.Length; i++)
        {
            string imageName = slots[i].gameObject.name == "Item" ? "Image" : "Image (" + i + ")";
            Transform imageTransform = slots[i].Find(imageName);
            if (imageTransform != null)
            {
                inventorySlots[i] = imageTransform.GetComponent<Image>();
                if (inventorySlots[i] != null)
                {
                    inventorySlots[i].sprite = null; // Atur slot kosong dengan null
                    inventorySlots[i].color = Color.black;
                }
            }
            else
            {
                Debug.LogWarning("Image tidak ditemukan di " + slots[i].gameObject.name);
            }
        }

        Debug.Log("Jumlah slot inventory: " + inventorySlots.Length);
    }

    // Mengatur item yang sedang dipilih
    public void SetCurrentPickableItem(PickableItem item)
    {
        // Simpan item yang dipilih dan aktifkan panel interaksi
        currentPickableItem = item;
        InteractPanel.SetActive(true);
        Debug.Log("SetCurrentPickableItem: " + item.Item.name);
    }

    // Menghapus item yang sedang dipilih
    public void ClearCurrentPickableItem(PickableItem item)
    {
        // Hapus item yang dipilih jika sesuai
        if (item == currentPickableItem)
        {
            currentPickableItem = null;
            InteractPanel.SetActive(false);
            Debug.Log("ClearCurrentPickableItem dipanggil.");
        }
    }

    private void Update()
    {
        // Cek input untuk mengambil item
        if (Input.GetKeyDown(KeyCode.E) && currentPickableItem != null)
        {
            Debug.Log("Tombol E ditekan, mencoba mengambil item...");
            PickUpItem(currentPickableItem);
        }
    }

    // Mengambil item dan menambahkannya ke inventory
    private void PickUpItem(PickableItem item)
    {
        // Pastikan slot inventory sudah diinisialisasi
        if (inventorySlots == null || inventorySlots.Length == 0)
        {
            Debug.LogError("inventorySlots tidak diinisialisasi!");
            return;
        }

        // Cari slot kosong
        Image targetSlot = FindEmptySlot();
        if (targetSlot == null)
        {
            Debug.LogError("Tidak ada slot kosong!");
            return;
        }

        // Tambahkan item ke slot
        targetSlot.sprite = item.Item.Icon;
        targetSlot.color = Color.white;

        // Set associatedItem pada slot UI
        DragAndDrop dragAndDrop = targetSlot.GetComponent<DragAndDrop>();
        if (dragAndDrop != null)
        {
            dragAndDrop.SetAssociatedItem(item);
            Debug.Log("AssociatedItem diatur untuk slot " + targetSlot.transform.parent.name + ": " + item.Item.name);
        }

        // Nonaktifkan item di scene
        item.gameObject.SetActive(false);
        ClearCurrentPickableItem(item);

        Debug.Log("Item " + item.Item.name + " ditambahkan ke slot " + targetSlot.transform.parent.name);
    }

    // Mencari slot kosong di inventory
    private Image FindEmptySlot()
    {
        // Mulai mencari slot kosong
        Debug.Log("Mencari slot kosong...");
        foreach (Image slot in inventorySlots)
        {
            if (slot != null && slot.sprite == null)
            {
                Debug.Log("Slot kosong ditemukan: " + slot.transform.parent.name);
                return slot;
            }
        }
        Debug.LogWarning("Tidak ada slot kosong!");
        return null;
    }

    // Menghapus teks inventory penuh
    private void ClearInventoryFullText()
    {
        // Pastikan teks inventory penuh ada, lalu sembunyikan
        if (InventoryFullText != null)
        {
            InventoryFullText.text = "";
            InventoryFullText.gameObject.SetActive(false);
            Debug.Log("UI Inventory Full disembunyikan!");
        }
    }

    // Cek apakah Lighter ada di HandIconPanel
   public bool IsLighterInHand()
{
    if (_handIconPanel == null)
    {
        Debug.LogWarning("HandIconPanel tidak diatur di InventoryManager!");
        return false;
    }

    Debug.Log("Jumlah child di HandIconPanel: " + _handIconPanel.childCount);
    foreach (Transform child in _handIconPanel)
    {
        Debug.Log("Memeriksa child: " + child.name);
        DragAndDrop dragAndDrop = child.GetComponent<DragAndDrop>();
        if (dragAndDrop != null)
        {
            Debug.Log("Slot di HandIconPanel: " + child.name + ", AssociatedItem: " + (dragAndDrop.AssociatedItem != null ? dragAndDrop.AssociatedItem.Item.name : "null"));
            if (dragAndDrop.AssociatedItem != null)
            {
                Debug.Log("Item Type: " + dragAndDrop.AssociatedItem.Item.Type);
                if (dragAndDrop.AssociatedItem.Item.Type == ItemType.Lighter)
                {
                    Debug.Log("Lighter ditemukan di HandIconPanel!");
                    return true;
                }
            }
        }
        else
        {
            Debug.LogWarning("DragAndDrop tidak ditemukan di slot HandIconPanel: " + child.name);
        }
    }
    Debug.Log("Lighter tidak ditemukan di HandIconPanel.");
    return false;
}
}