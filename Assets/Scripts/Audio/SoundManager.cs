using UnityEngine;
using System.Collections;

/// <summary>
/// Central sound manager for handling all audio playback in the game
/// Implements singleton pattern to ensure only one instance exists
/// Provides unified API for playing sounds with support for 2D and 3D audio
/// </summary>
public class SoundManager : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Whether to show debug information")]
    [SerializeField] private bool showDebugInfo = true;

    // Singleton instance
    private static SoundManager instance;
    
    // Flag to track if application is quitting
    // This prevents creating new GameObjects during cleanup
    private static bool isApplicationQuitting = false;

    // Volume settings (reserved for future implementation)
    // These fields are reserved but not yet implemented
    [Header("Volume Settings (Reserved)")]
    [Tooltip("Master volume (0.0 to 1.0) - Reserved for future implementation")]
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
    
    [Tooltip("SFX volume (0.0 to 1.0) - Reserved for future implementation")]
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

    [Header("Music Settings")]
    [Tooltip("Music volume (0.0 to 1.0)")]
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.2f;
    
    [Tooltip("Default fade duration for music transitions (in seconds)")]
    [SerializeField] private float defaultFadeDuration = 1f;

    // Audio sources
    private AudioSource audioSource2D; // For 2D sound effects
    private AudioSource musicAudioSource; // For background music

    // Music state
    private AudioClip currentMusicClip = null;
    private Coroutine fadeCoroutine = null;

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

        // Initialize audio sources
        InitializeAudioSource();
        InitializeMusicAudioSource();
    }

    void OnDestroy()
    {
        // Stop any running coroutines to prevent lingering operations
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
        
        // Clear instance reference when destroyed
        if (instance == this)
        {
            instance = null;
        }
    }
    
    void OnApplicationQuit()
    {
        // Mark that application is quitting to prevent creating new GameObjects
        isApplicationQuitting = true;
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

    /// <summary>
    /// Initialize the audio source component for background music
    /// </summary>
    private void InitializeMusicAudioSource()
    {
        // Create a child GameObject for music to keep it separate
        GameObject musicObject = transform.Find("MusicAudioSource")?.gameObject;
        if (musicObject == null)
        {
            musicObject = new GameObject("MusicAudioSource");
            musicObject.transform.SetParent(transform);
        }

        musicAudioSource = musicObject.GetComponent<AudioSource>();
        if (musicAudioSource == null)
        {
            musicAudioSource = musicObject.AddComponent<AudioSource>();
        }

        // Configure audio source for music playback
        musicAudioSource.spatialBlend = 0f; // 2D music
        musicAudioSource.playOnAwake = false;
        musicAudioSource.loop = true; // Music typically loops
        musicAudioSource.volume = musicVolume * masterVolume;
    }

    // ==================== Singleton Access ====================

    /// <summary>
    /// Get the singleton instance of SoundManager
    /// Prevents creating new GameObjects during application quit or scene cleanup
    /// </summary>
    public static SoundManager GetInstance()
    {
        // Don't create new instances if application is quitting
        // This prevents the "objects were not cleaned up" warning
        if (isApplicationQuitting)
        {
            return null;
        }
        
        if (instance == null)
        {
            // Try to find existing instance in scene
            instance = FindFirstObjectByType<SoundManager>();
            
            if (instance == null && !isApplicationQuitting)
            {
                // Only create new instance if application is not quitting
                // Check again to ensure we're not in cleanup phase
                if (Application.isPlaying)
                {
                    GameObject soundManagerObject = new GameObject("SoundManager");
                    instance = soundManagerObject.AddComponent<SoundManager>();
                }
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

    // ==================== Music Playback API ====================

    /// <summary>
    /// Play background music
    /// </summary>
    /// <param name="musicClip">Audio clip to play as background music</param>
    /// <param name="fadeIn">Whether to fade in the music (default: true)</param>
    /// <param name="fadeDuration">Duration of fade in seconds (uses default if not specified)</param>
    public void PlayMusic(AudioClip musicClip, bool fadeIn = true, float fadeDuration = -1f)
    {
        if (musicClip == null)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("SoundManager: Attempted to play null music clip");
            }
            return;
        }

        // If same music is already playing, do nothing
        if (currentMusicClip == musicClip && musicAudioSource != null && musicAudioSource.isPlaying)
        {
            if (showDebugInfo)
            {
                Debug.Log($"SoundManager: Music '{musicClip.name}' is already playing");
            }
            return;
        }

        // Stop any ongoing fade
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        // Initialize music audio source if needed
        if (musicAudioSource == null)
        {
            InitializeMusicAudioSource();
        }

        currentMusicClip = musicClip;
        musicAudioSource.clip = musicClip;

        if (fadeIn)
        {
            float duration = fadeDuration > 0f ? fadeDuration : defaultFadeDuration;
            fadeCoroutine = StartCoroutine(FadeInMusic(duration));
        }
        else
        {
            musicAudioSource.volume = musicVolume * masterVolume;
            musicAudioSource.Play();
        }

        if (showDebugInfo)
        {
            Debug.Log($"SoundManager: Playing music '{musicClip.name}' (fadeIn: {fadeIn})");
        }
    }

    /// <summary>
    /// Stop the currently playing music
    /// </summary>
    /// <param name="fadeOut">Whether to fade out the music (default: true)</param>
    /// <param name="fadeDuration">Duration of fade in seconds (uses default if not specified)</param>
    public void StopMusic(bool fadeOut = true, float fadeDuration = -1f)
    {
        if (musicAudioSource == null || !musicAudioSource.isPlaying)
        {
            return;
        }

        // Stop any ongoing fade
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (fadeOut)
        {
            float duration = fadeDuration > 0f ? fadeDuration : defaultFadeDuration;
            fadeCoroutine = StartCoroutine(FadeOutMusic(duration));
        }
        else
        {
            musicAudioSource.Stop();
            currentMusicClip = null;
        }

        if (showDebugInfo)
        {
            Debug.Log($"SoundManager: Stopping music (fadeOut: {fadeOut})");
        }
    }

    /// <summary>
    /// Change to a different music track with smooth transition
    /// </summary>
    /// <param name="newMusicClip">New music clip to play</param>
    /// <param name="fadeDuration">Duration of crossfade in seconds (uses default if not specified)</param>
    public void ChangeMusic(AudioClip newMusicClip, float fadeDuration = -1f)
    {
        if (newMusicClip == null)
        {
            StopMusic();
            return;
        }

        // If same music, do nothing
        if (currentMusicClip == newMusicClip && musicAudioSource != null && musicAudioSource.isPlaying)
        {
            return;
        }

        float duration = fadeDuration > 0f ? fadeDuration : defaultFadeDuration;
        
        // Stop any ongoing fade
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        // Start crossfade
        fadeCoroutine = StartCoroutine(CrossfadeMusic(newMusicClip, duration));
    }

    /// <summary>
    /// Check if music is currently playing
    /// </summary>
    public bool IsMusicPlaying()
    {
        return musicAudioSource != null && musicAudioSource.isPlaying;
    }

    /// <summary>
    /// Get the currently playing music clip
    /// </summary>
    public AudioClip GetCurrentMusic()
    {
        return currentMusicClip;
    }

    // ==================== Music Volume Control ====================

    /// <summary>
    /// Set the music volume
    /// If a fade coroutine is currently running, it will be stopped and the volume will be applied immediately
    /// If the music audio source is not yet initialized, it will be initialized to ensure the volume setting is preserved
    /// The fade coroutines are designed to dynamically read musicVolume each frame, so volume changes during fade will be smooth
    /// </summary>
    /// <param name="volume">Volume level (0.0 to 1.0)</param>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        
        // Stop any ongoing fade coroutine to prevent it from overriding the volume change
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
        
        // Initialize music audio source if needed (in case SetMusicVolume is called before music plays)
        if (musicAudioSource == null)
        {
            InitializeMusicAudioSource();
        }
        
        // Apply volume immediately
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = musicVolume * masterVolume;
        }
    }

    /// <summary>
    /// Get the current music volume
    /// </summary>
    public float GetMusicVolume()
    {
        return musicVolume;
    }

    // ==================== Music Fade Coroutines ====================

    /// <summary>
    /// Fade in music from silence
    /// Dynamically reads musicVolume each frame to respond to volume changes during fade
    /// </summary>
    private IEnumerator FadeInMusic(float duration)
    {
        if (musicAudioSource == null)
        {
            yield break;
        }

        musicAudioSource.volume = 0f;
        musicAudioSource.Play();

        float elapsedTime = 0f;
        float startVolume = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            // Recalculate target volume each frame to respond to volume changes
            float targetVolume = musicVolume * masterVolume;
            musicAudioSource.volume = Mathf.Lerp(startVolume, targetVolume, progress);
            yield return null;
        }

        // Ensure final volume matches current musicVolume setting
        musicAudioSource.volume = musicVolume * masterVolume;
        fadeCoroutine = null;
    }

    /// <summary>
    /// Fade out music to silence
    /// If volume is changed during fade out, the fade will continue smoothly
    /// </summary>
    private IEnumerator FadeOutMusic(float duration)
    {
        if (musicAudioSource == null)
        {
            yield break;
        }

        float startVolume = musicAudioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            // Fade to silence regardless of volume changes
            musicAudioSource.volume = Mathf.Lerp(startVolume, 0f, progress);
            yield return null;
        }

        musicAudioSource.Stop();
        // Set volume to current musicVolume setting for next playback
        musicAudioSource.volume = musicVolume * masterVolume;
        currentMusicClip = null;
        fadeCoroutine = null;
    }

    /// <summary>
    /// Crossfade from current music to new music
    /// Dynamically reads musicVolume each frame during fade in to respond to volume changes
    /// </summary>
    private IEnumerator CrossfadeMusic(AudioClip newMusicClip, float duration)
    {
        if (musicAudioSource == null)
        {
            yield break;
        }

        float startVolume = musicAudioSource.volume;
        float halfDuration = duration * 0.5f;

        // Fade out current music
        float elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / halfDuration;
            musicAudioSource.volume = Mathf.Lerp(startVolume, 0f, progress);
            yield return null;
        }

        // Switch to new music
        currentMusicClip = newMusicClip;
        musicAudioSource.clip = newMusicClip;
        musicAudioSource.volume = 0f;
        musicAudioSource.Play();

        // Fade in new music
        elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / halfDuration;
            // Recalculate target volume each frame to respond to volume changes
            float targetVolume = musicVolume * masterVolume;
            musicAudioSource.volume = Mathf.Lerp(0f, targetVolume, progress);
            yield return null;
        }

        // Ensure final volume matches current musicVolume setting
        musicAudioSource.volume = musicVolume * masterVolume;
        fadeCoroutine = null;

        if (showDebugInfo)
        {
            Debug.Log($"SoundManager: Crossfaded to music '{newMusicClip.name}'");
        }
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
        info += $"Music Volume: {musicVolume}\n";
        info += $"2D Audio Source: {(audioSource2D != null ? "Initialized" : "Not Initialized")}\n";
        info += $"Music Audio Source: {(musicAudioSource != null ? "Initialized" : "Not Initialized")}\n";
        info += $"Current Music: {(currentMusicClip != null ? currentMusicClip.name : "None")}\n";
        info += $"Music Playing: {IsMusicPlaying()}\n";
        return info;
    }
}

