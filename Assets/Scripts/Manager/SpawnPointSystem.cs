using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// New Spawn Point System for Save Point management
/// Handles one-time use spawn points, teleportation restrictions, and save point persistence
/// </summary>
public class SpawnPointSystem : MonoBehaviour
{
    [Header("Database Reference")]
    [Tooltip("Spawn Point Database ScriptableObject")]
    [SerializeField] private SpawnPointDatabase spawnPointDatabase;

    [Header("Settings")]
    [Tooltip("Whether to restore player energy when activating spawn points")]
    [SerializeField] private bool restoreEnergyOnActivation = true;

    [Tooltip("Default energy restore amount (0 = restore all)")]
#pragma warning disable CS0414 // Field is assigned but its value is never used (Reserved for future use)
    [SerializeField] private int defaultEnergyRestoreAmount = 0;
#pragma warning restore CS0414

    // Events
    public delegate void SpawnPointActivatedHandler(SpawnPointData spawnPoint);
    public event SpawnPointActivatedHandler OnSpawnPointActivated;

    public delegate void SpawnPointDeactivatedHandler(SpawnPointData spawnPoint);
    public event SpawnPointDeactivatedHandler OnSpawnPointDeactivated;

    // Cached references
    private PlayerController playerController;
    private PlayerEnergy playerEnergy;

    void Awake()
    {
        // Database reference will be set later by GameManager
        // If no database is assigned, system will use temporary storage
    }

    void Start()
    {
        // Get player references
        playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            playerEnergy = playerController.GetComponent<PlayerEnergy>();
        }

        // Load saved spawn point and spawn player there
        LoadAndSpawnAtSavedLocation();

