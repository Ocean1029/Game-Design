using UnityEngine;

/// <summary>
/// Transition Point component for scene transitions
/// Handles interaction with transition points and scene switching
/// </summary>
public class TransitionPoint : MonoBehaviour, IInteractable
{
    [Header("Transition Point Configuration")]
    [Tooltip("Unique identifier for this transition point")]
    [SerializeField] private string transitionPointId = "";
    
    [Tooltip("Display name shown in debug info")]
    [SerializeField] private string displayName = "";
    
    [Tooltip("Description of the transition point")]
    [SerializeField] private string description = "";

    [Header("Connection Information")]
    [Tooltip("Target scene name for this transition")]
    [SerializeField] private string targetSceneName = "";
    
    [Tooltip("Target transition point ID in the target scene")]
    [SerializeField] private string targetTransitionPointId = "";

    [Header("Transition Settings")]
    [Tooltip("Type of transition")]
    [SerializeField] private TransitionType transitionType = TransitionType.Bidirectional;
    
    [Tooltip("Whether this transition requires a key")]
    [SerializeField] private bool requiresKey = false;
    
    [Tooltip("Key ID required for this transition")]
    [SerializeField] private string requiredKeyId = "";
    
    [Tooltip("Whether to set player direction after transition")]
    [SerializeField] private bool setPlayerDirection = false;
    
    [Tooltip("Direction the player should face after transition")]
    [SerializeField] private Vector2 playerDirection = Vector2.right;

    [Header("Visual & Audio")]
    [Tooltip("Visual effect during transition")]
    [SerializeField] private GameObject transitionEffect;
    
    [Tooltip("Sound played during transition")]
    [SerializeField] private AudioClip transitionSound;
    
    [Tooltip("Transition duration in seconds")]
    [SerializeField] private float transitionDuration = 1.0f;

    [Header("UI Prompts")]
    [Tooltip("UI element shown when player can interact")]
    [SerializeField] private GameObject interactionPrompt;
    
    [Tooltip("UI element shown when key is required")]
    [SerializeField] private GameObject keyRequiredPrompt;

    [Header("Settings")]
    [Tooltip("Whether to show debug information")]
    [SerializeField] private bool showDebugInfo = false;

    // State
    private IInteractor currentInteractor = null;

    // References
    private TransitionPointSystem transitionPointSystem;
    private PlayerController playerController;

    void Start()
    {
        // Get references
        transitionPointSystem = FindFirstObjectByType<TransitionPointSystem>();
        playerController = FindFirstObjectByType<PlayerController>();

        // Generate ID if not set
        if (string.IsNullOrEmpty(transitionPointId))
        {
            transitionPointId = $"transition_{gameObject.name}_{transform.position.x:F0}_{transform.position.y:F0}";
        }

        // Generate display name if not set
        if (string.IsNullOrEmpty(displayName))
        {
            displayName = gameObject.name;
        }

        // Hide prompts initially
        ShowInteractionPrompt(false);
        ShowKeyRequiredPrompt(false);

        // Register this transition point with the transition point system
        RegisterTransitionPoint();

        if (showDebugInfo)
        {
            Debug.Log($"TransitionPoint: Initialized '{transitionPointId}' - {displayName}");
        }
    }

    // ==================== IInteractable Implementation ====================

    /// <summary>
    /// Called when an interactor enters the transition point's interaction zone
    /// </summary>
    public void OnInteractorEnterZone(IInteractor interactor)
    {
        if (showDebugInfo)
        {
            Debug.Log($"TransitionPoint: Interactor entered zone for '{transitionPointId}'");
        }

        currentInteractor = interactor;

        // Check if key is required
        if (requiresKey && !HasRequiredKey())
        {
            ShowKeyRequiredPrompt(true);
        }
        else
        {
            ShowInteractionPrompt(true);
        }
    }

    /// <summary>
    /// Called when an interactor exits the transition point's interaction zone
    /// </summary>
    public void OnInteractorExitZone(IInteractor interactor)
    {
        if (showDebugInfo)
        {
            Debug.Log($"TransitionPoint: Interactor exited zone for '{transitionPointId}'");
        }

        ShowInteractionPrompt(false);
        ShowKeyRequiredPrompt(false);
        currentInteractor = null;
    }

    /// <summary>
    /// Called when an interactor presses the interact button
    /// </summary>
    public bool Interact(IInteractor interactor)
    {
        if (showDebugInfo)
        {
            Debug.Log($"TransitionPoint: Interact called for '{transitionPointId}'");
        }

        PlayerController player = interactor.GetGameObject().GetComponent<PlayerController>();
        if (player == null)
        {
            Debug.LogWarning($"TransitionPoint: Only players can interact with transition point '{transitionPointId}'");
            return false;
        }

        // Check if key is required
        if (requiresKey && !HasRequiredKey())
        {
            Debug.LogWarning($"TransitionPoint: Required key '{requiredKeyId}' not found for transition point '{transitionPointId}'");
            return false;
        }

        // Attempt to transition
        return AttemptTransition();
    }

    /// <summary>
    /// Get the GameObject this interactable belongs to
    /// </summary>
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    // ==================== Transition Management ====================

