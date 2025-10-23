using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Individual slot UI component for backpack
/// Shows item outline when not collected, full image when collected
/// </summary>
public class BackpackSlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public Image backgroundImage;
    
    [Header("Visual Settings")]
    public Color emptyColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
    public Color collectedColor = Color.white;
    
    [Header("Background Settings")]
    public Color emptyBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    public Color collectedBackgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
    public Color highlightBackgroundColor = new Color(0.4f, 0.4f, 0.6f, 0.9f);
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    // References
    private ItemData itemData;
    private InventorySystem inventorySystem;
    
    void Awake()
    {
        // Auto-find icon image if not assigned
        if (iconImage == null)
        {
            iconImage = GetComponent<Image>();
        }
        
        // Auto-find background image if not assigned
        if (backgroundImage == null)
        {
            // Look for background image in children
            Image[] images = GetComponentsInChildren<Image>();
            foreach (Image img in images)
            {
                if (img != iconImage)
                {
                    backgroundImage = img;
                    break;
                }
            }
        }
    }
    
    /// <summary>
    /// Setup this slot with item data
    /// </summary>
    public void Setup(ItemData data, InventorySystem invSystem)
    {
        itemData = data;
        inventorySystem = invSystem;
        
        // Set icon sprite
        if (iconImage != null && itemData.icon != null)
        {
            iconImage.sprite = itemData.icon;
        }
        else if (showDebugInfo)
        {
            Debug.LogWarning($"BackpackSlotUI: No icon sprite for item '{itemData.itemName}'");
        }
        
        // Initial display update
        UpdateDisplay();
        
        if (showDebugInfo) Debug.Log($"BackpackSlotUI: Setup complete for '{itemData.itemName}'");
    }
    
    /// <summary>
    /// Update the visual display of this slot
    /// </summary>
    public void UpdateDisplay()
    {
        if (itemData == null || inventorySystem == null || iconImage == null)
            return;
        
        // Check if player has this item
        int quantity = inventorySystem.GetItemQuantity(itemData);
        bool hasItem = quantity > 0;
        
        // Update visual state
        if (hasItem)
        {
            // Has item - show full color (complete image)
            iconImage.color = collectedColor;
            
            // Update background color
            if (backgroundImage != null)
            {
                backgroundImage.color = collectedBackgroundColor;
            }
            
            if (showDebugInfo) Debug.Log($"BackpackSlotUI: Showing collected state for '{itemData.itemName}'");
        }
        else
        {
            // No item - show dimmed color (outline only)
            iconImage.color = emptyColor;
            
            // Update background color
            if (backgroundImage != null)
            {
                backgroundImage.color = emptyBackgroundColor;
            }
            
            if (showDebugInfo) Debug.Log($"BackpackSlotUI: Showing empty state for '{itemData.itemName}'");
        }
    }
    
    /// <summary>
    /// Handle slot click (for future expansion)
    /// </summary>
    public void OnSlotClicked()
    {
        if (itemData == null) return;
        
        if (showDebugInfo) Debug.Log($"BackpackSlotUI: Clicked slot for '{itemData.itemName}'");
        
        // Future: Could show item details, usage options, etc.
        // For now, just log the click
    }
    
    /// <summary>
    /// Get the item data for this slot
    /// </summary>
    public ItemData GetItemData()
    {
        return itemData;
    }
    
    /// <summary>
    /// Set highlight state for this slot
    /// </summary>
    public void SetHighlight(bool highlight)
    {
        if (backgroundImage == null) return;
        
        if (highlight)
        {
            backgroundImage.color = highlightBackgroundColor;
        }
        else
        {
            // Restore normal background color based on item state
            if (itemData != null && inventorySystem != null)
            {
                int quantity = inventorySystem.GetItemQuantity(itemData);
                bool hasItem = quantity > 0;
                backgroundImage.color = hasItem ? collectedBackgroundColor : emptyBackgroundColor;
            }
        }
    }
    
    /// <summary>
    /// Check if this slot represents a collected item
    /// </summary>
    public bool IsItemCollected()
    {
        if (itemData == null || inventorySystem == null)
            return false;
            
        return inventorySystem.GetItemQuantity(itemData) > 0;
    }
}
