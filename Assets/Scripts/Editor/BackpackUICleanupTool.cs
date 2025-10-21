using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// Tool to clean up existing backpack UI and set up the simple version
/// </summary>
public class BackpackUICleanupTool : EditorWindow
{
    [MenuItem("Tools/Inventory/Cleanup & Setup Simple Backpack")]
    public static void ShowWindow()
    {
        GetWindow<BackpackUICleanupTool>("Backpack UI Cleanup");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Backpack UI Cleanup & Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "This tool will:\n" +
            "1. Remove existing BackpackUI GameObject\n" +
            "2. Create new SimpleBackpackUI system\n" +
            "3. Set up all necessary references\n" +
            "4. Position in top-right corner",
            MessageType.Info
        );
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Cleanup & Setup Simple Backpack", GUILayout.Height(30)))
        {
            CleanupAndSetup();
        }
        
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "After cleanup:\n" +
            "• Old BackpackUI will be removed\n" +
            "• New SimpleBackpackUI will be created\n" +
            "• All references will be properly set\n" +
            "• No more error messages",
            MessageType.Info
        );
    }
    
    private void CleanupAndSetup()
    {
        // Step 1: Find and remove existing BackpackUI
        GameObject existingBackpackUI = GameObject.Find("BackpackUI");
        if (existingBackpackUI != null)
        {
            Debug.Log("BackpackUICleanupTool: Removing existing BackpackUI GameObject");
            DestroyImmediate(existingBackpackUI);
        }
        
        // Step 2: Find and remove any existing SimpleBackpackUI
        SimpleBackpackUI existingSimpleUI = FindFirstObjectByType<SimpleBackpackUI>();
        if (existingSimpleUI != null)
        {
            Debug.Log("BackpackUICleanupTool: Removing existing SimpleBackpackUI");
            DestroyImmediate(existingSimpleUI.gameObject);
        }
        
        // Step 3: Find Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("BackpackUICleanupTool: No Canvas found in scene!");
            EditorUtility.DisplayDialog("Error", "No Canvas found in scene. Please create a Canvas first.", "OK");
            return;
        }
        
        // Step 4: Create new SimpleBackpackUI system
        CreateSimpleBackpackSystem(canvas);
        
        Debug.Log("BackpackUICleanupTool: Cleanup and setup complete!");
        
        EditorUtility.DisplayDialog(
            "Success",
            "Backpack UI cleanup and setup complete!\n\n" +
            "✅ Removed old BackpackUI\n" +
            "✅ Created new SimpleBackpackUI\n" +
            "✅ Set up all references\n" +
            "✅ Positioned in top-right corner\n\n" +
            "No more error messages!",
            "OK"
        );
    }
    
    private void CreateSimpleBackpackSystem(Canvas canvas)
    {
        // Create main container
        GameObject container = new GameObject("SimpleBackpackContainer");
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
        
        // Create slot prefab
        GameObject slotPrefab = CreateSlotPrefab();
        
        // Create SimpleBackpackUI component
        SimpleBackpackUI backpackUI = container.AddComponent<SimpleBackpackUI>();
        
        // Assign references using SerializedObject
        SerializedObject serializedBackpack = new SerializedObject(backpackUI);
        serializedBackpack.FindProperty("backpackContainer").objectReferenceValue = container;
        serializedBackpack.FindProperty("slotPrefab").objectReferenceValue = slotPrefab;
        serializedBackpack.ApplyModifiedProperties();
        
        // Save slot prefab
        SaveSlotPrefab(slotPrefab);
        
        Debug.Log("BackpackUICleanupTool: SimpleBackpackUI system created successfully");
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
        
        Debug.Log($"BackpackUICleanupTool: Slot prefab saved to {prefabPath}");
    }
}
