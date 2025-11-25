using UnityEngine;
using TMPro;

/// <summary>
/// Manages creation and display of floating text messages
/// Singleton pattern for easy access from anywhere
/// </summary>
public class FloatingTextManager : MonoBehaviour
{
    private static FloatingTextManager instance;

    [Header("Prefab Reference")]
    [Tooltip("Prefab for floating text (must have FloatingText component)")]
    [SerializeField] private GameObject floatingTextPrefab;

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
    /// Show floating text at a world position
    /// </summary>
    /// <param name="text">Text to display</param>
    /// <param name="worldPosition">World position to spawn text</param>
    /// <param name="color">Optional text color</param>
    /// <param name="fontSize">Optional font size</param>
    public void ShowFloatingText(string text, Vector3 worldPosition, Color? color = null, float? fontSize = null)
    {
        if (floatingTextPrefab == null)
        {
            Debug.LogError("FloatingTextManager: floatingTextPrefab is not assigned!");
            return;
        }

        GameObject textObj;
        Vector3 spawnPosition;

        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // Screen space overlay - convert world to screen position
            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPosition + worldOffset);
            textObj = Instantiate(floatingTextPrefab, canvas.transform);
            textObj.transform.position = screenPos;
        }
        else
        {
            // World space or screen space camera
            spawnPosition = worldPosition + worldOffset;
            textObj = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
            
            if (canvas != null)
            {
                textObj.transform.SetParent(canvas.transform, true);
            }
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
        }
    }

    /// <summary>
    /// Show floating text at player's position
    /// </summary>
    /// <param name="text">Text to display</param>
    /// <param name="color">Optional text color</param>
    /// <param name="fontSize">Optional font size</param>
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

    /// <summary>
    /// Show key collected message
    /// </summary>
    /// <param name="keyName">Name of the key</param>
    /// <param name="worldPosition">Position to show text</param>
    public void ShowKeyCollected(string keyName, Vector3 worldPosition)
    {
        string message = $"+ {keyName}";  // Use + symbol for collected items
        Color keyColor = new Color(1f, 0.84f, 0f); // Gold color
        ShowFloatingText(message, worldPosition, keyColor, defaultFontSize * 1.2f);
    }
    public void ShowBombCollected(string bombName, Vector3 worldPosition)
    {
        string message = $"+ {bombName}";
        Color bombColor = new Color(1f, 0.3f, 0.1f); // 紅橙色（Bomb感覺）
        ShowFloatingText(message, worldPosition, bombColor, defaultFontSize * 1.2f);
    }
}

