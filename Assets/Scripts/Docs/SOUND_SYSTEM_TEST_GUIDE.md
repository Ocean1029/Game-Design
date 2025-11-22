# Sound System Test Guide

This document provides comprehensive testing procedures for the sound system implementation.

## Test Overview

The sound system consists of three main components:
1. **SoundManager** - Central sound playback manager (Singleton)
2. **InteractionSoundData** - ScriptableObject for sound configuration
3. **InteractableSoundComponent** - Optional component for interactable objects
4. **InteractionHandler Integration** - Event-driven sound playback

## Automated Testing

### Using SoundSystemTester

The `SoundSystemTester` script provides automated testing capabilities.

#### Setup

1. Create an empty GameObject in your test scene
2. Add the `SoundSystemTester` component to it
3. (Optional) Assign a test AudioClip to verify sound playback

#### Running Tests

**Method 1: Automatic Testing**
- Set `Run Tests On Start` to true in the Inspector
- Tests will run automatically when the scene starts

**Method 2: Manual Testing via Context Menu**
- Right-click on the `SoundSystemTester` component in Inspector
- Select "Run All Tests" from the context menu

**Method 3: Code Execution**
- Call `RunAllTests()` method from code

#### Test Coverage

The automated tests cover:

1. **SoundManager Initialization**
   - Verifies SoundManager can be instantiated
   - Checks GameObject creation

2. **Singleton Pattern**
   - Ensures only one instance exists
   - Verifies GetInstance() returns the same instance

3. **Sound Playback**
   - Tests PlaySound() method
   - Tests PlaySound2D() method
   - Verifies null clip handling

4. **Null Handling**
   - Tests graceful handling of null AudioClips
   - Tests graceful handling of null InteractionSoundData

5. **InteractionSoundData**
   - Tests ScriptableObject creation
   - Tests property access
   - Tests method calls with missing sounds

6. **InteractableSoundComponent**
   - Tests component creation
   - Tests methods with missing sound data
   - Verifies optional nature of component

7. **Integration**
   - Tests InteractionHandler presence
   - Verifies system integration

## Manual Testing Scenarios

### Scenario 1: Basic SoundManager Functionality

**Objective**: Verify SoundManager can play sounds

**Steps**:
1. Ensure SoundManager exists in scene (it will auto-create if missing)
2. In code or via tester, call:
   ```csharp
   SoundManager.GetInstance().PlaySound2D(testClip, 1f);
   ```
3. Verify sound plays

**Expected Result**: Sound plays without errors

---

### Scenario 2: Interaction Without Sound Configuration

**Objective**: Verify system works when interactable has no sound

**Steps**:
1. Create a GameObject with an `IInteractable` implementation
2. Do NOT add `InteractableSoundComponent`
3. Register it with InteractionHandler
4. Attempt interaction

**Expected Result**: 
- Interaction works normally
- No errors in console
- No sound plays (expected behavior)

---

### Scenario 3: Interaction With Sound Configuration

**Objective**: Verify sound plays when configured

**Steps**:
1. Create an InteractionSoundData asset:
   - Right-click in Project → Create → Game → Audio → Interaction Sound Data
   - Assign audio clips to desired sound types

2. Create a test interactable:
   - Create GameObject with `IInteractable` implementation
   - Add `InteractableSoundComponent`
   - Assign the InteractionSoundData to the component

3. Test interaction:
   - Register interactable with InteractionHandler
   - Call `TryInteract()`
   - Verify sound plays

**Expected Result**: 
- Interaction succeeds
- Appropriate sound plays (Success or Failure based on result)

---

### Scenario 4: Multiple Sound Types

**Objective**: Verify different sound types work correctly

**Steps**:
1. Create InteractionSoundData with multiple sound types:
   - Success Sound
   - Failure Sound
   - Enter Zone Sound
   - Exit Zone Sound

2. Create test interactable with this data

3. Test each interaction type:
   - Enter zone → Enter Zone Sound
   - Exit zone → Exit Zone Sound
   - Successful interaction → Success Sound
   - Failed interaction → Failure Sound

**Expected Result**: Each sound type plays at appropriate time

---

### Scenario 5: 2D vs 3D Sound

**Objective**: Verify 2D and 3D sound playback

**Steps**:
1. Create two interactables with same InteractionSoundData
2. Set one's `InteractableSoundComponent.playAs2D = true`
3. Set other's `playAs2D = false`
4. Test interactions with both

**Expected Result**:
- 2D sound: Plays at same volume regardless of distance
- 3D sound: Volume changes with distance (spatial audio)

