using UnityEngine;

/// <summary>
/// Optional component that can be attached to interactable objects to provide sound configuration
/// This component is completely optional - interactable objects work fine without it
/// When attached, it provides sound data that will be used by the interaction system
/// </summary>
public class InteractableSoundComponent : MonoBehaviour
{
    [Header("Sound Configuration")]
    [Tooltip("Sound data configuration for this interactable object. Leave empty if no sounds are needed.")]
    [SerializeField] private InteractionSoundData soundData;

    [Header("Sound Playback Settings")]
    [Tooltip("Whether to play sounds as 2D (true) or 3D (false). 3D sounds are positioned in world space.")]
    [SerializeField] private bool playAs2D = false;

    /// <summary>
    /// Get the sound data configuration
    /// Returns null if no sound data is assigned
    /// </summary>
    public InteractionSoundData GetSoundData()
    {
        return soundData;
    }

    /// <summary>
    /// Check if this component has sound data assigned
    /// </summary>
    public bool HasSoundData()
    {
        return soundData != null;
    }

    /// <summary>
    /// Get whether sounds should be played as 2D
    /// </summary>
    public bool ShouldPlayAs2D()
    {
        return playAs2D;
    }

    /// <summary>
    /// Get the position where sounds should be played
    /// For 3D sounds, this is the object's position
    /// For 2D sounds, this value is ignored
    /// </summary>
    public Vector3 GetSoundPosition()
    {
        return transform.position;
    }
}

