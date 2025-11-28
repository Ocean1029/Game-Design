using UnityEngine;
using UnityEngine.UI;

public class FloatingImage : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float lifetime = 10f;
    [SerializeField] private float floatDistance = 2f;

    private Image imageComponent;
    private Vector3 startPosition;
    private float elapsedTime = 0f;
    private Color startColor;

    void Awake()
    {
        imageComponent = GetComponent<Image>();
        transform.localScale = Vector3.one;
        if (imageComponent == null)
        {
            Debug.LogError("FloatingImage: Image component not found!");
            Destroy(gameObject);
            return;
        }

        startPosition = transform.position;
        startColor = imageComponent.color;
    }

    void Update()
    {
        elapsedTime += Time.unscaledDeltaTime;
        float progress = elapsedTime / lifetime;

        // Move upward
        float yOffset = Mathf.Lerp(0f, floatDistance, progress);
        transform.position = startPosition + Vector3.up * yOffset;

        // Fade out
        Color color = startColor;
        color.a = Mathf.Lerp(1f, 0f, progress);
        imageComponent.color = color;

        if (elapsedTime >= lifetime)
            Destroy(gameObject);
    }

    public void SetSprite(Sprite sprite)
    {
        if (imageComponent != null)
        {
            imageComponent.sprite = sprite;

            Color c = imageComponent.color;
            c.a = 1f;   // 強制不透明
            imageComponent.color = c;
        }
    }
}
