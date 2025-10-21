using UnityEngine;

/// <summary>
/// Default Spawn Point that automatically registers on scene load
/// Use this for initial player spawn points or permanent respawn locations
/// </summary>
public class DefaultSpawnPoint : MonoBehaviour
{
    [Header("Spawn Point Configuration")]
    [Tooltip("Unique identifier for this spawn point")]
    [SerializeField] private string spawnPointId = "default_spawn";
    
    [Tooltip("Display name shown in UI")]
    [SerializeField] private string displayName = "Starting Point";
    
    [Tooltip("Description of the spawn point location")]
    [SerializeField] private string locationDescription = "Default starting location";

    [Header("Spawn Point Properties")]
    [Tooltip("If true, this spawn point will be set as active when the scene loads")]
    [SerializeField] private bool setAsActiveOnStart = true;
    
    [Tooltip("If true, player can teleport to this spawn point via fast travel menu")]
    [SerializeField] private bool isTransportable = false;
    
    [Tooltip("Whether this spawn point restores player energy")]
    [SerializeField] private bool restoresEnergy = true;
    
    [Tooltip("Amount of energy to restore (0 = restore all)")]
    [SerializeField] private int energyRestoreAmount = 0;

    [Header("Visual & Audio")]
    [Tooltip("Visual indicator shown when this spawn point is active")]
    [SerializeField] private GameObject activeVisual;
    
    [Tooltip("Sound played when player spawns here")]
    [SerializeField] private AudioClip spawnSound;

    [Header("Settings")]
    [Tooltip("Whether to show debug information")]
    [SerializeField] private bool showDebugInfo = false;

    // References
    private SpawnPointSystem spawnPointSystem;
    private GameManager gameManager;
    private bool isInitialized = false;

    void Start()
    {
        // Try to initialize immediately
        if (!TryInitialize())
        {
            // If it fails, start a coroutine to retry
            StartCoroutine(RetryInitialization());
        }
    }

