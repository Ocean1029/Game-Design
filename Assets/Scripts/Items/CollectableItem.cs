using UnityEngine;

/// <summary>
/// Represents a collectable item in the world
/// Automatically adds to player's inventory when touched
/// </summary>
public class CollectableItem : MonoBehaviour
{
    [Header("Item Configuration")]
    [Tooltip("Item data defining this collectable")]
    [SerializeField] private ItemData itemData;
    
    [Tooltip("Quantity of this item")]
    [SerializeField] private int quantity = 1;
    
    [Header("Visual & Audio")]
    [Tooltip("Particle effect when collected (overrides ItemData)")]
    [SerializeField] private GameObject collectEffect;
    
    [Tooltip("Sound when collected (overrides ItemData)")]
    [SerializeField] private AudioClip collectSound;
    
    [Header("Persistence")]
    [Tooltip("Unique ID for this collectable instance (leave empty for auto-generate)")]
    [SerializeField] private string collectableId = "";

    private bool isCollected = false;

    void Start()
    {
        // Auto-generate ID if empty
        if (string.IsNullOrEmpty(collectableId) && itemData != null)
        {
            collectableId = $"{itemData.itemId}_{gameObject.scene.name}_{transform.position.x:F2}_{transform.position.y:F2}";
        }
        
        // Check if this item was already collected
        // 註解掉自動銷毀邏輯，讓鑰匙在每次遊戲開始時都重新出現
        /*
        if (!string.IsNullOrEmpty(collectableId) && SaveSystem.IsItemCollected(collectableId))
        {
            Debug.Log($"CollectableItem: '{collectableId}' was already collected, destroying");
            Destroy(gameObject);
        }
        */
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected) return;

        // Check if collider is player
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player == null) return;

        // Get InventorySystem component
        InventorySystem inventory = player.GetComponent<InventorySystem>();
        if (inventory == null)
        {
            Debug.LogWarning($"CollectableItem: InventorySystem not found on player! Cannot collect '{itemData.itemName}'");
            return;
        }

        // Collect the item
        CollectItem(inventory, collision.transform.position);
    }

    /// <summary>
    /// Collect this item and add it to player's inventory
    /// </summary>
    private void CollectItem(InventorySystem inventory, Vector3 playerPosition)
    {
        if (isCollected) return;
        if (itemData == null)
        {
            Debug.LogError("CollectableItem: itemData is null!");
            return;
        }
        
        isCollected = true;

        // Add to inventory
        bool success = inventory.AddItem(itemData, quantity);
        
        if (!success)
        {
            Debug.LogWarning($"CollectableItem: Failed to add {itemData.itemName} to inventory (full?)");
            isCollected = false; // Allow retry
            return;
        }

        // Mark this world item as collected (for persistence)
        if (!string.IsNullOrEmpty(collectableId))
        {
            SaveSystem.MarkItemCollected(collectableId);
            Debug.Log($"CollectableItem: Marked '{collectableId}' as permanently collected");
        }

        // Show floating text
        FloatingTextManager floatingTextManager = FloatingTextManager.GetInstance();
        if (floatingTextManager != null)
        {
            string message = quantity > 1 ? $"+ {itemData.itemName} x{quantity}" : $"+ {itemData.itemName}";
            Color itemColor = new Color(1f, 0.84f, 0f); // Gold color
            floatingTextManager.ShowFloatingText(message, transform.position, itemColor);
        }
        else
        {
            Debug.Log($"Collected {itemData.itemName} x{quantity}");
        }

        // Play collect sound through SoundManager (with fallback for backward compatibility)
        AudioClip sound = collectSound != null ? collectSound : itemData.collectSound;
        if (sound != null)
        {
            PlayCollectSound(sound, transform.position);
        }

        // Spawn collect effect
        GameObject effect = collectEffect;
        if (effect != null)
        {
            Instantiate(effect, transform.position, Quaternion.identity);
        }

        // Destroy item object
        Destroy(gameObject);
    }

    /// <summary>
    /// Set item data (useful for runtime spawning)
    /// </summary>
    public void SetItemData(ItemData data, int amount = 1)
    {
        itemData = data;
        quantity = amount;
        
        // Update visual if has sprite renderer
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && data != null && data.icon != null)
        {
            spriteRenderer.sprite = data.icon;
        }
    }

    /// <summary>
    /// Get the item data
    /// </summary>
    public ItemData GetItemData()
    {
        return itemData;
    }

    /// <summary>
    /// Play collect sound using SoundManager with backward compatibility fallback
    /// </summary>
    /// <param name="soundClip">Audio clip to play</param>
    /// <param name="position">World position where sound should be played</param>
    private void PlayCollectSound(AudioClip soundClip, Vector3 position)
    {
        // Try to use SoundManager first
        SoundManager soundManager = SoundManager.GetInstance();
        if (soundManager != null)
        {
            soundManager.PlaySound(soundClip, position, 1f);
        }
        else
        {
            // Fallback to original method if SoundManager is not available
            AudioSource.PlayClipAtPoint(soundClip, position);
        }
    }
}

