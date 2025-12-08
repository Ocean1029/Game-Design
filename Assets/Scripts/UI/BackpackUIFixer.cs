using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Runtime fix for backpack UI issues
/// This script can be attached to any GameObject to fix backpack UI problems
/// </summary>
public class BackpackUIFixer : MonoBehaviour
{
    [Header("Fix Options")]
    [SerializeField] private bool fixOnStart = true;
    [SerializeField] private bool showDebugInfo = true;
    
    void Start()
    {
        if (fixOnStart)
        {
            FixBackpackUI();
        }
    }
    
    /// <summary>
    /// Fix backpack UI issues at runtime
    /// </summary>
    [ContextMenu("Fix Backpack UI")]
    public void FixBackpackUI()
    {
        if (showDebugInfo) Debug.Log("BackpackUIFixer: Starting backpack UI fix...");
        
        // Step 1: Find and disable problematic BackpackUI
        BackpackUI oldBackpackUI = FindFirstObjectByType<BackpackUI>();
        if (oldBackpackUI != null)
        {
            if (showDebugInfo) Debug.Log("BackpackUIFixer: Found old BackpackUI, disabling it");
            oldBackpackUI.gameObject.SetActive(false);
        }
        
        // Step 2: Check if SimpleBackpackUI exists and is properly configured
        SimpleBackpackUI simpleBackpackUI = FindFirstObjectByType<SimpleBackpackUI>();
        if (simpleBackpackUI != null)
        {
            if (showDebugInfo) Debug.Log("BackpackUIFixer: Found SimpleBackpackUI, checking configuration");
            
            // Check if references are missing
            bool needsFix = false;
            
            // Check backpackContainer
            if (simpleBackpackUI.backpackContainer == null)
            {
                if (showDebugInfo) Debug.Log("BackpackUIFixer: backpackContainer is null, setting to self");
                simpleBackpackUI.backpackContainer = simpleBackpackUI.gameObject;
                needsFix = true;
            }
            
            // Check slotPrefab
            if (simpleBackpackUI.slotPrefab == null)
            {
                if (showDebugInfo) Debug.Log("BackpackUIFixer: slotPrefab is null, creating one");
                GameObject slotPrefab = CreateSlotPrefab();
                simpleBackpackUI.slotPrefab = slotPrefab;
                needsFix = true;
            }
            
            if (needsFix)
            {
                if (showDebugInfo) Debug.Log("BackpackUIFixer: Fixed SimpleBackpackUI references");
            }
            else
            {
                if (showDebugInfo) Debug.Log("BackpackUIFixer: SimpleBackpackUI is already properly configured");
            }
        }
        else
        {
            if (showDebugInfo) Debug.Log("BackpackUIFixer: No SimpleBackpackUI found, creating one");
            CreateSimpleBackpackUI();
        }
        
        if (showDebugInfo) Debug.Log("BackpackUIFixer: Backpack UI fix complete!");
    }
    
    /// <summary>
    /// Create a new SimpleBackpackUI system
    /// </summary>
    private void CreateSimpleBackpackUI()
    {
        // Find Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("BackpackUIFixer: No Canvas found!");
            return;
        }
        
        // Create container
        GameObject container = new GameObject("SimpleBackpackContainer");
        container.transform.SetParent(canvas.transform, false);
        
        // Set up RectTransform
        RectTransform rectTransform = container.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(1, 1);
        rectTransform.anchoredPosition = new Vector2(-20, -20);
        rectTransform.sizeDelta = new Vector2(300, 200);
        
        // Add Image component
        Image containerImage = container.AddComponent<Image>();
        containerImage.color = new Color(0, 0, 0, 0);
        
        // Create slot prefab
        GameObject slotPrefab = CreateSlotPrefab();
        
        // Add SimpleBackpackUI component
        SimpleBackpackUI backpackUI = container.AddComponent<SimpleBackpackUI>();
        
        // Set references directly
        backpackUI.backpackContainer = container;
        backpackUI.slotPrefab = slotPrefab;
        
        if (showDebugInfo) Debug.Log("BackpackUIFixer: Created new SimpleBackpackUI system");
    }
    
    /// <summary>
    /// Create a slot prefab
    /// </summary>
    private GameObject CreateSlotPrefab()
    {
        GameObject slot = new GameObject("SimpleSlot");
        
        RectTransform rectTransform = slot.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(60, 60);
        
        Image iconImage = slot.AddComponent<Image>();
        iconImage.preserveAspect = true;
        
        SimpleSlotUI slotUI = slot.AddComponent<SimpleSlotUI>();
        
        // Set icon image reference directly
        slotUI.iconImage = iconImage;
        
        Button button = slot.AddComponent<Button>();
        button.targetGraphic = iconImage;
        
        return slot;
    }
}
