using UnityEngine;

/// <summary>
/// Represents a door that can be opened with a key
/// Opens automatically when player with the correct key enters the trigger zone
/// </summary>
public class door : MonoBehaviour, IInteractable
{
    [Header("Door Configuration")]
    [Tooltip("Sprite shown when door is opened")]
    public Sprite doorOpenedSprite;
    
    [Tooltip("Tag of the key required to open this door")]
    public string requiredKeyTag = "key1";

    [Header("UI Prompts (Optional)")]
    [Tooltip("UI element shown when player is near but doesn't have key")]
    public GameObject needKeyPrompt;
    
    [Tooltip("UI element shown when player has the correct key")]
    public GameObject canOpenPrompt;

    private bool isOpened = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer == null)
        {
            Debug.LogError("Door is missing SpriteRenderer component!");
        }

        // Hide prompts at start
        if (needKeyPrompt != null) needKeyPrompt.SetActive(false);
        if (canOpenPrompt != null) canOpenPrompt.SetActive(false);
    }

    // ==================== IInteractable Implementation ====================

    /// <summary>
    /// Called when player enters the door's trigger zone
    /// </summary>
    public void OnPlayerEnterZone(PlayerController player)
    {
        if (isOpened) return;

        // Check if player has the required key
        GameObject carriedItem = player.GetCarriedItem();
        
        if (carriedItem != null && carriedItem.CompareTag(requiredKeyTag))
        {
            // Player has the key - open door automatically
            OpenDoor(player);
        }
        else
        {
            // Player doesn't have the key - show prompt
            if (needKeyPrompt != null)
            {
                needKeyPrompt.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Called when player exits the door's trigger zone
    /// </summary>
    public void OnPlayerExitZone(PlayerController player)
    {
        // Hide prompts when player leaves
        if (needKeyPrompt != null) needKeyPrompt.SetActive(false);
        if (canOpenPrompt != null) canOpenPrompt.SetActive(false);
    }

    /// <summary>
    /// Called when player presses interact button (not used for automatic doors)
    /// </summary>
    public bool Interact(PlayerController player)
    {
        // For doors, we use automatic opening in OnPlayerEnterZone
        // But this method can be used for manual doors in the future
        return false;
    }

    /// <summary>
    /// Get the GameObject this interactable belongs to
    /// </summary>
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    // ==================== Door Logic ====================

    /// <summary>
    /// Open the door and consume the key
    /// </summary>
    private void OpenDoor(PlayerController player)
    {
        if (isOpened) return;

        Debug.Log($"Door opened with {requiredKeyTag}!");

        // Change sprite to opened state
        if (spriteRenderer != null && doorOpenedSprite != null)
        {
            spriteRenderer.sprite = doorOpenedSprite;
        }
        else
        {
            Debug.LogError("Door is missing SpriteRenderer or doorOpenedSprite is not assigned!");
        }

        // Consume the key
        player.UseCarriedItem();

        // Disable collider so player can pass through
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Hide prompts
        if (needKeyPrompt != null) needKeyPrompt.SetActive(false);
        if (canOpenPrompt != null) canOpenPrompt.SetActive(false);

        isOpened = true;
    }
}
