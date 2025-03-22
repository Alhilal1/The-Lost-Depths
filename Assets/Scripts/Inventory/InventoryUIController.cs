using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    // Referensi UI untuk inventory
    [Header("UI References")]
    [Tooltip("Panel utama inventory di Canvas")]
    public GameObject InventoryPanel;
    
    [Tooltip("Grid panel untuk slot inventory")]
    public GridLayoutGroup GridPanel;
    
    [Tooltip("Objek player untuk mengontrol karakter")]
    public GameObject PlayerController;

    // Status inventory
    [Header("Inventory Status")]
    [Tooltip("Status apakah inventory sedang terbuka")]
    private bool isInventoryOpen = false;

    private void Start()
    {
        // Sembunyikan inventory saat game dimulai
        InventoryPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Cek input untuk membuka/menutup inventory
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleInventory();
        }
    }

    // Membuka atau menutup inventory
    private void ToggleInventory()
    {
        // Ubah status inventory
        isInventoryOpen = !isInventoryOpen;
        InventoryPanel.SetActive(isInventoryOpen);
        if (GridPanel != null)
        {
            GridPanel.gameObject.SetActive(isInventoryOpen);
        }

        // Atur kursor dan player berdasarkan status inventory
        if (isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (PlayerController != null)
            {
                PlayerController.SetActive(false);
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (PlayerController != null)
            {
                PlayerController.SetActive(true);
            }
        }
    }
}