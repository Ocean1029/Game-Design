# Inventory System Debug Guide

## Problem 1: Inventory UI Not Showing

### Step-by-Step Diagnosis

#### 1. Check if BackpackUI exists in scene

In Unity Hierarchy, look for:
- `BackpackUI` or `BackpackUI_Controller` GameObject
- It should be under your `Canvas`

**If missing**: Use the Inventory Setup Tool
1. `Window → Inventory → Setup Tool`
2. Follow the steps to create the complete Backpack UI

#### 2. Check BackpackUI Component References

Select the `BackpackUI` GameObject in Hierarchy, then in Inspector check:
- `Slot Container`: Should reference the `SlotContainer` GameObject
- `Slot Prefab`: Should reference `Assets/Prefabs/UI/InventorySlot.prefab`
- `Inventory Panel`: Should reference the `BackpackPanel` GameObject
- `Use Hint Text`: Should reference the text inside `UseHintPanel`
- `Use Hint Panel`: Should reference the `UseHintPanel` GameObject

**If any are missing**: Manually drag and assign them from Hierarchy

#### 3. Check if Player has InventorySystem

Select your Player GameObject, check for:
- `InventorySystem` component

**If missing**: 
- Add it manually: `Add Component → InventorySystem`
- Or use Setup Tool: `Add InventorySystem to Player` button

#### 4. Check if allGameItems list has items

In Player's `InventorySystem` component:
- Expand `All Game Items`
- Should show list of ItemData assets

**If empty**:
- Click `Auto Find All ItemData` button in the Inspector
- Or manually drag ItemData assets into the list

#### 5. Test keyboard input

Press `Tab` or `I` key in Play Mode

Check Console for messages:
```
BackpackUI: Start called
BackpackUI: Found player 'player'
BackpackUI: Found InventorySystem
BackpackUI: Creating slots for X items
BackpackUI: Created slot for 'Red Key'
BackpackUI: Created X slots total
```

**If you see**:
- `No items in allGameItems list!` → Go to step 4
- `slotContainer not assigned!` → Go to step 2
- `PlayerController not found!` → Check if Player has PlayerController component

#### 6. Check if slots are being created

When pressing `Tab`, check Console:
```
BackpackUI: Toggle inventory - isOpen = True
BackpackUI: Set inventoryPanel active = True
```

**If you see this but UI still not visible**:
- Check if `BackpackPanel` is active in Hierarchy (should be active when open)
- Check if Canvas is set to correct Render Mode
- Check if Canvas Scaler is properly configured

### Quick Fix Checklist

- [ ] BackpackUI GameObject exists in scene under Canvas
- [ ] BackpackUI has all references assigned (slotContainer, slotPrefab, inventoryPanel, etc.)
- [ ] Player has InventorySystem component
- [ ] InventorySystem → All Game Items has at least one ItemData
- [ ] InventorySlot prefab exists at `Assets/Prefabs/UI/InventorySlot.prefab`
- [ ] InventorySlot prefab has InventorySlotUI component
- [ ] Canvas exists and is properly configured

## Problem 2: Items Not Persisting Between Game Sessions

### How It Works Now

The system now automatically saves and loads collected items:

**When you collect an item**:
1. Item is added to inventory → `InventorySystem.AddItem()`
2. Inventory is saved → `SaveSystem.SaveInventory()`
3. World item is marked as collected → `SaveSystem.MarkItemCollected(collectableId)`
4. Item GameObject is destroyed

**When game starts**:
1. `InventorySystem.Start()` loads saved inventory → `LoadInventoryFromSave()`
2. `CollectableItem.Start()` checks if already collected → `SaveSystem.IsItemCollected()`
3. If collected, the item GameObject is immediately destroyed

### Testing Persistence

1. **Start game** → Inventory should be empty
2. **Collect a Red Key** → Should see "+ Red Key" floating text
3. **Press Tab** → Red Key slot should be filled/highlighted
4. **Stop game** (Stop Play Mode)
5. **Start game again** → Red Key should still be in inventory
6. **Go to where Red Key was** → Red Key GameObject should not be there (auto-destroyed)

### Verifying Save Data

#### Option 1: Check Console Logs

Look for these messages:
```
SaveSystem: Saved 1 items to inventory
SaveSystem: Marked world item 'key_red_MainScene_10.00_5.00' as permanently collected
```

