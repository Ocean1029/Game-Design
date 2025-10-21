using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Simple slot UI component - just an icon that shows if item is collected or not
/// No complex hierarchy, just a single image component
/// </summary>
public class SimpleSlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] public Image iconImage;
    
    [Header("Settings")]
    [SerializeField] private Color emptyColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
    [SerializeField] private Color collectedColor = Color.white;
    
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
        
        // Initial display update
        UpdateDisplay();
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
            // Has item - show full color
            iconImage.color = collectedColor;
        }
        else
        {
            // No item - show dimmed
            iconImage.color = emptyColor;
        }
    }
    
    /// <summary>
    /// Handle slot click (optional - for future expansion)
    /// </summary>
    public void OnSlotClicked()
    {
        // Could be used for item details, usage, etc.
        Debug.Log($"Clicked slot for: {itemData?.itemName}");
    }
}