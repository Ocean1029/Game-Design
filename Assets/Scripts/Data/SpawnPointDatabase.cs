using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ScriptableObject database for storing all spawn points in the game
/// This allows for easy management and persistence of save points
/// </summary>
[CreateAssetMenu(fileName = "SpawnPointDatabase", menuName = "Game Data/Spawn Point Database")]
public class SpawnPointDatabase : ScriptableObject
{
    [Header("Spawn Point Collection")]
    [Tooltip("All spawn points in the game")]
    [SerializeField] private List<SpawnPointData> allSpawnPoints = new List<SpawnPointData>();

    [Header("Current Active Spawn Point")]
    [Tooltip("The currently active spawn point (last used save point)")]
    [SerializeField] private SpawnPointData currentActiveSpawnPoint;

    [Header("Settings")]
    [Tooltip("Whether to automatically save this database when modified")]
    [SerializeField] private bool autoSave = true;

    /// <summary>
    /// Get all spawn points in the database
    /// </summary>
    public List<SpawnPointData> AllSpawnPoints => new List<SpawnPointData>(allSpawnPoints);

    /// <summary>
    /// Get the currently active spawn point
    /// </summary>
    public SpawnPointData CurrentActiveSpawnPoint => currentActiveSpawnPoint;

    /// <summary>
    /// Get spawn points that can be used for teleportation
    /// </summary>
    public List<SpawnPointData> TransportableSpawnPoints => 
        allSpawnPoints.Where(sp => sp.isTransportable && sp.CanActivate()).ToList();

    /// <summary>
    /// Get spawn points in a specific scene
    /// </summary>
    public List<SpawnPointData> GetSpawnPointsInScene(string sceneName)
    {
        return allSpawnPoints.Where(sp => sp.sceneName == sceneName).ToList();
    }

    /// <summary>
    /// Get a spawn point by its ID
    /// </summary>
    public SpawnPointData GetSpawnPointById(string spawnPointId)
    {
        return allSpawnPoints.FirstOrDefault(sp => sp.spawnPointId == spawnPointId);
    }

    /// <summary>
    /// Add or update a spawn point in the database
    /// </summary>
    public void AddOrUpdateSpawnPoint(SpawnPointData spawnPoint)
    {
        if (spawnPoint == null || string.IsNullOrEmpty(spawnPoint.spawnPointId))
        {
            Debug.LogWarning("SpawnPointDatabase: Cannot add null or invalid spawn point");
            return;
        }

        // Check if spawn point already exists
        int existingIndex = allSpawnPoints.FindIndex(sp => sp.spawnPointId == spawnPoint.spawnPointId);
        
        if (existingIndex >= 0)
        {
            // Update existing spawn point
            allSpawnPoints[existingIndex] = spawnPoint;
            Debug.Log($"SpawnPointDatabase: Updated spawn point '{spawnPoint.spawnPointId}'");
        }
        else
        {
            // Add new spawn point
            allSpawnPoints.Add(spawnPoint);
            Debug.Log($"SpawnPointDatabase: Added new spawn point '{spawnPoint.spawnPointId}'");
        }

        // Handle one-time use spawn points
        if (spawnPoint.isOneTimeUse && spawnPoint.isActive)
        {
            DeactivateOtherOneTimeSpawnPoints(spawnPoint.spawnPointId);
        }

        if (autoSave)
        {
            SaveDatabase();
        }
    }

    /// <summary>
    /// Set a spawn point as the current active spawn point
    /// </summary>
    public void SetActiveSpawnPoint(string spawnPointId)
    {
        SpawnPointData spawnPoint = GetSpawnPointById(spawnPointId);
        if (spawnPoint == null)
        {
            Debug.LogWarning($"SpawnPointDatabase: Spawn point '{spawnPointId}' not found");
            return;
        }

        // Deactivate current active spawn point
        if (currentActiveSpawnPoint != null)
        {
            currentActiveSpawnPoint.isActive = false;
        }

        // Set new active spawn point
        spawnPoint.isActive = true;
        currentActiveSpawnPoint = spawnPoint;

        Debug.Log($"SpawnPointDatabase: Set active spawn point to '{spawnPointId}'");

        if (autoSave)
        {
            SaveDatabase();
        }
    }

