using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

/// <summary>
/// Editor tool for creating the simplified backpack UI
/// Creates a simple container with slots directly in the top-right corner
/// </summary>
public class SimpleBackpackSetupTool : EditorWindow
{
    [MenuItem("Tools/Inventory/Setup Simple Backpack UI")]
    public static void ShowWindow()
    {
        GetWindow<SimpleBackpackSetupTool>("Simple Backpack Setup");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Simple Backpack UI Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "This tool will create a simplified backpack UI:\n" +
            "• Just a container in the top-right corner\n" +
            "• Slots directly inside the container\n" +
            "• No complex hierarchy\n" +
            "• Minimal visual elements",
            MessageType.Info
        );
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Create Simple Backpack UI", GUILayout.Height(30)))
        {
            CreateSimpleBackpackUI();
        }
        
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "After creation:\n" +
            "1. The container will be positioned in the top-right corner\n" +
            "2. Slots will be created for all items in InventorySystem\n" +
            "3. Each slot shows just an icon (colored if collected, dimmed if not)\n" +
            "4. No background, no text, no complex UI elements",
            MessageType.Info
        );
    }
    
    private void CreateSimpleBackpackUI()
    {
        // Find or create Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("SimpleBackpackSetupTool: No Canvas found in scene!");
            EditorUtility.DisplayDialog("Error", "No Canvas found in scene. Please create a Canvas first.", "OK");
            return;
        }
        
        // Check if SimpleBackpackUI already exists
        SimpleBackpackUI existingUI = FindFirstObjectByType<SimpleBackpackUI>();
        if (existingUI != null)
        {
            bool replace = EditorUtility.DisplayDialog(
                "Simple Backpack UI Exists",
                "A SimpleBackpackUI already exists in the scene. Do you want to replace it?",
                "Replace", "Cancel"
            );
            
            if (!replace) return;
            
            DestroyImmediate(existingUI.gameObject);
        }
        
        // Create main container
        GameObject container = CreateContainer(canvas);
        
        // Create slot prefab
        GameObject slotPrefab = CreateSlotPrefab();
        
        // Create SimpleBackpackUI component
        SimpleBackpackUI backpackUI = container.AddComponent<SimpleBackpackUI>();
        
        // Assign references
        SerializedObject serializedBackpack = new SerializedObject(backpackUI);
        serializedBackpack.FindProperty("backpackContainer").objectReferenceValue = container;
        serializedBackpack.FindProperty("slotPrefab").objectReferenceValue = slotPrefab;
        serializedBackpack.ApplyModifiedProperties();
        
        // Save prefab
        SaveSlotPrefab(slotPrefab);
        
        Debug.Log("SimpleBackpackSetupTool: Simple Backpack UI created successfully!");
        
        EditorUtility.DisplayDialog(
            "Success",
            "Simple Backpack UI created successfully!\n\n" +
            "Features:\n" +
            "• Container positioned in top-right corner\n" +
            "• Simple slots with just icons\n" +
            "• No complex hierarchy\n" +
            "• Minimal visual design",
            "OK"
        );
        
        // Select the created container
        Selection.activeGameObject = container;
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
        
        // Set initial position (will be calculated by SimpleBackpackUI)
        rectTransform.anchoredPosition = new Vector2(-20, -20);
        rectTransform.sizeDelta = new Vector2(300, 200);
        
        // Add Image component for optional background (transparent by default)
        Image containerImage = container.AddComponent<Image>();
        containerImage.color = new Color(0, 0, 0, 0); // Fully transparent
        
        return container;
    }
    
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
        
        // Assign icon image reference
        SerializedObject serializedSlot = new SerializedObject(slotUI);
        serializedSlot.FindProperty("iconImage").objectReferenceValue = iconImage;
        serializedSlot.ApplyModifiedProperties();
        
        // Add Button component for click handling (optional)
        Button button = slot.AddComponent<Button>();
        button.targetGraphic = iconImage;
        
        return slot;
    }
    
    private void SaveSlotPrefab(GameObject slotPrefab)
    {
        // Create prefab in Assets/Prefabs/UI/ folder
        string prefabPath = "Assets/Prefabs/UI/SimpleSlot.prefab";
        
        // Ensure directory exists
        string directory = System.IO.Path.GetDirectoryName(prefabPath);
        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
        }
        
        // Create prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(slotPrefab, prefabPath);
        
        // Destroy the temporary GameObject
        DestroyImmediate(slotPrefab);
        
        Debug.Log($"SimpleBackpackSetupTool: Slot prefab saved to {prefabPath}");
    }
}
