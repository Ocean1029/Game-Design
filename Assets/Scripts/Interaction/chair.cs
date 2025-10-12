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

    private IInteractor currentInteractor = null;

    void Start()
    {
        // Hide both prompts at start
        ShowPromptA(false);
        ShowPromptD(false);
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
}
