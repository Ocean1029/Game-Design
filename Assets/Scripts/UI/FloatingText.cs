using UnityEngine;
using TMPro;

/// <summary>
/// Displays floating text that appears and fades out
/// Used for key pickup notifications and other messages
/// </summary>
public class FloatingText : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("How long the text stays visible")]
    [SerializeField] private float lifetime = 2f;
    
    [Tooltip("Distance the text floats upward before fading")]
    [SerializeField] private float floatDistance = 2f;

    private TextMeshProUGUI textComponent;
    private Vector3 startPosition;
    private float elapsedTime = 0f;
    private Color startColor;

    void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        if (textComponent == null)
        {
            Debug.LogError("FloatingText: TextMeshProUGUI component not found!");
            Destroy(gameObject);
            return;
        }

        startPosition = transform.position;
        startColor = textComponent.color;
    }

    void Update()
    {
        elapsedTime += Time.unscaledDeltaTime;
        float progress = elapsedTime / lifetime;

        // Float upward
        float yOffset = Mathf.Lerp(0f, floatDistance, progress);
        transform.position = startPosition + Vector3.up * yOffset;

        // Fade out
        Color color = startColor;
        color.a = Mathf.Lerp(1f, 0f, progress);
        textComponent.color = color;

        // Destroy when lifetime is over
        if (elapsedTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Set the text to display
    /// </summary>
    public void SetText(string text)
    {
        if (textComponent != null)
        {
            textComponent.text = text;
        }
    }

    /// <summary>
    /// Set the text color
    /// </summary>
    public void SetColor(Color color)
    {
        if (textComponent != null)
        {
            startColor = color;
            textComponent.color = color;
        }
    }

    /// <summary>
    /// Set the font size
    /// </summary>
    public void SetFontSize(float size)
    {
        if (textComponent != null)
        {
            textComponent.fontSize = size;
        }
    }
}

