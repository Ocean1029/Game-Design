using UnityEngine;

/// <summary>
/// Represents a door that can be opened with a key
/// Opens automatically when an interactor with the correct key enters the trigger zone
/// </summary>
public class door : MonoBehaviour, IInteractable
{
    [Header("Door Configuration")]
    [Tooltip("Sprite shown when door is opened")]
    public Sprite doorOpenedSprite;
    
    [Tooltip("Tag of the key required to open this door")]
    public string requiredKeyTag = "key1";

    [Header("UI Prompts (Optional)")]
    [Tooltip("UI element shown when interactor is near but doesn't have key")]
    public GameObject needKeyPrompt;
    
    [Tooltip("UI element shown when interactor has the correct key")]
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
    /// Called when an interactor enters the door's trigger zone
    /// </summary>
    public void OnInteractorEnterZone(IInteractor interactor)
    {
        if (isOpened) return;

        // Try to get InteractionHandler component to check for carried items
        InteractionHandler handler = interactor.GetGameObject().GetComponent<InteractionHandler>();
        
        if (handler != null)
        {
            GameObject carriedItem = handler.GetCarriedItem();
            
            if (carriedItem != null && carriedItem.CompareTag(requiredKeyTag))
            {
                // Interactor has the key - open door automatically
                OpenDoor(handler);
            }
            else
            {
                // Interactor doesn't have the key - show prompt
                if (needKeyPrompt != null)
                {
                    needKeyPrompt.SetActive(true);
                }
            }
        }
        else
        {
            // Interactor can't carry items - show prompt
            if (needKeyPrompt != null)
            {
                needKeyPrompt.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Called when an interactor exits the door's trigger zone
    /// </summary>
    public void OnInteractorExitZone(IInteractor interactor)
    {
        // Hide prompts when interactor leaves
        if (needKeyPrompt != null) needKeyPrompt.SetActive(false);
        if (canOpenPrompt != null) canOpenPrompt.SetActive(false);
    }

    /// <summary>
    /// Called when an interactor presses interact button (not used for automatic doors)
    /// </summary>
    public bool Interact(IInteractor interactor)
    {
        // For doors, we use automatic opening in OnInteractorEnterZone
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
    private void OpenDoor(InteractionHandler handler)
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
        handler.UseCarriedItem();

        // Disable collider so interactor can pass through
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
