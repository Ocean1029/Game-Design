using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Controller for the backpack UI system
/// Manages the container and slots, handles item state updates
/// </summary>
public class BackpackUIController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject backpackContainer;
    public GameObject slotPrefab;
    public InventorySystem inventorySystem;
    
    [Header("Layout Settings")]
    public int slotsPerRow = 4;
    public float slotSize = 60f;
    public float slotSpacing = 10f;
    public float containerMargin = 20f;
    
    [Header("Visual Settings")]
    public Color emptySlotColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
    public Color collectedSlotColor = Color.white;
    public Color containerBackgroundColor = new Color(0, 0, 0, 0.1f);
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    
    // Slot management
    private Dictionary<string, BackpackSlotUI> slotMap = new Dictionary<string, BackpackSlotUI>();
    
    void Start()
    {
        if (showDebugInfo) Debug.Log("BackpackUIController: Start called");
        
        // Validate references
        if (backpackContainer == null)
        {
            Debug.LogError("BackpackUIController: backpackContainer not assigned!");
            return;
        }
        
        if (slotPrefab == null)
        {
            Debug.LogError("BackpackUIController: slotPrefab not assigned!");
            return;
        }
        
        if (inventorySystem == null)
        {
            Debug.LogError("BackpackUIController: inventorySystem not assigned!");
            return;
        }
        
        // Subscribe to inventory events
        inventorySystem.OnItemCollected += OnItemCollected;
        inventorySystem.OnItemUsed += OnItemUsed;
        inventorySystem.OnItemRemoved += OnItemRemoved;
        
        // Create slots for all game items
        CreateSlots();
        
        // Position container
        PositionContainer();
        
        if (showDebugInfo) Debug.Log("BackpackUIController: Setup complete");
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (inventorySystem != null)
        {
            inventorySystem.OnItemCollected -= OnItemCollected;
            inventorySystem.OnItemUsed -= OnItemUsed;
            inventorySystem.OnItemRemoved -= OnItemRemoved;
        }
    }
    
    /// <summary>
    /// Create slots for all items in the game
    /// </summary>
    private void CreateSlots()
    {
        List<ItemData> allGameItems = inventorySystem.GetAllGameItems();
        if (allGameItems == null || allGameItems.Count == 0)
        {
            Debug.LogWarning("BackpackUIController: No items in AllGameItems list!");
            return;
        }
        
        if (showDebugInfo) Debug.Log($"BackpackUIController: Creating slots for {allGameItems.Count} items");
        
        // Clear existing slots
        foreach (Transform child in backpackContainer.transform)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }
        slotMap.Clear();
        
        // Create slots for each item (filter out null items)
        int validItemCount = 0;
        for (int i = 0; i < allGameItems.Count; i++)
        {
            ItemData itemData = allGameItems[i];
            if (itemData != null)
            {
                CreateSlot(itemData, validItemCount);
                validItemCount++;
            }
            else
            {
                Debug.LogWarning($"BackpackUIController: Skipping null ItemData at index {i}");
            }
        }
        
        if (showDebugInfo) Debug.Log($"BackpackUIController: Created {slotMap.Count} slots from {validItemCount} valid items");
    }
    
    /// <summary>
    /// Create a single slot for an item
    /// </summary>
    private void CreateSlot(ItemData itemData, int index)
    {
        // Validate inputs
        if (itemData == null)
        {
            Debug.LogError("BackpackUIController: Cannot create slot for null ItemData!");
            return;
        }
        
        if (slotPrefab == null)
        {
            Debug.LogError("BackpackUIController: slotPrefab is null! Cannot create slot.");
            return;
        }
        
        if (backpackContainer == null)
        {
            Debug.LogError("BackpackUIController: backpackContainer is null! Cannot create slot.");
            return;
        }
        
        // Instantiate slot
        GameObject slotObj = Instantiate(slotPrefab, backpackContainer.transform);
        slotObj.name = $"Slot_{itemData.itemId}";
        
        // Get slot UI component
        BackpackSlotUI slotUI = slotObj.GetComponent<BackpackSlotUI>();
        if (slotUI == null)
        {
            Debug.LogError($"BackpackUIController: BackpackSlotUI component not found on slot prefab!");
            DestroyImmediate(slotObj);
            return;
        }
        
        // Setup slot
        slotUI.Setup(itemData, inventorySystem);
        
        // Position slot using grid layout
        PositionSlot(slotObj, index);
        
        // Add to map
        slotMap[itemData.itemId] = slotUI;
        
        if (showDebugInfo) Debug.Log($"BackpackUIController: Created slot for '{itemData.itemName}'");
    }
    
    /// <summary>
    /// Position a slot in the grid layout
    /// </summary>
    private void PositionSlot(GameObject slotObj, int index)
    {
        RectTransform rectTransform = slotObj.GetComponent<RectTransform>();
        if (rectTransform == null) return;
        
        // Calculate grid position
        int row = index / slotsPerRow;
        int col = index % slotsPerRow;
        
        // Calculate position
        float x = col * (slotSize + slotSpacing);
        float y = -row * (slotSize + slotSpacing);
        
        // Set position
        rectTransform.anchoredPosition = new Vector2(x, y);
        rectTransform.sizeDelta = new Vector2(slotSize, slotSize);
    }
    
    /// <summary>
    /// Position the container in the top-right corner
    /// </summary>
    private void PositionContainer()
    {
        RectTransform containerRect = backpackContainer.GetComponent<RectTransform>();
        if (containerRect == null) return;
        
        // Set anchors to top-right
        containerRect.anchorMin = new Vector2(1, 1);
        containerRect.anchorMax = new Vector2(1, 1);
        containerRect.pivot = new Vector2(1, 1);
        
        // Calculate container size based on slots
        List<ItemData> allGameItems = inventorySystem.GetAllGameItems();
        int totalSlots = allGameItems?.Count ?? 0;
        int rows = Mathf.CeilToInt((float)totalSlots / slotsPerRow);
        
        float containerWidth = slotsPerRow * slotSize + (slotsPerRow - 1) * slotSpacing;
        float containerHeight = rows * slotSize + (rows - 1) * slotSpacing;
        
        // Set container size and position
        containerRect.sizeDelta = new Vector2(containerWidth, containerHeight);
        containerRect.anchoredPosition = new Vector2(-containerMargin, -containerMargin);
        
        // Update background color
        Image containerImage = backpackContainer.GetComponent<Image>();
        if (containerImage != null)
        {
            containerImage.color = containerBackgroundColor;
        }
        
        if (showDebugInfo) Debug.Log($"BackpackUIController: Container positioned at top-right, size: {containerWidth}x{containerHeight}");
    }
    
    #region Event Handlers
    
    private void OnItemCollected(ItemData itemData, int quantity)
    {
        if (slotMap.ContainsKey(itemData.itemId))
        {
            slotMap[itemData.itemId].UpdateDisplay();
            if (showDebugInfo) Debug.Log($"BackpackUIController: Updated slot for collected item '{itemData.itemName}'");
        }
    }
    
    private void OnItemUsed(ItemData itemData)
    {
        if (slotMap.ContainsKey(itemData.itemId))
        {
            slotMap[itemData.itemId].UpdateDisplay();
            if (showDebugInfo) Debug.Log($"BackpackUIController: Updated slot for used item '{itemData.itemName}'");
        }
    }
    
    private void OnItemRemoved(ItemData itemData)
    {
        if (slotMap.ContainsKey(itemData.itemId))
        {
            if (showDebugInfo) Debug.Log($"BackpackUIController: Removing item '{itemData.itemName}' (ID: {itemData.itemId})");
            slotMap[itemData.itemId].UpdateDisplay();
            if (showDebugInfo) Debug.Log($"BackpackUIController: Updated slot for removed item '{itemData.itemName}'");
        }
        else
        {
            if (showDebugInfo) Debug.LogWarning($"BackpackUIController: Slot not found for item '{itemData.itemName}' (ID: {itemData.itemId})");
        }
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Refresh all slots display
    /// </summary>
    public void RefreshAllSlots()
    {
        foreach (var slot in slotMap.Values)
        {
            slot.UpdateDisplay();
        }
        
        if (showDebugInfo) Debug.Log("BackpackUIController: Refreshed all slots");
    }
    
    /// <summary>
    /// Check if the backpack is visible
    /// </summary>
    public bool IsVisible()
    {
        return backpackContainer != null && backpackContainer.activeInHierarchy;
    }
    
    #endregion
}
