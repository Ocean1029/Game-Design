using UnityEngine;

/// <summary>
/// Represents a key item that can be collected
/// Keys are automatically added to player's KeyInventory when touched
/// </summary>
public class Key : MonoBehaviour
{
    [Header("Key Configuration")]
    [Tooltip("Unique tag for this key (e.g., 'key1', 'key2')")]
    [SerializeField] private string keyTag = "key1";
    
    [Tooltip("Display name shown when collected (e.g., 'Red Key', 'Blue Key')")]
    [SerializeField] private string keyName = "Key";
    
    [Header("Visual & Audio")]
    [Tooltip("Sound played when key is collected")]
    [SerializeField] private AudioClip collectSound;
    
    [Tooltip("Particle effect when key is collected")]
    [SerializeField] private GameObject collectEffect;

    private bool isCollected = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected) return;

        // Check if collider is player
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player == null) return;

        // Get KeyInventory component
        KeyInventory keyInventory = player.GetComponent<KeyInventory>();
        if (keyInventory == null)
        {
            Debug.LogWarning($"Key: KeyInventory not found on player! Cannot collect key '{keyTag}'");
            return;
        }

        // Collect the key
        CollectKey(keyInventory, collision.transform.position);
    }

    /// <summary>
    /// Collect this key and add it to player's inventory
    /// </summary>
    private void CollectKey(KeyInventory keyInventory, Vector3 playerPosition)
    {
        if (isCollected) return;
        isCollected = true;

        // Add to inventory
        keyInventory.AddKey(keyTag, keyName);

        // Show floating text
        FloatingTextManager floatingTextManager = FloatingTextManager.GetInstance();
        if (floatingTextManager != null)
        {
            floatingTextManager.ShowKeyCollected(keyName, transform.position);
        }
        else
        {
            Debug.Log($"Collected {keyName}");
        }

        // Play collect sound
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // Spawn collect effect
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // Destroy key object
        Destroy(gameObject);
    }

    /// <summary>
    /// Get the key's tag
    /// </summary>
    public string GetKeyTag()
    {
        return keyTag;
    }

    /// <summary>
    /// Get the key's display name
    /// </summary>
    public string GetKeyName()
    {
        return keyName;
    }
}

