# Chair Save System Guide

## Overview
Chairs now function as save points! When the player sits on a chair, progress is automatically saved. The system includes fast travel between discovered chairs.

---

## Key Features

### 🪑 Auto-Save on Sit
- Player sits on a chair → Progress automatically saved
- That chair becomes the current respawn point
- Visual indicator shows which chair is the current save point

### ⚡ Quick Respawn (R Key)
- Press **R** to instantly return to the last chair you sat on
- Works from anywhere in the game
- Useful for recovering from falls or mistakes

### 🗺️ Fast Travel Menu (M Key)
- Press **M** to open the chair list
- Shows all chairs you've discovered
- Click any chair to teleport there instantly
- Works across different scenes!

---

## Setup Instructions

### 1. Setting Up a Chair

Open any existing chair GameObject in your scene and configure:

#### Required Settings
- **Chair ID**: Unique identifier (e.g., `"forest_chair_01"`, `"castle_entrance_chair"`)
- **Chair Name**: Display name shown in fast travel menu (e.g., `"Forest Resting Spot"`)
- **Sitpoint**: Transform where player sits (already configured)

#### Optional Settings
- **Location Description**: Brief description of the area
- **Active Spawn Visual**: GameObject to show when this is the current save point
- **Sit Sound**: Audio clip played when sitting (saving)

Example configuration:
```
Chair GameObject
├── Chair Script:
│   ├── chairId: "village_square_chair"
│   ├── chairName: "Village Square"
│   ├── locationDescription: "The central meeting place"
│   └── sitpoint: [Transform]
├── ActiveSpawnIndicator (child GameObject)
│   └── Particle System or Glow effect
```

### 2. Setting Up Fast Travel UI

#### Create the UI Structure

Create a new Canvas or use your existing one:

```
Canvas
└── FastTravelMenu (Panel)
    ├── Title (Text): "Fast Travel - Discovered Chairs"
    ├── ChairListContainer (Vertical Layout Group)
    │   └── [Chair buttons will spawn here]
    ├── NoChairsText (Text): "No chairs discovered yet..."
    └── CloseButton (Button): "Close (ESC)"
```

#### Add FastTravelUI Component

1. Create an empty GameObject named "FastTravelSystem"
2. Add the `FastTravelUI` component
3. Configure in Inspector:
   - **Menu Panel**: The FastTravelMenu panel
   - **Button Container**: The ChairListContainer transform
   - **Chair Button Prefab**: Create a button prefab (see below)
   - **No Chairs Text**: Reference to the NoChairsText
   - **Title Text**: Reference to the Title text

#### Create Chair Button Prefab

Create a button prefab in your Prefabs folder:

```
ChairButton (Prefab)
├── Button Component
└── Text (child): "Chair Name Here"
```

Button should have:
- Width: 300-400
- Height: 60-80
- Text component with appropriate font size

---

## Player Controls

### Default Keybindings

| Key | Action |
|-----|--------|
| **U** | Sit on chair (when near) |
| **D** | Stand up from chair |
| **R** | Respawn at last chair |
| **M** | Open/Close fast travel menu |
| **ESC** | Close fast travel menu |

These can be customized in PlayerController Inspector:
- `respawnKey` - Default: R
- `fastTravelMenuKey` - Default: M

---

## How It Works

### Sitting Flow

1. Player approaches a chair
2. "Press U to sit" prompt appears
3. Player presses U
4. **→ Player sits down**
5. **→ Progress automatically saved**
6. **→ Chair registered as spawn point**
7. **→ Visual indicator activated (if configured)**
8. **→ Sound plays (if configured)**

### Fast Travel Flow

1. Player presses M
2. Fast travel menu opens
3. Menu shows all discovered chairs
4. Current chair is highlighted and disabled
5. Player clicks a chair button
6. **→ If same scene**: Player teleports instantly
7. **→ If different scene**: Scene loads, player spawns at chair

### Respawn Flow

1. Player presses R (or dies)
2. System checks current spawn point
3. **→ Player teleports to last chair position**
4. **→ If sitting, stands up first**

---

## Code Examples

### Getting Current Save Point

```csharp
GameManager gm = GameManager.GetInstance();
SpawnPointData currentChair = gm.GetCurrentSpawnPoint();

if (currentChair != null)
{
    Debug.Log($"Current save: {currentChair.displayName}");
    Debug.Log($"Location: {currentChair.sceneName}");
}
```

### Checking Discovered Chairs

```csharp
List<SpawnPointData> chairs = GameManager.GetInstance().GetDiscoveredSpawnPoints();
Debug.Log($"Discovered {chairs.Count} chairs");

foreach (var chair in chairs)
{
    Debug.Log($"- {chair.displayName} in {chair.sceneName}");
}
```

### Manually Triggering Save

```csharp
// From another script, force a chair to save
chair chairComponent = chairObject.GetComponent<chair>();
if (chairComponent != null)
{
    // This will happen automatically when player sits,
    // but you can access chair properties like:
    string chairName = chairComponent.GetChairName();
    bool isCurrent = chairComponent.IsCurrentSpawnPoint();
}
```

