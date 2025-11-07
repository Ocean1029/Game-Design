using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages player's key inventory (non-physical items)
/// Keys are collected and displayed in UI
/// </summary>
public class KeyInventory : MonoBehaviour
{
    [Header("Key Collection")]
    [Tooltip("List of key tags that the player has collected")]
    private HashSet<string> collectedKeys = new HashSet<string>();

    // Events
    public delegate void KeyCollectedHandler(string keyTag, string keyName);
    public event KeyCollectedHandler OnKeyCollected;
    
    public delegate void KeyConsumedHandler(string keyTag);
    public event KeyConsumedHandler OnKeyConsumed;

    /// <summary>
    /// Add a key to the inventory
    /// </summary>
    /// <param name="keyTag">Tag of the key (e.g., "key1")</param>
    /// <param name="keyName">Display name of the key (e.g., "紅色鑰匙")</param>
    public void AddKey(string keyTag, string keyName = "")
    {
        if (string.IsNullOrEmpty(keyTag))
        {
            Debug.LogWarning("KeyInventory: Cannot add key with empty tag");
            return;
        }

        if (collectedKeys.Contains(keyTag))
        {
            Debug.Log($"KeyInventory: Already have key '{keyTag}'");
            return;
        }

        collectedKeys.Add(keyTag);
        
        // Use key name if provided, otherwise use tag
        string displayName = string.IsNullOrEmpty(keyName) ? keyTag : keyName;
        
        Debug.Log($"KeyInventory: Collected key '{keyTag}' - {displayName}");
        
        // Notify listeners
        OnKeyCollected?.Invoke(keyTag, displayName);
    }

    /// <summary>
    /// Check if player has a specific key
    /// </summary>
    /// <param name="keyTag">Tag of the key to check</param>
    /// <returns>True if player has the key</returns>
    public bool HasKey(string keyTag)
    {
        return collectedKeys.Contains(keyTag);
    }

    /// <summary>
    /// Remove a key from inventory (used when consuming a key)
    /// </summary>
    /// <param name="keyTag">Tag of the key to remove</param>
    public void RemoveKey(string keyTag)
    {
        if (collectedKeys.Contains(keyTag))
        {
            collectedKeys.Remove(keyTag);
            Debug.Log($"KeyInventory: Removed key '{keyTag}'");
            
            // Notify listeners that key was consumed
            OnKeyConsumed?.Invoke(keyTag);
        }
    }

    /// <summary>
    /// Get all collected keys
    /// </summary>
    /// <returns>Set of key tags</returns>
    public HashSet<string> GetAllKeys()
    {
        return new HashSet<string>(collectedKeys);
    }

    /// <summary>
    /// Get the number of keys collected
    /// </summary>
    /// <returns>Key count</returns>
    public int GetKeyCount()
    {
        return collectedKeys.Count;
    }

    /// <summary>
    /// Clear all keys (for testing or game reset)
    /// </summary>
    public void ClearAllKeys()
    {
        collectedKeys.Clear();
        Debug.Log("KeyInventory: All keys cleared");
    }
}

