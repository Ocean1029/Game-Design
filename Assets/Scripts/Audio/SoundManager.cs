using UnityEngine;

/// <summary>
/// Central sound manager for handling all audio playback in the game
/// Implements singleton pattern to ensure only one instance exists
/// Provides unified API for playing sounds with support for 2D and 3D audio
/// </summary>
public class SoundManager : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Whether to show debug information")]
    [SerializeField] private bool showDebugInfo = false;

    // Singleton instance
    private static SoundManager instance;

    // Volume settings (reserved for future implementation)
    // These fields are reserved but not yet implemented
    [Header("Volume Settings (Reserved)")]
    [Tooltip("Master volume (0.0 to 1.0) - Reserved for future implementation")]
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
    
    [Tooltip("SFX volume (0.0 to 1.0) - Reserved for future implementation")]
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

    // Audio source for 2D sounds
    private AudioSource audioSource2D;

    // ==================== Singleton & Initialization ====================

    void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize audio source for 2D sounds
        InitializeAudioSource();
    }

    void OnDestroy()
    {
        // Clear instance reference when destroyed
        if (instance == this)
        {
            instance = null;
        }
    }

    /// <summary>
    /// Initialize the audio source component for 2D sounds
    /// </summary>
    private void InitializeAudioSource()
    {
        audioSource2D = GetComponent<AudioSource>();
        if (audioSource2D == null)
        {
            audioSource2D = gameObject.AddComponent<AudioSource>();
        }

        // Configure audio source for 2D playback
        audioSource2D.spatialBlend = 0f; // 2D sound (no spatialization)
        audioSource2D.playOnAwake = false;
    }

    // ==================== Singleton Access ====================

    /// <summary>
    /// Get the singleton instance of SoundManager
    /// </summary>
    public static SoundManager GetInstance()
    {
        if (instance == null)
        {
            // Try to find existing instance in scene
            instance = FindFirstObjectByType<SoundManager>();
            
            if (instance == null)
            {
                // Create new instance if none exists
                GameObject soundManagerObject = new GameObject("SoundManager");
                instance = soundManagerObject.AddComponent<SoundManager>();
            }
        }
        
        return instance;
    }

    // ==================== Sound Playback API ====================

    /// <summary>
    /// Play a sound at a specific 3D position in the world
    /// This creates a temporary AudioSource at the position and plays the sound
    /// </summary>
    /// <param name="clip">Audio clip to play</param>
    /// <param name="position">World position where the sound should be played</param>
    /// <param name="volume">Volume of the sound (0.0 to 1.0), will be multiplied by master and SFX volume in future</param>
    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("SoundManager: Attempted to play null AudioClip");
            }
            return;
        }

        // Apply volume settings (reserved for future implementation)
        float finalVolume = volume * masterVolume * sfxVolume;

        // Play sound at position using Unity's built-in method
        AudioSource.PlayClipAtPoint(clip, position, finalVolume);

        if (showDebugInfo)
        {
            Debug.Log($"SoundManager: Playing sound '{clip.name}' at position {position} with volume {finalVolume}");
        }
    }

    /// <summary>
    /// Play a 2D sound that doesn't have spatial positioning
    /// Useful for UI sounds, system notifications, etc.
    /// </summary>
    /// <param name="clip">Audio clip to play</param>
    /// <param name="volume">Volume of the sound (0.0 to 1.0), will be multiplied by master and SFX volume in future</param>
    public void PlaySound2D(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("SoundManager: Attempted to play null AudioClip");
            }
            return;
        }

        if (audioSource2D == null)
        {
            InitializeAudioSource();
        }

        // Apply volume settings (reserved for future implementation)
        float finalVolume = volume * masterVolume * sfxVolume;

        // Play one shot sound using the 2D audio source
        audioSource2D.PlayOneShot(clip, finalVolume);

        if (showDebugInfo)
        {
            Debug.Log($"SoundManager: Playing 2D sound '{clip.name}' with volume {finalVolume}");
        }
    }

    /// <summary>
    /// Play a sound from an InteractionSoundData configuration
    /// This method checks if the sound data exists and plays the appropriate sound
    /// </summary>
    /// <param name="soundData">InteractionSoundData containing sound configuration</param>
    /// <param name="position">World position where the sound should be played (for 3D sounds)</param>
    /// <param name="is2D">Whether to play as 2D sound (true) or 3D sound (false)</param>
    public void PlayInteractionSound(InteractionSoundData soundData, Vector3 position, bool is2D = false)
    {
        if (soundData == null)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("SoundManager: Attempted to play sound with null InteractionSoundData");
            }
            return;
        }

        AudioClip clipToPlay = soundData.GetSoundClip();
        if (clipToPlay == null)
        {
            // Sound data exists but no clip assigned - this is acceptable
            if (showDebugInfo)
            {
                Debug.Log($"SoundManager: InteractionSoundData '{soundData.name}' has no sound clip assigned (this is acceptable)");
            }
            return;
        }

        if (is2D)
        {
            PlaySound2D(clipToPlay, soundData.Volume);
        }
        else
        {
            PlaySound(clipToPlay, position, soundData.Volume);
        }
    }

    // ==================== Volume Control (Reserved for Future Implementation) ====================

    /// <summary>
    /// Set the master volume
    /// Reserved for future implementation - currently does nothing
    /// </summary>
    /// <param name="volume">Volume level (0.0 to 1.0)</param>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        // TODO: Apply master volume to all audio sources when implemented
    }

    /// <summary>
    /// Get the current master volume
    /// Reserved for future implementation
    /// </summary>
    public float GetMasterVolume()
    {
        return masterVolume;
    }

    /// <summary>
    /// Set the SFX volume
    /// Reserved for future implementation - currently does nothing
    /// </summary>
    /// <param name="volume">Volume level (0.0 to 1.0)</param>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        // TODO: Apply SFX volume to all audio sources when implemented
    }

    /// <summary>
    /// Get the current SFX volume
    /// Reserved for future implementation
    /// </summary>
    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    // ==================== Debug Methods ====================

    /// <summary>
    /// Get debug information about the sound manager
    /// </summary>
    public string GetDebugInfo()
    {
        string info = "SoundManager Debug Info:\n";
        info += $"Master Volume: {masterVolume}\n";
        info += $"SFX Volume: {sfxVolume}\n";
        info += $"2D Audio Source: {(audioSource2D != null ? "Initialized" : "Not Initialized")}\n";
        return info;
    }
}

