using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Simplified Backpack UI - Just a container with slots in the top-right corner
/// No complex hierarchy, just slots directly in the container
/// </summary>
public class SimpleBackpackUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] public GameObject backpackContainer;
    [SerializeField] public GameObject slotPrefab;
    
    [Header("Settings")]
    [SerializeField] private int slotsPerRow = 4;
    [SerializeField] private float slotSize = 60f;
    [SerializeField] private float slotSpacing = 10f;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    
    // References
    private InventorySystem inventorySystem;
    private Dictionary<string, SimpleSlotUI> slotMap = new Dictionary<string, SimpleSlotUI>();
    
    void Start()
    {
        if (showDebugInfo) Debug.Log("SimpleBackpackUI: Start called");
        
        // Find inventory system
        inventorySystem = FindFirstObjectByType<InventorySystem>();
        if (inventorySystem == null)
        {
            Debug.LogError("SimpleBackpackUI: InventorySystem not found!");
            return;
        }
        
        // Validate references
        if (backpackContainer == null)
        {
            Debug.LogError("SimpleBackpackUI: backpackContainer not assigned!");
            return;
        }
        
        if (slotPrefab == null)
        {
            Debug.LogError("SimpleBackpackUI: slotPrefab not assigned!");
            return;
        }
        
        // Subscribe to inventory events
        inventorySystem.OnItemCollected += OnItemCollected;
        inventorySystem.OnItemUsed += OnItemUsed;
        inventorySystem.OnItemRemoved += OnItemRemoved;
        
        // Create slots for all game items
        CreateSlots();
        
        // Position container in top-right corner
        PositionContainer();
        
        if (showDebugInfo) Debug.Log("SimpleBackpackUI: Setup complete");
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
            Debug.LogWarning("SimpleBackpackUI: No items in AllGameItems list!");
            return;
        }
        
        if (showDebugInfo) Debug.Log($"SimpleBackpackUI: Creating slots for {allGameItems.Count} items");
        
        // Clear existing slots
        foreach (Transform child in backpackContainer.transform)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }
        slotMap.Clear();
        
        // Create slots for each item
        for (int i = 0; i < allGameItems.Count; i++)
        {
            ItemData itemData = allGameItems[i];
            CreateSlot(itemData, i);
        }
        
        if (showDebugInfo) Debug.Log($"SimpleBackpackUI: Created {slotMap.Count} slots");
    }
    
    /// <summary>
    /// Create a single slot for an item
    /// </summary>
    private void CreateSlot(ItemData itemData, int index)
    {
        // Instantiate slot
        GameObject slotObj = Instantiate(slotPrefab, backpackContainer.transform);
        slotObj.name = $"Slot_{itemData.itemId}";
        
        // Get slot UI component
        SimpleSlotUI slotUI = slotObj.GetComponent<SimpleSlotUI>();
        if (slotUI == null)
        {
            Debug.LogError($"SimpleBackpackUI: SimpleSlotUI component not found on slot prefab!");
            return;
        }
        
        // Setup slot
        slotUI.Setup(itemData, inventorySystem);
        
        // Position slot using simple grid layout
        PositionSlot(slotObj, index);
        
        // Add to map
        slotMap[itemData.itemId] = slotUI;
        
        if (showDebugInfo) Debug.Log($"SimpleBackpackUI: Created slot for '{itemData.itemName}'");
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
        containerRect.anchoredPosition = new Vector2(-20, -20); // 20px from edges
        
        if (showDebugInfo) Debug.Log($"SimpleBackpackUI: Container positioned at top-right, size: {containerWidth}x{containerHeight}");
    }
    
    #region Event Handlers
    
    private void OnItemCollected(ItemData itemData, int quantity)
    {
        if (slotMap.ContainsKey(itemData.itemId))
        {
            slotMap[itemData.itemId].UpdateDisplay();
        }
    }
    
    private void OnItemUsed(ItemData itemData)
    {
        if (slotMap.ContainsKey(itemData.itemId))
        {
            slotMap[itemData.itemId].UpdateDisplay();
        }
    }
    
    private void OnItemRemoved(ItemData itemData)
    {
        if (slotMap.ContainsKey(itemData.itemId))
        {
            slotMap[itemData.itemId].UpdateDisplay();
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