    /// <summary>
    /// Get the active spawn point for a specific scene
    /// </summary>
    public SpawnPointData GetActiveSpawnPointForScene(string sceneName)
    {
        // First check if current active spawn point is in the target scene
        if (currentActiveSpawnPoint != null && currentActiveSpawnPoint.sceneName == sceneName)
        {
            return currentActiveSpawnPoint;
        }

        // Look for any active spawn point in the target scene
        return allSpawnPoints.FirstOrDefault(sp => sp.sceneName == sceneName && sp.isActive);
    }

    /// <summary>
    /// Remove a spawn point from the database
    /// </summary>
    public void RemoveSpawnPoint(string spawnPointId)
    {
        int index = allSpawnPoints.FindIndex(sp => sp.spawnPointId == spawnPointId);
        if (index >= 0)
        {
            allSpawnPoints.RemoveAt(index);
            Debug.Log($"SpawnPointDatabase: Removed spawn point '{spawnPointId}'");

            // If we removed the current active spawn point, find a new one
            if (currentActiveSpawnPoint != null && currentActiveSpawnPoint.spawnPointId == spawnPointId)
            {
                currentActiveSpawnPoint = allSpawnPoints.FirstOrDefault(sp => sp.isActive);
            }

            if (autoSave)
            {
                SaveDatabase();
            }
        }
    }

    /// <summary>
    /// Clear all spawn points from the database
    /// </summary>
    public void ClearAllSpawnPoints()
    {
        allSpawnPoints.Clear();
        currentActiveSpawnPoint = null;
        Debug.Log("SpawnPointDatabase: Cleared all spawn points");

        if (autoSave)
        {
            SaveDatabase();
        }
    }

    /// <summary>
    /// Deactivate other one-time use spawn points when a new one is activated
    /// </summary>
    private void DeactivateOtherOneTimeSpawnPoints(string activeSpawnPointId)
    {
        foreach (var spawnPoint in allSpawnPoints)
        {
            if (spawnPoint.spawnPointId != activeSpawnPointId && spawnPoint.isOneTimeUse)
            {
                spawnPoint.isActive = false;
            }
        }
    }

    /// <summary>
    /// Save the database to disk
    /// </summary>
    public void SaveDatabase()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }

    /// <summary>
    /// Validate the database for any issues
    /// </summary>
    [ContextMenu("Validate Database")]
    public void ValidateDatabase()
    {
        Debug.Log("SpawnPointDatabase: Starting validation...");

        // Check for duplicate IDs
        var duplicateIds = allSpawnPoints.GroupBy(sp => sp.spawnPointId)
                                       .Where(g => g.Count() > 1)
                                       .Select(g => g.Key)
                                       .ToList();

        if (duplicateIds.Count > 0)
        {
            Debug.LogWarning($"SpawnPointDatabase: Found duplicate spawn point IDs: {string.Join(", ", duplicateIds)}");
        }

        // Check for invalid spawn points
        var invalidSpawnPoints = allSpawnPoints.Where(sp => !sp.CanActivate()).ToList();
        if (invalidSpawnPoints.Count > 0)
        {
            Debug.LogWarning($"SpawnPointDatabase: Found {invalidSpawnPoints.Count} invalid spawn points");
        }

        // Check current active spawn point
        if (currentActiveSpawnPoint != null && !currentActiveSpawnPoint.isActive)
        {
            Debug.LogWarning("SpawnPointDatabase: Current active spawn point is not marked as active");
        }

        Debug.Log($"SpawnPointDatabase: Validation complete. Total spawn points: {allSpawnPoints.Count}");
    }

    /// <summary>
    /// Get debug information about the database
    /// </summary>
    public string GetDebugInfo()
    {
        return $"SpawnPointDatabase: {allSpawnPoints.Count} total spawn points, " +
               $"Current active: {currentActiveSpawnPoint?.spawnPointId ?? "None"}, " +
               $"Transportable: {TransportableSpawnPoints.Count}";
    }
}
