using UnityEngine;
using TMPro;

public class FloatingTextManager : MonoBehaviour
{
    private static FloatingTextManager instance;

    [Header("Prefab Reference")]
    [Tooltip("Prefab for floating text (must have FloatingText component)")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private GameObject floatingImagePrefab;

    [Header("Default Settings")]
    [SerializeField] private float defaultFontSize = 24f;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1f, 0f);

    private Canvas canvas;
    private Camera mainCamera;

    void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        // Find canvas
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("FloatingTextManager: No Canvas found in parent. Creating floating text in world space.");
        }

        mainCamera = Camera.main;
    }

    /// <summary>
    /// Get the singleton instance
    /// </summary>
    public static FloatingTextManager GetInstance()
    {
        return instance;
    }

    /// <summary>
    /// Show floating text at a world position (returns FloatingText)
    /// </summary>
    public FloatingText ShowFloatingText(string text, Vector3 worldPosition, Color? color = null, float? fontSize = null)
    {
        if (floatingTextPrefab == null)
        {
            Debug.LogError("FloatingTextManager: floatingTextPrefab is not assigned!");
            return null;
        }

        GameObject textObj;

        // UI Overlay mode
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPosition + worldOffset);
            textObj = Instantiate(floatingTextPrefab, canvas.transform);
            textObj.transform.position = screenPos;
        }
        else
        {
            // World space or screen space camera
            Vector3 spawnPosition = worldPosition + worldOffset;
            textObj = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);

            if (canvas != null)
                textObj.transform.SetParent(canvas.transform, true);
        }

        // Setup floating text
        FloatingText floatingText = textObj.GetComponent<FloatingText>();
        if (floatingText != null)
        {
            floatingText.SetText(text);
            floatingText.SetColor(color ?? defaultColor);
            floatingText.SetFontSize(fontSize ?? defaultFontSize);
            
            // 套用 FontManager 的字體
            if (FontManager.Instance != null)
            {
                TextMeshProUGUI tmpText = textObj.GetComponent<TextMeshProUGUI>();
                if (tmpText != null)
                {
                    FontManager.Instance.ApplyPixellariFont(tmpText);
                }
            }
        }
        else
        {
            Debug.LogError("FloatingTextManager: FloatingText component not found on prefab!");
            Destroy(textObj);
            return null;
        }
    }

    /// <summary>
    /// Show floating image (icon popup)
    /// </summary>
    public void ShowFloatingImage(Sprite sprite, Vector3 worldPosition)
    {
        if (floatingImagePrefab == null)
        {
            Debug.LogError("FloatingTextManager: floatingImagePrefab not assigned!");
            return;
        }

        GameObject imgObj;

        // UI Overlay mode
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPosition + worldOffset);
            imgObj = Instantiate(floatingImagePrefab, canvas.transform);
            imgObj.transform.position = screenPos;
        }
        else
        {
            Vector3 spawnPosition = worldPosition + worldOffset;
            imgObj = Instantiate(floatingImagePrefab, spawnPosition, Quaternion.identity);

            if (canvas != null)
                imgObj.transform.SetParent(canvas.transform, true);
        }

        // Set sprite
        FloatingImage floatingImage = imgObj.GetComponent<FloatingImage>();
        if (floatingImage != null)
        {
            floatingImage.SetSprite(sprite);
        }
        else
        {
            Debug.LogError("FloatingTextManager: FloatingImage component not found on prefab!");
            Destroy(imgObj);
        }
    }

    /// <summary>
    /// Show floating text at player's position
    /// </summary>
    public void ShowFloatingTextAtPlayer(string text, Color? color = null, float? fontSize = null)
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            ShowFloatingText(text, player.transform.position, color, fontSize);
        }
        else
        {
            Debug.LogWarning("FloatingTextManager: Player not found!");
        }
    }

    public void ShowKeyCollected(string keyName, Vector3 worldPosition)
    {
        string message = $"+ {keyName}";
        Color keyColor = new Color(1f, 0.84f, 0f); // Gold
        ShowFloatingText(message, worldPosition, keyColor, defaultFontSize * 1.2f);
    }

    public void ShowBombCollected(string bombName, Vector3 worldPosition)
    {
        string message = $"+ {bombName}";
        Color bombColor = new Color(1f, 0.3f, 0.1f);
        ShowFloatingText(message, worldPosition, bombColor, defaultFontSize * 1.2f);
    }
}
