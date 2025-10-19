using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Manages scene transitions and player position tracking across scenes
/// Implements Hollow Knight-style scene exploration
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private GameObject player;

    // Scene position tracking
    private Dictionary<string, Vector3> scenePlayerPositions = new Dictionary<string, Vector3>();
    private string lastSceneName = "";

    // Events
    public delegate void SceneLoadedHandler(string sceneName);
    public event SceneLoadedHandler OnSceneTransitionComplete;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ==================== Scene Loading ====================

    /// <summary>
    /// Load a new scene by name
    /// </summary>
    public void LoadScene(string sceneName)
    {
        Debug.Log($"SceneTransitionManager: Loading scene - {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Load a new scene by build index
    /// </summary>
    public void LoadScene(int sceneIndex)
    {
        Debug.Log($"SceneTransitionManager: Loading scene - {sceneIndex}");
        SceneManager.LoadScene(sceneIndex);
    }

    /// <summary>
    /// Called when a new scene is loaded
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"SceneTransitionManager: Scene loaded - {scene.name}");
        
        // Save player position from previous scene
        if (!string.IsNullOrEmpty(lastSceneName) && player != null)
        {
            scenePlayerPositions[lastSceneName] = player.transform.position;
            Debug.Log($"SceneTransitionManager: Saved player position for scene '{lastSceneName}': {player.transform.position}");
        }
        
        // Update last scene name
        lastSceneName = scene.name;

        // Notify listeners
        OnSceneTransitionComplete?.Invoke(scene.name);
    }

    // ==================== Player Position Management ====================

    /// <summary>
    /// Move player to appropriate position when entering a scene
    /// Priority: Saved position > Default spawn point > Keep current position
    /// </summary>
    public void MovePlayerToScenePosition(string sceneName)
    {
        if (player == null)
        {
            Debug.LogWarning("SceneTransitionManager: No player to move!");
            return;
        }

        // 1. Try to use saved position for this scene
        if (scenePlayerPositions.ContainsKey(sceneName))
        {
            Vector3 savedPosition = scenePlayerPositions[sceneName];
            player.transform.position = savedPosition;
            Debug.Log($"SceneTransitionManager: Player moved to saved position in '{sceneName}': {savedPosition}");
            return;
        }

        // 2. Try to find default spawn point in scene
        GameObject defaultSpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
        if (defaultSpawnPoint != null)
        {
            player.transform.position = defaultSpawnPoint.transform.position;
            Debug.Log($"SceneTransitionManager: Player moved to default spawn point in '{sceneName}': {defaultSpawnPoint.transform.position}");
            return;
        }

        // 3. Last resort: Keep current position
        Debug.LogWarning($"SceneTransitionManager: No spawn point found for scene '{sceneName}'. Player position unchanged.");
    }

    /// <summary>
    /// Move player to a specific position
    /// </summary>
    public void MovePlayerToPosition(Vector3 position, string locationDescription = "")
    {
        if (player == null)
        {
            Debug.LogWarning("SceneTransitionManager: No player to move!");
            return;
        }

        player.transform.position = position;
        string logMessage = string.IsNullOrEmpty(locationDescription) 
            ? $"Player moved to {position}" 
            : $"Player moved to {locationDescription} at {position}";
        Debug.Log($"SceneTransitionManager: {logMessage}");
    }

    // ==================== Position Tracking ====================

    /// <summary>
    /// Save player's current position for the specified scene
    /// </summary>
    public void SavePlayerPositionForScene(string sceneName, Vector3 position)
    {
        scenePlayerPositions[sceneName] = position;
        Debug.Log($"SceneTransitionManager: Saved position for scene '{sceneName}': {position}");
    }

    /// <summary>
    /// Save player's current position for current scene
    /// </summary>
    public void SaveCurrentPlayerPosition()
    {
        if (player == null)
        {
            Debug.LogWarning("SceneTransitionManager: No player to save position!");
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name;
        SavePlayerPositionForScene(currentScene, player.transform.position);
    }

    /// <summary>
    /// Get player's saved position for a scene
    /// </summary>
    public Vector3? GetPlayerPositionForScene(string sceneName)
    {
        return scenePlayerPositions.ContainsKey(sceneName) ? scenePlayerPositions[sceneName] : null;
    }

    /// <summary>
    /// Clear all saved scene positions (useful for testing)
    /// </summary>
    public void ClearScenePositions()
    {
        scenePlayerPositions.Clear();
        Debug.Log("SceneTransitionManager: All scene positions cleared");
    }

    // ==================== Utility Methods ====================

    /// <summary>
    /// Set the player reference
    /// </summary>
    public void SetPlayer(GameObject playerObject)
    {
        player = playerObject;
        Debug.Log("SceneTransitionManager: Player reference set");
    }

    /// <summary>
    /// Get the current scene name
    /// </summary>
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Get the last scene name
    /// </summary>
    public string GetLastSceneName()
    {
        return lastSceneName;
    }
}

