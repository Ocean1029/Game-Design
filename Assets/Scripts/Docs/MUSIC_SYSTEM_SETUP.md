# Music System Setup Guide

This guide explains how to set up and use the background music system in the game.

## Overview

The music system is integrated into `SoundManager` and provides:
- Background music playback
- Smooth fade in/out transitions
- Crossfade between different tracks
- Separate volume control for music
- Scene-based automatic music management

## System Architecture

### SoundManager Music Features

The `SoundManager` now includes comprehensive music management:
- **Dedicated Music AudioSource**: Separate from sound effects for independent control
- **Fade Transitions**: Smooth fade in/out and crossfade between tracks
- **Volume Control**: Independent music volume setting
- **Loop Support**: Music automatically loops by default

### SceneMusic Component

The `SceneMusic` component allows each scene to automatically play its background music:
- Automatically plays music when scene loads
- Configurable fade in/out settings
- Handles music transitions between scenes

## Setup Instructions

### Method 1: Using SceneMusic Component (Recommended)

This is the easiest way to add music to a scene.

#### Step 1: Add SceneMusic to Scene

1. In your scene, create an empty GameObject (or use an existing manager object)
2. Add the `SceneMusic` component to it
3. Name it something like "SceneMusicManager" for clarity

#### Step 2: Configure Music

1. In the `SceneMusic` component Inspector:
   - Drag an AudioClip to the "Scene Music" field
   - Adjust "Music Volume" (0.0 to 1.0, default: 0.7)
   - Configure fade settings:
     - **Fade In On Load**: Enable for smooth start
     - **Fade In Duration**: Time for fade in (0 = use SoundManager default)
     - **Fade Out On Unload**: Enable for smooth stop
     - **Fade Out Duration**: Time for fade out (0 = use SoundManager default)
   - **Stop Current Music**: Whether to stop previous scene's music

#### Step 3: Test

1. Play the scene
2. Music should automatically start playing when scene loads
3. When transitioning to another scene, music should fade out (if configured)

### Method 2: Manual Music Control via Code

You can also control music programmatically:

```csharp
SoundManager soundManager = SoundManager.GetInstance();

// Play music with fade in
soundManager.PlayMusic(musicClip, fadeIn: true, fadeDuration: 2f);

// Change to different music (with crossfade)
soundManager.ChangeMusic(newMusicClip, fadeDuration: 2f);

// Stop music (with fade out)
soundManager.StopMusic(fadeOut: true, fadeDuration: 1f);

// Set music volume
soundManager.SetMusicVolume(0.8f);

// Check if music is playing
bool isPlaying = soundManager.IsMusicPlaying();

// Get current music
AudioClip current = soundManager.GetCurrentMusic();
```

## Configuration Examples

### Example 1: Simple Scene Music

```
Scene Music: [Your music clip]
Music Volume: 0.7
Fade In On Load: true
Fade In Duration: 0 (use default)
Fade Out On Unload: true
Fade Out Duration: 0 (use default)
Stop Current Music: true
```

### Example 2: Immediate Start (No Fade)

```
Scene Music: [Your music clip]
Music Volume: 0.6
Fade In On Load: false
Fade In Duration: 0
Fade Out On Unload: false
Fade Out Duration: 0
Stop Current Music: true
```

### Example 3: Long Fade In

```
Scene Music: [Your music clip]
Music Volume: 0.8
Fade In On Load: true
Fade In Duration: 3.0 (3 seconds)
Fade Out On Unload: true
Fade Out Duration: 2.0 (2 seconds)
Stop Current Music: true
```

## SoundManager Music API

### PlayMusic()

Play background music with optional fade in.

```csharp
void PlayMusic(AudioClip musicClip, bool fadeIn = true, float fadeDuration = -1f)
```

**Parameters**:
- `musicClip`: Audio clip to play
- `fadeIn`: Whether to fade in (default: true)
- `fadeDuration`: Fade duration in seconds (-1 = use default)

**Example**:
```csharp
SoundManager.GetInstance().PlayMusic(myMusicClip, fadeIn: true, fadeDuration: 2f);
```

### StopMusic()

Stop currently playing music with optional fade out.

```csharp
void StopMusic(bool fadeOut = true, float fadeDuration = -1f)
```

**Parameters**:
- `fadeOut`: Whether to fade out (default: true)
- `fadeDuration`: Fade duration in seconds (-1 = use default)

**Example**:
```csharp
SoundManager.GetInstance().StopMusic(fadeOut: true, fadeDuration: 1.5f);
```

### ChangeMusic()

Smoothly transition from current music to new music (crossfade).

```csharp
void ChangeMusic(AudioClip newMusicClip, float fadeDuration = -1f)
```

**Parameters**:
- `newMusicClip`: New music clip to play
- `fadeDuration`: Total crossfade duration in seconds (-1 = use default)

**Example**:
```csharp
SoundManager.GetInstance().ChangeMusic(bossMusicClip, fadeDuration: 3f);
```

### Volume Control

```csharp
// Set music volume (0.0 to 1.0)
void SetMusicVolume(float volume)

// Get current music volume
float GetMusicVolume()
```

**Example**:
```csharp
SoundManager soundManager = SoundManager.GetInstance();
soundManager.SetMusicVolume(0.5f); // Set to 50%
float currentVolume = soundManager.GetMusicVolume();
```

### Status Queries

```csharp
// Check if music is playing
bool IsMusicPlaying()

// Get currently playing music clip
AudioClip GetCurrentMusic()
```

## Advanced Usage

### Dynamic Music Changes

You can change music based on game events:

