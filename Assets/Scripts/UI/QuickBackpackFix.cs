using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Quick fix script for SimpleBackpackUI slotPrefab error
/// Attach this to any GameObject and it will fix the issue automatically
/// </summary>
public class QuickBackpackFix : MonoBehaviour
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
    /// Fix SimpleBackpackUI slotPrefab error
    /// </summary>
    [ContextMenu("Fix Backpack UI")]
    public void FixBackpackUI()
    {
        if (showDebugInfo) Debug.Log("QuickBackpackFix: Starting fix...");
        
        // Find SimpleBackpackUI
        SimpleBackpackUI backpackUI = FindFirstObjectByType<SimpleBackpackUI>();
        if (backpackUI == null)
        {
            Debug.LogError("QuickBackpackFix: No SimpleBackpackUI found in scene!");
            Debug.LogError("QuickBackpackFix: Please create a SimpleBackpackUI GameObject first");
            return;
        }
        
        if (showDebugInfo) Debug.Log("QuickBackpackFix: Found SimpleBackpackUI, checking configuration...");
        
        bool fixed = false;
        
        // Fix backpackContainer if null
        if (backpackUI.backpackContainer == null)
        {
            backpackUI.backpackContainer = backpackUI.gameObject;
            if (showDebugInfo) Debug.Log("QuickBackpackFix: Fixed backpackContainer reference");
            fixed = true;
        }
        
        // Fix slotPrefab if null
        if (backpackUI.slotPrefab == null)
        {
            GameObject slotPrefab = CreateSlotPrefab();
            backpackUI.slotPrefab = slotPrefab;
            if (showDebugInfo) Debug.Log("QuickBackpackFix: Created and assigned slotPrefab");
            fixed = true;
        }
        
        if (fixed)
        {
            if (showDebugInfo) Debug.Log("QuickBackpackFix: Fix complete! Restart the game to see changes.");
        }
        else
        {
            if (showDebugInfo) Debug.Log("QuickBackpackFix: No fixes needed, SimpleBackpackUI is already properly configured");
        }
    }
    
    /// <summary>
    /// Create a slot prefab for SimpleBackpackUI
    /// </summary>
    private GameObject CreateSlotPrefab()
    {
        // Create slot GameObject
        GameObject slot = new GameObject("SimpleSlot");
        
        // Add RectTransform
        RectTransform rectTransform = slot.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(60, 60);
        
        // Add Image component for the icon
        Image iconImage = slot.AddComponent<Image>();
        iconImage.preserveAspect = true; // Keep icon proportions
        
        // Add SimpleSlotUI component
        SimpleSlotUI slotUI = slot.AddComponent<SimpleSlotUI>();
        slotUI.iconImage = iconImage;
        
        // Add Button component for click handling
        Button button = slot.AddComponent<Button>();
        button.targetGraphic = iconImage;
        
        if (showDebugInfo) Debug.Log("QuickBackpackFix: Created SimpleSlot prefab");
        
        return slot;
    }
}
