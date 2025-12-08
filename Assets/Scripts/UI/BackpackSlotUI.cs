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
    private BackpackSlotConsumeEffect consumeEffect;
    
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
        
        // 添加消耗效果組件
        consumeEffect = GetComponent<BackpackSlotConsumeEffect>();
        if (consumeEffect == null && iconImage != null)
        {
            consumeEffect = iconImage.gameObject.AddComponent<BackpackSlotConsumeEffect>();
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
        UpdateDisplay(false);
    }
    
    /// <summary>
    /// Update the visual display of this slot
    /// </summary>
    /// <param name="playConsumeAnimation">是否播放消耗動畫</param>
    public void UpdateDisplay(bool playConsumeAnimation)
    {
        // Always log for debugging (temporarily)
        string itemName = itemData != null ? itemData.itemName : "NULL";
        Debug.Log($"[BackpackSlotUI] UpdateDisplay called for: {itemName}, playAnimation: {playConsumeAnimation}");
        
        if (itemData == null || inventorySystem == null || iconImage == null)
        {
            Debug.LogWarning($"[BackpackSlotUI] UpdateDisplay FAILED for '{itemName}' - itemData: {itemData != null}, inventorySystem: {inventorySystem != null}, iconImage: {iconImage != null}");
            return;
        }
        
        // Check if player has this item
        int quantity = inventorySystem.GetItemQuantity(itemData);
        bool hasItem = quantity > 0;
        
        Debug.Log($"[BackpackSlotUI] '{itemName}' - quantity: {quantity}, hasItem: {hasItem}");
        
        // 如果物品從有變成沒有，且需要播放動畫
        bool wasCollected = iconImage.color == collectedColor;
        if (wasCollected && !hasItem && playConsumeAnimation)
        {
            // 播放消耗動畫
            if (consumeEffect != null)
            {
                consumeEffect.PlayConsumeAnimation();
                Debug.Log($"[BackpackSlotUI] Playing consume animation for '{itemName}'");
            }
        }
        
        // Update visual state
        if (hasItem)
        {
            // Has item - show full color (complete image)
            if (iconImage != null)
            {
                iconImage.enabled = true;
                iconImage.color = collectedColor;
            }
            
            // Update background color
            if (backgroundImage != null)
            {
                backgroundImage.enabled = true;
                backgroundImage.color = collectedBackgroundColor;
            }
            
            if (showDebugInfo) Debug.Log($"BackpackSlotUI: Showing collected state for '{itemData.itemName}' - color: {collectedColor}");
        }
        else
        {
            // No item - hide the slot completely
            if (iconImage != null)
            {
                iconImage.enabled = false;
            }
            
            if (backgroundImage != null)
            {
                backgroundImage.enabled = false;
            }
            
            Debug.Log($"[BackpackSlotUI] Hiding empty slot for '{itemName}'");
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
    
    /// <summary>
    /// 動畫結束後刷新顯示狀態（確保變成半透明色）
    /// </summary>
    public void RefreshDisplayAfterAnimation()
    {
        if (itemData == null || inventorySystem == null || iconImage == null)
            return;
        
        int quantity = inventorySystem.GetItemQuantity(itemData);
        bool hasItem = quantity > 0;
        
        Debug.Log($"[BackpackSlotUI] RefreshDisplayAfterAnimation for '{itemData.itemName}' - hasItem: {hasItem}");
        
        if (!hasItem)
        {
            // 確保設置為半透明的未擁有狀態
            iconImage.color = emptyColor;
            
            if (backgroundImage != null)
            {
                backgroundImage.color = emptyBackgroundColor;
            }
            
            Debug.Log($"[BackpackSlotUI] Set to empty state (color: {emptyColor})");
        }
    }
}
