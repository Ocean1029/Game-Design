using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

/// <summary>
/// New Transition Point System for Scene Transition management
/// Handles 1-to-1 correspondence between transition points in different scenes
/// </summary>
public class TransitionPointSystem : MonoBehaviour
{
    [Header("Database Reference")]
    [Tooltip("Transition Point Database ScriptableObject")]
    [SerializeField] private TransitionPointDatabase transitionPointDatabase;

    [Header("Settings")]
    [Tooltip("Default transition duration if not specified in transition point")]
    [SerializeField] private float defaultTransitionDuration = 1.0f;

    [Tooltip("Whether to pause game during transition")]
    [SerializeField] private bool pauseGameDuringTransition = true;

    [Tooltip("Whether to fade screen during transition")]
#pragma warning disable CS0414 // Field is assigned but its value is never used (Reserved for future fade system)
    [SerializeField] private bool fadeScreenDuringTransition = true;
#pragma warning restore CS0414

    // Events
    public delegate void TransitionStartedHandler(TransitionPointData sourcePoint, TransitionPointData targetPoint);
    public event TransitionStartedHandler OnTransitionStarted;

    public delegate void TransitionCompletedHandler(TransitionPointData sourcePoint, TransitionPointData targetPoint);
    public event TransitionCompletedHandler OnTransitionCompleted;

    public delegate void TransitionFailedHandler(TransitionPointData sourcePoint, string reason);
    public event TransitionFailedHandler OnTransitionFailed;

    // State
    private bool isTransitioning = false;
    private TransitionPointData currentTransitionPoint;

    // Cached references
    private PlayerController playerController;
    private CameraFollow cameraFollow;

