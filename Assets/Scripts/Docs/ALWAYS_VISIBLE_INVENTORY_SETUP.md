# Always-Visible Inventory Setup Guide

## Overview

The inventory UI now displays permanently in the top-right corner of the screen, showing all possible items with filled/empty states. This provides constant visibility of collected items without needing to press Tab.

## Quick Setup (5 Minutes)

### Step 1: Open Inventory Setup Tool

1. In Unity menu: `Window → Inventory → Setup Tool`
2. The tool window will open

### Step 2: Auto-Setup Everything

In the Setup Tool window:

1. Click **`Auto Find Player`** button
2. Click **`Auto Find Canvas`** button
3. Click **`Add InventorySystem to Player`** button (if needed)
4. Click **`Create FloatingText Prefab`** button
5. Click **`Create InventorySlot Prefab`** button
6. Click **`Create Complete Backpack UI`** button (the big green one)

The tool will automatically:
- Create BackpackPanel positioned at top-right corner
- Set it to always visible
- Configure all references

### Step 3: Create Test Items

In the Setup Tool:
1. Click **`Create Test Red Key ItemData`** button
2. In Project window, find `Assets/ScriptableObjects/Items/Key_Red.asset`
3. In Inspector, set an Icon sprite for the key

### Step 4: Add Items to InventorySystem

1. Select your Player GameObject in Hierarchy
2. Find `InventorySystem` component in Inspector
3. Click **`Auto Find All ItemData`** button
   - This will find all ItemData assets and add them to the `All Game Items` list

### Step 5: Create Collectable Item in Scene

1. Right-click in Hierarchy → `Inventory → Create Collectable Item`
2. Select the newly created GameObject
3. In Inspector, assign:
   - `Item Data`: Drag the Red Key ItemData
   - `Quantity`: 1
4. Add a SpriteRenderer and assign a sprite
5. Position it in the scene where the player can reach it

### Step 6: Test!

1. Press Play
2. You should immediately see the Inventory UI in the top-right corner
3. The Red Key slot should be visible but empty/grayed out
4. Walk to the key and collect it
5. The slot should immediately light up showing you have the key
6. Stop and restart → The key should still be there, and the world item should be gone

## UI Layout

The backpack UI is positioned at the **top-right corner** with:

```
┌─────────────────────────┐
│      Inventory          │  ← Title
├─────────────────────────┤
│ [🔑] [  ] [  ] [  ]    │  ← Item slots (4 columns)
│ [  ] [  ] [  ] [  ]    │
│ [  ] [  ] [  ] [  ]    │
│ ...                     │
└─────────────────────────┘
  20px from edge
```

**Anchor**: Top-Right (1, 1)
**Size**: 400 x 600 pixels (adjustable)
**Offset**: -20px from corner

## Customization

### Adjust Position/Size

Select `BackpackPanel` in Hierarchy, then in RectTransform:
- **Anchors**: (1, 1) to (1, 1) keeps it at top-right
- **Pivot**: (1, 1) makes it expand from top-right
- **Anchored Position**: Adjust X and Y to move it
  - X: More negative = further left
  - Y: More negative = further down
- **Size Delta**: Adjust Width and Height

Examples:
```
Top-Right (default):   Anchored Position = (-20, -20)
Top-Left:              Anchors = (0,1), Position = (20, -20)
Bottom-Right:          Anchors = (1,0), Position = (-20, 20)
Center-Right:          Anchors = (1,0.5), Position = (-20, 0)
```

### Adjust Grid Layout

Select `SlotContainer` in Hierarchy, find `GridLayoutGroup`:
- **Cell Size**: Size of each slot (default 70 x 70)
- **Spacing**: Gap between slots (default 10 x 10)
- **Constraint Count**: Number of columns (default 4)

Examples:
```
Compact (4 columns):   Cell Size = 60x60, Spacing = 5x5, Count = 4
Wide (6 columns):      Cell Size = 50x50, Spacing = 8x8, Count = 6
Large icons (3 cols):  Cell Size = 90x90, Spacing = 15x15, Count = 3
```

### Change Background Opacity

Select `BackpackPanel`, find `Image` component:
- **Color**: Adjust alpha (A) value
  - 0.6 = 60% opaque (default, semi-transparent)
  - 0.8 = 80% opaque (more visible)
  - 0.3 = 30% opaque (more transparent)

### Disable Always-Visible (Return to Toggle Mode)

Select `BackpackUI` GameObject, find `BackpackUI` component:
- Uncheck `Always Visible`
- Now Tab key will toggle it on/off
- Panel will be hidden initially

## Features

### Always-Visible Mode

**Advantages**:
- Player can always see collected items
- No need to press Tab
- Game doesn't pause
- Immediate feedback when collecting items

**Perfect for**:
- Collection-focused games
- Games with few items (< 20)
- Quick item reference needed

### Toggle Mode (Optional)

Set `Always Visible = false` to enable:
- Press Tab or I to open/close
- Game pauses when open
- Cleaner screen when not needed

**Perfect for**:
- Games with many items
- Immersive gameplay where UI should be minimal
- Inventory management focus

## Item Slot Display

Each slot shows:
- **Icon**: Item sprite (from ItemData)
- **Quantity**: Number in bottom-right (if > 1 or max stack > 1)
- **Empty State**: Grayed out with semi-transparent overlay
- **Collected State**: Full color, no overlay
- **Can Use State**: Highlighted yellow border (when near usable object)

## Persistence

All collected items are automatically saved:
- When you collect an item → Saved immediately
- When you restart the game → Items are loaded
- World items you collected → Permanently disappear

No manual save required!

## Troubleshooting

### UI Not Showing

Check Console for:
```
BackpackUI: Start called
BackpackUI: Found player 'player'
BackpackUI: Found InventorySystem
BackpackUI: Creating slots for X items
```

If missing:
- `No items in allGameItems list!` → Add ItemData to InventorySystem
- `slotContainer not assigned!` → Re-run Setup Tool
- `PlayerController not found!` → Ensure Player has tag "Player"

### UI Position Wrong

1. Select `BackpackPanel` in Hierarchy
2. Check RectTransform values match:
   - Anchors: Min (1, 1), Max (1, 1)
   - Pivot: (1, 1)
   - Anchored Position: (-20, -20)

### Slots Not Showing Items

1. Select Player → InventorySystem
2. Check `All Game Items` has ItemData
3. Click `Auto Find All ItemData` if empty
4. Collect an item and check if slot updates

### Items Not Persisting

See `INVENTORY_DEBUG_GUIDE.md` for detailed diagnosis

## Tips

- **Minimize visual clutter**: Use semi-transparent background (alpha 0.4-0.6)
- **Scale for different resolutions**: Use Canvas Scaler with "Scale With Screen Size"
- **Compact layout**: 4 columns, 60x60 cells works well for top-right corner
- **Show only essential items**: Add only critical items to `allGameItems` list
- **Group by category**: Arrange ItemData in `allGameItems` list by type (Keys first, then Tools, etc.)

## Example Configuration

For a game with **8 collectible items** (displayed in top-right):

**BackpackPanel**:
- Size: 360 x 400
- Position: (-20, -20)
- Background: Black with 50% opacity

**SlotContainer Grid**:
- 4 columns x 2 rows
- Cell Size: 70 x 70
- Spacing: 8 x 8

**Result**: Compact, clean inventory display that doesn't obstruct gameplay!

