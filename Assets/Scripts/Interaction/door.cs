using UnityEngine;

/// <summary>
/// Represents a door that can be opened with a key
/// Opens automatically when an interactor with the correct key enters the trigger zone
/// Can also be used with manual items from inventory
/// </summary>
public class door : MonoBehaviour, IInteractable, IItemUsable
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

        // Try to get InventorySystem component to check for keys
        InventorySystem inventory = interactor.GetGameObject().GetComponent<InventorySystem>();
        
        if (inventory != null)
        {
            // Check if player has item that can open this door
            ItemData keyItem = inventory.GetItemForTag(requiredKeyTag);
            
            if (keyItem != null)
            {
                // Check if it's auto-use item
                if (keyItem.useType == ItemUseType.AutoUse)
                {
                    // Open door automatically
                    OpenDoor(inventory, keyItem);
                }
                else if (keyItem.useType == ItemUseType.Manual)
                {
                    // Register this door as usable
                    inventory.RegisterNearbyUsable(requiredKeyTag, gameObject);
                    
                    // Show can-use prompt
                    if (canOpenPrompt != null)
                    {
                        canOpenPrompt.SetActive(true);
                    }
                }
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
            // Interactor doesn't have InventorySystem - show prompt
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
        // Unregister from inventory system
        InventorySystem inventory = interactor.GetGameObject().GetComponent<InventorySystem>();
        if (inventory != null)
        {
            inventory.UnregisterNearbyUsable(requiredKeyTag);
        }
        
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
    private void OpenDoor(InventorySystem inventory, ItemData keyItem)
    {
        if (isOpened) return;

        Debug.Log($"Door opened with {keyItem.itemName}!");

        // Change sprite to opened state
        if (spriteRenderer != null && doorOpenedSprite != null)
        {
            spriteRenderer.sprite = doorOpenedSprite;
        }
        else
        {
            Debug.LogError("Door is missing SpriteRenderer or doorOpenedSprite is not assigned!");
        }

        // Consume the key if it's consumable
        if (keyItem.isConsumable)
        {
            inventory.RemoveItem(keyItem, 1);
        }

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

    // ==================== IItemUsable Implementation ====================

    /// <summary>
    /// Use an item on this door
    /// </summary>
    public bool UseItem(ItemData item, InventorySystem inventory)
    {
        if (isOpened)
        {
            Debug.Log("Door is already open");
            return false;
        }

        // Check if this is the correct item
        if (item.interactableTag != requiredKeyTag)
        {
            Debug.Log($"Cannot use {item.itemName} on this door");
            return false;
        }

        // Open the door
        OpenDoor(inventory, item);
        return true;
    }

    /// <summary>
    /// Get the tag that identifies what items can be used here
    /// </summary>
    public string GetUsableTag()
    {
        return requiredKeyTag;
    }
}