    void Awake()
    {
        // Database reference will be set later by GameManager
        // If no database is assigned, system will use temporary storage
        
        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // Get player references
        playerController = FindFirstObjectByType<PlayerController>();
        cameraFollow = FindFirstObjectByType<CameraFollow>();
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ==================== Transition Management ====================

    /// <summary>
    /// Attempt to transition through a transition point
    /// </summary>
    public bool TransitionThroughPoint(string transitionPointId)
    {
        if (isTransitioning)
        {
            Debug.LogWarning("TransitionPointSystem: Already transitioning, cannot start new transition");
            return false;
        }

        if (transitionPointDatabase == null)
        {
            Debug.LogError("TransitionPointSystem: No database assigned!");
            return false;
        }

        TransitionPointData sourcePoint = transitionPointDatabase.GetTransitionPointById(transitionPointId);
        if (sourcePoint == null)
        {
            Debug.LogWarning($"TransitionPointSystem: Transition point '{transitionPointId}' not found!");
            OnTransitionFailed?.Invoke(null, "Transition point not found");
            return false;
        }

        // Validate transition
        if (!sourcePoint.IsValid())
        {
            Debug.LogWarning($"TransitionPointSystem: Invalid transition point '{transitionPointId}'");
            OnTransitionFailed?.Invoke(sourcePoint, "Invalid transition point");
            return false;
        }

        // Check if transition is allowed in current direction
        if (!sourcePoint.CanTransition(TransitionDirection.Exit))
        {
            Debug.LogWarning($"TransitionPointSystem: Transition point '{transitionPointId}' cannot be used for exiting");
            OnTransitionFailed?.Invoke(sourcePoint, "Cannot exit through this transition point");
            return false;
        }

        // Check key requirement
        if (!sourcePoint.HasRequiredKey())
        {
            Debug.LogWarning($"TransitionPointSystem: Required key missing for transition point '{transitionPointId}'");
            OnTransitionFailed?.Invoke(sourcePoint, "Required key missing");
            return false;
        }

        // Get target transition point
        TransitionPointData targetPoint = transitionPointDatabase.GetTargetTransitionPoint(sourcePoint);
        if (targetPoint == null)
        {
            Debug.LogWarning($"TransitionPointSystem: Target transition point not found for '{transitionPointId}'");
            OnTransitionFailed?.Invoke(sourcePoint, "Target transition point not found");
            return false;
        }

        // Validate target transition point
        if (!targetPoint.CanTransition(TransitionDirection.Enter))
        {
            Debug.LogWarning($"TransitionPointSystem: Target transition point cannot be used for entering");
            OnTransitionFailed?.Invoke(sourcePoint, "Target transition point cannot be entered");
            return false;
        }

        // Start transition
        StartTransition(sourcePoint, targetPoint);
        return true;
    }

    /// <summary>
    /// Start the transition process
    /// </summary>
    private void StartTransition(TransitionPointData sourcePoint, TransitionPointData targetPoint)
    {
        isTransitioning = true;
        currentTransitionPoint = sourcePoint;

        Debug.Log($"TransitionPointSystem: Starting transition from '{sourcePoint.transitionPointId}' to '{targetPoint.transitionPointId}'");

        // Notify listeners
        OnTransitionStarted?.Invoke(sourcePoint, targetPoint);

        // Play transition sound
        if (sourcePoint.transitionSound != null)
        {
            AudioSource.PlayClipAtPoint(sourcePoint.transitionSound, sourcePoint.position);
        }

        // Show transition effect
        if (sourcePoint.transitionEffect != null)
        {
            GameObject effect = Instantiate(sourcePoint.transitionEffect, sourcePoint.position, Quaternion.identity);
            Destroy(effect, sourcePoint.transitionDuration > 0 ? sourcePoint.transitionDuration : defaultTransitionDuration);
        }

        // Pause game if specified
        if (pauseGameDuringTransition)
        {
            Time.timeScale = 0f;
        }

        // Start transition coroutine
        StartCoroutine(ExecuteTransition(sourcePoint, targetPoint));
    }

    /// <summary>
    /// Execute the transition
    /// </summary>
    private System.Collections.IEnumerator ExecuteTransition(TransitionPointData sourcePoint, TransitionPointData targetPoint)
    {
        float transitionDuration = sourcePoint.transitionDuration > 0 ? sourcePoint.transitionDuration : defaultTransitionDuration;

        // Wait for transition duration
        if (pauseGameDuringTransition)
        {
            yield return new WaitForSecondsRealtime(transitionDuration);
        }
        else
        {
            yield return new WaitForSeconds(transitionDuration);
        }

        // Resume game
        if (pauseGameDuringTransition)
        {
            Time.timeScale = 1f;
        }

        // Load target scene
        SceneManager.LoadScene(targetPoint.sceneName);

        // The actual positioning will happen in OnSceneLoaded
    }

    /// <summary>
    /// Called when a new scene is loaded
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!isTransitioning || currentTransitionPoint == null)
        {
            return;
        }

        // Get target transition point
        TransitionPointData targetPoint = transitionPointDatabase.GetTargetTransitionPoint(currentTransitionPoint);
        if (targetPoint == null)
        {
            Debug.LogError("TransitionPointSystem: Target transition point not found after scene load!");
            CompleteTransition(null, null);
            return;
        }

        // Position player at target transition point
        PositionPlayerAtTransitionPoint(targetPoint);

