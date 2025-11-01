using UnityEngine;

/// <summary>
/// Represents a bomb item that can be collected
/// Bombs are automatically added to player's BombInventory when touched
/// </summary>
public class Bomb : MonoBehaviour
{
    [Header("Bomb Configuration")]
    [Tooltip("Unique tag for this bomb (e.g., 'bomb1', 'bomb2')")]
    [SerializeField] private string bombTag = "bomb1";

    [Tooltip("Display name shown when collected (e.g., 'Small Bomb', 'Mega Bomb')")]
    [SerializeField] private string bombName = "Bomb";

    [Header("Visual & Audio")]
    [Tooltip("Sound played when bomb is collected")]
    [SerializeField] private AudioClip collectSound;

    [Tooltip("Particle effect when bomb is collected")]
    [SerializeField] private GameObject collectEffect;

    private bool isCollected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected) return;

        // Check if collider is player
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player == null) return;

        // Get BombInventory component
        BombInventory bombInventory = player.GetComponent<BombInventory>();
        if (bombInventory == null)
        {
            Debug.LogWarning($"Bomb: BombInventory not found on player! Cannot collect bomb '{bombTag}'");
            return;
        }

        // Collect the bomb
        CollectBomb(bombInventory);
    }

    /// <summary>
    /// Collect this bomb and add it to player's inventory
    /// </summary>
    private void CollectBomb(BombInventory bombInventory)
    {
        if (isCollected) return;
        isCollected = true;

        // Add to inventory
        bombInventory.AddBomb(bombTag, bombName);

        // Show floating text
        FloatingTextManager floatingTextManager = FloatingTextManager.GetInstance();
        if (floatingTextManager != null)
        {
            floatingTextManager.ShowBombCollected(bombName, transform.position);
        }
        else
        {
            Debug.Log($"Collected {bombName}");
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

        // Destroy bomb object
        Destroy(gameObject);
    }

    /// <summary>
    /// Get the bomb's tag
    /// </summary>
    public string GetBombTag()
    {
        return bombTag;
    }

    /// <summary>
    /// Get the bomb's display name
    /// </summary>
    public string GetBombName()
    {
        return bombName;
    }
}
