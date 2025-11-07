using UnityEngine;
using System.Collections;

/// <summary>
/// Save Point component for spawn points in the game
/// Handles interaction with save points and activation
/// </summary>
public class SavePoint : MonoBehaviour, IInteractable
{
    [Header("Save Point Configuration")]
    [Tooltip("Unique identifier for this save point")]
    [SerializeField] private string savePointId = "";
    
    [Tooltip("Display name shown in UI")]
    [SerializeField] private string displayName = "";
    
    [Tooltip("Description of the save point location")]
    [SerializeField] private string locationDescription = "";

    [Header("Save Point Properties")]
    [Tooltip("If true, this save point can only be used once")]
    [SerializeField] private bool isOneTimeUse = false;
    
    [Tooltip("If true, player can teleport to this save point")]
    [SerializeField] private bool isTransportable = true;
    
    [Tooltip("Whether this save point restores player energy")]
    [SerializeField] private bool restoresEnergy = true;
    
    [Tooltip("Amount of energy to restore (0 = restore all)")]
    [SerializeField] private int energyRestoreAmount = 0;

    [Header("UI Prompts")]
    [Tooltip("UI element shown when player can interact")]
    [SerializeField] private GameObject interactionPrompt;
    
    [Tooltip("UI element shown when save point is active")]
    [SerializeField] private GameObject activeVisual;

    [Header("Audio")]
    [Tooltip("Sound played when activating save point")]
    [SerializeField] private AudioClip activationSound;

    [Header("Settings")]
    [Tooltip("Whether to show debug information")]
    [SerializeField] private bool showDebugInfo = false;

    // State
    private bool isActive = false;
    private IInteractor currentInteractor = null;

    // References
    private SpawnPointSystem spawnPointSystem;
    private PlayerController playerController;

    void Start()
    {
        // Get references
        spawnPointSystem = FindFirstObjectByType<SpawnPointSystem>();
        playerController = FindFirstObjectByType<PlayerController>();

        // Generate ID if not set
        if (string.IsNullOrEmpty(savePointId))
        {
            savePointId = $"savepoint_{gameObject.name}_{transform.position.x:F0}_{transform.position.y:F0}";
        }

        // Generate display name if not set
        if (string.IsNullOrEmpty(displayName))
        {
            displayName = gameObject.name;
        }

        // Hide prompts initially
        ShowInteractionPrompt(false);
        UpdateActiveVisual();

        // Register this save point with the spawn point system
        RegisterSavePoint();

        if (showDebugInfo)
        {
            Debug.Log($"SavePoint: Initialized '{savePointId}' - {displayName}");
        }
    }

    // ==================== IInteractable Implementation ====================

    /// <summary>
    /// Called when an interactor enters the save point's interaction zone
    /// </summary>
    public void OnInteractorEnterZone(IInteractor interactor)
    {
        if (showDebugInfo)
        {
            Debug.Log($"SavePoint: Interactor entered zone for '{savePointId}'");
        }

        currentInteractor = interactor;

        // Show interaction prompt if player is not already interacting
        PlayerController player = interactor.GetGameObject().GetComponent<PlayerController>();
        if (player != null && !player.IsSitting())
        {
            ShowInteractionPrompt(true);
        }
    }

    /// <summary>
    /// Called when an interactor exits the save point's interaction zone
    /// </summary>
    public void OnInteractorExitZone(IInteractor interactor)
    {
        if (showDebugInfo)
        {
            Debug.Log($"SavePoint: Interactor exited zone for '{savePointId}'");
        }

        // Only hide prompts if player is not sitting
        PlayerController player = interactor.GetGameObject().GetComponent<PlayerController>();
        if (player == null || !player.IsSitting())
        {
            ShowInteractionPrompt(false);
            currentInteractor = null;
        }
    }

    /// <summary>
    /// Called when an interactor presses the interact button
    /// </summary>
    public bool Interact(IInteractor interactor)
    {
        if (showDebugInfo)
        {
            Debug.Log($"SavePoint: Interact called for '{savePointId}'");
        }

        PlayerController player = interactor.GetGameObject().GetComponent<PlayerController>();
        if (player == null)
        {
            Debug.LogWarning($"SavePoint: Only players can interact with save point '{savePointId}'");
            return false;
        }

        // Activate the save point
        ActivateSavePoint();
        return true;
    }

    /// <summary>
    /// Get the GameObject this interactable belongs to
    /// </summary>
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    // ==================== Save Point Management ====================

