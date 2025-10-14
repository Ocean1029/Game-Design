using UnityEngine;

/// <summary>
/// Represents a chair that any interactor can sit on
/// Implements IInteractable to work with the interaction system
/// </summary>
public class chair : MonoBehaviour, IInteractable
{
    [Header("Sitting Configuration")]
    [Tooltip("Position where the interactor will be placed when sitting")]
    public Transform sitpoint;

    [Header("UI Prompts")]
    [Tooltip("UI element shown when interactor can sit down")]
    public GameObject pressAPrompt;
    
    [Tooltip("UI element shown when interactor is sitting and can stand up")]
    public GameObject pressDPrompt;

    [Header("Spawn Point Configuration")]
    [Tooltip("Unique identifier for this chair as a spawn point")]
    [SerializeField] private string chairId = "";
    
    [Tooltip("Display name shown in fast travel menu")]
    [SerializeField] private string chairName = "";
    
    [Tooltip("Brief description of the chair's location")]
    [SerializeField] private string locationDescription = "";
    
    [Tooltip("Visual indicator shown when this is the current spawn point")]
    [SerializeField] private GameObject activeSpawnVisual;
    
    [Tooltip("Sound played when player sits on chair (saves progress)")]
    [SerializeField] private AudioClip sitSound;

    private IInteractor currentInteractor = null;

    void Start()
    {
        // Hide both prompts at start
        ShowPromptA(false);
        ShowPromptD(false);
        
        // Generate chair ID if not set
        if (string.IsNullOrEmpty(chairId))
        {
            chairId = $"chair_{gameObject.name}_{transform.position.x:F0}_{transform.position.y:F0}";
        }
        
        // Generate chair name if not set
        if (string.IsNullOrEmpty(chairName))
        {
            chairName = gameObject.name;
        }
        
        // Update spawn point status
        UpdateSpawnPointStatus();
    }

    // ==================== IInteractable Implementation ====================

    /// <summary>
    /// Called when an interactor enters the chair's interaction zone
    /// </summary>
    public void OnInteractorEnterZone(IInteractor interactor)
    {
        Debug.Log("Interactor entered chair zone");
        currentInteractor = interactor;
        
        // Check if this is a player and if they're already sitting
        PlayerController player = interactor.GetGameObject().GetComponent<PlayerController>();
        if (player != null && !player.IsSitting())
        {
            ShowPromptA(true);
            Debug.Log("Showing sit prompt");
        }
    }

    /// <summary>
    /// Called when an interactor exits the chair's interaction zone
    /// </summary>
    public void OnInteractorExitZone(IInteractor interactor)
    {
        // Check if this is a player and if they're sitting
        PlayerController player = interactor.GetGameObject().GetComponent<PlayerController>();
        
        // Only clear prompts if player is not sitting
        // (when sitting, player should not leave the zone)
        if (player == null || !player.IsSitting())
        {
            ShowPromptA(false);
            ShowPromptD(false);
            currentInteractor = null;
        }
    }

    /// <summary>
    /// Called when an interactor presses the interact button while near the chair
    /// </summary>
    public bool Interact(IInteractor interactor)
    {
        Debug.Log("Chair Interact() called");
        
        // Try to get PlayerController component from the interactor
        PlayerController player = interactor.GetGameObject().GetComponent<PlayerController>();
        
        if (player == null)
        {
            Debug.LogWarning("Chair: Only players can sit on chairs");
            return false;
        }
        
        if (player.IsSitting())
        {
            // Player is already sitting, can't sit again
            Debug.Log("Player is already sitting");
            return false;
        }

        // Make the player sit down
        Debug.Log("Making player sit on chair");
        player.SitOnChair(this);
        
        // Save progress by setting this chair as spawn point
        SaveProgress();
        
        // Update UI prompts
        ShowPromptA(false);
        ShowPromptD(true);
        
        return true;
    }

    /// <summary>
    /// Get the GameObject this interactable belongs to
    /// </summary>
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    // ==================== Public Methods ====================

    /// <summary>
    /// Show or hide the "Press U to sit" prompt
    /// </summary>
    public void ShowPromptA(bool show)
    {
        if (pressAPrompt != null)
        {
            pressAPrompt.SetActive(show);
        }
    }

    /// <summary>
    /// Show or hide the "Press D to stand" prompt
    /// </summary>
    public void ShowPromptD(bool show)
    {
        if (pressDPrompt != null)
        {
            pressDPrompt.SetActive(show);
        }
    }

    // ==================== Spawn Point Methods ====================

    /// <summary>
    /// Save progress by registering this chair as a spawn point
    /// </summary>
    private void SaveProgress()
    {
        GameManager gameManager = GameManager.GetInstance();
        if (gameManager == null)
        {
            Debug.LogWarning("Chair: GameManager not found! Cannot save progress.");
            return;
        }

        // Register this chair as a spawn point
        Vector3 spawnPosition = sitpoint != null ? sitpoint.position : transform.position;
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        gameManager.RegisterSpawnPoint(chairId, chairName, locationDescription, spawnPosition, currentScene);
        gameManager.SetCurrentSpawnPoint(chairId, spawnPosition, currentScene);
        
        // Play sit sound
        if (sitSound != null)
        {
            AudioSource.PlayClipAtPoint(sitSound, transform.position);
        }
        
        // Update visual indicator
        UpdateSpawnPointStatus();
        
        Debug.Log($"Chair: Progress saved at '{chairName}' ({chairId})");
    }

    /// <summary>
    /// Update the visual indicator for spawn point status
    /// </summary>
    private void UpdateSpawnPointStatus()
    {
        if (activeSpawnVisual == null) return;

        GameManager gameManager = GameManager.GetInstance();
        if (gameManager == null) return;

        GameManager.SpawnPointData currentSpawn = gameManager.GetCurrentSpawnPoint();
        bool isCurrentSpawn = currentSpawn != null && currentSpawn.spawnPointId == chairId;
        
        activeSpawnVisual.SetActive(isCurrentSpawn);
        
        if (isCurrentSpawn)
        {
            Debug.Log($"Chair: '{chairName}' is now the current spawn point");
        }
    }

    /// <summary>
    /// Get the chair's unique ID
    /// </summary>
    public string GetChairId()
    {
        return chairId;
    }

    /// <summary>
    /// Get the chair's display name
    /// </summary>
    public string GetChairName()
    {
        return chairName;
    }

    /// <summary>
    /// Get the chair's location description
    /// </summary>
    public string GetLocationDescription()
    {
        return locationDescription;
    }

    /// <summary>
    /// Check if this chair is the current spawn point
    /// </summary>
    public bool IsCurrentSpawnPoint()
    {
        GameManager gameManager = GameManager.GetInstance();
        if (gameManager == null) return false;

        GameManager.SpawnPointData currentSpawn = gameManager.GetCurrentSpawnPoint();
        return currentSpawn != null && currentSpawn.spawnPointId == chairId;
    }
}
