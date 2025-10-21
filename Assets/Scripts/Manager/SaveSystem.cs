using UnityEngine;

/// <summary>
/// Simple save system for persisting spawn point data
/// Uses PlayerPrefs for simple key-value storage
/// </summary>
public static class SaveSystem
{
    // PlayerPrefs keys
    private const string KEY_LAST_SPAWN_POINT_ID = "LastSpawnPointID";
    private const string KEY_LAST_SPAWN_SCENE = "LastSpawnScene";
    private const string KEY_HAS_SAVE_DATA = "HasSaveData";

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
        return $"SaveSystem: Last spawn point = '{spawnPointId}' in scene '{sceneName}'";
    }
}

