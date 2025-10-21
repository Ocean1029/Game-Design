using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Simple save system for persisting spawn point and inventory data
/// Uses PlayerPrefs for simple key-value storage
/// </summary>
public static class SaveSystem
{
    // PlayerPrefs keys
    private const string KEY_LAST_SPAWN_POINT_ID = "LastSpawnPointID";
    private const string KEY_LAST_SPAWN_SCENE = "LastSpawnScene";
    private const string KEY_HAS_SAVE_DATA = "HasSaveData";
    private const string KEY_INVENTORY_DATA = "InventoryData";  // JSON of inventory items
    private const string KEY_COLLECTED_WORLD_ITEMS = "CollectedWorldItems";  // JSON array of collected world item IDs

    /// <summary>
    /// Save the current active spawn point
    /// </summary>
    public static void SaveSpawnPoint(SpawnPointData spawnPoint)
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning("SaveSystem: Cannot save null spawn point");
            return;
        }

        PlayerPrefs.SetString(KEY_LAST_SPAWN_POINT_ID, spawnPoint.spawnPointId);
        PlayerPrefs.SetString(KEY_LAST_SPAWN_SCENE, spawnPoint.sceneName);
        PlayerPrefs.SetInt(KEY_HAS_SAVE_DATA, 1);
        PlayerPrefs.Save();

        Debug.Log($"SaveSystem: Saved spawn point '{spawnPoint.spawnPointId}' in scene '{spawnPoint.sceneName}'");
    }

    /// <summary>
    /// Load the last saved spawn point ID
    /// Returns null if no save data exists
    /// </summary>
    public static string LoadLastSpawnPointID()
    {
        if (!HasSaveData())
        {
            return null;
        }

        return PlayerPrefs.GetString(KEY_LAST_SPAWN_POINT_ID, null);
    }

    /// <summary>
    /// Load the last saved spawn scene name
    /// Returns null if no save data exists
    /// </summary>
    public static string LoadLastSpawnScene()
    {
        if (!HasSaveData())
        {
            return null;
        }

        return PlayerPrefs.GetString(KEY_LAST_SPAWN_SCENE, null);
    }

    /// <summary>
    /// Check if save data exists
    /// </summary>
    public static bool HasSaveData()
    {
        return PlayerPrefs.GetInt(KEY_HAS_SAVE_DATA, 0) == 1;
    }

    /// <summary>
    /// Clear all save data
    /// </summary>
    public static void ClearSaveData()
    {
        PlayerPrefs.DeleteKey(KEY_LAST_SPAWN_POINT_ID);
        PlayerPrefs.DeleteKey(KEY_LAST_SPAWN_SCENE);
        PlayerPrefs.DeleteKey(KEY_HAS_SAVE_DATA);
        PlayerPrefs.DeleteKey(KEY_INVENTORY_DATA);
        PlayerPrefs.DeleteKey(KEY_COLLECTED_WORLD_ITEMS);
        PlayerPrefs.Save();

        Debug.Log("SaveSystem: Save data cleared");
    }

    /// <summary>
    /// Get debug information about current save data
    /// </summary>
    public static string GetDebugInfo()
    {
        if (!HasSaveData())
        {
            return "SaveSystem: No save data found";
        }

        string spawnPointId = LoadLastSpawnPointID();
        string sceneName = LoadLastSpawnScene();
        List<string> collectedItems = LoadCollectedItems();
        return $"SaveSystem: Last spawn point = '{spawnPointId}' in scene '{sceneName}', Collected items = {collectedItems.Count}";
    }
    
    // ==================== Inventory Save/Load ====================
    
    /// <summary>
    /// Save collected items (only their IDs and quantities)
    /// </summary>
    public static void SaveInventory(List<InventoryItem> items)
    {
        InventorySaveData saveData = new InventorySaveData();
        saveData.items = new List<ItemSaveData>();
        
        foreach (var item in items)
        {
            saveData.items.Add(new ItemSaveData 
            { 
                itemId = item.itemData.itemId, 
                quantity = item.quantity 
            });
        }
        
        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(KEY_INVENTORY_DATA, json);
        PlayerPrefs.Save();
        
        Debug.Log($"SaveSystem: Saved {items.Count} items to inventory");
    }
    
    /// <summary>
    /// Load inventory item data (IDs and quantities)
    /// </summary>
    public static List<ItemSaveData> LoadInventoryData()
    {
        string json = PlayerPrefs.GetString(KEY_INVENTORY_DATA, "");
        
        if (string.IsNullOrEmpty(json))
        {
            return new List<ItemSaveData>();
        }
        
        try
        {
            InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);
            return saveData.items ?? new List<ItemSaveData>();
        }
        catch
        {
            Debug.LogWarning("SaveSystem: Failed to load inventory data");
            return new List<ItemSaveData>();
        }
    }
    
    // ==================== World Item Collection Tracking ====================
    
    /// <summary>
    /// Mark a world item as collected (so it won't respawn)
    /// </summary>
    public static void MarkItemCollected(string collectableId)
    {
        List<string> collectedItems = LoadCollectedWorldItems();
        
        if (!collectedItems.Contains(collectableId))
        {
            collectedItems.Add(collectableId);
            SaveCollectedWorldItems(collectedItems);
            Debug.Log($"SaveSystem: Marked world item '{collectableId}' as collected");
        }
    }
    
    /// <summary>
    /// Check if a world item has been collected
    /// </summary>
    public static bool IsItemCollected(string collectableId)
    {
        List<string> collectedItems = LoadCollectedWorldItems();
        return collectedItems.Contains(collectableId);
    }
    
    /// <summary>
    /// Load list of collected world item IDs
    /// </summary>
    private static List<string> LoadCollectedWorldItems()
    {
        string json = PlayerPrefs.GetString(KEY_COLLECTED_WORLD_ITEMS, "");
        
        if (string.IsNullOrEmpty(json))
        {
            return new List<string>();
        }
        
        try
        {
            CollectedItemsList listData = JsonUtility.FromJson<CollectedItemsList>(json);
            return listData.collectedIds ?? new List<string>();
        }
        catch
        {
            Debug.LogWarning("SaveSystem: Failed to load collected world items");
            return new List<string>();
        }
    }
    
    /// <summary>
    /// Save list of collected world item IDs
    /// </summary>
    private static void SaveCollectedWorldItems(List<string> collectedIds)
    {
        CollectedItemsList listData = new CollectedItemsList();
        listData.collectedIds = collectedIds;
        
        string json = JsonUtility.ToJson(listData);
        PlayerPrefs.SetString(KEY_COLLECTED_WORLD_ITEMS, json);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Legacy method: Get list of collected item type IDs
    /// </summary>
    public static List<string> LoadCollectedItems()
    {
        List<ItemSaveData> items = LoadInventoryData();
        List<string> itemIds = new List<string>();
        
        foreach (var item in items)
        {
            itemIds.Add(item.itemId);
        }
        
        return itemIds;
    }
}

/// <summary>
/// Helper class for JSON serialization of inventory
/// </summary>
[System.Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> items = new List<ItemSaveData>();
}

/// <summary>
/// Helper class for individual item save data
/// </summary>
[System.Serializable]
public class ItemSaveData
{
    public string itemId;
    public int quantity;
}

/// <summary>
/// Helper class for JSON serialization of collected world items list
/// </summary>
[System.Serializable]
public class CollectedItemsList
{
    public List<string> collectedIds = new List<string>();
}

