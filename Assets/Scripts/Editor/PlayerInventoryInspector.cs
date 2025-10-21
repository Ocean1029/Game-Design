using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Comprehensive tool to check and adjust player's collected items
/// Provides detailed inventory management and debugging capabilities
/// </summary>
public class PlayerInventoryInspector : EditorWindow
{
    [MenuItem("Tools/Inventory/Player Inventory Inspector")]
    public static void ShowWindow()
    {
        GetWindow<PlayerInventoryInspector>("Player Inventory Inspector");
    }
    
    private InventorySystem inventorySystem;
    private Vector2 scrollPosition;
    private bool showDebugInfo = true;
    private bool showCollectedWorldItems = true;
    
    void OnGUI()
    {
        GUILayout.Label("Player Inventory Inspector", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        // Find InventorySystem
        if (inventorySystem == null)
        {
            inventorySystem = FindFirstObjectByType<InventorySystem>();
        }
        
        if (inventorySystem == null)
        {
            EditorGUILayout.HelpBox("No InventorySystem found in scene!", MessageType.Error);
            return;
        }
        
        EditorGUILayout.HelpBox(
            "This tool allows you to:\n" +
            "• View current inventory items\n" +
            "• Add/remove items for testing\n" +
            "• Check collected world items\n" +
            "• Clear inventory data\n" +
            "• Debug save/load system",
            MessageType.Info
        );
        
        GUILayout.Space(10);
        
        // Options
        showDebugInfo = EditorGUILayout.Toggle("Show Debug Info", showDebugInfo);
        showCollectedWorldItems = EditorGUILayout.Toggle("Show Collected World Items", showCollectedWorldItems);
        
        GUILayout.Space(10);
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        // Current Inventory
        DrawCurrentInventory();
        
        GUILayout.Space(10);
        
        // Quick Actions
        DrawQuickActions();
        
        GUILayout.Space(10);
        
        // Item Management
        DrawItemManagement();
        
        GUILayout.Space(10);
        
        // Collected World Items
        if (showCollectedWorldItems)
        {
            DrawCollectedWorldItems();
        }
        
        GUILayout.Space(10);
        
        // Save System Info
        DrawSaveSystemInfo();
        
        EditorGUILayout.EndScrollView();
    }
    
    private void DrawCurrentInventory()
    {
        GUILayout.Label("Current Inventory", EditorStyles.boldLabel);
        
        List<InventoryItem> items = inventorySystem.GetAllItems();
        
        if (items.Count == 0)
        {
            EditorGUILayout.HelpBox("Inventory is empty", MessageType.Info);
        }
        else
        {
            EditorGUILayout.LabelField($"Total Items: {items.Count}");
            EditorGUILayout.Space();
            
            foreach (var item in items)
            {
                EditorGUILayout.BeginHorizontal();
                
                // Item icon (if available)
                if (item.itemData.icon != null)
                {
                    EditorGUILayout.ObjectField(item.itemData.icon, typeof(Sprite), false, GUILayout.Width(20), GUILayout.Height(20));
                }
                else
                {
                    EditorGUILayout.LabelField("", GUILayout.Width(20), GUILayout.Height(20));
                }
                
                // Item info
                EditorGUILayout.LabelField($"• {item.itemData.itemName}", GUILayout.Width(200));
                EditorGUILayout.LabelField($"x{item.quantity}", GUILayout.Width(50));
                EditorGUILayout.LabelField($"({item.itemData.itemId})", GUILayout.Width(100));
                
                // Remove button
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    inventorySystem.RemoveItem(item.itemData, item.quantity);
                }
                
                EditorGUILayout.EndHorizontal();
            }
        }
    }
    