On next game start:
```
InventorySystem: Loading 1 items from save
InventorySystem: Loaded 1x Red Key from save
CollectableItem: 'key_red_MainScene_10.00_5.00' was already collected, destroying
```

#### Option 2: Use GameManager Debug

In Play Mode, you can call from Console or Debug window:
```csharp
SaveSystem.GetDebugInfo()
```

Should return something like:
```
SaveSystem: Last spawn point = 'chair_001' in scene 'MainScene', Collected items = 1
```

### Clearing Save Data

If you need to reset everything for testing:

**Method 1**: Through GameManager
```csharp
GameManager.Instance.ClearSaveData();
```

**Method 2**: Directly
```csharp
SaveSystem.ClearSaveData();
```

**Method 3**: Unity Menu (if you add it to GameSystemTools)

### Common Issues

**Issue**: "Items not saving"
- Check Console for `SaveSystem: Saved X items` message
- Verify `PlayerPrefs` is working (some platforms have restrictions)

**Issue**: "Items respawn even after collection"
- Check if CollectableItem has unique `collectableId`
- Default auto-generates from: itemId + sceneName + position
- Can manually set in Inspector for consistency

**Issue**: "Loaded items not showing in UI"
- Ensure `allGameItems` contains all ItemData that might be saved
- Items load into inventory but UI needs ItemData reference to display

**Issue**: "Wrong items loaded"
- Check if ItemData `itemId` is unique across all items
- Two items with same ID will conflict

## Testing Workflow

### First Time Setup

1. Create ItemData for "Red Key"
   - Set `itemId = "key_red"`
   - Set name, description, icon

2. Add to Player
   - Player → InventorySystem → All Game Items
   - Click `Auto Find All ItemData` button

3. Create CollectableItem in scene
   - Right-click Hierarchy → Inventory → Create Collectable Item
   - Assign Red Key ItemData
   - Position in scene

4. Create/Check Backpack UI
   - Use Inventory Setup Tool if not exists
   - Verify all references

5. Test!

### Expected Behavior

**Session 1**:
1. Press Play
2. Walk to Red Key → Collect it
3. Press Tab → See Red Key in inventory
4. Press Stop

**Session 2**:
1. Press Play
2. Red Key already in inventory (check Tab)
3. Red Key GameObject doesn't exist in scene
4. You can use the key immediately

## Debug Commands

Useful for testing in Play Mode (add to a debug console or create buttons):

```csharp
// Check save data
Debug.Log(SaveSystem.GetDebugInfo());

// Clear everything
SaveSystem.ClearSaveData();

// Check if specific world item collected
bool collected = SaveSystem.IsItemCollected("key_red_MainScene_10.00_5.00");

// Get inventory count
InventorySystem inv = FindFirstObjectByType<InventorySystem>();
Debug.Log($"Inventory has {inv.GetAllItems().Count} items");
```

## Console Messages Reference

### Normal Startup (With Saved Data)
```
InventorySystem: Loading 2 items from save
InventorySystem: Loaded 1x Red Key from save
InventorySystem: Loaded 1x Blue Key from save
InventorySystem: Loaded 2 items total
CollectableItem: 'key_red_MainScene_10.00_5.00' was already collected, destroying
CollectableItem: 'key_blue_MainScene_15.00_3.00' was already collected, destroying
BackpackUI: Start called
BackpackUI: Found player 'player'
BackpackUI: Found InventorySystem
BackpackUI: Creating slots for 5 items
BackpackUI: Created slot for 'Red Key'
BackpackUI: Created slot for 'Blue Key'
...
BackpackUI: Created 5 slots total
```

### Collecting New Item
```
InventorySystem: Added 1x Red Key
SaveSystem: Saved 1 items to inventory
SaveSystem: Marked world item 'key_red_MainScene_10.00_5.00' as permanently collected
CollectableItem: Marked 'key_red_MainScene_10.00_5.00' as permanently collected
```

### Opening Inventory
```
BackpackUI: Toggle inventory - isOpen = True
BackpackUI: Set inventoryPanel active = True
```

## Tips

- **Always check Console** for error/warning messages
- **ItemData IDs must be unique** across all items
- **CollectableItem IDs auto-generate** from position (deterministic)
- **Manual IDs** can be set if you want consistent IDs across scenes
- **Save data persists** even in Editor Play Mode (stored in EditorPrefs)

