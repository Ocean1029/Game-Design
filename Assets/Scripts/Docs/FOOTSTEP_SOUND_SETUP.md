# Player Footstep Sound Setup Guide

This guide explains how to set up and configure the player footstep sound system.

## Overview

The `PlayerFootstepSound` component automatically plays footstep sounds when the player is walking on the ground. It integrates with the existing sound system (`SoundManager`) and player movement system.

## Features

- **Automatic Playback**: Plays footstep sounds automatically when player is moving and grounded
- **Speed-Based Timing**: Adjusts footstep interval based on movement speed (optional)
- **Configurable Settings**: Customize volume, timing intervals, and movement thresholds
- **SoundManager Integration**: Uses SoundManager for unified audio management with fallback support

## Setup Instructions

### Step 1: Add Component to Player

1. Select the Player GameObject in the scene
2. In the Inspector, click "Add Component"
3. Search for "Player Footstep Sound" and add it

**Note**: The component requires `PlayerController`, `PlayerMovement`, and `PlayerStateMachine` components. These should already be on the player.

### Step 2: Configure Audio Clip

1. In the `PlayerFootstepSound` component, find the "Footstep Sound" field
2. Drag an AudioClip from your project into this field
3. The audio clip should be a short footstep sound effect

**Tip**: For best results, use a short, looping-friendly sound clip (0.1-0.3 seconds).

### Step 3: Adjust Settings (Optional)

The component has several configurable settings:

#### Basic Settings

- **Footstep Sound**: The audio clip to play for footsteps
- **Footstep Volume**: Volume level (0.0 to 1.0), default is 0.5

#### Timing Settings

- **Min Step Interval**: Minimum time between footsteps (default: 0.3 seconds)
- **Max Step Interval**: Maximum time between footsteps (default: 0.5 seconds)
- **Min Movement Speed**: Minimum horizontal speed to trigger footsteps (default: 0.1)

#### Advanced Settings

- **Adjust Interval By Speed**: If enabled, faster movement = more frequent footsteps
- **Speed Multiplier**: Multiplier for speed-based interval adjustment (default: 1.0)

### Step 4: Test

1. Play the game
2. Move the player left or right
3. Footstep sounds should play automatically when walking on the ground
4. Sounds should stop when player stops moving or jumps

## Configuration Examples

### Example 1: Standard Walking

```
Footstep Sound: [Your footstep clip]
Footstep Volume: 0.5
Min Step Interval: 0.3
Max Step Interval: 0.5
Min Movement Speed: 0.1
Adjust Interval By Speed: true
Speed Multiplier: 1.0
```

### Example 2: Faster, More Frequent Steps

```
Footstep Sound: [Your footstep clip]
Footstep Volume: 0.6
Min Step Interval: 0.2
Max Step Interval: 0.35
Min Movement Speed: 0.1
Adjust Interval By Speed: true
Speed Multiplier: 1.5
```

### Example 3: Slower, Less Frequent Steps

```
Footstep Sound: [Your footstep clip]
Footstep Volume: 0.4
Min Step Interval: 0.4
Max Step Interval: 0.6
Min Movement Speed: 0.15
Adjust Interval By Speed: false
Speed Multiplier: 1.0
```

## How It Works

### Detection Logic

The component checks every frame:
1. Is player in `Moving` state? (from PlayerStateMachine)
2. Is player grounded? (from PlayerMovement)
3. Is horizontal speed above minimum threshold?

If all conditions are met, it accumulates time and plays a footstep sound when the interval is reached.

### Speed-Based Timing

When "Adjust Interval By Speed" is enabled:
- Faster movement = shorter interval between steps
- Slower movement = longer interval between steps
- Creates more natural footstep rhythm

The interval is calculated as:
```
adjustedInterval = Lerp(maxInterval, minInterval, speedFactor * speedMultiplier)
```

### Sound Playback

Footstep sounds are played through `SoundManager`:
- If SoundManager exists: Uses `SoundManager.PlaySound()`
- If SoundManager doesn't exist: Falls back to `AudioSource.PlayClipAtPoint()`

This ensures backward compatibility and unified audio management.

## Troubleshooting

### Issue: No footstep sounds playing

**Possible Causes**:
1. AudioClip not assigned
   - **Solution**: Assign an AudioClip to the "Footstep Sound" field

2. Player not in Moving state
   - **Solution**: Check that PlayerStateMachine is working correctly
   - Verify player can move and state changes to Moving

3. Player not grounded
   - **Solution**: Check PlayerMovement ground detection
   - Verify groundCheck Transform is set correctly

4. Movement speed too low
   - **Solution**: Lower the "Min Movement Speed" value

### Issue: Footsteps too frequent/infrequent

**Solution**: Adjust the step interval settings:
- For more frequent: Decrease Min/Max Step Interval
- For less frequent: Increase Min/Max Step Interval

### Issue: Footsteps don't match movement speed

**Solution**: 
- Enable "Adjust Interval By Speed"
- Adjust "Speed Multiplier" to fine-tune the relationship

### Issue: Footsteps too loud/quiet

**Solution**: Adjust the "Footstep Volume" slider (0.0 to 1.0)

## Advanced Usage

### Runtime Configuration

You can modify footstep settings at runtime:

```csharp
PlayerFootstepSound footstep = player.GetComponent<PlayerFootstepSound>();

// Change footstep sound
footstep.SetFootstepSound(newClip);

// Change volume
footstep.SetFootstepVolume(0.7f);

// Enable/disable footsteps
footstep.SetFootstepEnabled(false);
```

### Different Sounds for Different Surfaces

For future expansion, you could:
1. Detect ground surface type (using tags or layers)
2. Switch footstep sound based on surface
3. Use different timing for different surfaces

Example implementation:
```csharp
// In PlayerFootstepSound, add surface detection
private AudioClip GetFootstepSoundForSurface()
{
    // Detect surface type (e.g., using raycast)
    // Return appropriate sound clip
    return footstepSound; // Default for now
}
```

## Integration with SoundManager

The footstep sound system integrates with the existing SoundManager:
- All footstep sounds go through SoundManager
- Future volume controls will affect footstep sounds
- Consistent audio management across the game

## Best Practices

1. **Audio Clip Selection**:
   - Use short, non-looping clips (0.1-0.3 seconds)
   - Ensure clips are normalized (consistent volume)
   - Consider using variations for more natural sound

2. **Timing Settings**:
   - Start with default values
   - Adjust based on player movement speed
   - Test with actual gameplay

3. **Volume Settings**:
   - Keep volume moderate (0.4-0.6) to avoid overpowering other sounds
   - Consider player's position in audio mix

4. **Performance**:
   - The component is lightweight and efficient
   - No performance impact from normal usage

## Testing Checklist

- [ ] Component added to player
- [ ] AudioClip assigned
- [ ] Footsteps play when walking
- [ ] Footsteps stop when idle
- [ ] Footsteps stop when jumping
- [ ] Footsteps resume when landing and walking
- [ ] Volume is appropriate
- [ ] Timing feels natural
- [ ] No errors in console

## Future Enhancements

Potential improvements for the future:
- Different sounds for different surface types (grass, stone, metal, etc.)
- Left/right foot alternation
- Sound variations for more natural feel
- Integration with animation system for precise timing
- Volume adjustment based on movement speed

