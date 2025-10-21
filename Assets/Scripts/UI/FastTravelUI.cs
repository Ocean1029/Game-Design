using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Fast Travel UI for teleporting to discovered spawn points
/// Shows available teleportable spawn points and allows player to select destination
/// </summary>
public class FastTravelUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Main panel for fast travel UI")]
    [SerializeField] private GameObject fastTravelPanel;
    
    [Tooltip("Scroll view content for spawn point list")]
    [SerializeField] private Transform spawnPointListContent;
    
    [Tooltip("Prefab for spawn point list item")]
    [SerializeField] private GameObject spawnPointListItemPrefab;
    
    [Tooltip("Close button")]
    [SerializeField] private Button closeButton;
    
    [Tooltip("Title text")]
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("Settings")]
    [Tooltip("Whether to show debug information")]
    [SerializeField] private bool showDebugInfo = false;

    [Tooltip("Whether to show one-time use spawn points")]
    [SerializeField] private bool showOneTimeUseSpawnPoints = false;

    // State
    private bool isOpen = false;
    private List<SpawnPointData> availableSpawnPoints = new List<SpawnPointData>();
    private List<GameObject> spawnPointListItems = new List<GameObject>();

    // References
    private GameManager gameManager;
    private PlayerController playerController;

    void Start()
    {
        // Get references
        gameManager = GameManager.GetInstance();
        playerController = FindFirstObjectByType<PlayerController>();

        // Initialize UI
        InitializeUI();

        // Subscribe to events
        SubscribeToEvents();

        // Hide UI initially
        SetUIVisibility(false);

        if (showDebugInfo)
        {
            Debug.Log("FastTravelUI: Initialized");
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        UnsubscribeFromEvents();
    }

    void Update()
    {
        // Input is now handled by PlayerController
        // This prevents double input detection when M key is pressed
    }

    // ==================== Initialization ====================

    /// <summary>
    /// Initialize the UI components
    /// </summary>
    private void InitializeUI()
    {
        // Set up close button
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseFastTravelUI);
        }

        // Set title text
        if (titleText != null)
        {
            titleText.text = "Fast Travel";
        }

        // Ensure panel is inactive initially
        if (fastTravelPanel != null)
        {
            fastTravelPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Subscribe to relevant events
    /// </summary>
    private void SubscribeToEvents()
    {
        if (gameManager != null)
        {
            SpawnPointSystem spawnSystem = gameManager.GetSpawnPointSystem();
            if (spawnSystem != null)
            {
                spawnSystem.OnSpawnPointActivated += OnSpawnPointActivated;
                spawnSystem.OnSpawnPointDeactivated += OnSpawnPointDeactivated;
            }
        }
    }

    /// <summary>
    /// Unsubscribe from events
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        if (gameManager != null)
        {
            SpawnPointSystem spawnSystem = gameManager.GetSpawnPointSystem();
            if (spawnSystem != null)
            {
                spawnSystem.OnSpawnPointActivated -= OnSpawnPointActivated;
                spawnSystem.OnSpawnPointDeactivated -= OnSpawnPointDeactivated;
            }
        }
    }

    // ==================== Input Handling ====================
    // Note: M key input is handled by PlayerController to prevent double input detection
    // ESC key is still handled here for convenience

    void LateUpdate()
    {
        // Check for escape key to close (only when open)
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseFastTravelUI();
        }
    }

    // ==================== UI Management ====================

    /// <summary>
    /// Open the fast travel UI
    /// </summary>
    public void OpenFastTravelUI()
    {
        if (isOpen)
        {
            if (showDebugInfo)
            {
                Debug.Log("FastTravelUI: UI is already open");
            }
            return;
        }

        if (gameManager == null)
        {
            Debug.LogError("FastTravelUI: Cannot open - GameManager is null! Make sure GameManager exists in the scene.");
            return;
        }

        if (fastTravelPanel == null)
        {
            Debug.LogError("FastTravelUI: Cannot open - fastTravelPanel is not assigned! Please assign it in the Inspector.");
            return;
        }

        // Pause the game
        Time.timeScale = 0f;

        // Update available spawn points
        UpdateAvailableSpawnPoints();

        // Show UI
        SetUIVisibility(true);
        isOpen = true;

        // Update spawn point list
        UpdateSpawnPointList();

        if (showDebugInfo)
        {
            Debug.Log($"FastTravelUI: Opened with {availableSpawnPoints.Count} available spawn points");
        }

        // Show warning if no spawn points available
        if (availableSpawnPoints.Count == 0)
        {
            Debug.LogWarning("FastTravelUI: No teleportable spawn points available. Players need to activate save points first.");
        }
    }

    /// <summary>
    /// Close the fast travel UI
    /// </summary>
    public void CloseFastTravelUI()
    {
        if (!isOpen)
        {
            return;
        }

        // Resume the game
        Time.timeScale = 1f;

        // Hide UI
        SetUIVisibility(false);
        isOpen = false;

        if (showDebugInfo)
        {
            Debug.Log("FastTravelUI: Closed");
        }
    }

    /// <summary>
    /// Set the visibility of the UI
    /// </summary>
    private void SetUIVisibility(bool visible)
    {
        if (fastTravelPanel != null)
        {
            fastTravelPanel.SetActive(visible);
        }
    }

    // ==================== Spawn Point Management ====================

    /// <summary>
    /// Update the list of available spawn points
    /// </summary>
    private void UpdateAvailableSpawnPoints()
    {
        if (gameManager == null)
        {
            Debug.LogWarning("FastTravelUI: GameManager is null, cannot update spawn points");
            availableSpawnPoints.Clear();
            return;
        }

        // Get all teleportable spawn points
        List<SpawnPointData> spawnPoints = gameManager.GetTransportableSpawnPoints();
        
        if (spawnPoints == null)
        {
            Debug.LogWarning("FastTravelUI: GetTransportableSpawnPoints returned null");
            availableSpawnPoints.Clear();
            return;
        }

        availableSpawnPoints = spawnPoints;

        // Filter out one-time use spawn points if not showing them
        if (!showOneTimeUseSpawnPoints)
        {
            availableSpawnPoints = availableSpawnPoints.Where(sp => !sp.isOneTimeUse).ToList();
        }

        // Sort by scene name, then by display name
        availableSpawnPoints = availableSpawnPoints.OrderBy(sp => sp.sceneName).ThenBy(sp => sp.displayName).ToList();

        if (showDebugInfo)
        {
            Debug.Log($"FastTravelUI: Updated available spawn points, found {availableSpawnPoints.Count} teleportable spawn points");
        }
    }

    /// <summary>
    /// Update the spawn point list UI
    /// </summary>
    private void UpdateSpawnPointList()
    {
        // Clear existing list items
        ClearSpawnPointList();

        // Create new list items
        foreach (SpawnPointData spawnPoint in availableSpawnPoints)
        {
            CreateSpawnPointListItem(spawnPoint);
        }

        if (showDebugInfo)
        {
            Debug.Log($"FastTravelUI: Updated spawn point list with {availableSpawnPoints.Count} items");
        }
    }

    /// <summary>
    /// Clear the spawn point list
    /// </summary>
    private void ClearSpawnPointList()
    {
        foreach (GameObject item in spawnPointListItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        spawnPointListItems.Clear();
    }

    /// <summary>
    /// Create a spawn point list item
    /// </summary>
    private void CreateSpawnPointListItem(SpawnPointData spawnPoint)
    {
        if (spawnPointListItemPrefab == null)
        {
            Debug.LogError("FastTravelUI: spawnPointListItemPrefab is not assigned! Please assign the prefab in the Inspector.");
            return;
        }

        if (spawnPointListContent == null)
        {
            Debug.LogError("FastTravelUI: spawnPointListContent is not assigned! Please assign the content transform in the Inspector.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("FastTravelUI: Attempted to create list item for null spawn point");
            return;
        }

        // Create the list item
        GameObject listItem = Instantiate(spawnPointListItemPrefab, spawnPointListContent);
        if (listItem == null)
        {
            Debug.LogError("FastTravelUI: Failed to instantiate spawn point list item");
            return;
        }

        spawnPointListItems.Add(listItem);

        // Set up the list item
        SetupSpawnPointListItem(listItem, spawnPoint);

        if (showDebugInfo)
        {
            Debug.Log($"FastTravelUI: Created list item for spawn point '{spawnPoint.spawnPointId}'");
        }
    }

    /// <summary>
    /// Set up a spawn point list item
    /// </summary>
    private void SetupSpawnPointListItem(GameObject listItem, SpawnPointData spawnPoint)
    {
        if (showDebugInfo)
        {
            Debug.Log($"FastTravelUI: Setting up list item for '{spawnPoint.displayName}' (ID: {spawnPoint.spawnPointId})");
        }

        // Find UI components - try multiple methods
        Button teleportButton = listItem.GetComponentInChildren<Button>();
        
        // Try to find text components by name (direct children first, then recursive)
        TextMeshProUGUI nameText = FindTextComponent(listItem.transform, "NameText");
        TextMeshProUGUI sceneText = FindTextComponent(listItem.transform, "SceneText");
        TextMeshProUGUI descriptionText = FindTextComponent(listItem.transform, "DescriptionText");
        
        // Try to find active indicator
        Image activeIndicator = FindImageComponent(listItem.transform, "ActiveIndicator");

        // Set text content - prioritize button text for simple prefabs
        TextMeshProUGUI buttonText = teleportButton?.GetComponentInChildren<TextMeshProUGUI>();
        
        if (nameText != null)
        {
            nameText.text = spawnPoint.displayName;
            if (showDebugInfo)
            {
                Debug.Log($"FastTravelUI: Set name text to '{spawnPoint.displayName}'");
            }
        }
        else if (buttonText != null)
        {
            // Fallback: use button text for simple prefabs
            buttonText.text = spawnPoint.displayName;
            if (showDebugInfo)
            {
                Debug.Log($"FastTravelUI: Used button text for '{spawnPoint.displayName}' (simple prefab)");
            }
        }
        else
        {
            Debug.LogWarning($"FastTravelUI: No text component found for '{spawnPoint.displayName}'");
        }

        // Optional text components (only show warnings if debug is enabled)
        if (sceneText != null)
        {
            sceneText.text = spawnPoint.sceneName;
        }
        else if (showDebugInfo)
        {
            Debug.LogWarning("FastTravelUI: SceneText not found in list item prefab");
        }

        if (descriptionText != null)
        {
            descriptionText.text = spawnPoint.locationDescription;
        }
        else if (showDebugInfo)
        {
            Debug.LogWarning("FastTravelUI: DescriptionText not found in list item prefab");
        }

        // Optional active indicator (only show warnings if debug is enabled)
        if (activeIndicator != null)
        {
            activeIndicator.gameObject.SetActive(spawnPoint.isActive);
        }
        else if (showDebugInfo)
        {
            Debug.LogWarning("FastTravelUI: ActiveIndicator not found in list item prefab");
        }

        // Set up teleport button
        if (teleportButton != null)
        {
            teleportButton.onClick.RemoveAllListeners();
            teleportButton.onClick.AddListener(() => TeleportToSpawnPoint(spawnPoint.spawnPointId));
            
            // Disable button if spawn point is in current scene and player is already there
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (spawnPoint.sceneName == currentScene && playerController != null)
            {
                float distance = Vector3.Distance(playerController.transform.position, spawnPoint.position);
                if (distance < 1f)
                {
                    teleportButton.interactable = false;
                }
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"FastTravelUI: Button configured for '{spawnPoint.displayName}'");
            }
        }
        else
        {
            Debug.LogError($"FastTravelUI: Button not found in list item prefab for '{spawnPoint.displayName}'!");
        }
    }

    /// <summary>
    /// Find a TextMeshProUGUI component by GameObject name (recursive search)
    /// </summary>
    private TextMeshProUGUI FindTextComponent(Transform parent, string name)
    {
        // Try direct child first
        Transform child = parent.Find(name);
        if (child != null)
        {
            TextMeshProUGUI text = child.GetComponent<TextMeshProUGUI>();
            if (text != null) return text;
        }

        // Try recursive search in all children
        TextMeshProUGUI[] allTexts = parent.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var text in allTexts)
        {
            if (text.gameObject.name == name)
            {
                return text;
            }
        }

        return null;
    }

    /// <summary>
    /// Find an Image component by GameObject name (recursive search)
    /// </summary>
    private Image FindImageComponent(Transform parent, string name)
    {
        // Try direct child first
        Transform child = parent.Find(name);
        if (child != null)
        {
            Image image = child.GetComponent<Image>();
            if (image != null) return image;
        }

        // Try recursive search in all children
        Image[] allImages = parent.GetComponentsInChildren<Image>();
        foreach (var image in allImages)
        {
            if (image.gameObject.name == name)
            {
                return image;
            }
        }

        return null;
    }

    // ==================== Teleportation ====================

    /// <summary>
    /// Teleport to a specific spawn point
    /// </summary>
    private void TeleportToSpawnPoint(string spawnPointId)
    {
        if (gameManager == null)
        {
            Debug.LogWarning("FastTravelUI: GameManager not found, cannot teleport");
            return;
        }

        // Attempt to teleport
        bool success = gameManager.TeleportToSpawnPoint(spawnPointId);
        
        if (success)
        {
            // Close the UI
            CloseFastTravelUI();
            
            if (showDebugInfo)
            {
                Debug.Log($"FastTravelUI: Teleporting to spawn point '{spawnPointId}'");
            }
        }
        else
        {
            Debug.LogWarning($"FastTravelUI: Failed to teleport to spawn point '{spawnPointId}'");
        }
    }

    // ==================== Event Handlers ====================

    /// <summary>
    /// Called when a spawn point is activated
    /// </summary>
    private void OnSpawnPointActivated(SpawnPointData spawnPoint)
    {
        if (isOpen)
        {
            UpdateSpawnPointList();
        }
    }

    /// <summary>
    /// Called when a spawn point is deactivated
    /// </summary>
    private void OnSpawnPointDeactivated(SpawnPointData spawnPoint)
    {
        if (isOpen)
        {
            UpdateSpawnPointList();
        }
    }

    // ==================== Public Methods ====================

    /// <summary>
    /// Check if the fast travel UI is open
    /// </summary>
    public bool IsOpen()
    {
        return isOpen;
    }

    /// <summary>
    /// Get the number of available spawn points
    /// </summary>
    public int GetAvailableSpawnPointCount()
    {
        return availableSpawnPoints.Count;
    }

    /// <summary>
    /// Refresh the spawn point list
    /// </summary>
    public void RefreshSpawnPointList()
    {
        if (isOpen)
        {
            UpdateAvailableSpawnPoints();
            UpdateSpawnPointList();
        }
    }

    // ==================== Debug Methods ====================

    /// <summary>
    /// Get debug information about the fast travel UI
    /// </summary>
    public string GetDebugInfo()
    {
        return $"FastTravelUI: Open={isOpen}, AvailableSpawnPoints={availableSpawnPoints.Count}, ShowOneTimeUse={showOneTimeUseSpawnPoints}";
    }
}