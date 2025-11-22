using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Editor tool to automatically setup background music in scenes
/// Provides one-click setup for SceneMusic component with default music file
/// </summary>
public class MusicSetupTool
{
    /// <summary>
    /// Setup music for the current active scene
    /// Automatically finds or creates SceneMusic component and assigns the default music
    /// </summary>
    [MenuItem("Tools/Audio/Setup Scene Music", false, 10)]
    public static void SetupSceneMusic()
    {
        // Get the active scene
        var activeScene = EditorSceneManager.GetActiveScene();
        if (!activeScene.isLoaded)
        {
            EditorUtility.DisplayDialog(
                "Scene Not Loaded",
                "Please load a scene first before setting up music.",
                "OK"
            );
            return;
        }

        // Ensure SoundManager exists (it will be created automatically at runtime, but we can create it in editor too)
        SoundManager soundManager = Object.FindObjectOfType<SoundManager>();
        if (soundManager == null)
        {
            GameObject soundManagerObject = new GameObject("SoundManager");
            soundManager = soundManagerObject.AddComponent<SoundManager>();
            Undo.RegisterCreatedObjectUndo(soundManagerObject, "Create SoundManager");
            Debug.Log("MusicSetupTool: Created SoundManager GameObject (required for music playback)");
        }
        else
        {
            Debug.Log($"MusicSetupTool: Found existing SoundManager on {soundManager.gameObject.name}");
        }

        // Find or create SceneMusic component
        SceneMusic sceneMusic = Object.FindObjectOfType<SceneMusic>();
        GameObject musicObject = null;

        if (sceneMusic == null)
        {
            // Create a new GameObject for music
            musicObject = new GameObject("MusicManager");
            sceneMusic = musicObject.AddComponent<SceneMusic>();
            
            // Register undo operation
            Undo.RegisterCreatedObjectUndo(musicObject, "Create MusicManager");
            
            Debug.Log("MusicSetupTool: Created new MusicManager GameObject with SceneMusic component");
        }
        else
        {
            musicObject = sceneMusic.gameObject;
            Debug.Log($"MusicSetupTool: Found existing SceneMusic component on {musicObject.name}");
        }

        // Find the music file
        AudioClip musicClip = FindMusicFile();
        
        if (musicClip == null)
        {
            EditorUtility.DisplayDialog(
                "Music File Not Found",
                "Could not find song18.mp3 in Assets/music/ folder.\n\n" +
                "Please ensure the music file exists, or manually assign it in the SceneMusic component.",
                "OK"
            );
            Debug.LogWarning("MusicSetupTool: Music file not found. Please assign manually.");
        }
        else
        {
            // Set the music using reflection to access private field
            // We'll use SerializedObject to properly set the private field
            SerializedObject serializedObject = new SerializedObject(sceneMusic);
            SerializedProperty musicProperty = serializedObject.FindProperty("sceneMusic");
            
            if (musicProperty != null)
            {
                musicProperty.objectReferenceValue = musicClip;
                serializedObject.ApplyModifiedProperties();
                
                Debug.Log($"MusicSetupTool: Assigned music clip '{musicClip.name}' to SceneMusic component");
            }
            else
            {
                // Fallback: use public method if available
                sceneMusic.SetSceneMusic(musicClip);
                Debug.Log($"MusicSetupTool: Assigned music clip '{musicClip.name}' using SetSceneMusic method");
            }
        }

        // Configure default settings using SerializedObject
        ConfigureDefaultSettings(sceneMusic);

        // Mark scene as dirty to save changes
        EditorSceneManager.MarkSceneDirty(activeScene);
        
        // Select the music object in hierarchy
        Selection.activeGameObject = musicObject;
        
        string soundManagerStatus = soundManager != null ? $"✓ Found on {soundManager.gameObject.name}" : "⚠ Will be created at runtime";
        
        EditorUtility.DisplayDialog(
            "Music Setup Complete",
            "Scene music has been configured!\n\n" +
            $"• SoundManager: {soundManagerStatus}\n" +
            $"• Music GameObject: {musicObject.name}\n" +
            $"• Music Clip: {(musicClip != null ? musicClip.name : "Not assigned - please assign manually")}\n" +
            "• Default settings applied\n\n" +
            "You can adjust the settings in the Inspector.\n\n" +
            "Note: Music will automatically play when you enter Play mode.",
            "OK"
        );
    }

    /// <summary>
    /// Find the default music file (song18.mp3) in the Assets/music folder
    /// </summary>
    private static AudioClip FindMusicFile()
    {
        // Try to find song18.mp3
        string[] guids = AssetDatabase.FindAssets("song18 t:AudioClip");
        
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            
            if (clip != null)
            {
                Debug.Log($"MusicSetupTool: Found music file at {path}");
                return clip;
            }
        }

        // Fallback: search in Assets/music folder
        string musicFolderPath = "Assets/music";
        if (AssetDatabase.IsValidFolder(musicFolderPath))
        {
            string[] musicGuids = AssetDatabase.FindAssets("t:AudioClip", new[] { musicFolderPath });
            
            if (musicGuids.Length > 0)
            {
                // Use the first audio clip found
                string path = AssetDatabase.GUIDToAssetPath(musicGuids[0]);
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                
                if (clip != null)
                {
                    Debug.Log($"MusicSetupTool: Found music file at {path} (fallback)");
                    return clip;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Configure default settings for SceneMusic component
    /// Sets reasonable default values for music playback
    /// </summary>
    private static void ConfigureDefaultSettings(SceneMusic sceneMusic)
    {
        SerializedObject serializedObject = new SerializedObject(sceneMusic);
        
        // Set music volume (default: 0.7)
        SerializedProperty volumeProperty = serializedObject.FindProperty("musicVolume");
        if (volumeProperty != null)
        {
            volumeProperty.floatValue = 0.7f;
        }

        // Enable fade in on load
        SerializedProperty fadeInProperty = serializedObject.FindProperty("fadeInOnLoad");
        if (fadeInProperty != null)
        {
            fadeInProperty.boolValue = true;
        }

        // Set fade in duration (0 = use SoundManager default)
        SerializedProperty fadeInDurationProperty = serializedObject.FindProperty("fadeInDuration");
        if (fadeInDurationProperty != null)
        {
            fadeInDurationProperty.floatValue = 0f; // Use SoundManager default
        }

        // Enable fade out on unload
        SerializedProperty fadeOutProperty = serializedObject.FindProperty("fadeOutOnUnload");
        if (fadeOutProperty != null)
        {
            fadeOutProperty.boolValue = true;
        }

        // Set fade out duration (0 = use SoundManager default)
        SerializedProperty fadeOutDurationProperty = serializedObject.FindProperty("fadeOutDuration");
        if (fadeOutDurationProperty != null)
        {
            fadeOutDurationProperty.floatValue = 0f; // Use SoundManager default
        }

        // Stop current music before playing (for smooth transitions)
        SerializedProperty stopCurrentProperty = serializedObject.FindProperty("stopCurrentMusic");
        if (stopCurrentProperty != null)
        {
            stopCurrentProperty.boolValue = true;
        }

        serializedObject.ApplyModifiedProperties();
        
        Debug.Log("MusicSetupTool: Applied default settings to SceneMusic component");
    }

    /// <summary>
    /// Validate menu item - only enable when a scene is loaded
    /// </summary>
    [MenuItem("Tools/Audio/Setup Scene Music", true)]
    public static bool ValidateSetupSceneMusic()
    {
        return EditorSceneManager.GetActiveScene().isLoaded;
    }
}