    /// <summary>
    /// Activate this save point
    /// </summary>
    public void ActivateSavePoint()
    {
        if (spawnPointSystem == null)
        {
            Debug.LogError($"SavePoint: SpawnPointSystem not found! Cannot activate '{savePointId}'");
            return;
        }

        // Create spawn point data
        SpawnPointData spawnPointData = new SpawnPointData(
            savePointId,
            displayName,
            locationDescription,
            transform.position,
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        )
        {
            isOneTimeUse = this.isOneTimeUse,
            isTransportable = this.isTransportable,
            restoresEnergy = this.restoresEnergy,
            energyRestoreAmount = this.energyRestoreAmount,
            activationSound = this.activationSound,
            activeVisual = this.activeVisual
        };

        // Activate through spawn point system
        spawnPointSystem.ActivateSpawnPoint(savePointId);

        // Update visual state
        isActive = true;
        UpdateActiveVisual();

        // Hide interaction prompt
        ShowInteractionPrompt(false);

        // Play activation sound
        if (activationSound != null)
        {
            AudioSource.PlayClipAtPoint(activationSound, transform.position);
        }

        Debug.Log($"SavePoint: Activated '{savePointId}' - {displayName}");
    }

    /// <summary>
    /// Deactivate this save point
    /// </summary>
    public void DeactivateSavePoint()
    {
        isActive = false;
        UpdateActiveVisual();
        
        Debug.Log($"SavePoint: Deactivated '{savePointId}'");
    }

    /// <summary>
    /// Register this save point with the spawn point system
    /// </summary>
    private void RegisterSavePoint()
    {
        if (spawnPointSystem == null)
        {
            Debug.LogWarning($"SavePoint: Cannot register '{savePointId}' - SpawnPointSystem not found");
            return;
        }

        // Create spawn point data for registration
        SpawnPointData spawnPointData = new SpawnPointData(
            savePointId,
            displayName,
            locationDescription,
            transform.position,
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        )
        {
            isOneTimeUse = this.isOneTimeUse,
            isTransportable = this.isTransportable,
            restoresEnergy = this.restoresEnergy,
            energyRestoreAmount = this.energyRestoreAmount,
            activationSound = this.activationSound,
            activeVisual = this.activeVisual
        };

        // Add to spawn point system
        spawnPointSystem.AddSpawnPoint(spawnPointData);

        if (showDebugInfo)
        {
            Debug.Log($"SavePoint: Registered '{savePointId}' with SpawnPointSystem");
        }
    }

    // ==================== UI Management ====================

    /// <summary>
    /// Show or hide the interaction prompt
    /// </summary>
    private void ShowInteractionPrompt(bool show)
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(show);
        }
    }

    /// <summary>
    /// Update the active visual indicator
    /// </summary>
    private void UpdateActiveVisual()
    {
        if (activeVisual != null)
        {
            activeVisual.SetActive(isActive);
        }
    }

    // ==================== Public Methods ====================

    /// <summary>
    /// Get the save point ID
    /// </summary>
    public string GetSavePointId()
    {
        return savePointId;
    }

    /// <summary>
    /// Get the display name
    /// </summary>
    public string GetDisplayName()
    {
        return displayName;
    }

    /// <summary>
    /// Get the location description
    /// </summary>
    public string GetLocationDescription()
    {
        return locationDescription;
    }

    /// <summary>
    /// Check if this save point is active
    /// </summary>
    public bool IsActive()
    {
        return isActive;
    }

    /// <summary>
    /// Check if this save point is one-time use
    /// </summary>
    public bool IsOneTimeUse()
    {
        return isOneTimeUse;
    }

    /// <summary>
    /// Check if this save point is transportable
    /// </summary>
    public bool IsTransportable()
    {
        return isTransportable;
    }

    /// <summary>
    /// Set the save point properties
    /// </summary>
    public void SetSavePointProperties(bool oneTimeUse, bool teleportable, bool restoresEnergy, int energyAmount)
    {
        this.isOneTimeUse = oneTimeUse;
        this.isTransportable = teleportable;
        this.restoresEnergy = restoresEnergy;
        this.energyRestoreAmount = energyAmount;
    }

    // ==================== Debug Methods ====================

    /// <summary>
    /// Get debug information about this save point
    /// </summary>
    public string GetDebugInfo()
    {
        return $"SavePoint[{savePointId}]: {displayName} at {transform.position} (Active: {isActive}, OneTime: {isOneTimeUse}, Transportable: {isTransportable})";
    }

    /// <summary>
    /// Draw debug information in the scene view
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (showDebugInfo)
        {
            // Draw save point info
            Gizmos.color = isActive ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.5f);

            // Draw connection to spawn point system
            if (spawnPointSystem != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, spawnPointSystem.transform.position);
            }
        }
    }
}
