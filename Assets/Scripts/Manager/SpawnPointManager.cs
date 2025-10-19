using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages spawn point (chair) system - registration, discovery, and teleportation
/// Handles all spawn point related functionality
/// </summary>
public class SpawnPointManager : MonoBehaviour
{
    // Spawn point system data
    private List<SpawnPointData> discoveredSpawnPoints = new List<SpawnPointData>();
    private SpawnPointData currentSpawnPoint = null;

    // ==================== Spawn Point Registration ====================

    /// <summary>
    /// Register a new spawn point (chair)
    /// </summary>
    public void RegisterSpawnPoint(string spawnPointId, string displayName, string locationDescription, Vector3 position, string sceneName)
    {
        // Check if spawn point already exists
        SpawnPointData existingSpawn = discoveredSpawnPoints.Find(sp => sp.spawnPointId == spawnPointId);
        
        if (existingSpawn != null)
        {
            // Update existing spawn point
            existingSpawn.position = position;
            existingSpawn.sceneName = sceneName;
            Debug.Log($"SpawnPointManager: Updated existing spawn point '{spawnPointId}'");
        }
        else
        {
            // Add new spawn point
            SpawnPointData newSpawn = new SpawnPointData
            {
                spawnPointId = spawnPointId,
                displayName = displayName,
                locationDescription = locationDescription,
                position = position,
                sceneName = sceneName
            };
            
            discoveredSpawnPoints.Add(newSpawn);
            Debug.Log($"SpawnPointManager: Registered new spawn point '{spawnPointId}' - {displayName}");
        }
    }

    // ==================== Current Spawn Point Management ====================

    /// <summary>
    /// Set the current spawn point by ID
    /// </summary>
    public void SetCurrentSpawnPoint(string spawnPointId)
    {
        SpawnPointData spawnPoint = discoveredSpawnPoints.Find(sp => sp.spawnPointId == spawnPointId);
        
        if (spawnPoint != null)
        {
            currentSpawnPoint = spawnPoint;
            Debug.Log($"SpawnPointManager: Current spawn point set to '{spawnPointId}' - {spawnPoint.displayName}");
        }
        else
        {
            Debug.LogWarning($"SpawnPointManager: Spawn point '{spawnPointId}' not found!");
        }
    }

    /// <summary>
    /// Set the current spawn point by ID, position, and scene
    /// </summary>
    public void SetCurrentSpawnPoint(string spawnPointId, Vector3 position, string sceneName)
    {
        // First register the spawn point if it doesn't exist
        SpawnPointData spawnPoint = discoveredSpawnPoints.Find(sp => sp.spawnPointId == spawnPointId);
        
        if (spawnPoint == null)
        {
            // Register with default values
            RegisterSpawnPoint(spawnPointId, spawnPointId, "", position, sceneName);
            spawnPoint = discoveredSpawnPoints.Find(sp => sp.spawnPointId == spawnPointId);
        }
        else
        {
            // Update position and scene
            spawnPoint.position = position;
            spawnPoint.sceneName = sceneName;
        }

        currentSpawnPoint = spawnPoint;
        Debug.Log($"SpawnPointManager: Current spawn point set to '{spawnPointId}' at {position} in {sceneName}");
    }

    // ==================== Query Methods ====================

    /// <summary>
    /// Get the current spawn point
    /// </summary>
    public SpawnPointData GetCurrentSpawnPoint()
    {
        return currentSpawnPoint;
    }

    /// <summary>
    /// Get all discovered spawn points
    /// </summary>
    public List<SpawnPointData> GetDiscoveredSpawnPoints()
    {
        return new List<SpawnPointData>(discoveredSpawnPoints);
    }

    /// <summary>
    /// Check if a spawn point is discovered
    /// </summary>
    public bool IsSpawnPointDiscovered(string spawnPointId)
    {
        return discoveredSpawnPoints.Exists(sp => sp.spawnPointId == spawnPointId);
    }

    /// <summary>
    /// Get spawn point data by ID
    /// </summary>
    public SpawnPointData GetSpawnPointById(string spawnPointId)
    {
        return discoveredSpawnPoints.Find(sp => sp.spawnPointId == spawnPointId);
    }

    // ==================== Utility Methods ====================

    /// <summary>
    /// Clear all spawn points (useful for testing)
    /// </summary>
    public void ClearSpawnPoints()
    {
        discoveredSpawnPoints.Clear();
        currentSpawnPoint = null;
        Debug.Log("SpawnPointManager: All spawn points cleared");
    }

    /// <summary>
    /// Get total number of discovered spawn points
    /// </summary>
    public int GetSpawnPointCount()
    {
        return discoveredSpawnPoints.Count;
    }
}

// ==================== Spawn Point Data Class ====================

/// <summary>
/// Data structure for spawn point information
/// </summary>
[System.Serializable]
public class SpawnPointData
{
    public string spawnPointId;
    public string displayName;
    public string locationDescription;
    public Vector3 position;
    public string sceneName;
}

