using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Spawn Settings")]
    [Tooltip("Should the player be moved to spawn point on scene load?")]
    [SerializeField] private bool movePlayerToSpawnOnLoad = true;

    private static GameManager instance;
    private bool isInitialized = false;

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

        if (movePlayerToSpawnOnLoad && player != null)
        {
            MovePlayerToSpawnPoint();
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
    /// Get the singleton instance
    /// </summary>
    public static GameManager GetInstance()
    {
        return instance;
    }
}


