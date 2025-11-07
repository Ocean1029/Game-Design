using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Editor tool to create a backpack UI system
/// Creates a container in the top-right corner with slots for all game items
/// Shows item outlines when not collected, full images when collected
/// </summary>
public class BackpackUICreator : EditorWindow
{
    [MenuItem("Tools/Inventory/Create Backpack UI")]
    public static void ShowWindow()
    {
        GetWindow<BackpackUICreator>("Backpack UI Creator");
    }
    
    [Header("UI Settings")]
    private int slotsPerRow = 4;
    private float slotSize = 60f;
    private float slotSpacing = 10f;
    private float containerMargin = 20f;
    
    [Header("Visual Settings")]
    private Color emptySlotColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
    private Color collectedSlotColor = Color.white;
    private Color containerBackgroundColor = new Color(0, 0, 0, 0.1f);
    
    void OnGUI()
    {
        GUILayout.Label("Backpack UI Creator", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "This tool will create a backpack UI system:\n" +
            "• Container positioned in top-right corner\n" +
            "• Slots for all game items\n" +
            "• Empty slots show item outlines\n" +
            "• Collected slots show full images\n" +
            "• Automatic layout and positioning",
            MessageType.Info
        );
        
        GUILayout.Space(10);
        
        // Settings
        GUILayout.Label("Layout Settings", EditorStyles.boldLabel);
        slotsPerRow = EditorGUILayout.IntField("Slots Per Row", slotsPerRow);
        slotSize = EditorGUILayout.FloatField("Slot Size", slotSize);
        slotSpacing = EditorGUILayout.FloatField("Slot Spacing", slotSpacing);
        containerMargin = EditorGUILayout.FloatField("Container Margin", containerMargin);
        
        GUILayout.Space(10);
        
        GUILayout.Label("Visual Settings", EditorStyles.boldLabel);
        emptySlotColor = EditorGUILayout.ColorField("Empty Slot Color", emptySlotColor);
        collectedSlotColor = EditorGUILayout.ColorField("Collected Slot Color", collectedSlotColor);
        containerBackgroundColor = EditorGUILayout.ColorField("Container Background", containerBackgroundColor);
        
        GUILayout.Space(20);
        
        if (GUILayout.Button("Create Backpack UI", GUILayout.Height(40)))
        {
            CreateBackpackUI();
        }
        
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "After creation:\n" +
            "• BackpackContainer will appear in Hierarchy\n" +
            "• Positioned in top-right corner\n" +
            "• Slots will be created for all items\n" +
            "• Ready to use immediately",
            MessageType.Info
        );
    }
    
    private void CreateBackpackUI()
    {
        Debug.Log("BackpackUICreator: Starting creation...");
        
        // Find Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("BackpackUICreator: No Canvas found in scene!");
            EditorUtility.DisplayDialog("Error", "No Canvas found in scene. Please create a Canvas first.", "OK");
            return;
        }
        
        // Find InventorySystem
        InventorySystem inventorySystem = FindFirstObjectByType<InventorySystem>();
        if (inventorySystem == null)
        {
            Debug.LogError("BackpackUICreator: No InventorySystem found in scene!");
            EditorUtility.DisplayDialog("Error", "No InventorySystem found in scene. Please add InventorySystem first.", "OK");
            return;
        }
        
        // Check if BackpackUI already exists
        BackpackUIController existingUI = FindFirstObjectByType<BackpackUIController>();
        if (existingUI != null)
        {
            bool replace = EditorUtility.DisplayDialog(
                "Backpack UI Exists",
                "A BackpackUI already exists in the scene. Do you want to replace it?",
                "Replace", "Cancel"
            );
            
            if (!replace) return;
            
            Debug.Log("BackpackUICreator: Removing existing BackpackUI");
            DestroyImmediate(existingUI.gameObject);
        }
        
        // Create the backpack system
        CreateBackpackSystem(canvas, inventorySystem);
        
        Debug.Log("BackpackUICreator: Creation complete!");
        
        EditorUtility.DisplayDialog(
            "Success",
            "Backpack UI created successfully!\n\n" +
            "Features:\n" +
            "• Top-right positioning\n" +
            "• Automatic slot creation\n" +
            "• Item state visualization\n" +
            "• Ready to use",
            "OK"
        );
    }
    
    private void CreateBackpackSystem(Canvas canvas, InventorySystem inventorySystem)
    {
        // Create main container
        GameObject container = CreateContainer(canvas);
        
        // Create slot prefab
        GameObject slotPrefab = CreateSlotPrefab();
        
        // Create controller
        BackpackUIController controller = container.AddComponent<BackpackUIController>();
        
        // Set controller properties
        controller.slotsPerRow = slotsPerRow;
        controller.slotSize = slotSize;
        controller.slotSpacing = slotSpacing;
        controller.containerMargin = containerMargin;
        controller.emptySlotColor = emptySlotColor;
        controller.collectedSlotColor = collectedSlotColor;
        controller.containerBackgroundColor = containerBackgroundColor;
        
        // Set references
        controller.backpackContainer = container;
        controller.slotPrefab = slotPrefab;
        controller.inventorySystem = inventorySystem;
        
        Debug.Log("BackpackUICreator: Backpack system created");
    }
    
    private GameObject CreateContainer(Canvas canvas)
    {
        // Create container GameObject
        GameObject container = new GameObject("BackpackContainer");
        container.transform.SetParent(canvas.transform, false);
        
        // Add RectTransform
        RectTransform rectTransform = container.AddComponent<RectTransform>();
        
        // Set anchors to top-right corner
        rectTransform.anchorMin = new Vector2(1, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(1, 1);
        
        // Set initial position (will be calculated by controller)
        rectTransform.anchoredPosition = new Vector2(-containerMargin, -containerMargin);
        rectTransform.sizeDelta = new Vector2(300, 200);
        
        // Add Image component for background
        Image containerImage = container.AddComponent<Image>();
        containerImage.color = containerBackgroundColor;
        
        return container;
    }
    
    private GameObject CreateSlotPrefab()
    {
        // Create slot GameObject
        GameObject slot = new GameObject("BackpackSlot");
        
        // Add RectTransform
        RectTransform rectTransform = slot.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(slotSize, slotSize);
        
        // Add background Image component
        GameObject background = new GameObject("Background");
        background.transform.SetParent(slot.transform, false);
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
        
        Image backgroundImage = background.AddComponent<Image>();
        backgroundImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // Add icon Image component
        GameObject icon = new GameObject("Icon");
        icon.transform.SetParent(slot.transform, false);
        RectTransform iconRect = icon.AddComponent<RectTransform>();
        iconRect.anchorMin = Vector2.zero;
        iconRect.anchorMax = Vector2.one;
        iconRect.sizeDelta = Vector2.zero;
        iconRect.anchoredPosition = Vector2.zero;
        
        Image iconImage = icon.AddComponent<Image>();
        iconImage.preserveAspect = true;
        
        // Add BackpackSlotUI component
        BackpackSlotUI slotUI = slot.AddComponent<BackpackSlotUI>();
        slotUI.iconImage = iconImage;
        slotUI.backgroundImage = backgroundImage;
        slotUI.emptyColor = emptySlotColor;
        slotUI.collectedColor = collectedSlotColor;
        
        // Add Button component for interaction
        Button button = slot.AddComponent<Button>();
        button.targetGraphic = backgroundImage;
        
        return slot;
    }
}
