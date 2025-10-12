using UnityEngine;

/// <summary>
/// Represents a rappelling cable that allows an interactor to descend to a lower area
/// Requires interaction button press to start rappelling
/// Implements IInteractable to work with the interaction system
/// </summary>
public class cable : MonoBehaviour, IInteractable
{
    [Header("Rappelling Configuration")]
    [Tooltip("Target position where interactor will land after rappelling")]
    public Transform targetFloorPoint;
    
    [Tooltip("Animator trigger name for cable animation")]
    public string animationTriggerName = "player_enter";

    [Header("UI Prompts (Optional)")]
    [Tooltip("UI element shown when interactor can use the cable")]
    public GameObject usePrompt;

    private Animator cableAnimator;
    private bool hasBeenUsed = false;
    private IInteractor currentInteractor = null;

    void Start()
    {
        // Get animator component if exists
        cableAnimator = GetComponent<Animator>();
        
        if (cableAnimator == null)
        {
            Debug.LogWarning("Cable object does not have an Animator component. Rappelling will work but without cable animation.");
        }

        if (targetFloorPoint == null)
        {
            Debug.LogError("Cable: targetFloorPoint is not assigned! Interactor will not be able to rappel.");
        }

        // Hide prompt at start
        if (usePrompt != null)
        {
            usePrompt.SetActive(false);
        }
    }

    // ==================== IInteractable Implementation ====================

    /// <summary>
    /// Called when an interactor enters the cable's interaction zone
    /// </summary>
    public void OnInteractorEnterZone(IInteractor interactor)
    {
        // Don't allow interaction if already used
        if (hasBeenUsed)
        {
            return;
        }

        Debug.Log("Interactor entered cable zone");
        currentInteractor = interactor;

        // Show interaction prompt
        if (usePrompt != null)
        {
            usePrompt.SetActive(true);
            Debug.Log("Showing rappel prompt");
        }
    }

    /// <summary>
    /// Called when an interactor exits the cable's interaction zone
    /// </summary>
    public void OnInteractorExitZone(IInteractor interactor)
    {
        // Hide prompt when interactor leaves
        if (usePrompt != null)
        {
            usePrompt.SetActive(false);
        }

        currentInteractor = null;
    }

    /// <summary>
    /// Called when an interactor presses the interact button while near the cable
    /// </summary>
    public bool Interact(IInteractor interactor)
    {
        // Prevent multiple uses
        if (hasBeenUsed)
        {
            Debug.Log("Cable has already been used");
            return false;
        }

        if (targetFloorPoint == null)
        {
            Debug.LogError("Cable: Cannot start rappelling - targetFloorPoint is missing!");
            return false;
        }

        Debug.Log("Cable Interact() called - starting rappelling");

        // Play cable animation if available
        if (cableAnimator != null)
        {
            cableAnimator.SetTrigger(animationTriggerName);
        }

        // Start rappelling sequence (player-specific for now)
        // Note: For full decoupling, consider using an IRappelable interface
        if (interactor is PlayerController player)
        {
            player.StartRappelling(targetFloorPoint);
            
            // Hide prompt
            if (usePrompt != null)
            {
                usePrompt.SetActive(false);
            }

            // Disable the trigger to prevent repeated activation
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = false;
            }

            hasBeenUsed = true;
            return true;
        }

        Debug.LogWarning("Cable: Interactor is not a PlayerController, cannot rappel");
        return false;
    }

    /// <summary>
    /// Get the GameObject this interactable belongs to
    /// </summary>
    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
