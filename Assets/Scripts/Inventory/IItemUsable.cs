using UnityEngine;

/// <summary>
/// Interface for objects that can be used with items
/// Implement this on doors, crafting stations, etc.
/// </summary>
public interface IItemUsable
{
    /// <summary>
    /// Use an item on this object
    /// </summary>
    /// <param name="item">Item being used</param>
    /// <param name="inventory">Reference to player's inventory</param>
    /// <returns>True if item was successfully used</returns>
    bool UseItem(ItemData item, InventorySystem inventory);
    
    /// <summary>
    /// Get the tag that identifies what items can be used here
    /// </summary>
    string GetUsableTag();
}

