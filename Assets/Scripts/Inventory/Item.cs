using UnityEngine;

public enum ItemType
{
    None,
    Lighter,
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    // Data item
    [Header("Item Data")]
    [Tooltip("Nama item")]
    public string Name;
    
    [Tooltip("Deskripsi item")]
    public string Description;
    
    [Tooltip("Ikon item untuk ditampilkan di UI")]
    public Sprite Icon;
    
    [Tooltip("Tipe item (misalnya Lighter)")]
    public ItemType Type;
}