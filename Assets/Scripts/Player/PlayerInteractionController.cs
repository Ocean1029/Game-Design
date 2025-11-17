using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages player interactions with world objects, NPCs, and items
/// Handles carrying items (like keys) and detecting nearby interactable objects
/// Integrates with SoundManager to play interaction sounds
/// </summary>
public class InteractionHandler : MonoBehaviour
{
    [Header("Item Carrying")]
    [SerializeField] private Transform holdPoint;
    
    private GameObject carriedItem = null;
    private List<IInteractable> nearbyInteractables = new List<IInteractable>();
    private IInteractable currentInteractable = null;

    // Sound manager reference (cached for performance)
    private SoundManager soundManager;

    void Start()
    {
        // Initialize sound manager reference
        soundManager = SoundManager.GetInstance();
    }

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

            // Play enter zone sound if available
            PlayInteractionSound(interactable, InteractionSoundType.EnterZone);
        }
    }

    /// <summary>
    /// Unregister an interactable object when player exits its zone
    /// </summary>
    public void UnregisterInteractable(IInteractable interactable)
    {
        // Play exit zone sound before removing (if available)
        PlayInteractionSound(interactable, InteractionSoundType.ExitZone);

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
    /// <param name="interactor">Reference to the IInteractor (player, NPC, etc.)</param>
    /// <returns>True if interaction was successful</returns>
    public bool TryInteract(IInteractor interactor)
    {
        if (currentInteractable != null)
        {
            bool interactionResult = currentInteractable.Interact(interactor);
            
            // Play appropriate sound based on interaction result
            if (interactionResult)
            {
                PlayInteractionSound(currentInteractable, InteractionSoundType.Success);
            }
            else
            {
                PlayInteractionSound(currentInteractable, InteractionSoundType.Failure);
            }
            
            return interactionResult;
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

    // ==================== Sound Integration ====================

    /// <summary>
    /// Play an interaction sound for the given interactable object
    /// This method checks if the interactable has sound configuration and plays the appropriate sound
    /// If no sound is configured, this method does nothing (gracefully handles missing sounds)
    /// </summary>
    /// <param name="interactable">The interactable object to play sound for</param>
    /// <param name="soundType">Type of interaction sound to play</param>
    private void PlayInteractionSound(IInteractable interactable, InteractionSoundType soundType)
    {
        // Early return if sound manager is not available
        if (soundManager == null)
        {
            soundManager = SoundManager.GetInstance();
            if (soundManager == null)
            {
                return;
            }
        }

        // Get the GameObject from the interactable
        GameObject interactableObject = interactable.GetGameObject();
        if (interactableObject == null)
        {
            return;
        }

        // Try to get InteractableSoundComponent from the interactable object
        InteractableSoundComponent soundComponent = interactableObject.GetComponent<InteractableSoundComponent>();
        if (soundComponent == null || !soundComponent.HasSoundData())
        {
            // No sound component or no sound data - this is acceptable, just return silently
            return;
        }

        // Get sound data and check if it has the requested sound type
        InteractionSoundData soundData = soundComponent.GetSoundData();
        if (soundData == null || !soundData.HasSound(soundType))
        {
            // Sound data exists but doesn't have this specific sound type - this is acceptable
            return;
        }

        // Get the sound clip for this interaction type
        AudioClip soundClip = soundData.GetSoundClip(soundType);
        if (soundClip == null)
        {
            return;
        }

        // Determine playback position and whether to play as 2D
        Vector3 soundPosition = soundComponent.GetSoundPosition();
        bool playAs2D = soundComponent.ShouldPlayAs2D();

        // Play the sound through SoundManager
        if (playAs2D)
        {
            soundManager.PlaySound2D(soundClip, soundData.Volume);
        }
        else
        {
            soundManager.PlaySound(soundClip, soundPosition, soundData.Volume);
        }
    }
}