    private void DrawQuickActions()
    {
        GUILayout.Label("Quick Actions", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Clear All Items"))
        {
            if (EditorUtility.DisplayDialog("Confirm", "Clear all items from inventory?", "Clear", "Cancel"))
            {
                inventorySystem.ClearInventory();
            }
        }
        
        if (GUILayout.Button("Refresh Display"))
        {
            // Trigger refresh
            BackpackUIController backpackUI = FindFirstObjectByType<BackpackUIController>();
            if (backpackUI != null)
            {
                backpackUI.RefreshAllSlots();
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Clear Save Data"))
        {
            if (EditorUtility.DisplayDialog("Confirm", "Clear all save data? This will reset inventory and collected items.", "Clear", "Cancel"))
            {
                SaveSystem.ClearSaveData();
                Debug.Log("Save data cleared!");
            }
        }
        
        if (GUILayout.Button("Reload from Save"))
        {
            // Force reload inventory
            inventorySystem.ClearInventory();
            // The inventory will reload automatically on next Start()
            Debug.Log("Inventory will reload from save data on next game start");
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawItemManagement()
    {
        GUILayout.Label("Item Management", EditorStyles.boldLabel);
        
        List<ItemData> allGameItems = inventorySystem.GetAllGameItems();
        
        if (allGameItems.Count == 0)
        {
            EditorGUILayout.HelpBox("No items in AllGameItems list", MessageType.Warning);
            return;
        }
        
        EditorGUILayout.LabelField($"Available Items: {allGameItems.Count}");
        EditorGUILayout.Space();
        
        foreach (var itemData in allGameItems)
        {
            if (itemData == null) continue;
            
            EditorGUILayout.BeginHorizontal();
            
            // Item icon
            if (itemData.icon != null)
            {
                EditorGUILayout.ObjectField(itemData.icon, typeof(Sprite), false, GUILayout.Width(20), GUILayout.Height(20));
            }
            else
            {
                EditorGUILayout.LabelField("", GUILayout.Width(20), GUILayout.Height(20));
            }
            
            // Item name
            EditorGUILayout.LabelField(itemData.itemName, GUILayout.Width(150));
            
            // Current quantity
            int currentQuantity = inventorySystem.GetItemQuantity(itemData);
            EditorGUILayout.LabelField($"Current: {currentQuantity}", GUILayout.Width(80));
            
            // Add button
            if (GUILayout.Button("+1", GUILayout.Width(40)))
            {
                inventorySystem.AddItem(itemData, 1);
            }
            
            // Add 5 button
            if (GUILayout.Button("+5", GUILayout.Width(40)))
            {
                inventorySystem.AddItem(itemData, 5);
            }
            
            // Remove button
            if (currentQuantity > 0 && GUILayout.Button("-1", GUILayout.Width(40)))
            {
                inventorySystem.RemoveItem(itemData, 1);
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }
    
    private void DrawCollectedWorldItems()
    {
        GUILayout.Label("Collected World Items", EditorStyles.boldLabel);
        
        List<string> collectedItems = SaveSystem.LoadCollectedWorldItems();
        
        if (collectedItems.Count == 0)
        {
            EditorGUILayout.HelpBox("No world items collected", MessageType.Info);
        }
        else
        {
            EditorGUILayout.LabelField($"Collected Items: {collectedItems.Count}");
            EditorGUILayout.Space();
            
            foreach (var itemId in collectedItems)
            {
                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.LabelField($"• {itemId}", GUILayout.Width(300));
                
                if (GUILayout.Button("Uncollect", GUILayout.Width(80)))
                {
                    if (EditorUtility.DisplayDialog("Confirm", $"Uncollect '{itemId}'? This will make it respawn.", "Uncollect", "Cancel"))
                    {
                        // Remove from collected list
                        List<string> updatedList = SaveSystem.LoadCollectedWorldItems();
                        updatedList.Remove(itemId);
                        SaveSystem.SaveCollectedWorldItems(updatedList);
                        Debug.Log($"Uncollected: {itemId}");
                    }
                }
                
                EditorGUILayout.EndHorizontal();
            }
        }
    }
    
    private void DrawSaveSystemInfo()
    {
        GUILayout.Label("Save System Info", EditorStyles.boldLabel);
        
        if (showDebugInfo)
        {
            string debugInfo = SaveSystem.GetDebugInfo();
            EditorGUILayout.TextArea(debugInfo, GUILayout.Height(100));
        }
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Show Debug Info"))
        {
            Debug.Log(SaveSystem.GetDebugInfo());
        }
        
        if (GUILayout.Button("Test Save"))
        {
            // Test saving current state
            List<InventoryItem> items = inventorySystem.GetAllItems();
            SaveSystem.SaveInventory(items);
            Debug.Log("Test save completed");
        }
        
        EditorGUILayout.EndHorizontal();
    }
}
