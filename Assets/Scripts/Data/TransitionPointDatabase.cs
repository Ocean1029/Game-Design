using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ScriptableObject database for storing all transition points in the game
/// Manages 1-to-1 correspondence between transition points in different scenes
/// </summary>
[CreateAssetMenu(fileName = "TransitionPointDatabase", menuName = "Game Data/Transition Point Database")]
public class TransitionPointDatabase : ScriptableObject
{
    [Header("Scene Transition Data")]
    [Tooltip("Transition points organized by scene name")]
    [SerializeField] private List<SceneTransitionData> sceneTransitionData = new List<SceneTransitionData>();

    [Header("Settings")]
    [Tooltip("Whether to automatically save this database when modified")]
    [SerializeField] private bool autoSave = true;

    [Tooltip("Default transition duration if not specified")]
#pragma warning disable CS0414 // Field is assigned but its value is never used (Reserved for future transition system)
    [SerializeField] private float defaultTransitionDuration = 1.0f;
#pragma warning restore CS0414

    /// <summary>
    /// Get transition points for a specific scene
    /// </summary>
    public List<TransitionPointData> GetTransitionPointsForScene(string sceneName)
    {
        var sceneData = sceneTransitionData.FirstOrDefault(s => s.sceneName == sceneName);
        return sceneData?.transitionPoints ?? new List<TransitionPointData>();
    }

    /// <summary>
    /// Get a transition point by its ID
    /// </summary>
    public TransitionPointData GetTransitionPointById(string transitionPointId)
    {
        foreach (var sceneData in sceneTransitionData)
        {
            var transitionPoint = sceneData.transitionPoints.FirstOrDefault(tp => tp.transitionPointId == transitionPointId);
            if (transitionPoint != null)
            {
                return transitionPoint;
            }
        }
        return null;
    }

    /// <summary>
    /// Get a transition point by scene and index
    /// </summary>
    public TransitionPointData GetTransitionPointBySceneAndIndex(string sceneName, int index)
    {
        var sceneData = sceneTransitionData.FirstOrDefault(s => s.sceneName == sceneName);
        if (sceneData != null && index >= 0 && index < sceneData.transitionPoints.Count)
        {
            return sceneData.transitionPoints[index];
        }
        return null;
    }

    /// <summary>
    /// Add or update a transition point
    /// </summary>
    public void AddOrUpdateTransitionPoint(TransitionPointData transitionPoint)
    {
        if (transitionPoint == null || !transitionPoint.IsValid())
        {
            Debug.LogWarning("TransitionPointDatabase: Cannot add null or invalid transition point");
            return;
        }

        // Find or create scene data
        var sceneData = sceneTransitionData.FirstOrDefault(s => s.sceneName == transitionPoint.sceneName);
        if (sceneData == null)
        {
            sceneData = new SceneTransitionData
            {
                sceneName = transitionPoint.sceneName,
                transitionPoints = new List<TransitionPointData>()
            };
            sceneTransitionData.Add(sceneData);
        }

        // Check if transition point already exists
        int existingIndex = sceneData.transitionPoints.FindIndex(tp => tp.transitionPointId == transitionPoint.transitionPointId);
        
        if (existingIndex >= 0)
        {
            // Update existing transition point
            sceneData.transitionPoints[existingIndex] = transitionPoint;
            Debug.Log($"TransitionPointDatabase: Updated transition point '{transitionPoint.transitionPointId}' in scene '{transitionPoint.sceneName}'");
        }
        else
        {
            // Add new transition point
            sceneData.transitionPoints.Add(transitionPoint);
            Debug.Log($"TransitionPointDatabase: Added new transition point '{transitionPoint.transitionPointId}' to scene '{transitionPoint.sceneName}'");
        }

        // Validate and update target transition point index
        UpdateTargetTransitionPointIndex(transitionPoint);

        if (autoSave)
        {
            SaveDatabase();
        }
    }

    /// <summary>
    /// Remove a transition point
    /// </summary>
    public void RemoveTransitionPoint(string transitionPointId)
    {
        foreach (var sceneData in sceneTransitionData)
        {
            int index = sceneData.transitionPoints.FindIndex(tp => tp.transitionPointId == transitionPointId);
            if (index >= 0)
            {
                sceneData.transitionPoints.RemoveAt(index);
                Debug.Log($"TransitionPointDatabase: Removed transition point '{transitionPointId}' from scene '{sceneData.sceneName}'");

                // Update indices for remaining transition points
                UpdateIndicesForScene(sceneData.sceneName);

                if (autoSave)
                {
                    SaveDatabase();
                }
                return;
            }
        }
    }

    /// <summary>
    /// Clear all transition points for a scene
    /// </summary>
    public void ClearTransitionPointsForScene(string sceneName)
    {
        var sceneData = sceneTransitionData.FirstOrDefault(s => s.sceneName == sceneName);
        if (sceneData != null)
        {
            sceneData.transitionPoints.Clear();
            Debug.Log($"TransitionPointDatabase: Cleared all transition points for scene '{sceneName}'");

            if (autoSave)
            {
                SaveDatabase();
            }
        }
    }