---

### Scenario 6: Edge Cases

**Objective**: Verify system handles edge cases gracefully

**Test Cases**:

1. **Null AudioClip in InteractionSoundData**
   - Create InteractionSoundData with null clips
   - Attempt interaction
   - **Expected**: No error, no sound plays

2. **SoundManager Missing**
   - Temporarily remove SoundManager from scene
   - Attempt interaction
   - **Expected**: No error, system continues without sound

3. **Multiple InteractionHandlers**
   - Verify only one SoundManager instance exists
   - **Expected**: Singleton pattern prevents duplicates

4. **Volume Settings**
   - Test with volume = 0
   - Test with volume = 1
   - Test with volume > 1 (should be clamped)
   - **Expected**: All handled correctly

---

## Integration Testing

### Test with Existing Interactables

Test the sound system with existing game objects:

1. **Chair (chair.cs)**
   - Add InteractableSoundComponent
   - Configure InteractionSoundData
   - Test sitting interaction

2. **Door (door.cs / PhysicalDoor.cs)**
   - Add InteractableSoundComponent
   - Configure InteractionSoundData
   - Test door opening/closing

3. **Key (Key.cs)**
   - Now integrated with SoundManager
   - Uses PlayCollectSound() method with SoundManager fallback
   - Test key collection to verify sound plays through SoundManager

4. **CollectableItem (CollectableItem.cs)**
   - Now integrated with SoundManager
   - Uses PlayCollectSound() method with SoundManager fallback
   - Test item collection to verify sound plays through SoundManager

---

## Performance Testing

### Test Scenarios

1. **Rapid Interactions**
   - Perform many interactions quickly
   - Verify no audio source leaks
   - Check memory usage

2. **Multiple Simultaneous Sounds**
   - Trigger multiple interactions at once
   - Verify all sounds play correctly
   - Check for audio clipping

3. **Scene Transitions**
   - Test sound system across scene changes
   - Verify SoundManager persists (DontDestroyOnLoad)
   - Check for memory leaks

---

## Debugging Tips

### Enable Debug Output

1. **SoundManager Debug**
   - Set `showDebugInfo = true` in SoundManager Inspector
   - All sound playback will log to console

2. **InteractionHandler Debug**
   - Check console for interaction events
   - Verify PlayInteractionSound() is called

### Common Issues

**Issue**: Sounds not playing
- Check AudioClip is assigned
- Verify SoundManager exists
- Check volume settings
- Verify InteractableSoundComponent is attached

**Issue**: Item pickup sounds not playing
- Verify Key.cs or CollectableItem.cs is using PlayCollectSound() method
- Check that SoundManager exists (will fallback to AudioSource.PlayClipAtPoint if not)
- Verify collectSound AudioClip is assigned in Inspector

**Issue**: Multiple SoundManagers
- Check singleton pattern
- Ensure only one instance in scene

**Issue**: Sounds play but too quiet/loud
- Adjust volume in InteractionSoundData
- Check master/SFX volume (when implemented)

---

## Test Checklist

Use this checklist to verify system functionality:

- [ ] SoundManager initializes correctly
- [ ] Singleton pattern works (only one instance)
- [ ] PlaySound() works with valid clip
- [ ] PlaySound2D() works with valid clip
- [ ] Null clip handling works (no errors)
- [ ] InteractionSoundData can be created
- [ ] InteractableSoundComponent is optional (works without it)
- [ ] Interaction without sound config works
- [ ] Interaction with sound config plays sound
- [ ] Success sound plays on successful interaction
- [ ] Failure sound plays on failed interaction
- [ ] Enter zone sound plays (if configured)
- [ ] Exit zone sound plays (if configured)
- [ ] 2D sound playback works
- [ ] 3D sound playback works
- [ ] Volume settings are respected
- [ ] System handles missing SoundManager gracefully
- [ ] No errors in console during normal operation

---

## Test Results Template

When running tests, document results:

```
Test Date: [Date]
Tester: [Name]
Unity Version: [Version]

Test Results:
- SoundManager Initialization: [PASS/FAIL]
- Singleton Pattern: [PASS/FAIL]
- Sound Playback: [PASS/FAIL]
- Null Handling: [PASS/FAIL]
- Integration: [PASS/FAIL]

Issues Found:
[List any issues]

Notes:
[Additional observations]
```

---

## Next Steps

After completing tests:

1. If all tests pass: Proceed to Plan 3 (Item Pickup Sound Integration)
2. If issues found: Document and fix before proceeding
3. If performance issues: Optimize before production use

