using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Represents a single slot in the inventory UI
/// Shows item icon, quantity, and empty/filled state
/// </summary>
public class InventorySlotUI : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("Background image (frame)")]
    [SerializeField] private Image backgroundImage;
    
    [Tooltip("Item icon image")]
    [SerializeField] private Image iconImage;
    
    [Tooltip("Quantity text")]
    [SerializeField] private TextMeshProUGUI quantityText;
    
    [Tooltip("Overlay for empty state")]
    [SerializeField] private Image emptyOverlay;
    
    [Header("Visual Settings")]
    [Tooltip("Color when item is collected")]
    [SerializeField] private Color collectedColor = Color.white;
    
    [Tooltip("Color when item is not collected (empty)")]
    [SerializeField] private Color emptyColor = new Color(1f, 1f, 1f, 0.3f);
    
    [Tooltip("Background color when can be used")]
    [SerializeField] private Color canUseHighlight = new Color(1f, 1f, 0f, 0.5f);

    private ItemData itemData;
    private InventorySystem inventorySystem;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }

    /// <summary>
    /// Setup this slot with item data
    /// </summary>
    public void Setup(ItemData data, InventorySystem inventory)
    {
        itemData = data;
        inventorySystem = inventory;
        
        // Set icon
        if (iconImage != null && itemData.icon != null)
        {
            iconImage.sprite = itemData.icon;
        }
        
        // Initial display update
        UpdateDisplay();
    }

    /// <summary>
    /// Update the visual display based on inventory state
    /// </summary>
    public void UpdateDisplay()
    {
        if (itemData == null || inventorySystem == null) return;

        bool hasItem = inventorySystem.HasItem(itemData);
        int quantity = inventorySystem.GetItemQuantity(itemData);

        // Update icon color
        if (iconImage != null)
        {
            iconImage.color = hasItem ? collectedColor : emptyColor;
        }

        // Update quantity text
        if (quantityText != null)
        {
            if (hasItem && quantity > 1)
            {
                quantityText.gameObject.SetActive(true);
                quantityText.text = $"x{quantity}";
            }
            else
            {
                quantityText.gameObject.SetActive(false);
            }
        }

        // Update empty overlay
        if (emptyOverlay != null)
        {
            emptyOverlay.gameObject.SetActive(!hasItem);
        }
    }

    /// <summary>
    /// Highlight this slot (e.g., when item can be used)
    /// </summary>
    public void SetHighlight(bool highlighted)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = highlighted ? canUseHighlight : Color.white;
        }
    }

    /// <summary>
    /// Called when slot is clicked
    /// </summary>
    private void OnClick()
    {
        if (itemData == null || inventorySystem == null) return;

        bool hasItem = inventorySystem.HasItem(itemData);
        
        if (!hasItem)
        {
            Debug.Log($"You don't have {itemData.itemName} yet");
            return;
        }

        // Show item info or try to use it
        if (itemData.useType == ItemUseType.Manual)
        {
            bool success = inventorySystem.UseItem(itemData);
            if (!success)
            {
                Debug.Log($"{itemData.itemName}: {itemData.description}");
            }
        }
        else
        {
            Debug.Log($"{itemData.itemName}: {itemData.description}");
        }
    }

    /// <summary>
    /// Get the item data for this slot
    /// </summary>
    public ItemData GetItemData()
    {
        return itemData;
    }
}

