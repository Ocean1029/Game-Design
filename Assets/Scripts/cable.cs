using UnityEngine;

/// <summary>
/// Represents a rappelling cable that allows the player to descend to a lower area
/// Automatically triggers when player enters the zone
/// </summary>
public class cable : MonoBehaviour
{
    [Header("Rappelling Configuration")]
    [Tooltip("Target position where player will land after rappelling")]
    public Transform targetFloorPoint;
    
    [Tooltip("Animator trigger name for cable animation")]
    public string animationTriggerName = "player_enter";

    private Animator cableAnimator;
    private bool hasBeenUsed = false;

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
            Debug.LogError("Cable: targetFloorPoint is not assigned! Player will not be able to rappel.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Prevent multiple uses
        if (hasBeenUsed) return;

        // Check if the player entered the trigger zone
        PlayerController playerController = other.GetComponent<PlayerController>();

        if (playerController == null)
        {
            // Not the player, ignore
            return;
        }

        if (targetFloorPoint == null)
        {
            Debug.LogError("Cable: Cannot start rappelling - targetFloorPoint is missing!");
            return;
        }

        Debug.Log("Player triggered cable rappelling");

        // Play cable animation if available
        if (cableAnimator != null)
        {
            cableAnimator.SetTrigger(animationTriggerName);
        }

        // Start player rappelling sequence
        playerController.StartRappelling(targetFloorPoint);

        // Disable the trigger to prevent repeated activation
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        hasBeenUsed = true;
    }
}
