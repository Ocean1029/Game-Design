using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Game Manager for Metroidvania-style game
/// Integrates Spawn Point System and Transition Point System
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Database References")]
    [Tooltip("Spawn Point Database ScriptableObject")]
    [SerializeField] private SpawnPointDatabase spawnPointDatabase;
    
    [Tooltip("Transition Point Database ScriptableObject")]
    [SerializeField] private TransitionPointDatabase transitionPointDatabase;

    [Header("Player Reference")]
    [Tooltip("Player GameObject reference")]
    [SerializeField] private GameObject player;

    [Header("System References")]
    [Tooltip("Spawn Point System component")]
    [SerializeField] private SpawnPointSystem spawnPointSystem;
    
    [Tooltip("Transition Point System component")]
    [SerializeField] private TransitionPointSystem transitionPointSystem;

    [Header("Settings")]
    [Tooltip("Whether to show debug information")]
    [SerializeField] private bool showDebugInfo = false;

    // Singleton instance
    private static GameManager instance;

    // Events
    public delegate void GameInitializedHandler();
    public event GameInitializedHandler OnGameInitialized;

    public delegate void SceneTransitionStartedHandler(string fromScene, string toScene);
    public event SceneTransitionStartedHandler OnSceneTransitionStarted;

    public delegate void SceneTransitionCompletedHandler(string sceneName);
    public event SceneTransitionCompletedHandler OnSceneTransitionCompleted;

    // ==================== Singleton & Initialization ====================

    void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize systems in Awake to ensure they're ready before any Start() calls
        InitializeSystems();
    }

    void Start()
    {
        // Subscribe to system events
        SubscribeToSystemEvents();

        // Notify initialization complete
        OnGameInitialized?.Invoke();

        if (showDebugInfo)
        {
            Debug.Log("GameManager: Game initialized successfully");
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        UnsubscribeFromSystemEvents();
    }

    /// <summary>
    /// Initialize all game systems
    /// </summary>
    private void InitializeSystems()
    {
        // Initialize Spawn Point System
        if (spawnPointSystem == null)
        {
            spawnPointSystem = gameObject.AddComponent<SpawnPointSystem>();
        }

        if (spawnPointDatabase != null)
        {
            spawnPointSystem.SetSpawnPointDatabase(spawnPointDatabase);
            if (showDebugInfo)
            {
                Debug.Log("GameManager: Spawn Point Database assigned");
            }
        }
        else
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("GameManager: No Spawn Point Database assigned. System will use temporary storage.");
            }
        }

        // Initialize Transition Point System
        if (transitionPointSystem == null)
        {
            transitionPointSystem = gameObject.AddComponent<TransitionPointSystem>();
        }

        if (transitionPointDatabase != null)
        {
            transitionPointSystem.SetTransitionPointDatabase(transitionPointDatabase);
            if (showDebugInfo)
            {
                Debug.Log("GameManager: Transition Point Database assigned");
            }
        }
        else
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("GameManager: No Transition Point Database assigned. System will use temporary storage.");
            }
        }

        // Set player reference if not assigned
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>()?.gameObject;
        }

        if (showDebugInfo)
        {
            Debug.Log("GameManager: Systems initialized");
        }
    }

    /// <summary>
    /// Subscribe to system events
    /// </summary>
    private void SubscribeToSystemEvents()
    {
        // Subscribe to spawn point system events
        if (spawnPointSystem != null)
        {
            spawnPointSystem.OnSpawnPointActivated += OnSpawnPointActivated;
            spawnPointSystem.OnSpawnPointDeactivated += OnSpawnPointDeactivated;
        }

        // Subscribe to transition point system events
        if (transitionPointSystem != null)
        {
            transitionPointSystem.OnTransitionStarted += OnTransitionStarted;
            transitionPointSystem.OnTransitionCompleted += OnTransitionCompleted;
            transitionPointSystem.OnTransitionFailed += OnTransitionFailed;
        }

        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Unsubscribe from system events
    /// </summary>
    private void UnsubscribeFromSystemEvents()
    {
        // Unsubscribe from spawn point system events
        if (spawnPointSystem != null)
        {
            spawnPointSystem.OnSpawnPointActivated -= OnSpawnPointActivated;
            spawnPointSystem.OnSpawnPointDeactivated -= OnSpawnPointDeactivated;
        }

        // Unsubscribe from transition point system events
        if (transitionPointSystem != null)
        {
            transitionPointSystem.OnTransitionStarted -= OnTransitionStarted;
            transitionPointSystem.OnTransitionCompleted -= OnTransitionCompleted;
            transitionPointSystem.OnTransitionFailed -= OnTransitionFailed;
        }

        // Unsubscribe from scene loaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ==================== Singleton Access ====================

    /// <summary>
    /// Get the singleton instance
    /// </summary>
    public static GameManager GetInstance()
    {
        return instance;
    }

    // ==================== Spawn Point System API ====================

    /// <summary>
    /// Activate a spawn point
    /// </summary>
    public void ActivateSpawnPoint(string spawnPointId)
    {
        if (spawnPointSystem != null)
        {
            spawnPointSystem.ActivateSpawnPoint(spawnPointId);
        }
    }

    /// <summary>
    /// Get the currently active spawn point
    /// </summary>
    public SpawnPointData GetCurrentActiveSpawnPoint()
    {
        return spawnPointSystem?.GetCurrentActiveSpawnPoint();
    }

    /// <summary>
    /// Get spawn points that can be used for teleportation
    /// </summary>
    public List<SpawnPointData> GetTransportableSpawnPoints()
    {
        return spawnPointSystem?.GetTransportableSpawnPoints() ?? new List<SpawnPointData>();
    }

    /// <summary>
    /// Teleport player to a specific spawn point
    /// </summary>
    public bool TeleportToSpawnPoint(string spawnPointId)
    {
        return spawnPointSystem?.TeleportToSpawnPoint(spawnPointId) ?? false;
    }

    /// <summary>
    /// Respawn player at the last active spawn point
    /// </summary>
    public bool RespawnPlayer()
    {
        return spawnPointSystem?.RespawnPlayer() ?? false;
    }

    // ==================== Transition Point System API ====================

    /// <summary>
    /// Transition through a transition point
    /// </summary>
    public bool TransitionThroughPoint(string transitionPointId)
    {
        return transitionPointSystem?.TransitionThroughPoint(transitionPointId) ?? false;
    }

    /// <summary>
    /// Get transition points in the current scene
    /// </summary>
    public List<TransitionPointData> GetTransitionPointsInCurrentScene()
    {
        return transitionPointSystem?.GetTransitionPointsInCurrentScene() ?? new List<TransitionPointData>();
    }

    /// <summary>
    /// Get transition points in a specific scene
    /// </summary>
    public List<TransitionPointData> GetTransitionPointsInScene(string sceneName)
    {
        return transitionPointSystem?.GetTransitionPointsInScene(sceneName) ?? new List<TransitionPointData>();
    }

    /// <summary>
    /// Check if a transition is valid
    /// </summary>
    public bool IsTransitionValid(string transitionPointId)
    {
        return transitionPointSystem?.IsTransitionValid(transitionPointId) ?? false;
    }

    /// <summary>
    /// Check if the system is currently transitioning
    /// </summary>
    public bool IsTransitioning()
    {
        return transitionPointSystem?.IsTransitioning() ?? false;
    }

    // ==================== Scene Management ====================

    /// <summary>
    /// Load a scene by name
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (IsTransitioning())
        {
            Debug.LogWarning("GameManager: Cannot load scene while transitioning");
            return;
        }

        OnSceneTransitionStarted?.Invoke(SceneManager.GetActiveScene().name, sceneName);
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Load a scene by build index
    /// </summary>
    public void LoadScene(int sceneIndex)
    {
        if (IsTransitioning())
        {
            Debug.LogWarning("GameManager: Cannot load scene while transitioning");
            return;
        }

        OnSceneTransitionStarted?.Invoke(SceneManager.GetActiveScene().name, $"Scene_{sceneIndex}");
        SceneManager.LoadScene(sceneIndex);
    }

    // ==================== Event Handlers ====================

    /// <summary>
    /// Called when a spawn point is activated
    /// </summary>
    private void OnSpawnPointActivated(SpawnPointData spawnPoint)
    {
        if (showDebugInfo)
        {
            Debug.Log($"GameManager: Spawn point activated - {spawnPoint.spawnPointId}");
        }
    }

    /// <summary>
    /// Called when a spawn point is deactivated
    /// </summary>
    private void OnSpawnPointDeactivated(SpawnPointData spawnPoint)
    {
        if (showDebugInfo)
        {
            Debug.Log($"GameManager: Spawn point deactivated - {spawnPoint.spawnPointId}");
        }
    }

    /// <summary>
    /// Called when a transition starts
    /// </summary>
    private void OnTransitionStarted(TransitionPointData sourcePoint, TransitionPointData targetPoint)
    {
        if (showDebugInfo)
        {
            Debug.Log($"GameManager: Transition started from {sourcePoint.sceneName} to {targetPoint.sceneName}");
        }
    }

    /// <summary>
    /// Called when a transition completes
    /// </summary>
    private void OnTransitionCompleted(TransitionPointData sourcePoint, TransitionPointData targetPoint)
    {
        if (showDebugInfo)
        {
            Debug.Log($"GameManager: Transition completed to {targetPoint.sceneName}");
        }

        OnSceneTransitionCompleted?.Invoke(targetPoint.sceneName);
    }

    /// <summary>
    /// Called when a transition fails
    /// </summary>
    private void OnTransitionFailed(TransitionPointData sourcePoint, string reason)
    {
        Debug.LogWarning($"GameManager: Transition failed - {reason}");
    }

    /// <summary>
    /// Called when a scene is loaded
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (showDebugInfo)
        {
            Debug.Log($"GameManager: Scene loaded - {scene.name}");
        }

        // Update player reference if needed
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>()?.gameObject;
        }

        OnSceneTransitionCompleted?.Invoke(scene.name);
    }

    // ==================== Database Management ====================

    /// <summary>
    /// Set the spawn point database
    /// </summary>
    public void SetSpawnPointDatabase(SpawnPointDatabase database)
    {
        spawnPointDatabase = database;
        if (spawnPointSystem != null)
        {
            spawnPointSystem.SetSpawnPointDatabase(database);
        }
    }

    /// <summary>
    /// Set the transition point database
    /// </summary>
    public void SetTransitionPointDatabase(TransitionPointDatabase database)
    {
        transitionPointDatabase = database;
        if (transitionPointSystem != null)
        {
            transitionPointSystem.SetTransitionPointDatabase(database);
        }
    }

    // ==================== System Access ====================

    /// <summary>
    /// Get the spawn point system
    /// </summary>
    public SpawnPointSystem GetSpawnPointSystem()
    {
        return spawnPointSystem;
    }

    /// <summary>
    /// Get the transition point system
    /// </summary>
    public TransitionPointSystem GetTransitionPointSystem()
    {
        return transitionPointSystem;
    }

    /// <summary>
    /// Get the player GameObject
    /// </summary>
    public GameObject GetPlayer()
    {
        return player;
    }

    // ==================== Save System API ====================

    /// <summary>
    /// Clear all save data (useful for testing or "New Game")
    /// </summary>
    public void ClearSaveData()
    {
        SaveSystem.ClearSaveData();
        Debug.Log("GameManager: Save data cleared");
    }

    /// <summary>
    /// Check if save data exists
    /// </summary>
    public bool HasSaveData()
    {
        return SaveSystem.HasSaveData();
    }

    /// <summary>
    /// Get save data information
    /// </summary>
    public string GetSaveDataInfo()
    {
        return SaveSystem.GetDebugInfo();
    }

    // ==================== Debug Methods ====================

    /// <summary>
    /// Get debug information about the game manager
    /// </summary>
    public string GetDebugInfo()
    {
        string info = "GameManager Debug Info:\n";
        
        if (spawnPointSystem != null)
        {
            info += $"Spawn Point System: {spawnPointSystem.GetDebugInfo()}\n";
        }
        
        if (transitionPointSystem != null)
        {
            info += $"Transition Point System: {transitionPointSystem.GetDebugInfo()}\n";
        }
        
        info += $"Current Scene: {SceneManager.GetActiveScene().name}\n";
        info += $"Player: {(player != null ? "Found" : "Not Found")}\n";
        info += $"Is Transitioning: {IsTransitioning()}\n";
        info += $"\n{SaveSystem.GetDebugInfo()}\n";

        return info;
    }

    /// <summary>
    /// Validate all systems
    /// </summary>
    [ContextMenu("Validate All Systems")]
    public void ValidateAllSystems()
    {
        Debug.Log("GameManager: Starting system validation...");

        if (spawnPointSystem != null)
        {
            spawnPointSystem.ValidateSpawnPointSystem();
        }

        if (transitionPointSystem != null)
        {
            transitionPointSystem.ValidateTransitionPointSystem();
        }

        Debug.Log("GameManager: System validation complete");
    }
}
