using UnityEngine;

/// <summary>
/// Triggers scene transition when player enters the trigger zone
/// Place this on doors, portals, or level exits
/// </summary>
public class SceneTransition : MonoBehaviour
{
    [Header("Target Scene")]
    [Tooltip("Name of the scene to load (must match exactly)")]
    [SerializeField] private string targetSceneName;

    [Header("Visual Feedback (Optional)")]
    [Tooltip("UI prompt shown when player is near")]
    [SerializeField] private GameObject transitionPrompt;

    private bool playerInZone = false;

    void Start()
    {
        // Hide prompt at start
        if (transitionPrompt != null)
        {
            transitionPrompt.SetActive(false);
        }
    }

    void Update()
    {
        // Check for interaction input when player is in zone
        if (playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            TriggerTransition();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if player entered the zone
        if (collision.CompareTag("Player"))
        {
            playerInZone = true;
            
            if (transitionPrompt != null)
            {
                transitionPrompt.SetActive(true);
            }
            
            Debug.Log($"SceneTransition: Player entered zone. Press E to go to {targetSceneName}");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if player left the zone
        if (collision.CompareTag("Player"))
        {
            playerInZone = false;
            
            if (transitionPrompt != null)
            {
                transitionPrompt.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Trigger the scene transition
    /// </summary>
    private void TriggerTransition()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("SceneTransition: Target scene name is empty!");
            return;
        }

        GameManager gameManager = GameManager.GetInstance();
        
        if (gameManager != null)
        {
            gameManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError("SceneTransition: GameManager not found! Make sure GameManager exists in the first scene.");
        }
    }
}


