using UnityEngine;

/// <summary>
/// Bomb endpoint - Shows success message when player touches this object
/// </summary>
public class BombEndpoint : MonoBehaviour
{
    [Header("Message Settings")]
    [Tooltip("Success message to display")]
    [SerializeField] private string successMessage = "Stage 1 Prototype Complete!";
    
    [Tooltip("Message color")]
    [SerializeField] private Color messageColor = new Color(0f, 1f, 0f); // Green
    
    [Tooltip("Font size")]
    [SerializeField] private float fontSize = 36f;
    
    [Header("Audio & Visual")]
    [Tooltip("Sound effect when triggered")]
    [SerializeField] private AudioClip triggerSound;
    
    [Tooltip("Visual effect when triggered")]
    [SerializeField] private GameObject triggerEffect;

    private bool isTriggered = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggered) return;

        // Check if it's the player
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player == null) return;

        // Trigger success message
        TriggerSuccess(player.transform.position);
    }

    /// <summary>
    /// Trigger success completion message
    /// </summary>
    private void TriggerSuccess(Vector3 playerPosition)
    {
        if (isTriggered) return;
        isTriggered = true;

        // Show floating text
        FloatingTextManager floatingTextManager = FloatingTextManager.GetInstance();
        if (floatingTextManager != null)
        {
            floatingTextManager.ShowFloatingText(
                successMessage, 
                transform.position, 
                messageColor, 
                fontSize
            );
        }
        else
        {
            Debug.LogWarning($"BombEndpoint: FloatingTextManager not found! Message: {successMessage}");
        }

        // Play sound effect
        if (triggerSound != null)
        {
            AudioSource.PlayClipAtPoint(triggerSound, transform.position);
        }

        // Spawn visual effect
        if (triggerEffect != null)
        {
            Instantiate(triggerEffect, transform.position, Quaternion.identity);
        }

        Debug.Log($"BombEndpoint: Success message triggered - {successMessage}");
    }

    /// <summary>
    /// Set success message
    /// </summary>
    public void SetSuccessMessage(string message)
    {
        successMessage = message;
    }

    /// <summary>
    /// Set message color
    /// </summary>
    public void SetMessageColor(Color color)
    {
        messageColor = color;
    }
}
