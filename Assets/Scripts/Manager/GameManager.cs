using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Core game coordinator - manages and coordinates all sub-managers
/// Provides unified API for game systems
/// Singleton pattern ensures only one instance exists
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private GameObject player;
    
    [Header("UI Reference")]
    [SerializeField] private Canvas gameCanvas;

    // Singleton instance
    private static GameManager instance;

    // Sub-managers
    private SpawnPointManager spawnPointManager;
    private SceneTransitionManager sceneTransitionManager;
    private PersistenceManager persistenceManager;

    // ==================== Singleton & Initialization ====================

    void Awake()
    {
        // Singleton pattern - ensure only one GameManager exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeManagers();
    }

    /// <summary>
    /// Initialize all sub-managers
    /// </summary>
    private void InitializeManagers()
    {
        // Create or get sub-managers
        spawnPointManager = GetOrCreateManager<SpawnPointManager>();
        sceneTransitionManager = GetOrCreateManager<SceneTransitionManager>();
        persistenceManager = GetOrCreateManager<PersistenceManager>();

        // Set references
        sceneTransitionManager.SetPlayer(player);
        persistenceManager.SetPlayer(player);
        persistenceManager.SetCanvas(gameCanvas);

        // Initialize persistence
        persistenceManager.Initialize();

        // Subscribe to scene transition events
        sceneTransitionManager.OnSceneTransitionComplete += OnSceneTransitioned;

        Debug.Log("GameManager: All managers initialized");
    }

    /// <summary>
    /// Get or create a manager component
    /// </summary>
    private T GetOrCreateManager<T>() where T : MonoBehaviour
    {
        T manager = GetComponent<T>();
        if (manager == null)
        {
            manager = gameObject.AddComponent<T>();
            Debug.Log($"GameManager: Created {typeof(T).Name}");
        }
        return manager;
    }

    /// <summary>
    /// Called when scene transition is complete
    /// </summary>
    private void OnSceneTransitioned(string sceneName)
    {
        Debug.Log($"GameManager: Scene transition to '{sceneName}' complete");
        
        // Move player to appropriate position
        sceneTransitionManager.MovePlayerToScenePosition(sceneName);
    }

    /// <summary>
    /// Get the singleton instance
    /// </summary>
    public static GameManager GetInstance()
    {
        return instance;
    }

    // ==================== Scene Management API ====================

    /// <summary>
    /// Load a new scene by name
    /// </summary>
    public void LoadScene(string sceneName)
    {
        sceneTransitionManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Load a new scene by build index
    /// </summary>
    public void LoadScene(int sceneIndex)
    {
        sceneTransitionManager.LoadScene(sceneIndex);
    }

    // ==================== Spawn Point API ====================

    /// <summary>
    /// Register a new spawn point (chair)
    /// </summary>
    public void RegisterSpawnPoint(string spawnPointId, string displayName, string locationDescription, Vector3 position, string sceneName)
    {
        spawnPointManager.RegisterSpawnPoint(spawnPointId, displayName, locationDescription, position, sceneName);
    }

    /// <summary>
    /// Set the current spawn point by ID
    /// </summary>
    public void SetCurrentSpawnPoint(string spawnPointId)
    {
        spawnPointManager.SetCurrentSpawnPoint(spawnPointId);
    }

    /// <summary>
    /// Set the current spawn point by ID, position, and scene
    /// </summary>
    public void SetCurrentSpawnPoint(string spawnPointId, Vector3 position, string sceneName)
    {
        spawnPointManager.SetCurrentSpawnPoint(spawnPointId, position, sceneName);
    }

    /// <summary>
    /// Teleport player to a specific spawn point (chair)
    /// This is for fast travel - player actively chooses to teleport to a chair
    /// </summary>
    public void TeleportToSpawnPoint(string spawnPointId)
    {
        SpawnPointData spawnPoint = spawnPointManager.GetSpawnPointById(spawnPointId);
        
        if (spawnPoint == null)
        {
            Debug.LogWarning($"GameManager: Cannot teleport to '{spawnPointId}' - spawn point not found!");
            return;
        }

        // Set as current spawn point
        spawnPointManager.SetCurrentSpawnPoint(spawnPointId);

        // Save current position before teleporting
        sceneTransitionManager.SaveCurrentPlayerPosition();

        // If spawn point is in different scene, load that scene
        string currentScene = SceneManager.GetActiveScene().name;
        if (spawnPoint.sceneName != currentScene)
        {
            Debug.Log($"GameManager: Teleporting to scene {spawnPoint.sceneName}");
            LoadScene(spawnPoint.sceneName);
            
            // After scene loads, move player to spawn point
            // This will be handled by a callback
            StartCoroutine(MovePlayerAfterSceneLoad(spawnPoint));
        }
        else
        {
            // Teleport within same scene
            sceneTransitionManager.MovePlayerToPosition(spawnPoint.position, $"spawn point '{spawnPointId}'");
        }
    }

    /// <summary>
    /// Coroutine to move player after scene loads
    /// </summary>
    private System.Collections.IEnumerator MovePlayerAfterSceneLoad(SpawnPointData spawnPoint)
    {
        // Wait for scene to load
        yield return new WaitForSeconds(0.1f);
        
        // Move player to spawn point
        sceneTransitionManager.MovePlayerToPosition(spawnPoint.position, $"spawn point '{spawnPoint.spawnPointId}'");
    }

    /// <summary>
    /// Respawn player at current spawn point (R key functionality)
    /// </summary>
    public void RespawnPlayer()
    {
        SpawnPointData currentSpawnPoint = spawnPointManager.GetCurrentSpawnPoint();
        
        if (currentSpawnPoint == null)
        {
            Debug.LogWarning("GameManager: No current spawn point to respawn at!");
            return;
        }

        TeleportToSpawnPoint(currentSpawnPoint.spawnPointId);
    }

    /// <summary>
    /// Get the current spawn point
    /// </summary>
    public SpawnPointData GetCurrentSpawnPoint()
    {
        return spawnPointManager.GetCurrentSpawnPoint();
    }

    /// <summary>
    /// Get all discovered spawn points
    /// </summary>
    public System.Collections.Generic.List<SpawnPointData> GetDiscoveredSpawnPoints()
    {
        return spawnPointManager.GetDiscoveredSpawnPoints();
    }

    /// <summary>
    /// Check if a spawn point is discovered
    /// </summary>
    public bool IsSpawnPointDiscovered(string spawnPointId)
    {
        return spawnPointManager.IsSpawnPointDiscovered(spawnPointId);
    }

    /// <summary>
    /// Clear all spawn points (useful for testing)
    /// </summary>
    public void ClearSpawnPoints()
    {
        spawnPointManager.ClearSpawnPoints();
    }

    // ==================== Scene Position API ====================

    /// <summary>
    /// Save player's current position for the specified scene
    /// </summary>
    public void SavePlayerPositionForScene(string sceneName, Vector3 position)
    {
        sceneTransitionManager.SavePlayerPositionForScene(sceneName, position);
    }

    /// <summary>
    /// Get player's saved position for a scene
    /// </summary>
    public Vector3? GetPlayerPositionForScene(string sceneName)
    {
        return sceneTransitionManager.GetPlayerPositionForScene(sceneName);
    }

    /// <summary>
    /// Clear all saved scene positions (useful for testing)
    /// </summary>
    public void ClearScenePositions()
    {
        sceneTransitionManager.ClearScenePositions();
    }

    // ==================== Manager Access ====================

    /// <summary>
    /// Get the spawn point manager
    /// </summary>
    public SpawnPointManager GetSpawnPointManager()
    {
        return spawnPointManager;
    }

    /// <summary>
    /// Get the scene transition manager
    /// </summary>
    public SceneTransitionManager GetSceneTransitionManager()
    {
        return sceneTransitionManager;
    }

    /// <summary>
    /// Get the persistence manager
    /// </summary>
    public PersistenceManager GetPersistenceManager()
    {
        return persistenceManager;
    }

    // ==================== Cleanup ====================

    void OnDestroy()
    {
        if (sceneTransitionManager != null)
        {
            sceneTransitionManager.OnSceneTransitionComplete -= OnSceneTransitioned;
        }
    }
}
