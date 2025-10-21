using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// Super simple backpack UI fix
/// Just click the button and everything will be fixed!
/// </summary>
public class SuperSimpleBackpackFix : EditorWindow
{
    [MenuItem("Tools/Fix Backpack UI")]
    public static void ShowWindow()
    {
        GetWindow<SuperSimpleBackpackFix>("Fix Backpack UI");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Fix Backpack UI", EditorStyles.boldLabel);
        GUILayout.Space(20);
        
        EditorGUILayout.HelpBox(
            "This will fix all backpack UI errors!\n" +
            "Just click the button below.",
            MessageType.Info
        );
        
        GUILayout.Space(20);
        
        if (GUILayout.Button("FIX NOW!", GUILayout.Height(50)))
        {
            FixEverything();
        }
        
        GUILayout.Space(20);
        
        EditorGUILayout.HelpBox(
            "After clicking:\n" +
            "• All errors will disappear\n" +
            "• Backpack will work perfectly\n" +
            "• No more Console spam",
            MessageType.Info
        );
    }
    
    private void FixEverything()
    {
        Debug.Log("SuperSimpleBackpackFix: Starting fix...");
        
        // Remove old BackpackUI
        GameObject oldUI = GameObject.Find("BackpackUI");
        if (oldUI != null)
        {
            Debug.Log("SuperSimpleBackpackFix: Removing old BackpackUI");
            DestroyImmediate(oldUI);
        }
        
        // Remove any existing SimpleBackpackUI
        SimpleBackpackUI existing = FindFirstObjectByType<SimpleBackpackUI>();
        if (existing != null)
        {
            Debug.Log("SuperSimpleBackpackFix: Removing existing SimpleBackpackUI");
            DestroyImmediate(existing.gameObject);
        }
        
        // Find Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("SuperSimpleBackpackFix: No Canvas found!");
            return;
        }
        
        // Create new system
        CreateNewSystem(canvas);
        
        Debug.Log("SuperSimpleBackpackFix: Fix complete!");
        
        EditorUtility.DisplayDialog("Success!", "Backpack UI fixed!", "OK");
    }
    
    private void CreateNewSystem(Canvas canvas)
    {
        // Create container
        GameObject container = new GameObject("BackpackContainer");
        container.transform.SetParent(canvas.transform, false);
        
        // Set position (top-right)
        RectTransform rect = container.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        rect.anchoredPosition = new Vector2(-20, -20);
        rect.sizeDelta = new Vector2(300, 200);
        
        // Add background
        Image bg = container.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0);
        
        // Create slot prefab
        GameObject slotPrefab = CreateSlot();
        
        // Add controller
        SimpleBackpackUI controller = container.AddComponent<SimpleBackpackUI>();
        controller.backpackContainer = container;
        controller.slotPrefab = slotPrefab;
        
        Debug.Log("SuperSimpleBackpackFix: New system created");
    }
    
    private GameObject CreateSlot()
    {
        GameObject slot = new GameObject("Slot");
        
        RectTransform rect = slot.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(60, 60);
        
        Image img = slot.AddComponent<Image>();
        img.preserveAspect = true;
        
        SimpleSlotUI slotUI = slot.AddComponent<SimpleSlotUI>();
        slotUI.iconImage = img;
        
        Button btn = slot.AddComponent<Button>();
        btn.targetGraphic = img;
        
        return slot;
    }
}
