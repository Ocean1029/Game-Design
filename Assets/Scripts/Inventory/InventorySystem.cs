using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Main inventory system that manages player's items
/// Handles item collection, usage, and storage
/// </summary>
public class InventorySystem : MonoBehaviour
{
    [Header("Inventory Settings")]
    [Tooltip("Maximum number of different items that can be carried")]
    [SerializeField] private int maxInventorySlots = 20;
    
    [Header("All Possible Items")]
    [Tooltip("List of all items that exist in the game (for UI display)")]
    [SerializeField] private List<ItemData> allGameItems = new List<ItemData>();

    // Current inventory
    private List<InventoryItem> inventory = new List<InventoryItem>();
    
    // Nearby usable items (for manual use)
    private Dictionary<string, GameObject> nearbyUsableObjects = new Dictionary<string, GameObject>();

    // Events
    public delegate void ItemCollectedHandler(ItemData item, int quantity);
    public event ItemCollectedHandler OnItemCollected;
    
    public delegate void ItemUsedHandler(ItemData item);
    public event ItemUsedHandler OnItemUsed;
    
    public delegate void ItemRemovedHandler(ItemData item);
    public event ItemRemovedHandler OnItemRemoved;
    
    public delegate void CanUseItemHandler(ItemData item, bool canUse);
    public event CanUseItemHandler OnCanUseItemChanged;
    
    void Start()
    {
        // Load saved inventory
        LoadInventoryFromSave();
    }

    /// <summary>
    /// Add an item to inventory
    /// </summary>
    public bool AddItem(ItemData itemData, int quantity = 1)
    {
        if (itemData == null)
        {
            Debug.LogWarning("InventorySystem: Trying to add null item");
            return false;
        }

        // Check if item already exists in inventory
        InventoryItem existingItem = inventory.FirstOrDefault(i => i.itemData == itemData);
        
        if (existingItem != null)
        {
            // Try to add to existing stack
            int overflow = existingItem.AddQuantity(quantity);
            
            if (overflow > 0)
            {
                // Stack full, create new stack if space available
                if (inventory.Count < maxInventorySlots)
                {
                    inventory.Add(new InventoryItem(itemData, overflow));
                }
                else
                {
                    Debug.LogWarning($"InventorySystem: Inventory full! Cannot add {overflow} more {itemData.itemName}");
                    OnItemCollected?.Invoke(itemData, quantity - overflow);
                    return false;
                }
            }
        }
        else
        {
            // New item
            if (inventory.Count < maxInventorySlots)
            {
                inventory.Add(new InventoryItem(itemData, quantity));
            }
            else
            {
                Debug.LogWarning($"InventorySystem: Inventory full! Cannot add {itemData.itemName}");
                return false;
            }
        }

        Debug.Log($"InventorySystem: Added {quantity}x {itemData.itemName}");
        OnItemCollected?.Invoke(itemData, quantity);

        // Save inventory
        SaveInventoryToSave();

        // Check if this item can be used nearby
        CheckCanUseItem(itemData);

        return true;
    }

    /// <summary>
    /// Remove an item from inventory
    /// </summary>
    public bool RemoveItem(ItemData itemData, int quantity = 1)
    {
        InventoryItem item = inventory.FirstOrDefault(i => i.itemData == itemData);
        
        if (item == null)
        {
            Debug.LogWarning($"InventorySystem: Don't have {itemData.itemName}");
            return false;
        }

        bool isEmpty = item.RemoveQuantity(quantity);
        
        if (isEmpty)
        {
            inventory.Remove(item);
        }

        Debug.Log($"InventorySystem: Removed {quantity}x {itemData.itemName}");
        OnItemRemoved?.Invoke(itemData);

        // Save inventory
        SaveInventoryToSave();

        return true;
    }

    /// <summary>
    /// Check if player has an item
    /// </summary>
    public bool HasItem(ItemData itemData)
    {
        return inventory.Any(i => i.itemData == itemData);
    }

    /// <summary>
    /// Check if player has an item by ID
    /// </summary>
    public bool HasItem(string itemId)
    {
        return inventory.Any(i => i.itemData.itemId == itemId);
    }

    /// <summary>
    /// Get item by ID
    /// </summary>
    public ItemData GetItemById(string itemId)
    {
        InventoryItem item = inventory.FirstOrDefault(i => i.itemData.itemId == itemId);
        return item?.itemData;
    }

    /// <summary>
    /// Get item quantity
    /// </summary>
    public int GetItemQuantity(ItemData itemData)
    {
        return inventory.Where(i => i.itemData == itemData).Sum(i => i.quantity);
    }

    /// <summary>
    /// Get all items in inventory
    /// </summary>
    public List<InventoryItem> GetAllItems()
    {
        return new List<InventoryItem>(inventory);
    }

    /// <summary>
    /// Get all possible items (for UI display of empty slots)
    /// </summary>
    public List<ItemData> GetAllGameItems()
    {
        return new List<ItemData>(allGameItems);
    }

