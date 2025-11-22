using UnityEngine;

/// <summary>
/// ScriptableObject for defining sound configurations for interactable objects
/// Allows designers to configure sounds for different interaction types without modifying code
/// All sound clips are optional - objects can work without sounds assigned
/// </summary>
[CreateAssetMenu(fileName = "NewInteractionSoundData", menuName = "Game/Audio/Interaction Sound Data")]
public class InteractionSoundData : ScriptableObject
{
    [Header("Interaction Sounds")]
    [Tooltip("Sound played when interaction succeeds (e.g., door opens, item collected)")]
    [SerializeField] private AudioClip successSound;
    
    [Tooltip("Sound played when interaction fails (e.g., missing key, invalid action)")]
    [SerializeField] private AudioClip failureSound;
    
    [Tooltip("Sound played when interactor enters the interaction zone")]
    [SerializeField] private AudioClip enterZoneSound;
    
    [Tooltip("Sound played when interactor exits the interaction zone")]
    [SerializeField] private AudioClip exitZoneSound;

    [Header("Item Interaction Sounds")]
    [Tooltip("Sound played when an item is picked up")]
    [SerializeField] private AudioClip pickupSound;
    
    [Tooltip("Sound played when an item is used")]
    [SerializeField] private AudioClip useSound;

    [Header("Volume Settings")]
    [Tooltip("Volume multiplier for all sounds in this configuration (0.0 to 1.0)")]
    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    // ==================== Public Properties ====================

    /// <summary>
    /// Get the success sound clip
    /// </summary>
    public AudioClip SuccessSound => successSound;

    /// <summary>
    /// Get the failure sound clip
    /// </summary>
    public AudioClip FailureSound => failureSound;

    /// <summary>
    /// Get the enter zone sound clip
    /// </summary>
    public AudioClip EnterZoneSound => enterZoneSound;

    /// <summary>
    /// Get the exit zone sound clip
    /// </summary>
    public AudioClip ExitZoneSound => exitZoneSound;

    /// <summary>
    /// Get the pickup sound clip
    /// </summary>
    public AudioClip PickupSound => pickupSound;

    /// <summary>
    /// Get the use sound clip
    /// </summary>
    public AudioClip UseSound => useSound;

    /// <summary>
    /// Get the volume multiplier
    /// </summary>
    public float Volume => volume;

    // ==================== Public Methods ====================

    /// <summary>
    /// Get a sound clip based on interaction type
    /// Returns null if no sound is assigned for the given type
    /// </summary>
    /// <param name="interactionType">Type of interaction to get sound for</param>
    /// <returns>AudioClip for the interaction type, or null if not assigned</returns>
    public AudioClip GetSoundClip(InteractionSoundType interactionType)
    {
        return interactionType switch
        {
            InteractionSoundType.Success => successSound,
            InteractionSoundType.Failure => failureSound,
            InteractionSoundType.EnterZone => enterZoneSound,
            InteractionSoundType.ExitZone => exitZoneSound,
            InteractionSoundType.Pickup => pickupSound,
            InteractionSoundType.Use => useSound,
            _ => null
        };
    }

    /// <summary>
    /// Get the default sound clip (success sound)
    /// This is a convenience method for when interaction type is not specified
    /// </summary>
    /// <returns>Success sound clip, or null if not assigned</returns>
    public AudioClip GetSoundClip()
    {
        return successSound;
    }

    /// <summary>
    /// Check if a specific sound type is assigned
    /// </summary>
    /// <param name="interactionType">Type of interaction to check</param>
    /// <returns>True if sound is assigned, false otherwise</returns>
    public bool HasSound(InteractionSoundType interactionType)
    {
        return GetSoundClip(interactionType) != null;
    }
}

/// <summary>
/// Enumeration of different interaction sound types
/// Used to identify which sound should be played for a specific interaction event
/// </summary>
public enum InteractionSoundType
{
    /// <summary>
    /// Sound played when interaction succeeds
    /// </summary>
    Success,
    
    /// <summary>
    /// Sound played when interaction fails
    /// </summary>
    Failure,
    
    /// <summary>
    /// Sound played when interactor enters interaction zone
    /// </summary>
    EnterZone,
    
    /// <summary>
    /// Sound played when interactor exits interaction zone
    /// </summary>
    ExitZone,
    
    /// <summary>
    /// Sound played when an item is picked up
    /// </summary>
    Pickup,
    
    /// <summary>
    /// Sound played when an item is used
    /// </summary>
    Use
}

