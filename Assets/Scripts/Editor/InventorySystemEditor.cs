using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Custom inspector for InventorySystem to make setup easier
/// </summary>
[CustomEditor(typeof(InventorySystem))]
public class InventorySystemEditor : Editor
{
    private SerializedProperty maxInventorySlots;
    private SerializedProperty allGameItems;

    void OnEnable()
    {
        maxInventorySlots = serializedObject.FindProperty("maxInventorySlots");
        allGameItems = serializedObject.FindProperty("allGameItems");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        InventorySystem inventory = (InventorySystem)target;

        GUILayout.Label("Inventory System", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Settings
        EditorGUILayout.PropertyField(maxInventorySlots);
        EditorGUILayout.Space();

        // All Game Items
        EditorGUILayout.LabelField("All Game Items", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(allGameItems, true);

        EditorGUILayout.Space();

        // Quick actions
        GUILayout.Label("Quick Actions", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Auto Find All ItemData"))
        {
            AutoFindAllItems();
        }

        if (GUILayout.Button("Clear List"))
        {
            if (EditorUtility.DisplayDialog("Confirm", "Clear all items from list?", "Clear", "Cancel"))
            {
                allGameItems.ClearArray();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Runtime info (only in play mode)
        if (Application.isPlaying)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Runtime Info", EditorStyles.boldLabel);
            
            List<InventoryItem> items = inventory.GetAllItems();
            
            if (items.Count > 0)
            {
                EditorGUILayout.LabelField($"Current Items: {items.Count}");
                
                EditorGUILayout.Space();
                foreach (var item in items)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"• {item.itemData.itemName}", GUILayout.Width(150));
                    EditorGUILayout.LabelField($"x{item.quantity}", GUILayout.Width(50));
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Inventory is empty", MessageType.Info);
            }

            EditorGUILayout.Space();

            // Test buttons
            if (GUILayout.Button("Clear Inventory (Test)"))
            {
                inventory.ClearInventory();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void AutoFindAllItems()
    {
        // Find all ItemData assets
        string[] guids = AssetDatabase.FindAssets("t:ItemData");
        List<ItemData> foundItems = new List<ItemData>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
            if (item != null)
            {
                foundItems.Add(item);
            }
        }

        if (foundItems.Count == 0)
        {
            EditorUtility.DisplayDialog("Notice", "No ItemData assets found", "OK");
            return;
        }

        // Clear and add all
        allGameItems.ClearArray();
        
        foreach (ItemData item in foundItems.OrderBy(i => i.itemId))
        {
            allGameItems.InsertArrayElementAtIndex(allGameItems.arraySize);
            allGameItems.GetArrayElementAtIndex(allGameItems.arraySize - 1).objectReferenceValue = item;
        }

        serializedObject.ApplyModifiedProperties();

        EditorUtility.DisplayDialog("Success", 
            $"Found and added {foundItems.Count} ItemData:\n" +
            string.Join("\n", foundItems.Select(i => $"• {i.itemName}")), 
            "OK");
    }
}

