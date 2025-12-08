using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages player's bomb inventory (collectible and usable bombs)
/// Bombs are collected and displayed in UI, and can be consumed when used
/// </summary>
public class BombInventory : MonoBehaviour
{
    [Header("Bomb Collection")]
    [Tooltip("List of bomb tags that the player has collected")]
    private HashSet<string> collectedBombs = new HashSet<string>();

    // Events
    public delegate void BombCollectedHandler(string bombTag, string bombName);
    public event BombCollectedHandler OnBombCollected;
    
    public delegate void BombConsumedHandler(string bombTag);
    public event BombConsumedHandler OnBombConsumed;

    /// <summary>
    /// Add a bomb to the inventory
    /// </summary>
    /// <param name="bombTag">Tag of the bomb (e.g., "bomb1")</param>
    /// <param name="bombName">Display name of the bomb (e.g., "火焰炸彈")</param>
    public void AddBomb(string bombTag, string bombName = "")
    {
        if (string.IsNullOrEmpty(bombTag))
        {
            Debug.LogWarning("BombInventory: Cannot add bomb with empty tag");
            return;
        }

        if (collectedBombs.Contains(bombTag))
        {
            Debug.Log($"BombInventory: Already have bomb '{bombTag}'");
            return;
        }

        collectedBombs.Add(bombTag);

        // Use bomb name if provided, otherwise use tag
        string displayName = string.IsNullOrEmpty(bombName) ? bombTag : bombName;

        Debug.Log($"BombInventory: Collected bomb '{bombTag}' - {displayName}");

        // Notify listeners (for UI updates or sound effects)
        OnBombCollected?.Invoke(bombTag, displayName);
    }

    /// <summary>
    /// Check if player has a specific bomb
    /// </summary>
    /// <param name="bombTag">Tag of the bomb to check</param>
    /// <returns>True if player has the bomb</returns>
    public bool HasBomb(string bombTag)
    {
        return collectedBombs.Contains(bombTag);
    }

    /// <summary>
    /// Remove a bomb from inventory (used when consuming a bomb)
    /// </summary>
    /// <param name="bombTag">Tag of the bomb to remove</param>
    public void RemoveBomb(string bombTag)
    {
        if (collectedBombs.Contains(bombTag))
        {
            collectedBombs.Remove(bombTag);
            Debug.Log($"BombInventory: Removed bomb '{bombTag}'");

            // Notify listeners (for UI updates)
            OnBombConsumed?.Invoke(bombTag);
        }
    }

    /// <summary>
    /// Get all collected bombs
    /// </summary>
    /// <returns>Set of bomb tags</returns>
    public HashSet<string> GetAllBombs()
    {
        return new HashSet<string>(collectedBombs);
    }

    /// <summary>
    /// Get the number of bombs collected
    /// </summary>
    /// <returns>Bomb count</returns>
    public int GetBombCount()
    {
        return collectedBombs.Count;
    }

    /// <summary>
    /// Clear all bombs (for testing or game reset)
    /// </summary>
    public void ClearAllBombs()
    {
        collectedBombs.Clear();
        Debug.Log("BombInventory: All bombs cleared");
    }
}
