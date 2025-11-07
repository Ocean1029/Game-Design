using UnityEngine;

/// <summary>
/// Represents an item instance in the inventory
/// </summary>
[System.Serializable]
public class InventoryItem
{
    public ItemData itemData;
    public int quantity;
    
    public InventoryItem(ItemData data, int amount = 1)
    {
        itemData = data;
        quantity = amount;
    }
    
    /// <summary>
    /// Add to stack
    /// </summary>
    /// <returns>Amount that couldn't be added (overflow)</returns>
    public int AddQuantity(int amount)
    {
        int maxAdd = itemData.maxStackSize - quantity;
        int actualAdd = Mathf.Min(amount, maxAdd);
        quantity += actualAdd;
        return amount - actualAdd; // Return overflow
    }
    
    /// <summary>
    /// Remove from stack
    /// </summary>
    /// <returns>True if stack is now empty</returns>
    public bool RemoveQuantity(int amount)
    {
        quantity -= amount;
        return quantity <= 0;
    }
    
    /// <summary>
    /// Check if can add more to this stack
    /// </summary>
    public bool CanAddMore()
    {
        return quantity < itemData.maxStackSize;
    }
}

