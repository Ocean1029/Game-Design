using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays bomb collection status in UI
/// Shows bomb outline that lights up when bomb is collected
/// </summary>
public class BombUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Image showing the bomb outline (always visible)")]
    [SerializeField] private Image bombOutlineImage;
    
    [Tooltip("Image showing the filled bomb (visible when collected)")]
    [SerializeField] private Image bombFilledImage;
    
    [Tooltip("Tag of the bomb this UI represents (match ItemData.interactableTag)")]
    [SerializeField] private string bombTag = "bomb1";
    
    [Header("Visual Settings")]
    [Tooltip("Color of outline when bomb is not collected")]
    [SerializeField] private Color outlineColorEmpty = new Color(1f, 1f, 1f, 0.3f);
    
    [Tooltip("Color of outline when bomb is collected")]
    [SerializeField] private Color outlineColorFilled = new Color(1f, 1f, 1f, 1f);
    
    [Tooltip("Fade in duration when bomb is collected")]
    [SerializeField] private float fadeInDuration = 0.5f;

    private InventorySystem inventorySystem;
    private bool isCollected = false;

    void Start()
    {
        // Find player and get inventory system
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            inventorySystem = player.GetComponent<InventorySystem>();
            if (inventorySystem != null)
            {
                inventorySystem.OnItemCollected += OnItemCollected;
                inventorySystem.OnItemRemoved += OnItemRemoved;

                // Check if bomb already exists in inventory
                ItemData bombItem = inventorySystem.GetItemForTag(bombTag);
                if (bombItem != null)
                {
                    UpdateBombDisplay(true);
                }
            }
            else
            {
                Debug.LogWarning("BombUI: InventorySystem not found on player!");
            }
        }
        else
        {
            Debug.LogWarning("BombUI: PlayerController not found!");
        }

        // Initialize UI
        if (bombFilledImage != null)
            bombFilledImage.gameObject.SetActive(false);

        if (bombOutlineImage != null)
            bombOutlineImage.color = outlineColorEmpty;
    }

    void OnDestroy()
    {
        if (inventorySystem != null)
        {
            inventorySystem.OnItemCollected -= OnItemCollected;
            inventorySystem.OnItemRemoved -= OnItemRemoved;
        }
    }

    private void OnItemCollected(ItemData item, int quantity)
    {
        if (item.interactableTag == bombTag)
        {
            UpdateBombDisplay(true);
        }
    }

    private void OnItemRemoved(ItemData item)
    {
        if (item.interactableTag == bombTag)
        {
            UpdateBombDisplay(false);
        }
    }

    private void UpdateBombDisplay(bool collected)
    {
        isCollected = collected;

        if (bombFilledImage != null)
        {
            bombFilledImage.gameObject.SetActive(collected);
            StartCoroutine(collected ? FadeInBomb() : FadeOutBomb());
        }

        if (bombOutlineImage != null)
            bombOutlineImage.color = collected ? outlineColorFilled : outlineColorEmpty;
    }

    private System.Collections.IEnumerator FadeInBomb()
    {
        if (bombFilledImage == null) yield break;

        float elapsed = 0f;
        Color start = bombFilledImage.color;
        start.a = 0f;
        Color end = start;
        end.a = 1f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeInDuration;
            bombFilledImage.color = Color.Lerp(start, end, t);
            yield return null;
        }

        bombFilledImage.color = end;
    }

    private System.Collections.IEnumerator FadeOutBomb()
    {
        if (bombFilledImage == null) yield break;

        float elapsed = 0f;
        Color start = bombFilledImage.color;
        Color end = start;
        end.a = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeInDuration;
            bombFilledImage.color = Color.Lerp(start, end, t);
            yield return null;
        }

        bombFilledImage.color = end;
        bombFilledImage.gameObject.SetActive(false);
    }

    public bool IsCollected()
    {
        return isCollected;
    }
}
