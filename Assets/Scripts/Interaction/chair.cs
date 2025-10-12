using UnityEngine;

/// <summary>
/// Represents a chair that the player can sit on
/// Implements IInteractable to work with the new player interaction system
/// </summary>
public class chair : MonoBehaviour, IInteractable
{
    [Header("Sitting Configuration")]
    [Tooltip("Position where the player will be placed when sitting")]
    public Transform sitpoint;

    [Header("UI Prompts")]
    [Tooltip("UI element shown when player can sit down")]
    public GameObject pressAPrompt;
    
    [Tooltip("UI element shown when player is sitting and can stand up")]
    public GameObject pressDPrompt;

    private PlayerController currentPlayer = null;

    void Start()
    {
        // Hide both prompts at start
        ShowPromptA(false);
        ShowPromptD(false);
    }

    // ==================== IInteractable Implementation ====================

    /// <summary>
    /// Called when player enters the chair's interaction zone
    /// </summary>
    public void OnPlayerEnterZone(PlayerController player)
    {
        Debug.Log("Player entered chair zone");
        currentPlayer = player;
        
        // Show sit prompt only if not already sitting
        if (!player.IsSitting())
        {
            ShowPromptA(true);
            Debug.Log("Showing sit prompt");
        }
    }

    /// <summary>
    /// Called when player exits the chair's interaction zone
    /// </summary>
    public void OnPlayerExitZone(PlayerController player)
    {
        // Only clear prompts if player is not sitting
        // (when sitting, player should not leave the zone)
        if (!player.IsSitting())
        {
            ShowPromptA(false);
            ShowPromptD(false);
            currentPlayer = null;
        }
    }

    /// <summary>
    /// Called when player presses the interact button while near the chair
    /// </summary>
    public bool Interact(PlayerController player)
    {
        Debug.Log("Chair Interact() called");
        
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
