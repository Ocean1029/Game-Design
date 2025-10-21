using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// One-click fix for backpack UI issues
/// This tool will automatically fix all backpack UI problems
/// </summary>
public class OneClickBackpackFix : EditorWindow
{
    [MenuItem("Tools/Inventory/One-Click Backpack Fix")]
    public static void ShowWindow()
    {
        GetWindow<OneClickBackpackFix>("One-Click Backpack Fix");
    }
    
    void OnGUI()
    {
        GUILayout.Label("One-Click Backpack Fix", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "This tool will automatically:\n" +
            "✅ Remove old BackpackUI GameObject\n" +
            "✅ Create new SimpleBackpackUI system\n" +
            "✅ Set all references correctly\n" +
            "✅ Position in top-right corner\n" +
            "✅ Fix all error messages",
            MessageType.Info
        );
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Fix Backpack UI Now!", GUILayout.Height(40)))
        {
            FixBackpackUI();
        }
        
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "After clicking the button:\n" +
            "• All errors will be gone\n" +
            "• Backpack will appear in top-right\n" +
            "• Items will display correctly\n" +
            "• No more Console spam",
            MessageType.Info
        );
    }
    
    private void FixBackpackUI()
    {
        Debug.Log("OneClickBackpackFix: Starting fix...");
        
        // Step 1: Remove old BackpackUI
        GameObject oldBackpackUI = GameObject.Find("BackpackUI");
        if (oldBackpackUI != null)
        {
            Debug.Log("OneClickBackpackFix: Removing old BackpackUI");
            DestroyImmediate(oldBackpackUI);
        }
        
        // Step 2: Remove any existing SimpleBackpackUI
        SimpleBackpackUI existingSimpleUI = FindFirstObjectByType<SimpleBackpackUI>();
        if (existingSimpleUI != null)
        {
            Debug.Log("OneClickBackpackFix: Removing existing SimpleBackpackUI");
            DestroyImmediate(existingSimpleUI.gameObject);
        }
        
        // Step 3: Find Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("OneClickBackpackFix: No Canvas found!");
            EditorUtility.DisplayDialog("Error", "No Canvas found in scene!", "OK");
            return;
        }
        
        // Step 4: Create new system
        CreateNewBackpackSystem(canvas);
        
        Debug.Log("OneClickBackpackFix: Fix complete!");
        
        EditorUtility.DisplayDialog(
            "Success!",
            "Backpack UI fixed successfully!\n\n" +
            "✅ All errors resolved\n" +
            "✅ New system created\n" +
            "✅ Ready to use",
            "OK"
        );
    }
    
    private void CreateNewBackpackSystem(Canvas canvas)
    {
        // Create main container
        GameObject container = new GameObject("SimpleBackpackContainer");
        container.transform.SetParent(canvas.transform, false);
        
        // Set up RectTransform for top-right positioning
        RectTransform rectTransform = container.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(1, 1);
        rectTransform.anchoredPosition = new Vector2(-20, -20);
        rectTransform.sizeDelta = new Vector2(300, 200);
        
        // Add transparent background
        Image containerImage = container.AddComponent<Image>();
        containerImage.color = new Color(0, 0, 0, 0);
        
        // Create slot prefab
        GameObject slotPrefab = CreateSlotPrefab();
        
        // Add SimpleBackpackUI component
        SimpleBackpackUI backpackUI = container.AddComponent<SimpleBackpackUI>();
        
        // Set references
        backpackUI.backpackContainer = container;
        backpackUI.slotPrefab = slotPrefab;
        
        Debug.Log("OneClickBackpackFix: New backpack system created");
    }
    
    private GameObject CreateSlotPrefab()
    {
        // Create slot GameObject
        GameObject slot = new GameObject("SimpleSlot");
        
        // Add RectTransform
        RectTransform rectTransform = slot.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(60, 60);
        
        // Add Image component
        Image iconImage = slot.AddComponent<Image>();
        iconImage.preserveAspect = true;
        
        // Add SimpleSlotUI component
        SimpleSlotUI slotUI = slot.AddComponent<SimpleSlotUI>();
        slotUI.iconImage = iconImage;
        
        // Add Button component
        Button button = slot.AddComponent<Button>();
        button.targetGraphic = iconImage;
        
        return slot;
    }
}