    /// <summary>
    /// Use an item
    /// </summary>
    public bool UseItem(ItemData itemData)
    {
        if (!HasItem(itemData))
        {
            Debug.LogWarning($"InventorySystem: Don't have {itemData.itemName}");
            return false;
        }

        // Check if there's a nearby usable object
        if (itemData.useType == ItemUseType.Manual)
        {
            if (nearbyUsableObjects.ContainsKey(itemData.interactableTag))
            {
                GameObject target = nearbyUsableObjects[itemData.interactableTag];
                
                // Try to use item on target
                IItemUsable usable = target.GetComponent<IItemUsable>();
                if (usable != null)
                {
                    bool success = usable.UseItem(itemData, this);
                    
                    if (success)
                    {
                        // Play use sound
                        if (itemData.useSound != null)
                        {
                            AudioSource.PlayClipAtPoint(itemData.useSound, transform.position);
                        }
                        
                        // Show use message
                        FloatingTextManager floatingText = FloatingTextManager.GetInstance();
                        if (floatingText != null)
                        {
                            floatingText.ShowFloatingTextAtPlayer(itemData.GetUseMessage());
                        }
                        
                        // Remove if consumable
                        if (itemData.isConsumable)
                        {
                            RemoveItem(itemData, 1);
                        }
                        
                        OnItemUsed?.Invoke(itemData);
                        return true;
                    }
                }
            }
            else
            {
                Debug.Log($"InventorySystem: No usable target nearby for {itemData.itemName}");
                
                FloatingTextManager floatingText = FloatingTextManager.GetInstance();
                if (floatingText != null)
                {
                    floatingText.ShowFloatingTextAtPlayer("Cannot use here", Color.red);
                }
                
                return false;
            }
        }

        return false;
    }

    /// <summary>
    /// Register a nearby usable object
    /// </summary>
    public void RegisterNearbyUsable(string tag, GameObject obj)
    {
        if (!nearbyUsableObjects.ContainsKey(tag))
        {
            nearbyUsableObjects[tag] = obj;
            Debug.Log($"InventorySystem: Registered nearby usable '{tag}'");
            
            // Check if player has any items that can be used with this object
            CheckCanUseItems();
        }
    }

    /// <summary>
    /// Unregister a nearby usable object
    /// </summary>
    public void UnregisterNearbyUsable(string tag)
    {
        if (nearbyUsableObjects.ContainsKey(tag))
        {
            nearbyUsableObjects.Remove(tag);
            Debug.Log($"InventorySystem: Unregistered nearby usable '{tag}'");
            
            // Update can-use status
            CheckCanUseItems();
        }
    }

    /// <summary>
    /// Check if any items can be used with nearby objects
    /// </summary>
    private void CheckCanUseItems()
    {
        foreach (var item in inventory)
        {
            CheckCanUseItem(item.itemData);
        }
    }

    /// <summary>
    /// Check if a specific item can be used
    /// </summary>
    private void CheckCanUseItem(ItemData itemData)
    {
        if (itemData.useType == ItemUseType.Manual)
        {
            bool canUse = nearbyUsableObjects.ContainsKey(itemData.interactableTag);
            OnCanUseItemChanged?.Invoke(itemData, canUse);
        }
    }

    /// <summary>
    /// Get an item that can be used nearby
    /// </summary>
    public ItemData GetUsableItemNearby()
    {
        foreach (var item in inventory)
        {
            if (item.itemData.useType == ItemUseType.Manual &&
                nearbyUsableObjects.ContainsKey(item.itemData.interactableTag))
            {
                return item.itemData;
            }
        }
        return null;
    }

    /// <summary>
    /// Check if player has item that can interact with tag
    /// </summary>
    public bool HasItemForTag(string tag)
    {
        return inventory.Any(i => i.itemData.interactableTag == tag);
    }

    /// <summary>
    /// Get item that can interact with tag
    /// </summary>
    public ItemData GetItemForTag(string tag)
    {
        InventoryItem item = inventory.FirstOrDefault(i => i.itemData.interactableTag == tag);
        return item?.itemData;
    }

    /// <summary>
    /// Clear all items (for testing)
    /// </summary>
    public void ClearInventory()
    {
        inventory.Clear();
        SaveInventoryToSave();
        Debug.Log("InventorySystem: Inventory cleared");
    }
    
    // ==================== Save/Load ====================
    
    /// <summary>
    /// Save current inventory to SaveSystem
    /// </summary>
    private void SaveInventoryToSave()
    {
        SaveSystem.SaveInventory(inventory);
    }
    
    /// <summary>
    /// Load inventory from SaveSystem
    /// </summary>
    private void LoadInventoryFromSave()
    {
        List<ItemSaveData> savedItems = SaveSystem.LoadInventoryData();
        
        if (savedItems.Count == 0)
        {
            Debug.Log("InventorySystem: No saved inventory data found");
            return;
        }
        
        Debug.Log($"InventorySystem: Loading {savedItems.Count} items from save");
        
        foreach (var savedItem in savedItems)
        {
            // Find ItemData in allGameItems by ID
            ItemData itemData = allGameItems.FirstOrDefault(i => i.itemId == savedItem.itemId);
            
            if (itemData != null)
            {
                // Add to inventory without triggering events or saving again
                inventory.Add(new InventoryItem(itemData, savedItem.quantity));
                Debug.Log($"InventorySystem: Loaded {savedItem.quantity}x {itemData.itemName} from save");
            }
            else
            {
                Debug.LogWarning($"InventorySystem: Could not find ItemData for saved item ID '{savedItem.itemId}'");
            }
        }
        
        Debug.Log($"InventorySystem: Loaded {inventory.Count} items total");
    }
}