        // Complete transition
        CompleteTransition(currentTransitionPoint, targetPoint);
    }

    /// <summary>
    /// Position player at the target transition point
    /// </summary>
    private void PositionPlayerAtTransitionPoint(TransitionPointData targetPoint)
    {
        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerController>();
        }

        if (playerController != null)
        {
            // Set player position
            playerController.transform.position = targetPoint.position;

            // Set player direction if specified
            if (targetPoint.setPlayerDirection)
            {
                // Assuming player has a method to set facing direction
                // This would need to be implemented in PlayerController
                // playerController.SetFacingDirection(targetPoint.playerDirection);
            }

            Debug.Log($"TransitionPointSystem: Positioned player at transition point '{targetPoint.transitionPointId}' at {targetPoint.position}");
        }
        else
        {
            Debug.LogWarning("TransitionPointSystem: PlayerController not found, cannot position player!");
        }

        // Update camera position
        if (cameraFollow == null)
        {
            cameraFollow = FindFirstObjectByType<CameraFollow>();
        }

        if (cameraFollow != null)
        {
            // Camera will automatically follow the player
            Debug.Log("TransitionPointSystem: Camera will follow player to new position");
        }
    }

    /// <summary>
    /// Complete the transition
    /// </summary>
    private void CompleteTransition(TransitionPointData sourcePoint, TransitionPointData targetPoint)
    {
        isTransitioning = false;
        currentTransitionPoint = null;

        if (sourcePoint != null && targetPoint != null)
        {
            Debug.Log($"TransitionPointSystem: Completed transition from '{sourcePoint.transitionPointId}' to '{targetPoint.transitionPointId}'");
            OnTransitionCompleted?.Invoke(sourcePoint, targetPoint);
        }
        else
        {
            Debug.LogWarning("TransitionPointSystem: Transition completed with null points");
        }
    }

    // ==================== Query Methods ====================

    /// <summary>
    /// Get all transition points in the current scene
    /// </summary>
    public List<TransitionPointData> GetTransitionPointsInCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        return GetTransitionPointsInScene(currentScene);
    }

    /// <summary>
    /// Get all transition points in a specific scene
    /// </summary>
    public List<TransitionPointData> GetTransitionPointsInScene(string sceneName)
    {
        return transitionPointDatabase?.GetTransitionPointsForScene(sceneName) ?? new List<TransitionPointData>();
    }

    /// <summary>
    /// Get a transition point by ID
    /// </summary>
    public TransitionPointData GetTransitionPointById(string transitionPointId)
    {
        return transitionPointDatabase?.GetTransitionPointById(transitionPointId);
    }

    /// <summary>
    /// Get a transition point by scene and index
    /// </summary>
    public TransitionPointData GetTransitionPointBySceneAndIndex(string sceneName, int index)
    {
        return transitionPointDatabase?.GetTransitionPointBySceneAndIndex(sceneName, index);
    }

    /// <summary>
    /// Check if a transition is valid
    /// </summary>
    public bool IsTransitionValid(string transitionPointId)
    {
        if (transitionPointDatabase == null)
        {
            return false;
        }

        TransitionPointData sourcePoint = transitionPointDatabase.GetTransitionPointById(transitionPointId);
        return sourcePoint != null && transitionPointDatabase.IsTransitionValid(sourcePoint);
    }

    /// <summary>
    /// Check if the system is currently transitioning
    /// </summary>
    public bool IsTransitioning()
    {
        return isTransitioning;
    }

    // ==================== Database Management ====================

    /// <summary>
    /// Set the transition point database
    /// </summary>
    public void SetTransitionPointDatabase(TransitionPointDatabase database)
    {
        transitionPointDatabase = database;
        Debug.Log("TransitionPointSystem: Database reference updated");
    }

    /// <summary>
    /// Add a new transition point to the database
    /// </summary>
    public void AddTransitionPoint(TransitionPointData transitionPoint)
    {
        if (transitionPointDatabase != null)
        {
            transitionPointDatabase.AddOrUpdateTransitionPoint(transitionPoint);
        }
    }

    /// <summary>
    /// Remove a transition point from the database
    /// </summary>
    public void RemoveTransitionPoint(string transitionPointId)
    {
        if (transitionPointDatabase != null)
        {
            transitionPointDatabase.RemoveTransitionPoint(transitionPointId);
        }
    }

    /// <summary>
    /// Clear all transition points for a scene
    /// </summary>
    public void ClearTransitionPointsForScene(string sceneName)
    {
        if (transitionPointDatabase != null)
        {
            transitionPointDatabase.ClearTransitionPointsForScene(sceneName);
        }
    }

    // ==================== Debug Methods ====================

    /// <summary>
    /// Get debug information about the transition point system
    /// </summary>
    public string GetDebugInfo()
    {
        if (transitionPointDatabase == null)
        {
            return "TransitionPointSystem: No database assigned";
        }

        return $"TransitionPointSystem: {transitionPointDatabase.GetDebugInfo()}";
    }

    /// <summary>
    /// Validate the transition point system
    /// </summary>
    [ContextMenu("Validate Transition Point System")]
    public void ValidateTransitionPointSystem()
    {
        if (transitionPointDatabase != null)
        {
            transitionPointDatabase.ValidateDatabase();
        }
        else
        {
            Debug.LogError("TransitionPointSystem: No database assigned for validation!");
        }
    }
}
