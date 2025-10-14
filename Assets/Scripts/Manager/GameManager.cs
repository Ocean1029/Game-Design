using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Manages game-wide systems and handles scene transitions
/// Ensures Player and UI persist across scene changes
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Persistent Objects")]
    [Tooltip("The player that should persist across scenes")]
    [SerializeField] private GameObject player;
    
    [Tooltip("The UI canvas that should persist across scenes")]
    [SerializeField] private Canvas gameCanvas;
    
    [Header("Spawn Point System")]
    [Tooltip("Whether to move player to spawn point when scene loads")]
    [SerializeField] private bool movePlayerToSpawnOnLoad = true;
    
    private static GameManager instance;
    private bool isInitialized = false;
    
    // Spawn point system
    private List<SpawnPointData> discoveredSpawnPoints = new List<SpawnPointData>();
    private SpawnPointData currentSpawnPoint = null;

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

        InitializePersistentObjects();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Make player and UI persist across scenes
    /// </summary>
    private void InitializePersistentObjects()
    {
        if (isInitialized) return;

        // Make player persistent
        if (player != null)
        {
            DontDestroyOnLoad(player);
            Debug.Log("GameManager: Player set to persist across scenes");
        }
        else
        {
            Debug.LogWarning("GameManager: No player assigned!");
        }

        // Make UI Canvas persistent
        if (gameCanvas != null)
        {
            DontDestroyOnLoad(gameCanvas.gameObject);
            Debug.Log("GameManager: UI Canvas set to persist across scenes");
        }
        else
        {
            Debug.LogWarning("GameManager: No canvas assigned!");
        }

        isInitialized = true;
    }

    /// <summary>
    /// Called when a new scene is loaded
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"GameManager: Scene loaded - {scene.name}");
        
        // Move player to spawn point if enabled
        if (movePlayerToSpawnOnLoad && currentSpawnPoint != null)
        {
            MovePlayerToCurrentSpawnPoint();
        }
    }

    /// <summary>
    /// Move player to the spawn point in the current scene
    /// </summary>
    private void MovePlayerToSpawnPoint()
    {
        // Look for a spawn point in the new scene
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
        
        if (spawnPoint != null)
        {
            player.transform.position = spawnPoint.transform.position;
            Debug.Log($"GameManager: Player moved to spawn point at {spawnPoint.transform.position}");
        }
        else
        {
            Debug.LogWarning("GameManager: No spawn point found in scene. Add a GameObject with tag 'PlayerSpawn'");
        }
    }

    /// <summary>
    /// Load a new scene by name
    /// </summary>
    public void LoadScene(string sceneName)
    {
        Debug.Log($"GameManager: Loading scene - {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Load a new scene by build index
    /// </summary>
    public void LoadScene(int sceneIndex)
    {
        Debug.Log($"GameManager: Loading scene - {sceneIndex}");
        SceneManager.LoadScene(sceneIndex);
    }

    /// <summary>
    /// Move player to the current spawn point
    /// </summary>
    private void MovePlayerToCurrentSpawnPoint()
    {
        if (currentSpawnPoint == null || player == null)
        {
            Debug.LogWarning("GameManager: No current spawn point or player to move");
            return;
        }

        // If spawn point is in different scene, load that scene
        if (currentSpawnPoint.sceneName != SceneManager.GetActiveScene().name)
        {
            Debug.Log($"GameManager: Loading scene {currentSpawnPoint.sceneName} for spawn point");
            SceneManager.LoadScene(currentSpawnPoint.sceneName);
            return;
        }

        // Move player to spawn point position
        player.transform.position = currentSpawnPoint.position;
        Debug.Log($"GameManager: Player moved to spawn point '{currentSpawnPoint.spawnPointId}' at {currentSpawnPoint.position}");
    }

    // ==================== Spawn Point System ====================

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
            Debug.Log($"GameManager: Updated existing spawn point '{spawnPointId}'");
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
            Debug.Log($"GameManager: Registered new spawn point '{spawnPointId}' - {displayName}");
        }
    }

    /// <summary>
    /// Set the current spawn point by ID
    /// </summary>
    public void SetCurrentSpawnPoint(string spawnPointId)
    {
        SpawnPointData spawnPoint = discoveredSpawnPoints.Find(sp => sp.spawnPointId == spawnPointId);
        
        if (spawnPoint != null)
        {
            currentSpawnPoint = spawnPoint;
            Debug.Log($"GameManager: Current spawn point set to '{spawnPointId}' - {spawnPoint.displayName}");
        }
        else
        {
            Debug.LogWarning($"GameManager: Spawn point '{spawnPointId}' not found!");
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
        Debug.Log($"GameManager: Current spawn point set to '{spawnPointId}' at {position} in {sceneName}");
    }

    /// <summary>
    /// Teleport player to a specific spawn point
    /// </summary>
    public void TeleportToSpawnPoint(string spawnPointId)
    {
        SpawnPointData spawnPoint = discoveredSpawnPoints.Find(sp => sp.spawnPointId == spawnPointId);
        
        if (spawnPoint == null)
        {
            Debug.LogWarning($"GameManager: Cannot teleport to '{spawnPointId}' - spawn point not found!");
            return;
        }

        // Set as current spawn point
        currentSpawnPoint = spawnPoint;

        // If spawn point is in different scene, load that scene
        if (spawnPoint.sceneName != SceneManager.GetActiveScene().name)
        {
            Debug.Log($"GameManager: Teleporting to scene {spawnPoint.sceneName}");
            SceneManager.LoadScene(spawnPoint.sceneName);
        }
        else
        {
            // Teleport within same scene
            if (player != null)
            {
                player.transform.position = spawnPoint.position;
                Debug.Log($"GameManager: Teleported to '{spawnPointId}' at {spawnPoint.position}");
            }
        }
    }

    /// <summary>
    /// Respawn player at current spawn point
    /// </summary>
    public void RespawnPlayer()
    {
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
    /// Clear all spawn points (useful for testing)
    /// </summary>
    public void ClearSpawnPoints()
    {
        discoveredSpawnPoints.Clear();
        currentSpawnPoint = null;
        Debug.Log("GameManager: All spawn points cleared");
    }

    /// <summary>
    /// Get the singleton instance
    /// </summary>
    public static GameManager GetInstance()
    {
        return instance;
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
}


