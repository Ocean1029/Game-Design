using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Component that automatically plays background music when a scene loads
/// Attach this to a GameObject in the scene to set the scene's background music
/// Music will automatically fade in when scene loads and fade out when scene unloads
/// </summary>
public class SceneMusic : MonoBehaviour
{
    [Header("Music Configuration")]
    [Tooltip("Background music to play in this scene")]
    [SerializeField] private AudioClip sceneMusic;
    
    [Tooltip("Volume of the scene music (0.0 to 1.0)")]
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.7f;

    [Header("Fade Settings")]
    [Tooltip("Whether to fade in music when scene loads")]
    [SerializeField] private bool fadeInOnLoad = true;
    
    [Tooltip("Fade in duration in seconds (uses SoundManager default if 0)")]
    [SerializeField] private float fadeInDuration = 0f;
    
    [Tooltip("Whether to fade out music when scene unloads")]
    [SerializeField] private bool fadeOutOnUnload = true;
    
    [Tooltip("Fade out duration in seconds (uses SoundManager default if 0)")]
    [SerializeField] private float fadeOutDuration = 0f;

    [Header("Settings")]
    [Tooltip("Whether to stop current music before playing this scene's music")]
    [SerializeField] private bool stopCurrentMusic = true;

    private SoundManager soundManager;

    void Start()
    {
        // Get SoundManager reference
        soundManager = SoundManager.GetInstance();
        
        if (soundManager == null)
        {
            Debug.LogWarning("SceneMusic: SoundManager not found! Music will not play.");
            return;
        }

        // Play scene music
        if (sceneMusic != null)
        {
            PlaySceneMusic();
        }
        else
        {
            // If no music assigned, stop current music if configured
            if (stopCurrentMusic)
            {
                soundManager.StopMusic(fadeOutOnUnload, fadeOutDuration > 0f ? fadeOutDuration : -1f);
            }
        }
    }

    void OnDestroy()
    {
        // Fade out music when scene unloads (if configured)
        if (fadeOutOnUnload && soundManager != null && sceneMusic != null)
        {
            // Only fade out if this scene's music is currently playing
            if (soundManager.GetCurrentMusic() == sceneMusic)
            {
                soundManager.StopMusic(true, fadeOutDuration > 0f ? fadeOutDuration : -1f);
            }
        }
    }

    /// <summary>
    /// Play the scene music
    /// </summary>
    private void PlaySceneMusic()
    {
        if (soundManager == null || sceneMusic == null)
        {
            return;
        }

        // Check if this music is already playing
        if (soundManager.GetCurrentMusic() == sceneMusic && soundManager.IsMusicPlaying())
        {
            // Music is already playing, do nothing
            return;
        }

        // Stop current music if configured
        if (stopCurrentMusic && soundManager.IsMusicPlaying())
        {
            // Use crossfade for smooth transition
            soundManager.ChangeMusic(sceneMusic, fadeInDuration > 0f ? fadeInDuration * 2f : -1f);
        }
        else
        {
            // Play new music with fade in
            soundManager.PlayMusic(sceneMusic, fadeInOnLoad, fadeInDuration > 0f ? fadeInDuration : -1f);
        }

        // Set volume if specified
        if (musicVolume != soundManager.GetMusicVolume())
        {
            soundManager.SetMusicVolume(musicVolume);
        }
    }

    /// <summary>
    /// Set the scene music clip
    /// </summary>
    public void SetSceneMusic(AudioClip clip)
    {
        sceneMusic = clip;
        
        // If music is already playing and we change the clip, restart it
        if (soundManager != null && soundManager.IsMusicPlaying())
        {
            PlaySceneMusic();
        }
    }

    /// <summary>
    /// Get the current scene music clip
    /// </summary>
    public AudioClip GetSceneMusic()
    {
        return sceneMusic;
    }

    /// <summary>
    /// Manually trigger music playback (useful for delayed starts)
    /// </summary>
    public void PlayMusic()
    {
        PlaySceneMusic();
    }

    /// <summary>
    /// Stop the scene music
    /// </summary>
    public void StopMusic()
    {
        if (soundManager != null)
        {
            soundManager.StopMusic(fadeOutOnUnload, fadeOutDuration > 0f ? fadeOutDuration : -1f);
        }
    }
}