        // Check if there's an active spawn point, if not, try to find default
        CheckForDefaultSpawnPoint();
    }

    /// <summary>
    /// Load saved spawn point from save system and spawn player there
    /// </summary>
    private void LoadAndSpawnAtSavedLocation()
    {
        // Check if there's save data
        if (!SaveSystem.HasSaveData())
        {
            Debug.Log("SpawnPointSystem: No save data found, player will spawn at default location");
            return;
        }

        // Load the saved spawn point ID
        string savedSpawnPointId = SaveSystem.LoadLastSpawnPointID();
        string savedSceneName = SaveSystem.LoadLastSpawnScene();

        if (string.IsNullOrEmpty(savedSpawnPointId))
        {
            Debug.LogWarning("SpawnPointSystem: Save data corrupted, spawn point ID is null");
            return;
        }

        Debug.Log($"SaveSystem: Found save data - Spawn Point: '{savedSpawnPointId}', Scene: '{savedSceneName}'");

        // Get current scene
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // Check if the saved spawn point is in the database
        SpawnPointData savedSpawnPoint = spawnPointDatabase?.GetSpawnPointById(savedSpawnPointId);
        
        if (savedSpawnPoint != null)
        {
            // Spawn point exists in database
            Debug.Log($"SpawnPointSystem: Found saved spawn point '{savedSpawnPointId}' in database");
            
            // Set it as active
            spawnPointDatabase.SetActiveSpawnPoint(savedSpawnPointId);
            
            // If it's in the current scene, teleport player there
            if (savedSpawnPoint.sceneName == currentScene)
            {
                StartCoroutine(SpawnPlayerAtSavedLocation(savedSpawnPoint));
            }
            else
            {
                Debug.Log($"SpawnPointSystem: Saved spawn point is in different scene '{savedSpawnPoint.sceneName}', current scene is '{currentScene}'");
            }
        }
        else
        {
            // Spawn point not in database yet (might be registered later)
            Debug.Log($"SpawnPointSystem: Saved spawn point '{savedSpawnPointId}' not in database yet, will wait for registration");
            
            // Wait for the spawn point to be registered
            StartCoroutine(WaitForSpawnPointRegistration(savedSpawnPointId, savedSceneName, currentScene));
        }
    }

    /// <summary>
    /// Wait for a spawn point to be registered, then spawn player there
    /// </summary>
    private System.Collections.IEnumerator WaitForSpawnPointRegistration(string spawnPointId, string savedSceneName, string currentScene)
    {
        int maxRetries = 20; // Wait up to 2 seconds
        int retryCount = 0;

        while (retryCount < maxRetries)
        {
            yield return new WaitForSeconds(0.1f);
            retryCount++;

            SpawnPointData spawnPoint = spawnPointDatabase?.GetSpawnPointById(spawnPointId);
            if (spawnPoint != null)
            {
                Debug.Log($"SpawnPointSystem: Spawn point '{spawnPointId}' registered, spawning player");
                
                // Set as active
                spawnPointDatabase.SetActiveSpawnPoint(spawnPointId);
                
                // If it's in current scene, teleport player
                if (spawnPoint.sceneName == currentScene)
                {
                    TeleportPlayerToPosition(spawnPoint.position);
                }
                
                yield break;
            }
        }

        Debug.LogWarning($"SpawnPointSystem: Timeout waiting for spawn point '{spawnPointId}' to register");
    }

    /// <summary>
    /// Spawn player at saved location after a short delay
    /// </summary>
    private System.Collections.IEnumerator SpawnPlayerAtSavedLocation(SpawnPointData spawnPoint)
    {
        // Wait a bit for player to be fully initialized
        yield return new WaitForSeconds(0.2f);

        if (playerController != null)
        {
            TeleportPlayerToPosition(spawnPoint.position);
            Debug.Log($"SpawnPointSystem: Player spawned at saved location '{spawnPoint.displayName}'");
        }
        else
        {
            Debug.LogWarning("SpawnPointSystem: Cannot spawn player - PlayerController not found");
        }
    }

    /// <summary>
    /// Check if there's an active spawn point, if not, look for PlayerSpawn tag
    /// </summary>
    private void CheckForDefaultSpawnPoint()
    {
        if (spawnPointDatabase == null) return;

        // If there's already an active spawn point, we're good
        SpawnPointData currentActive = GetCurrentActiveSpawnPoint();
        if (currentActive != null)
        {
            Debug.Log($"SpawnPointSystem: Active spawn point found: '{currentActive.spawnPointId}'");
            return;
        }

        // Try to find a DefaultSpawnPoint component in the scene
        DefaultSpawnPoint defaultSpawn = FindFirstObjectByType<DefaultSpawnPoint>();
        if (defaultSpawn != null)
        {
            Debug.Log("SpawnPointSystem: DefaultSpawnPoint component found, it will auto-register");
            return;
        }

        // Fallback: Look for PlayerSpawn tag
        GameObject playerSpawnObj = GameObject.FindGameObjectWithTag("PlayerSpawn");
        if (playerSpawnObj != null)
        {
            Debug.Log($"SpawnPointSystem: Found PlayerSpawn tag at {playerSpawnObj.transform.position}, consider adding DefaultSpawnPoint component");
        }
        else
        {
            Debug.LogWarning("SpawnPointSystem: No active spawn point, no DefaultSpawnPoint component, and no PlayerSpawn tag found. Player respawn may not work correctly.");
        }
    }

    // ==================== Spawn Point Management ====================

    /// <summary>
    /// Activate a spawn point (save progress)
    /// </summary>
    public void ActivateSpawnPoint(string spawnPointId)
    {
        if (spawnPointDatabase == null)
        {
            Debug.LogError("SpawnPointSystem: No database assigned!");
            return;
        }

        SpawnPointData spawnPoint = spawnPointDatabase.GetSpawnPointById(spawnPointId);
        if (spawnPoint == null)
        {
            Debug.LogWarning($"SpawnPointSystem: Spawn point '{spawnPointId}' not found in database!");
            return;
        }

        // Update spawn point data with current scene information
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        spawnPoint.sceneName = currentScene;

        // Add or update the spawn point in database
        spawnPointDatabase.AddOrUpdateSpawnPoint(spawnPoint);

        // Set as active spawn point
        spawnPointDatabase.SetActiveSpawnPoint(spawnPointId);

        // Handle one-time use spawn points
        if (spawnPoint.isOneTimeUse)
        {
            HandleOneTimeUseSpawnPoint(spawnPoint);
        }

        // Restore energy if specified
        if (restoreEnergyOnActivation && playerEnergy != null && spawnPoint.restoresEnergy)
        {
            if (spawnPoint.energyRestoreAmount > 0)
            {
                playerEnergy.AddEnergy(spawnPoint.energyRestoreAmount);
            }
            else
            {
                playerEnergy.RestoreAllEnergy();
            }
        }

        // Play activation sound
        if (spawnPoint.activationSound != null)
        {
            AudioSource.PlayClipAtPoint(spawnPoint.activationSound, spawnPoint.position);
        }

        // Update visual indicator
        UpdateSpawnPointVisuals(spawnPoint, true);

        // Save to persistent storage
        SaveSystem.SaveSpawnPoint(spawnPoint);

        // Notify listeners
        OnSpawnPointActivated?.Invoke(spawnPoint);

        Debug.Log($"SpawnPointSystem: Activated spawn point '{spawnPoint.spawnPointId}' - {spawnPoint.displayName}");
    }

    /// <summary>
    /// Get the currently active spawn point
    /// </summary>
    public SpawnPointData GetCurrentActiveSpawnPoint()
    {
        return spawnPointDatabase?.CurrentActiveSpawnPoint;
    }

    /// <summary>
    /// Get the active spawn point for a specific scene
    /// </summary>
    public SpawnPointData GetActiveSpawnPointForScene(string sceneName)
    {
        return spawnPointDatabase?.GetActiveSpawnPointForScene(sceneName);
    }

    /// <summary>
    /// Get all spawn points that can be used for teleportation
    /// </summary>
    public List<SpawnPointData> GetTransportableSpawnPoints()
    {
        return spawnPointDatabase?.TransportableSpawnPoints ?? new List<SpawnPointData>();
    }

    /// <summary>
    /// Get all spawn points in a specific scene
    /// </summary>
    public List<SpawnPointData> GetSpawnPointsInScene(string sceneName)
    {
        return spawnPointDatabase?.GetSpawnPointsInScene(sceneName) ?? new List<SpawnPointData>();
    }

    /// <summary>
    /// Teleport player to a specific spawn point
    /// </summary>
    public bool TeleportToSpawnPoint(string spawnPointId)
    {
        if (spawnPointDatabase == null)
        {
            Debug.LogError("SpawnPointSystem: No database assigned!");
            return false;
        }

        SpawnPointData spawnPoint = spawnPointDatabase.GetSpawnPointById(spawnPointId);
        if (spawnPoint == null)
        {
            Debug.LogWarning($"SpawnPointSystem: Spawn point '{spawnPointId}' not found!");
            return false;
        }

        if (!spawnPoint.CanTeleportTo())
        {
            Debug.LogWarning($"SpawnPointSystem: Cannot teleport to spawn point '{spawnPointId}' - not teleportable!");
            return false;
        }

        // Check if spawn point is in different scene
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (spawnPoint.sceneName != currentScene)
        {
            // Load the target scene first
            GameManager newGameManager = GameManager.GetInstance();
            if (newGameManager != null)
            {
                newGameManager.LoadScene(spawnPoint.sceneName);
            }
            else
            {
                // Fallback to direct scene loading
                UnityEngine.SceneManagement.SceneManager.LoadScene(spawnPoint.sceneName);
            }
            
            // The actual teleportation will happen after scene loads
            StartCoroutine(TeleportAfterSceneLoad(spawnPoint));
            return true;
        }
        else
        {
            // Teleport within same scene
            TeleportPlayerToPosition(spawnPoint.position);
            return true;
        }
    }

    /// <summary>
    /// Respawn player at the last active spawn point
    /// If no spawn point exists, respawn at default PlayerSpawn in current scene
    /// </summary>
    public bool RespawnPlayer()
    {
        SpawnPointData activeSpawnPoint = GetCurrentActiveSpawnPoint();
        
        // If no active spawn point, try to find default spawn point in current scene
        if (activeSpawnPoint == null)
        {
            Debug.LogWarning("SpawnPointSystem: No active spawn point found. Looking for default PlayerSpawn...");
            return RespawnAtDefaultSpawnPoint();
        }

        // Validate spawn point has scene name
        if (string.IsNullOrEmpty(activeSpawnPoint.sceneName))
        {
            Debug.LogWarning($"SpawnPointSystem: Spawn point '{activeSpawnPoint.spawnPointId}' has no scene name. Using default spawn point.");
            return RespawnAtDefaultSpawnPoint();
        }

        Debug.Log($"SpawnPointSystem: Respawning player at '{activeSpawnPoint.spawnPointId}' - {activeSpawnPoint.displayName}");

        // Check if we need to load a different scene
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (activeSpawnPoint.sceneName != currentScene)
        {
            // Load the spawn point's scene
            GameManager newGameManager = GameManager.GetInstance();
            if (newGameManager != null)
            {
                newGameManager.LoadScene(activeSpawnPoint.sceneName);
            }
            else
            {
                // Fallback to direct scene loading
                UnityEngine.SceneManagement.SceneManager.LoadScene(activeSpawnPoint.sceneName);
            }
            
            // The actual respawn will happen after scene loads
            StartCoroutine(RespawnAfterSceneLoad(activeSpawnPoint));
            return true;
        }
        else
        {
            // Respawn in same scene
            TeleportPlayerToPosition(activeSpawnPoint.position);
            HandlePostRespawnState(activeSpawnPoint);
            return true;
        }
    }

    /// <summary>
    /// Respawn at the default spawn point in current scene (PlayerSpawn tag)
    /// </summary>
    private bool RespawnAtDefaultSpawnPoint()
    {
        // Try to find PlayerSpawn tag in current scene
        GameObject defaultSpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
        
        if (defaultSpawnPoint != null)
        {
            TeleportPlayerToPosition(defaultSpawnPoint.transform.position);
            Debug.Log($"SpawnPointSystem: Respawned at default PlayerSpawn at {defaultSpawnPoint.transform.position}");
            return true;
        }
        
        Debug.LogWarning("SpawnPointSystem: No default PlayerSpawn found in current scene. Player position unchanged.");
        return false;
    }

    // ==================== Helper Methods ====================

    /// <summary>
    /// Handle one-time use spawn point activation
    /// </summary>
    private void HandleOneTimeUseSpawnPoint(SpawnPointData newSpawnPoint)
    {
        // Deactivate all other one-time use spawn points
        var allSpawnPoints = spawnPointDatabase.AllSpawnPoints;
        foreach (var spawnPoint in allSpawnPoints)
        {
            if (spawnPoint.isOneTimeUse && 
                spawnPoint.spawnPointId != newSpawnPoint.spawnPointId && 
                spawnPoint.isActive)
            {
                DeactivateSpawnPoint(spawnPoint);
            }
        }
    }

    /// <summary>
    /// Deactivate a spawn point
    /// </summary>
    private void DeactivateSpawnPoint(SpawnPointData spawnPoint)
    {
        spawnPoint.isActive = false;
        UpdateSpawnPointVisuals(spawnPoint, false);
        OnSpawnPointDeactivated?.Invoke(spawnPoint);
        
        Debug.Log($"SpawnPointSystem: Deactivated spawn point '{spawnPoint.spawnPointId}'");
    }

    /// <summary>
    /// Update visual indicators for spawn points
    /// </summary>
    private void UpdateSpawnPointVisuals(SpawnPointData spawnPoint, bool isActive)
    {
        if (spawnPoint.activeVisual != null)
        {
            spawnPoint.activeVisual.SetActive(isActive);
        }
    }

    /// <summary>
    /// Teleport player to a specific position
    /// </summary>
    private void TeleportPlayerToPosition(Vector3 position)
    {
        if (playerController != null)
        {
            // Trigger teleport animation before teleporting
            PlayerAnimationController animationController = playerController.GetComponent<PlayerAnimationController>();
            if (animationController != null && animationController.IsAnimationSystemReady())
            {
                animationController.TriggerTeleport();
            }

            // Use the PlayerMovement's Teleport method which handles ground detection
            PlayerMovement playerMovement = playerController.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.Teleport(position);
                Debug.Log($"SpawnPointSystem: Teleported player to {position} (with ground detection)");
            }
            else
            {
                // Fallback to direct position setting if PlayerMovement not found
                playerController.transform.position = position;
                Debug.LogWarning($"SpawnPointSystem: PlayerMovement not found, teleported to {position} without ground detection");
            }
        }
    }

    private void HandlePostRespawnState(SpawnPointData spawnPoint)
    {
        if (spawnPoint == null)
        {
            return;
        }

        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerController>();
        }

        if (playerController != null && playerEnergy == null)
        {
            playerEnergy = playerController.GetComponent<PlayerEnergy>();
        }

        if (playerController == null)
        {
            Debug.LogWarning("SpawnPointSystem: PlayerController not found during respawn handling");
            RestoreEnergyToFull();
            return;
        }

        if (spawnPoint.isChairSpawn)
        {
            // When respawning at a chair, restore energy but don't sit the player down
            // The player should be standing and able to move immediately
            RestoreEnergyToFull();
            Debug.Log($"SpawnPointSystem: Player respawned at chair '{spawnPoint.spawnPointId}' (standing, energy restored)");
        }
        else
        {
            RestoreEnergyToFull();
        }
    }

    private void RestoreEnergyToFull()
    {
        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerController>();
        }

        if (playerController != null && playerEnergy == null)
        {
            playerEnergy = playerController.GetComponent<PlayerEnergy>();
        }

        if (playerEnergy != null)
        {
            playerEnergy.RestoreAllEnergy();
            Debug.Log("SpawnPointSystem: Restored player energy to full after respawn");
        }
        else
        {
            Debug.LogWarning("SpawnPointSystem: PlayerEnergy component not found during energy restoration");
        }
    }

    private chair FindChairById(string chairId)
    {
        if (string.IsNullOrEmpty(chairId))
        {
            return null;
        }

        chair[] chairs = FindObjectsOfType<chair>();
        foreach (var chairInstance in chairs)
        {
            if (chairInstance != null && chairInstance.GetChairId() == chairId)
            {
                return chairInstance;
            }
        }

        return null;
    }

    /// <summary>
    /// Coroutine to teleport after scene loads
    /// </summary>
    private System.Collections.IEnumerator TeleportAfterSceneLoad(SpawnPointData spawnPoint)
    {
        // Wait for scene to load
        yield return new WaitForSeconds(0.1f);
        
        // Teleport player to spawn point position
        TeleportPlayerToPosition(spawnPoint.position);
        
        Debug.Log($"SpawnPointSystem: Teleported to spawn point '{spawnPoint.spawnPointId}' after scene load");
    }

    /// <summary>
    /// Coroutine to respawn after scene loads
    /// </summary>
    private System.Collections.IEnumerator RespawnAfterSceneLoad(SpawnPointData spawnPoint)
    {
        // Wait for scene to load
        yield return new WaitForSeconds(0.1f);
        
        // Respawn player at spawn point position
        TeleportPlayerToPosition(spawnPoint.position);
        
        HandlePostRespawnState(spawnPoint);
        
        Debug.Log($"SpawnPointSystem: Respawned at '{spawnPoint.spawnPointId}' after scene load");
    }

    // ==================== Database Management ====================

    /// <summary>
    /// Set the spawn point database
    /// </summary>
    public void SetSpawnPointDatabase(SpawnPointDatabase database)
    {
        spawnPointDatabase = database;
        Debug.Log("SpawnPointSystem: Database reference updated");
    }

    /// <summary>
    /// Add a new spawn point to the database
    /// </summary>
    public void AddSpawnPoint(SpawnPointData spawnPoint)
    {
        if (spawnPointDatabase != null)
        {
            spawnPointDatabase.AddOrUpdateSpawnPoint(spawnPoint);
        }
    }

    /// <summary>
    /// Remove a spawn point from the database
    /// </summary>
    public void RemoveSpawnPoint(string spawnPointId)
    {
        if (spawnPointDatabase != null)
        {
            spawnPointDatabase.RemoveSpawnPoint(spawnPointId);
        }
    }

    /// <summary>
    /// Clear all spawn points from the database
    /// </summary>
    public void ClearAllSpawnPoints()
    {
        if (spawnPointDatabase != null)
        {
            spawnPointDatabase.ClearAllSpawnPoints();
        }
    }

    // ==================== Debug Methods ====================

    /// <summary>
    /// Get debug information about the spawn point system
    /// </summary>
    public string GetDebugInfo()
    {
        if (spawnPointDatabase == null)
        {
            return "SpawnPointSystem: No database assigned";
        }

        return $"SpawnPointSystem: {spawnPointDatabase.GetDebugInfo()}";
    }

    /// <summary>
    /// Validate the spawn point system
    /// </summary>
    [ContextMenu("Validate Spawn Point System")]
    public void ValidateSpawnPointSystem()
    {
        if (spawnPointDatabase != null)
        {
            spawnPointDatabase.ValidateDatabase();
        }
        else
        {
            Debug.LogError("SpawnPointSystem: No database assigned for validation!");
        }
    }
}
