using UnityEngine;

/// <summary>
/// Triggers scene transition when player interacts
/// Place this on doors, portals, or level exits
/// Implements IInteractable for consistency with other interactive objects
/// </summary>
public class SceneTransition : MonoBehaviour, IInteractable
{
    [Header("Target Scene")]
    [Tooltip("Name of the scene to load (must match exactly)")]
    [SerializeField] private string targetSceneName;

    [Header("Visual Feedback (Optional)")]
    [Tooltip("UI prompt shown when player is near (Press E to enter)")]
    [SerializeField] private GameObject transitionPrompt;

    [Header("Transition Settings")]
    [Tooltip("Whether this transition requires interaction or is automatic")]
    [SerializeField] private bool requiresInteraction = true;

    private IInteractor currentInteractor = null;

    void Start()
    {
        // Hide prompt at start
        if (transitionPrompt != null)
        {
            transitionPrompt.SetActive(false);
        }
    }

    // ==================== IInteractable Implementation ====================

    /// <summary>
    /// Called when an interactor enters the transition zone
    /// </summary>
    public void OnInteractorEnterZone(IInteractor interactor)
    {
        currentInteractor = interactor;
        
        // Show transition prompt
        if (transitionPrompt != null)
        {
            transitionPrompt.SetActive(true);
        }
        
        Debug.Log($"SceneTransition: Interactor entered zone. Press E to go to {targetSceneName}");

        // If automatic transition, trigger immediately
        if (!requiresInteraction)
        {
            TriggerTransition();
        }
    }

    /// <summary>
    /// Called when an interactor exits the transition zone
    /// </summary>
    public void OnInteractorExitZone(IInteractor interactor)
    {
        currentInteractor = null;
        
        // Hide transition prompt
        if (transitionPrompt != null)
        {
            transitionPrompt.SetActive(false);
        }
    }

    /// <summary>
    /// Called when an interactor presses the interact button
    /// </summary>
    public bool Interact(IInteractor interactor)
    {
        if (!requiresInteraction)
        {
            return false; // Already handled in OnInteractorEnterZone
        }

        TriggerTransition();
        return true;
    }

    /// <summary>
    /// Get the GameObject this interactable belongs to
    /// </summary>
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    // ==================== Scene Transition Logic ====================

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
            Debug.Log($"SceneTransition: Transitioning to scene '{targetSceneName}'");
            gameManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError("SceneTransition: GameManager not found! Make sure GameManager exists in the first scene.");
        }
    }
}
