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
    private bool isCollected = false;

    void Start()
    {
        // Find KeyInventory on player
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            keyInventory = player.GetComponent<KeyInventory>();
            if (keyInventory == null)
            {
                Debug.LogWarning("KeyUI: KeyInventory not found on player!");
            }
            else
            {
                // Subscribe to key collection event
                keyInventory.OnKeyCollected += OnKeyCollected;
                
                // Check if key is already collected
                UpdateKeyDisplay(keyInventory.HasKey(keyTag));
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
    /// Update the visual display of the key
    /// </summary>
    private void UpdateKeyDisplay(bool collected)
    {
        isCollected = collected;

        if (keyFilledImage != null)
        {
            keyFilledImage.gameObject.SetActive(collected);
            
            if (collected)
            {
                // Fade in effect
                StartCoroutine(FadeInKey());
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
    /// Check if this key is collected
    /// </summary>
    public bool IsCollected()
    {
        return isCollected;
    }
}