### Teleporting to Specific Chair

```csharp
// Teleport player to a specific chair by ID
GameManager.GetInstance().TeleportToSpawnPoint("village_square_chair");
```

---

## Best Practices

### Chair Naming
Use descriptive, memorable names:
- ✅ Good: `"Forest Entrance"`, `"Castle Balcony"`, `"Hidden Cave Rest"`
- ❌ Bad: `"Chair1"`, `"Spawn02"`, `"CP_A"`

### Chair ID Naming
Use consistent, unique identifiers:
- Format: `location_type_number`
- Examples: `"forest_entrance_chair"`, `"castle_balcony_chair_01"`

### Visual Feedback
Add visual indicators to chairs:
- **Inactive**: Normal chair appearance
- **Discovered**: Subtle glow or particle effect
- **Current Save**: Bright glow or special effect

### Sound Design
Recommended audio:
- **Sit Sound**: Soft, calming sound (chair creak, cushion compress)
- **Save Sound**: Satisfying confirmation sound (bell chime, soft ding)

### Strategic Placement
Place chairs at:
- Level entrances
- Before difficult sections
- After completing major areas
- Near important NPCs or shops
- Safe zones

---

## Troubleshooting

### "No chairs discovered yet"
- Make sure you've sat on at least one chair
- Check that chair has unique `chairId` set
- Verify GameManager exists in scene

### Fast travel menu doesn't open
- Check that FastTravelUI component exists in scene
- Verify menuPanel is assigned in Inspector
- Make sure M key isn't being used by other scripts

### Can't teleport to chair
- Verify chair ID matches between chair and spawn point
- Check that chair has been sat on (discovered)
- Ensure target scene name is correct

### Chair visual indicator not updating
- Check that `activeSpawnVisual` GameObject is assigned
- Verify the Update() method is running (not disabled)
- Make sure GameManager is accessible

### Respawn (R key) doesn't work
- Sit on a chair first to set spawn point
- Check that GameManager exists
- Verify respawnKey setting in PlayerController

---

## Advanced Usage

### Death System Integration

```csharp
public class PlayerHealth : MonoBehaviour
{
    void OnDeath()
    {
        StartCoroutine(RespawnAfterDelay(2f));
    }
    
    IEnumerator RespawnAfterDelay(float delay)
    {
        // Show death screen
        yield return new WaitForSeconds(delay);
        
        // Respawn at last chair
        GameManager.GetInstance().RespawnPlayer();
        
        // Reset health
        ResetHealth();
    }
}
```

### Save Game Data with Chairs

```csharp
// When player sits on chair, also save game data
public class SaveSystem : MonoBehaviour
{
    void OnChairSit(string chairId)
    {
        // Save spawn point
        GameManager.GetInstance().SetCurrentSpawnPoint(chairId, position, scene);
        
        // Save other game data
        SavePlayerStats();
        SaveInventory();
        SaveQuestProgress();
    }
}
```

### Custom Fast Travel Costs

```csharp
// Modify FastTravelUI to require currency
private void OnChairButtonClicked(string chairId)
{
    int travelCost = 100; // gold cost
    
    if (PlayerInventory.GetGold() >= travelCost)
    {
        PlayerInventory.RemoveGold(travelCost);
        GameManager.GetInstance().TeleportToSpawnPoint(chairId);
        CloseMenu();
    }
    else
    {
        ShowMessage("Not enough gold!");
    }
}
```

---

## Files Modified/Created

### Modified
- `Assets/Scripts/Interaction/chair.cs` - Added save point functionality
- `Assets/Scripts/Player/PlayerController.cs` - Added R and M key handling
- `Assets/Scripts/Manager/GameManager.cs` - Spawn point system (from previous)

### Created
- `Assets/Scripts/UI/FastTravelUI.cs` - Fast travel menu manager
- `Assets/Scripts/Manager/SpawnPoint.cs` - Generic spawn point component (optional)

---

## Quick Reference

### Chair Script Properties
```csharp
chair.GetChairId()          // Get unique ID
chair.GetChairName()        // Get display name
chair.IsCurrentSpawnPoint() // Check if current save
```

### GameManager Methods
```csharp
SetCurrentSpawnPoint(id, pos, scene)  // Set save point
RespawnPlayer()                        // Respawn at save
TeleportToSpawnPoint(id)              // Fast travel
GetDiscoveredSpawnPoints()            // Get chair list
GetCurrentSpawnPoint()                // Get current save
```

### FastTravelUI Methods
```csharp
OpenMenu()    // Show fast travel menu
CloseMenu()   // Hide fast travel menu
ToggleMenu()  // Toggle menu visibility
IsMenuOpen()  // Check if menu is open
```

---

**System Complete!** ✅  
Your chairs are now fully functional save points with fast travel!

**Created:** October 12, 2025  
**System Type:** Chair + Save Point + Fast Travel Integration