```csharp
public class BossFightTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip bossMusic;
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Change to boss music when player enters boss area
            SoundManager.GetInstance().ChangeMusic(bossMusic, fadeDuration: 2f);
        }
    }
}
```

### Conditional Music Playback

```csharp
public class ConditionalMusic : MonoBehaviour
{
    [SerializeField] private AudioClip victoryMusic;
    
    public void OnVictory()
    {
        // Play victory music only if no music is currently playing
        SoundManager soundManager = SoundManager.GetInstance();
        if (!soundManager.IsMusicPlaying())
        {
            soundManager.PlayMusic(victoryMusic, fadeIn: true);
        }
    }
}
```

### Music Volume Adjustment

```csharp
// Gradually increase music volume
IEnumerator IncreaseMusicVolume(float targetVolume, float duration)
{
    SoundManager soundManager = SoundManager.GetInstance();
    float startVolume = soundManager.GetMusicVolume();
    float elapsedTime = 0f;
    
    while (elapsedTime < duration)
    {
        elapsedTime += Time.deltaTime;
        float progress = elapsedTime / duration;
        float currentVolume = Mathf.Lerp(startVolume, targetVolume, progress);
        soundManager.SetMusicVolume(currentVolume);
        yield return null;
    }
    
    soundManager.SetMusicVolume(targetVolume);
}
```

## Integration with Scene System

### Automatic Scene Music

When using `SceneMusic` component:
1. Music automatically plays when scene loads
2. Music automatically stops/fades when scene unloads
3. Smooth transitions between scenes

### Manual Scene Music Control

If you need more control, you can manually manage music in scene scripts:

```csharp
public class SceneMusicController : MonoBehaviour
{
    [SerializeField] private AudioClip sceneMusic;
    
    void Start()
    {
        SoundManager.GetInstance().PlayMusic(sceneMusic, fadeIn: true);
    }
    
    void OnDestroy()
    {
        SoundManager.GetInstance().StopMusic(fadeOut: true);
    }
}
```

## Best Practices

### Audio Clip Selection

1. **Format**: Use compressed formats (OGG Vorbis) for music to save space
2. **Length**: Music should be loopable (seamless loop points)
3. **Volume**: Normalize music files to consistent levels
4. **Sample Rate**: 44.1kHz is standard for music

### Volume Levels

- **Music Volume**: Typically 0.5-0.8 (should be lower than SFX)
- **Fade Duration**: 1-3 seconds for smooth transitions
- **Balance**: Music should not overpower sound effects

### Performance

- Music uses a single AudioSource (efficient)
- Fade coroutines are lightweight
- No performance impact from normal usage

## Troubleshooting

### Issue: Music not playing

**Possible Causes**:
1. AudioClip not assigned
   - **Solution**: Assign AudioClip to SceneMusic component or PlayMusic() call

2. SoundManager not initialized
   - **Solution**: SoundManager auto-creates, but check if it exists in scene

3. Music volume is 0
   - **Solution**: Check SoundManager music volume setting

### Issue: Music doesn't fade

**Possible Causes**:
1. Fade duration set to 0
   - **Solution**: Set fade duration > 0 or use default

2. Fade disabled
   - **Solution**: Enable "Fade In On Load" or "Fade Out On Unload"

### Issue: Music overlaps between scenes

**Solution**: 
- Enable "Stop Current Music" in SceneMusic component
- Or manually call StopMusic() before playing new music

### Issue: Music too loud/quiet

**Solution**: 
- Adjust "Music Volume" in SoundManager Inspector
- Or use SetMusicVolume() in code
- Check individual SceneMusic volume settings

## Testing Checklist

- [ ] Music plays when scene loads
- [ ] Music fades in smoothly (if enabled)
- [ ] Music loops correctly
- [ ] Music stops/fades when scene unloads
- [ ] Music transitions smoothly between scenes
- [ ] Volume is appropriate (not too loud/quiet)
- [ ] Music doesn't overlap with sound effects
- [ ] No errors in console

## Example Scenarios

### Scenario 1: Main Menu Music

1. Create empty GameObject in main menu scene
2. Add `SceneMusic` component
3. Assign main menu music clip
4. Set volume to 0.6
5. Enable fade in (2 seconds)
6. Music plays automatically when menu loads

### Scenario 2: Level Music with Boss Fight

1. Add `SceneMusic` to level scene with level music
2. Create trigger zone for boss area
3. In trigger script, call `ChangeMusic(bossMusic)` when player enters
4. Music smoothly transitions to boss music
5. When boss defeated, change back to level music

### Scenario 3: Dynamic Music Based on Player State

```csharp
public class DynamicMusicController : MonoBehaviour
{
    [SerializeField] private AudioClip normalMusic;
    [SerializeField] private AudioClip dangerMusic;
    [SerializeField] private PlayerController player;
    
    void Update()
    {
        SoundManager soundManager = SoundManager.GetInstance();
        
        // Change music based on player health or state
        if (player.IsInDanger() && soundManager.GetCurrentMusic() != dangerMusic)
        {
            soundManager.ChangeMusic(dangerMusic, fadeDuration: 1f);
        }
        else if (!player.IsInDanger() && soundManager.GetCurrentMusic() != normalMusic)
        {
            soundManager.ChangeMusic(normalMusic, fadeDuration: 1f);
        }
    }
}
```

## Future Enhancements

Potential improvements:
- Music layers (multiple tracks that can be mixed)
- Adaptive music based on gameplay intensity
- Music state machine for complex transitions
- Integration with save system for music preferences
- Music playlist system for variety

