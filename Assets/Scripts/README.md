# Scripts Folder Structure

This document explains the organization of scripts in this project.

## Folder Structure

### 📁 Player/
Contains all scripts related to player character control and systems.

**Scripts:**
- `PlayerController.cs` - Main player controller that coordinates all player subsystems
- `PlayerMovement.cs` - Handles player physics, walking, and jumping
- `PlayerState.cs` - Enumeration of all possible player states
- `PlayerStateMachine.cs` - Manages player state transitions and input locking
- `PlayerAnimationController.cs` - Controls player animation parameters
- `PlayerEnergy.cs` - Manages energy system for jump limitations

**Purpose:** These scripts work together to create a complete player control system. The PlayerController acts as the central coordinator, using other components for specific tasks.

---

### 📁 Interaction/
Contains all scripts related to player interactions with world objects.

**Scripts:**
- `IInteractable.cs` - Interface that all interactable objects must implement
- `InteractionHandler.cs` - Manages player interactions and item carrying
- `chair.cs` - Chair object that player can sit on to restore energy
- `door.cs` - Door object that requires a key to open
- `cable.cs` - Rappelling cable for descending to lower areas

**Purpose:** Implements a unified interaction system using the IInteractable interface. All interactive objects follow the same pattern for consistency.

---

### 📁 Camera/
Contains scripts related to camera control.

**Scripts:**
- `CameraFollow.cs` - Makes camera smoothly follow the player with configurable settings

**Purpose:** Handles all camera behavior including following the player, boundaries, and smooth movement.

---

### 📁 UI/
Contains scripts related to user interface.

**Scripts:**
- `EnergyUIDisplay.cs` - Displays player energy as colored blocks in the UI

**Purpose:** Manages all UI elements and their updates based on game state.

---

### 📁 Manager/
Contains game-level management scripts.

**Scripts:**
- `GameManager.cs` - Singleton that manages game-wide systems and scene persistence
- `SceneTransition.cs` - Triggers scene transitions when player enters trigger zones

**Purpose:** Handles high-level game management, scene transitions, and persistent objects across scenes.

---

### 📁 Tools/
Contains development and debug tools.

**Scripts:**
- `JumpMeasureTool.cs` - Debug tool for measuring jump height and distance

**Purpose:** Development utilities that help with game tuning. These scripts can be disabled or removed in production builds.

---

## Adding New Scripts

When adding new scripts to the project, please follow these guidelines:

1. **Identify the Category**: Determine which category your script belongs to
2. **Place in Appropriate Folder**: Add the script to the corresponding folder
3. **Follow Naming Conventions**: Use PascalCase for class names
4. **Add Documentation**: Include XML documentation comments (///) for public methods
5. **Update This README**: If you create a new category, document it here

## Dependencies Between Categories

```
Player ←→ Interaction (bidirectional communication)
Player ← UI (UI reads from player systems)
Player ← Camera (camera follows player)
Manager → All (managers can access any system)
Tools → Player (tools measure player behavior)
```

---

**Last Updated:** October 12, 2025
**Organization Completed By:** AI Assistant

