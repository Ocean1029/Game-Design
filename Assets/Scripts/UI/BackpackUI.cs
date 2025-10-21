using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Manages the backpack UI display
/// Shows all possible items with empty/filled states
/// </summary>
public class BackpackUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Container for inventory slots")]
    [SerializeField] private Transform slotContainer;
    
    [Tooltip("Prefab for inventory slot")]
    [SerializeField] private GameObject slotPrefab;
    
    [Tooltip("Panel to show/hide inventory (should be always visible)")]
    [SerializeField] private GameObject inventoryPanel;
    
    [Header("Use Hint")]
    [Tooltip("Text showing what item can be used")]
    [SerializeField] private TextMeshProUGUI useHintText;
    
    [Tooltip("Panel containing use hint")]
    [SerializeField] private GameObject useHintPanel;
    
    [Header("Display Settings")]
    [Tooltip("Always show inventory UI (no toggle)")]
    [SerializeField] private bool alwaysVisible = true;

    private InventorySystem inventorySystem;
    private Dictionary<string, InventorySlotUI> slotUIMap = new Dictionary<string, InventorySlotUI>();

    void Start()
    {
        Debug.Log("BackpackUI: Start called");
        
        // Validate references
        if (slotContainer == null) Debug.LogError("BackpackUI: slotContainer not assigned!");
        if (slotPrefab == null) Debug.LogError("BackpackUI: slotPrefab not assigned!");
        if (inventoryPanel == null) Debug.LogError("BackpackUI: inventoryPanel not assigned!");
        
        // Find InventorySystem on player
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            Debug.Log($"BackpackUI: Found player '{player.name}'");
            inventorySystem = player.GetComponent<InventorySystem>();
            if (inventorySystem != null)
            {
                Debug.Log("BackpackUI: Found InventorySystem");
                
                // Subscribe to events
                inventorySystem.OnItemCollected += OnItemCollected;
                inventorySystem.OnItemUsed += OnItemUsed;
                inventorySystem.OnItemRemoved += OnItemRemoved;
                inventorySystem.OnCanUseItemChanged += OnCanUseItemChanged;
                
                // Create slots for all possible items
                CreateInventorySlots();
            }
            else
            {
                Debug.LogError("BackpackUI: InventorySystem not found on player!");
            }
        }
        else
        {
            Debug.LogError("BackpackUI: PlayerController not found!");
        }

        // Show/hide inventory based on settings
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(alwaysVisible);
            if (alwaysVisible)
            {
                Debug.Log("BackpackUI: Inventory panel set to always visible");
            }
            else
            {
                Debug.Log("BackpackUI: Inventory panel hidden initially");
            }
        }
        
        // Hide use hint initially
        if (useHintPanel != null)
        {
            useHintPanel.SetActive(false);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (inventorySystem != null)
        {
            inventorySystem.OnItemCollected -= OnItemCollected;
            inventorySystem.OnItemUsed -= OnItemUsed;
            inventorySystem.OnItemRemoved -= OnItemRemoved;
            inventorySystem.OnCanUseItemChanged -= OnCanUseItemChanged;
        }
    }

    void Update()
    {
        // Use item with E key
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryUseItem();
        }
        
        // Optional: Toggle inventory with Tab key (only if alwaysVisible is false)
        if (!alwaysVisible && (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I)))
        {
            ToggleInventory();
        }
    }

    /// <summary>
    /// Create UI slots for all possible items
    /// </summary>
    private void CreateInventorySlots()
    {
        if (slotContainer == null || slotPrefab == null)
        {
            Debug.LogError("BackpackUI: slotContainer or slotPrefab not assigned!");
            return;
        }

        List<ItemData> allItems = inventorySystem.GetAllGameItems();
        Debug.Log($"BackpackUI: Creating slots for {allItems.Count} items");
        
        if (allItems.Count == 0)
        {
            Debug.LogWarning("BackpackUI: No items in allGameItems list! Please add ItemData to Player → InventorySystem → All Game Items");
            return;
        }
        
        foreach (ItemData itemData in allItems)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotContainer);
            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
            
            if (slotUI != null)
            {
                slotUI.Setup(itemData, inventorySystem);
                slotUIMap[itemData.itemId] = slotUI;
                Debug.Log($"BackpackUI: Created slot for '{itemData.itemName}'");
            }
            else
            {
                Debug.LogError("BackpackUI: Slot prefab doesn't have InventorySlotUI component!");
            }
        }
        
        Debug.Log($"BackpackUI: Created {slotUIMap.Count} slots total");
    }

    /// <summary>
    /// Toggle inventory panel (only used when alwaysVisible is false)
    /// </summary>
    public void ToggleInventory()
    {
        if (alwaysVisible)
        {
            Debug.LogWarning("BackpackUI: Cannot toggle - inventory is set to always visible");
            return;
        }
        
        bool isOpen = inventoryPanel != null && inventoryPanel.activeSelf;
        isOpen = !isOpen;
        Debug.Log($"BackpackUI: Toggle inventory - isOpen = {isOpen}");
        
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(isOpen);
            Debug.Log($"BackpackUI: Set inventoryPanel active = {isOpen}");
        }
        else
        {
            Debug.LogError("BackpackUI: inventoryPanel is null!");
        }
        
        // Pause/unpause game when toggling
        Time.timeScale = isOpen ? 0f : 1f;
        
        // Update all slots when opening
        if (isOpen)
        {
            UpdateAllSlots();
        }
    }

    /// <summary>
    /// Update all slot displays
    /// </summary>
    private void UpdateAllSlots()
    {
        foreach (var kvp in slotUIMap)
        {
            kvp.Value.UpdateDisplay();
        }
    }

    /// <summary>
    /// Try to use an item that can be used nearby
    /// </summary>
    private void TryUseItem()
    {
        ItemData usableItem = inventorySystem.GetUsableItemNearby();
        if (usableItem != null)
        {
            inventorySystem.UseItem(usableItem);
        }
    }

    /// <summary>
    /// Called when an item is collected
    /// </summary>
    private void OnItemCollected(ItemData item, int quantity)
    {
        if (slotUIMap.ContainsKey(item.itemId))
        {
            slotUIMap[item.itemId].UpdateDisplay();
        }
    }

    /// <summary>
    /// Called when an item is used
    /// </summary>
    private void OnItemUsed(ItemData item)
    {
        if (slotUIMap.ContainsKey(item.itemId))
        {
            slotUIMap[item.itemId].UpdateDisplay();
        }
    }

    /// <summary>
    /// Called when an item is removed
    /// </summary>
    private void OnItemRemoved(ItemData item)
    {
        if (slotUIMap.ContainsKey(item.itemId))
        {
            slotUIMap[item.itemId].UpdateDisplay();
        }
    }

    /// <summary>
    /// Called when can-use status changes
    /// </summary>
    private void OnCanUseItemChanged(ItemData item, bool canUse)
    {
        if (canUse)
        {
            ShowUseHint(item);
        }
        else
        {
            // Check if there are other usable items
            ItemData usableItem = inventorySystem.GetUsableItemNearby();
            if (usableItem != null)
            {
                ShowUseHint(usableItem);
            }
            else
            {
                HideUseHint();
            }
        }
    }

    /// <summary>
    /// Show use hint for an item
    /// </summary>
    private void ShowUseHint(ItemData item)
    {
        if (useHintPanel != null && useHintText != null)
        {
            useHintPanel.SetActive(true);
            useHintText.text = item.GetCanUseHintMessage();
        }
    }

    /// <summary>
    /// Hide use hint
    /// </summary>
    private void HideUseHint()
    {
        if (useHintPanel != null)
        {
            useHintPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Check if inventory is visible
    /// </summary>
    public bool IsOpen()
    {
        if (alwaysVisible)
        {
            return true;  // Always considered "open" when always visible
        }
        
        return inventoryPanel != null && inventoryPanel.activeSelf;
    }
}

