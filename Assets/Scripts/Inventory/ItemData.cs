using UnityEngine;

/// <summary>
/// Defines the category of items in the game
/// </summary>
public enum ItemCategory
{
    Key,            // Keys
    Tool,           // Tools
    Material,       // Materials
    Consumable,     // Consumables
    QuestItem       // Quest Items
}

/// <summary>
/// Defines how an item is used
/// </summary>
public enum ItemUseType
{
    AutoUse,        // Automatic use (triggers near target, e.g., key opens door)
    Manual,         // Manual use (player must actively use)
    Passive         // Passive item (just need to own it, e.g., quest item)
}

/// <summary>
/// ScriptableObject that defines an item's properties
/// </summary>
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    [Tooltip("Unique identifier for this item")]
    public string itemId;
    
    [Tooltip("Display name shown to player")]
    public string itemName;
    
    [Tooltip("Description of the item")]
    [TextArea(3, 5)]
    public string description;
    
    [Tooltip("Icon shown in inventory UI")]
    public Sprite icon;
    
    [Header("Item Properties")]
    [Tooltip("Category of item")]
    public ItemCategory itemCategory = ItemCategory.QuestItem;
    
    [Tooltip("How this item is used")]
    public ItemUseType useType = ItemUseType.Manual;
    
    [Tooltip("Can this item be used multiple times?")]
    public bool isConsumable = false;
    
    [Tooltip("Maximum stack size (1 = no stacking)")]
    public int maxStackSize = 1;
    
    [Header("Usage")]
    [Tooltip("Tag of objects this item can interact with (e.g., 'door_red' for red key)")]
    public string interactableTag;
    
    [Tooltip("Message shown when item is used")]
    public string useMessage = "Used {itemName}";
    
    [Tooltip("Message shown when item can be used nearby")]
    public string canUseHintMessage = "Press E to use {itemName}";
    
    [Header("Audio")]
    [Tooltip("Sound played when item is collected")]
    public AudioClip collectSound;
    
    [Tooltip("Sound played when item is used")]
    public AudioClip useSound;
    
    /// <summary>
    /// Get formatted use message
    /// </summary>
    public string GetUseMessage()
    {
        return useMessage.Replace("{itemName}", itemName);
    }
    
    /// <summary>
    /// Get formatted hint message
    /// </summary>
    public string GetCanUseHintMessage()
    {
        return canUseHintMessage.Replace("{itemName}", itemName);
    }
}

