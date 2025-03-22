using UnityEngine;

public class PickableItem : MonoBehaviour
{
    // Data item
    [Header("Item Data")]
    [Tooltip("Item yang terkait dengan objek ini")]
    public Item Item;

    // Referensi ke InventoryManager
    [Header("References")]
    [Tooltip("Referensi ke InventoryManager di scene")]
    private InventoryManager inventoryManager;

    private void Start()
    {
        // Cari InventoryManager di scene
        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager tidak ditemukan di scene!");
        }
    }

    // Dipanggil saat player masuk ke trigger item
    private void OnTriggerEnter(Collider other)
    {
        // Cek apakah yang masuk adalah player
        if (other.gameObject.CompareTag("Player") && inventoryManager != null)
        {
            inventoryManager.SetCurrentPickableItem(this);
        }
    }

    // Dipanggil saat player keluar dari trigger item
    private void OnTriggerExit(Collider other)
    {
        // Cek apakah yang keluar adalah player
        if (other.gameObject.CompareTag("Player") && inventoryManager != null)
        {
            inventoryManager.ClearCurrentPickableItem(this);
        }
    }
}