    /// <summary>
    /// Get the target transition point for a given transition
    /// </summary>
    public TransitionPointData GetTargetTransitionPoint(TransitionPointData sourceTransitionPoint)
    {
        if (sourceTransitionPoint == null || !sourceTransitionPoint.IsValid())
        {
            return null;
        }

        // Find target scene data
        var targetSceneData = sceneTransitionData.FirstOrDefault(s => s.sceneName == sourceTransitionPoint.targetSceneName);
        if (targetSceneData == null)
        {
            Debug.LogWarning($"TransitionPointDatabase: Target scene '{sourceTransitionPoint.targetSceneName}' not found");
            return null;
        }

        // Find target transition point
        if (sourceTransitionPoint.targetTransitionPointIndex >= 0 && 
            sourceTransitionPoint.targetTransitionPointIndex < targetSceneData.transitionPoints.Count)
        {
            return targetSceneData.transitionPoints[sourceTransitionPoint.targetTransitionPointIndex];
        }

        // Fallback: find by ID
        var targetTransitionPoint = targetSceneData.transitionPoints.FirstOrDefault(tp => tp.transitionPointId == sourceTransitionPoint.targetTransitionPointId);
        if (targetTransitionPoint == null)
        {
            Debug.LogWarning($"TransitionPointDatabase: Target transition point '{sourceTransitionPoint.targetTransitionPointId}' not found in scene '{sourceTransitionPoint.targetSceneName}'");
        }

        return targetTransitionPoint;
    }

    /// <summary>
    /// Check if a transition is valid (both source and target exist)
    /// </summary>
    public bool IsTransitionValid(TransitionPointData sourceTransitionPoint)
    {
        if (sourceTransitionPoint == null || !sourceTransitionPoint.IsValid())
        {
            return false;
        }

        var targetTransitionPoint = GetTargetTransitionPoint(sourceTransitionPoint);
        return targetTransitionPoint != null && targetTransitionPoint.IsValid();
    }

    /// <summary>
    /// Update the target transition point index for a transition point
    /// </summary>
    private void UpdateTargetTransitionPointIndex(TransitionPointData transitionPoint)
    {
        if (string.IsNullOrEmpty(transitionPoint.targetTransitionPointId))
        {
            return;
        }

        var targetSceneData = sceneTransitionData.FirstOrDefault(s => s.sceneName == transitionPoint.targetSceneName);
        if (targetSceneData != null)
        {
            int targetIndex = targetSceneData.transitionPoints.FindIndex(tp => tp.transitionPointId == transitionPoint.targetTransitionPointId);
            if (targetIndex >= 0)
            {
                transitionPoint.targetTransitionPointIndex = targetIndex;
            }
        }
    }

    /// <summary>
    /// Update indices for all transition points in a scene
    /// </summary>
    private void UpdateIndicesForScene(string sceneName)
    {
        var sceneData = sceneTransitionData.FirstOrDefault(s => s.sceneName == sceneName);
        if (sceneData != null)
        {
            for (int i = 0; i < sceneData.transitionPoints.Count; i++)
            {
                sceneData.transitionPoints[i].targetTransitionPointIndex = i;
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
        Debug.Log("TransitionPointDatabase: Starting validation...");

        int totalTransitionPoints = 0;
        int invalidTransitions = 0;
        int duplicateIds = 0;

        foreach (var sceneData in sceneTransitionData)
        {
            totalTransitionPoints += sceneData.transitionPoints.Count;

            // Check for duplicate IDs within scene
            var sceneDuplicateIds = sceneData.transitionPoints.GroupBy(tp => tp.transitionPointId)
                                                             .Where(g => g.Count() > 1)
                                                             .Select(g => g.Key)
                                                             .ToList();
            duplicateIds += sceneDuplicateIds.Count;

            if (sceneDuplicateIds.Count > 0)
            {
                Debug.LogWarning($"TransitionPointDatabase: Found duplicate transition point IDs in scene '{sceneData.sceneName}': {string.Join(", ", sceneDuplicateIds)}");
            }

            // Check for invalid transitions
            foreach (var transitionPoint in sceneData.transitionPoints)
            {
                if (!IsTransitionValid(transitionPoint))
                {
                    invalidTransitions++;
                }
            }
        }

        if (duplicateIds > 0)
        {
            Debug.LogWarning($"TransitionPointDatabase: Found {duplicateIds} duplicate transition point IDs");
        }

        if (invalidTransitions > 0)
        {
            Debug.LogWarning($"TransitionPointDatabase: Found {invalidTransitions} invalid transitions");
        }

        Debug.Log($"TransitionPointDatabase: Validation complete. Total transition points: {totalTransitionPoints} across {sceneTransitionData.Count} scenes");
    }

    /// <summary>
    /// Get debug information about the database
    /// </summary>
    public string GetDebugInfo()
    {
        int totalTransitionPoints = sceneTransitionData.Sum(s => s.transitionPoints.Count);
        return $"TransitionPointDatabase: {totalTransitionPoints} transition points across {sceneTransitionData.Count} scenes";
    }
}

/// <summary>
/// Data structure for organizing transition points by scene
/// </summary>
[System.Serializable]
public class SceneTransitionData
{
    [Tooltip("Name of the scene")]
    public string sceneName;
    
    [Tooltip("List of transition points in this scene")]
    public List<TransitionPointData> transitionPoints = new List<TransitionPointData>();
}