    /// <summary>
    /// Try to initialize and register this default spawn point
    /// Returns true if successful, false otherwise
    /// </summary>
    private bool TryInitialize()
    {
        if (isInitialized) return true;

        // Get references
        gameManager = GameManager.GetInstance();
        if (gameManager == null)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning($"DefaultSpawnPoint: GameManager not found yet for '{spawnPointId}', will retry...");
            }
            return false;
        }

        spawnPointSystem = gameManager.GetSpawnPointSystem();
        if (spawnPointSystem == null)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning($"DefaultSpawnPoint: SpawnPointSystem not found yet for '{spawnPointId}', will retry...");
            }
            return false;
        }

        // Successfully got references, now initialize
        InitializeSpawnPoint();
        return true;
    }

    /// <summary>
    /// Retry initialization if it failed initially
    /// </summary>
    private System.Collections.IEnumerator RetryInitialization()
    {
        int retryCount = 0;
        int maxRetries = 10;

        while (retryCount < maxRetries && !isInitialized)
        {
            yield return new WaitForSeconds(0.1f);
            retryCount++;

            if (TryInitialize())
            {
                if (showDebugInfo)
                {
                    Debug.Log($"DefaultSpawnPoint: Successfully initialized on retry {retryCount}");
                }
                yield break;
            }
        }

        // If we get here, initialization failed after all retries
        Debug.LogError($"DefaultSpawnPoint: Failed to initialize '{spawnPointId}' after {maxRetries} retries. Make sure GameManager exists in the scene!");
    }

    /// <summary>
    /// Initialize and register this default spawn point
    /// </summary>
    private void InitializeSpawnPoint()
    {
        if (isInitialized) return;

        // Generate ID if not set
        if (string.IsNullOrEmpty(spawnPointId))
        {
            spawnPointId = $"default_spawn_{gameObject.name}_{transform.position.x:F0}_{transform.position.y:F0}";
        }

        // Generate display name if not set
        if (string.IsNullOrEmpty(displayName))
        {
            displayName = "Default Spawn Point";
        }

        // Register the spawn point
        RegisterSpawnPoint();

        // Set as active ONLY if specified AND there's no save data
        // This prevents overwriting player's saved spawn point
        if (setAsActiveOnStart && !SaveSystem.HasSaveData())
        {
            ActivateSpawnPoint();
            
            if (showDebugInfo)
            {
                Debug.Log($"DefaultSpawnPoint: No save data found, setting '{spawnPointId}' as active spawn point");
            }
        }
        else if (setAsActiveOnStart && showDebugInfo)
        {
            Debug.Log($"DefaultSpawnPoint: Save data exists, skipping auto-activation of '{spawnPointId}'");
        }

        // Mark as initialized
        isInitialized = true;

        if (showDebugInfo)
        {
            Debug.Log($"DefaultSpawnPoint: Initialized '{spawnPointId}' - {displayName}");
        }
    }

    /// <summary>
    /// Register this spawn point with the spawn point system
    /// </summary>
    private void RegisterSpawnPoint()
    {
        // Create spawn point data
        SpawnPointData spawnPointData = new SpawnPointData(
            spawnPointId,
            displayName,
            locationDescription,
            transform.position,
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        )
        {
            isOneTimeUse = false,
            isTransportable = this.isTransportable,
            restoresEnergy = this.restoresEnergy,
            energyRestoreAmount = this.energyRestoreAmount,
            activationSound = this.spawnSound,
            activeVisual = this.activeVisual
        };

        // Add to spawn point system
        spawnPointSystem.AddSpawnPoint(spawnPointData);

        if (showDebugInfo)
        {
            Debug.Log($"DefaultSpawnPoint: Registered '{spawnPointId}' with SpawnPointSystem");
        }
    }

    /// <summary>
    /// Activate this spawn point as the current active spawn point
    /// </summary>
    private void ActivateSpawnPoint()
    {
        if (gameManager == null)
        {
            Debug.LogWarning($"DefaultSpawnPoint: Cannot activate '{spawnPointId}' - GameManager not found");
            return;
        }

        gameManager.ActivateSpawnPoint(spawnPointId);
        
        // Update visual status
        UpdateVisualStatus();

        if (showDebugInfo)
        {
            Debug.Log($"DefaultSpawnPoint: Activated '{spawnPointId}' as current spawn point");
        }
    }

    /// <summary>
    /// Update the visual status of the spawn point
    /// </summary>
    private void UpdateVisualStatus()
    {
        if (activeVisual == null || spawnPointSystem == null) return;

        SpawnPointData currentActive = spawnPointSystem.GetCurrentActiveSpawnPoint();
        bool isActive = (currentActive != null && currentActive.spawnPointId == spawnPointId);
        activeVisual.SetActive(isActive);
    }

    /// <summary>
    /// Check if this spawn point is currently active
    /// </summary>
    public bool IsActive()
    {
        if (spawnPointSystem == null) return false;
        
        SpawnPointData currentActive = spawnPointSystem.GetCurrentActiveSpawnPoint();
        return currentActive != null && currentActive.spawnPointId == spawnPointId;
    }

    /// <summary>
    /// Get debug information about this spawn point
    /// </summary>
    public string GetDebugInfo()
    {
        return $"DefaultSpawnPoint[{spawnPointId}]: {displayName} at {transform.position} (Active on start: {setAsActiveOnStart}, Transportable: {isTransportable})";
    }

    /// <summary>
    /// Draw debug information in the scene view
    /// </summary>
    void OnDrawGizmos()
    {
        // Draw spawn point indicator
        Gizmos.color = setAsActiveOnStart ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        // Draw icon
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 1f);
    }

    void OnDrawGizmosSelected()
    {
        // Draw detailed info when selected
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1f);
        
        // Draw spawn direction indicator
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.right * 0.5f);
    }
}