    /// <summary>
    /// Attempt to transition through this point
    /// </summary>
    private bool AttemptTransition()
    {
        if (transitionPointSystem == null)
        {
            Debug.LogError($"TransitionPoint: TransitionPointSystem not found! Cannot transition through '{transitionPointId}'");
            return false;
        }

        // Check if transition is valid
        if (!transitionPointSystem.IsTransitionValid(transitionPointId))
        {
            Debug.LogWarning($"TransitionPoint: Invalid transition configuration for '{transitionPointId}'");
            return false;
        }

        // Start transition
        bool success = transitionPointSystem.TransitionThroughPoint(transitionPointId);
        
        if (success)
        {
            // Hide prompts during transition
            ShowInteractionPrompt(false);
            ShowKeyRequiredPrompt(false);
            
            // Play transition effect
            if (transitionEffect != null)
            {
                GameObject effect = Instantiate(transitionEffect, transform.position, Quaternion.identity);
                Destroy(effect, transitionDuration);
            }

            Debug.Log($"TransitionPoint: Started transition through '{transitionPointId}' to '{targetSceneName}'");
        }

        return success;
    }

    /// <summary>
    /// Check if the player has the required key
    /// </summary>
    private bool HasRequiredKey()
    {
        if (!requiresKey || string.IsNullOrEmpty(requiredKeyId))
        {
            return true;
        }

        // TODO: Implement key checking logic
        // This would check the player's inventory for the required key
        // For now, return true as placeholder
        return true;
    }

    /// <summary>
    /// Register this transition point with the transition point system
    /// </summary>
    private void RegisterTransitionPoint()
    {
        if (transitionPointSystem == null)
        {
            Debug.LogWarning($"TransitionPoint: Cannot register '{transitionPointId}' - TransitionPointSystem not found");
            return;
        }

        // Create transition point data for registration
        TransitionPointData transitionPointData = new TransitionPointData(
            transitionPointId,
            displayName,
            transform.position,
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            targetSceneName,
            targetTransitionPointId
        )
        {
            description = this.description,
            transitionType = this.transitionType,
            requiresKey = this.requiresKey,
            requiredKeyId = this.requiredKeyId,
            setPlayerDirection = this.setPlayerDirection,
            playerDirection = this.playerDirection,
            transitionEffect = this.transitionEffect,
            transitionSound = this.transitionSound,
            transitionDuration = this.transitionDuration
        };

        // Add to transition point system
        transitionPointSystem.AddTransitionPoint(transitionPointData);

        if (showDebugInfo)
        {
            Debug.Log($"TransitionPoint: Registered '{transitionPointId}' with TransitionPointSystem");
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
    /// Show or hide the key required prompt
    /// </summary>
    private void ShowKeyRequiredPrompt(bool show)
    {
        if (keyRequiredPrompt != null)
        {
            keyRequiredPrompt.SetActive(show);
        }
    }

    // ==================== Public Methods ====================

    /// <summary>
    /// Get the transition point ID
    /// </summary>
    public string GetTransitionPointId()
    {
        return transitionPointId;
    }

    /// <summary>
    /// Get the display name
    /// </summary>
    public string GetDisplayName()
    {
        return displayName;
    }

    /// <summary>
    /// Get the target scene name
    /// </summary>
    public string GetTargetSceneName()
    {
        return targetSceneName;
    }

    /// <summary>
    /// Get the target transition point ID
    /// </summary>
    public string GetTargetTransitionPointId()
    {
        return targetTransitionPointId;
    }

    /// <summary>
    /// Check if this transition point requires a key
    /// </summary>
    public bool RequiresKey()
    {
        return requiresKey;
    }

    /// <summary>
    /// Get the required key ID
    /// </summary>
    public string GetRequiredKeyId()
    {
        return requiredKeyId;
    }

    /// <summary>
    /// Set the transition point properties
    /// </summary>
    public void SetTransitionPointProperties(string targetScene, string targetId, TransitionType type, bool needsKey, string keyId)
    {
        targetSceneName = targetScene;
        targetTransitionPointId = targetId;
        transitionType = type;
        requiresKey = needsKey;
        requiredKeyId = keyId;
    }

    // ==================== Debug Methods ====================

    /// <summary>
    /// Get debug information about this transition point
    /// </summary>
    public string GetDebugInfo()
    {
        return $"TransitionPoint[{transitionPointId}]: {displayName} -> {targetSceneName}[{targetTransitionPointId}] (Type: {transitionType}, KeyRequired: {requiresKey})";
    }

    /// <summary>
    /// Draw debug information in the scene view
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (showDebugInfo)
        {
            // Draw transition point info
            Gizmos.color = requiresKey ? Color.red : Color.cyan;
            Gizmos.DrawWireCube(transform.position, Vector3.one);

            // Draw direction arrow
            Gizmos.color = Color.white;
            Vector3 direction = new Vector3(playerDirection.x, playerDirection.y, 0) * 2f;
            Gizmos.DrawRay(transform.position, direction);

            // Draw connection to transition point system
            if (transitionPointSystem != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, transitionPointSystem.transform.position);
            }
        }
    }
}
