using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays key collection status in UI
/// Shows key outline that lights up when key is collected
/// </summary>
public class KeyUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Image showing the key outline (always visible)")]
    [SerializeField] private Image keyOutlineImage;
    
    [Tooltip("Image showing the filled key (visible when collected)")]
    [SerializeField] private Image keyFilledImage;
    
    [Tooltip("Tag of the key this UI represents")]
    [SerializeField] private string keyTag = "key1";
    
    [Header("Visual Settings")]
    [Tooltip("Color of outline when key is not collected")]
    [SerializeField] private Color outlineColorEmpty = new Color(1f, 1f, 1f, 0.3f);
    
    [Tooltip("Color of outline when key is collected")]
    [SerializeField] private Color outlineColorFilled = new Color(1f, 1f, 1f, 1f);
    
    [Tooltip("Fade in duration when key is collected")]
    [SerializeField] private float fadeInDuration = 0.5f;

    private KeyInventory keyInventory;
    private InventorySystem inventorySystem;
    private bool isCollected = false;

    void Start()
    {
        // Find KeyInventory on player
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            // 訂閱 KeyInventory 事件（如果有）
            keyInventory = player.GetComponent<KeyInventory>();
            if (keyInventory != null)
            {
                keyInventory.OnKeyCollected += OnKeyCollected;
                keyInventory.OnKeyConsumed += OnKeyConsumed;
                
                // Check if key is already collected
                UpdateKeyDisplay(keyInventory.HasKey(keyTag));
            }
            
            // 訂閱 InventorySystem 事件（如果有）
            inventorySystem = player.GetComponent<InventorySystem>();
            if (inventorySystem != null)
            {
                inventorySystem.OnItemCollected += OnItemCollectedFromInventory;
                inventorySystem.OnItemRemoved += OnItemRemovedFromInventory;
                
                // Check if key is already in inventory
                ItemData keyItem = inventorySystem.GetItemForTag(keyTag);
                if (keyItem != null)
                {
                    UpdateKeyDisplay(true);
                }
            }
            
            // 警告：如果兩個系統都沒有
            if (keyInventory == null && inventorySystem == null)
            {
                Debug.LogWarning("KeyUI: Neither KeyInventory nor InventorySystem found on player!");
            }
        }
        else
        {
            Debug.LogWarning("KeyUI: PlayerController not found!");
        }

        // Initialize display
        if (keyFilledImage != null)
        {
            keyFilledImage.gameObject.SetActive(false);
        }
        
        if (keyOutlineImage != null)
        {
            keyOutlineImage.color = outlineColorEmpty;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (keyInventory != null)
        {
            keyInventory.OnKeyCollected -= OnKeyCollected;
            keyInventory.OnKeyConsumed -= OnKeyConsumed;
        }
        
        if (inventorySystem != null)
        {
            inventorySystem.OnItemCollected -= OnItemCollectedFromInventory;
            inventorySystem.OnItemRemoved -= OnItemRemovedFromInventory;
        }
    }

    /// <summary>
    /// Called when any key is collected
    /// </summary>
    private void OnKeyCollected(string collectedKeyTag, string keyName)
    {
        if (collectedKeyTag == keyTag)
        {
            UpdateKeyDisplay(true);
        }
    }
    
    /// <summary>
    /// Called when any key is consumed (from KeyInventory)
    /// </summary>
    private void OnKeyConsumed(string consumedKeyTag)
    {
        if (consumedKeyTag == keyTag)
        {
            UpdateKeyDisplay(false);
        }
    }
    
    /// <summary>
    /// Called when an item is collected (from InventorySystem)
    /// </summary>
    private void OnItemCollectedFromInventory(ItemData item, int quantity)
    {
        // 檢查這個物品的 interactableTag 是否與這個 KeyUI 的 keyTag 一致
        if (item.interactableTag == keyTag)
        {
            UpdateKeyDisplay(true);
        }
    }
    
    /// <summary>
    /// Called when an item is removed (from InventorySystem)
    /// </summary>
    private void OnItemRemovedFromInventory(ItemData item)
    {
        Debug.Log($"KeyUI: Item removed - {item.itemName}, interactableTag: '{item.interactableTag}', keyTag: '{keyTag}'");
        
        // 檢查這個物品的 interactableTag 是否與這個 KeyUI 的 keyTag 一致
        if (item.interactableTag == keyTag)
        {
            Debug.Log($"KeyUI: Tag matched! Updating display to dark");
            UpdateKeyDisplay(false);
        }
        else
        {
            Debug.Log($"KeyUI: Tag not matched. '{item.interactableTag}' != '{keyTag}'");
        }
    }

    /// <summary>
    /// Update the visual display of the key
    /// </summary>
    private void UpdateKeyDisplay(bool collected)
    {
        isCollected = collected;

        if (keyFilledImage != null)
        {
            if (collected)
            {
                keyFilledImage.gameObject.SetActive(true);
                // Fade in effect
                StartCoroutine(FadeInKey());
            }
            else
            {
                // Fade out effect
                StartCoroutine(FadeOutKey());
            }
        }

        if (keyOutlineImage != null)
        {
            keyOutlineImage.color = collected ? outlineColorFilled : outlineColorEmpty;
        }
    }

    /// <summary>
    /// Fade in animation for collected key
    /// </summary>
    private System.Collections.IEnumerator FadeInKey()
    {
        if (keyFilledImage == null) yield break;

        float elapsed = 0f;
        Color startColor = keyFilledImage.color;
        startColor.a = 0f;
        keyFilledImage.color = startColor;

        Color targetColor = startColor;
        targetColor.a = 1f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeInDuration;
            keyFilledImage.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        keyFilledImage.color = targetColor;
    }
    
    /// <summary>
    /// Fade out animation for consumed key
    /// </summary>
    private System.Collections.IEnumerator FadeOutKey()
    {
        if (keyFilledImage == null) yield break;

        float elapsed = 0f;
        Color startColor = keyFilledImage.color;
        Color targetColor = startColor;
        targetColor.a = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeInDuration;
            keyFilledImage.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        keyFilledImage.color = targetColor;
        keyFilledImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// Check if this key is collected
    /// </summary>
    public bool IsCollected()
    {
        return isCollected;
    }
}

