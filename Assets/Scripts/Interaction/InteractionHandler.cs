using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages player interactions with world objects, NPCs, and items
/// Handles carrying items (like keys) and detecting nearby interactable objects
/// </summary>
public class InteractionHandler : MonoBehaviour
{
    [Header("Item Carrying")]
    [SerializeField] private Transform holdPoint;
    
    private GameObject carriedItem = null;
    private List<IInteractable> nearbyInteractables = new List<IInteractable>();
    private IInteractable currentInteractable = null;

    /// <summary>
    /// Get the currently carried item (e.g., key)
    /// </summary>
    public GameObject GetCarriedItem()
    {
        return carriedItem;
    }

    /// <summary>
    /// Check if player is carrying a specific item by tag
    /// </summary>
    /// <param name="itemTag">Tag to check for (e.g., "key1")</param>
    public bool IsCarrying(string itemTag)
    {
        return carriedItem != null && carriedItem.CompareTag(itemTag);
    }

    /// <summary>
    /// Pick up an item and attach it to the hold point
    /// </summary>
    /// <param name="item">The GameObject to pick up</param>
    public void PickUpItem(GameObject item)
    {
        if (carriedItem != null)
        {
            Debug.LogWarning("InteractionHandler: Already carrying an item!");
            return;
        }

        carriedItem = item;

        if (holdPoint != null)
        {
            carriedItem.transform.SetParent(holdPoint);
            carriedItem.transform.localPosition = Vector3.zero;
            
            // Disable collider to prevent interference
            Collider2D itemCollider = carriedItem.GetComponent<Collider2D>();
            if (itemCollider != null)
            {
                itemCollider.enabled = false;
            }

            Debug.Log($"Picked up item: {item.name}");
        }
        else
        {
            Debug.LogError("InteractionHandler: HoldPoint is not assigned in the Inspector!");
        }
    }

    /// <summary>
    /// Use/consume the currently carried item
    /// </summary>
    public void UseCarriedItem()
    {
        if (carriedItem != null)
        {
            Debug.Log($"Used item: {carriedItem.name}");
            Destroy(carriedItem);
            carriedItem = null;
        }
    }

    /// <summary>
    /// Drop the currently carried item
    /// </summary>
    public void DropCarriedItem()
    {
        if (carriedItem != null)
        {
            carriedItem.transform.SetParent(null);
            
            // Re-enable collider
            Collider2D itemCollider = carriedItem.GetComponent<Collider2D>();
            if (itemCollider != null)
            {
                itemCollider.enabled = true;
            }

            Debug.Log($"Dropped item: {carriedItem.name}");
            carriedItem = null;
        }
    }

    /// <summary>
    /// Register an interactable object when player enters its zone
    /// </summary>
    public void RegisterInteractable(IInteractable interactable)
    {
        if (!nearbyInteractables.Contains(interactable))
        {
            nearbyInteractables.Add(interactable);
            
            // If this is the first interactable, make it current
            if (currentInteractable == null)
            {
                currentInteractable = interactable;
            }
        }
    }

    /// <summary>
    /// Unregister an interactable object when player exits its zone
    /// </summary>
    public void UnregisterInteractable(IInteractable interactable)
    {
        nearbyInteractables.Remove(interactable);
        
        // If the current interactable left, switch to another one or null
        if (currentInteractable == interactable)
        {
            currentInteractable = nearbyInteractables.Count > 0 ? nearbyInteractables[0] : null;
        }
    }

    /// <summary>
    /// Interact with the current nearby interactable object
    /// </summary>
    /// <param name="player">Reference to the PlayerController</param>
    /// <returns>True if interaction was successful</returns>
    public bool TryInteract(PlayerController player)
    {
        if (currentInteractable != null)
        {
            return currentInteractable.Interact(player);
        }
        return false;
    }

    /// <summary>
    /// Check if there are any interactable objects nearby
    /// </summary>
    public bool HasNearbyInteractable()
    {
        return currentInteractable != null;
    }

    /// <summary>
    /// Get the current interactable object
    /// </summary>
    public IInteractable GetCurrentInteractable()
    {
        return currentInteractable;
    }

    /// <summary>
    /// Clear all registered interactables (useful when changing scenes or states)
    /// </summary>
    public void ClearInteractables()
    {
        nearbyInteractables.Clear();
        currentInteractable = null;
    }
}

