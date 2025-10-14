using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Manages the fast travel menu UI for chair-based spawn points
/// Allows players to teleport between discovered chairs and set current spawn point
/// </summary>
public class FastTravelUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The main menu panel GameObject")]
    [SerializeField] private GameObject menuPanel;
    
    [Tooltip("Container where chair buttons will be spawned")]
    [SerializeField] private Transform buttonContainer;
    
    [Tooltip("Prefab for chair buttons")]
    [SerializeField] private GameObject chairButtonPrefab;
    
    [Tooltip("Text shown when no chairs are discovered")]
    [SerializeField] private Text noChairsText;
    
    [Tooltip("Title text of the menu")]
    [SerializeField] private Text titleText;
    
    [Header("Settings")]
    [Tooltip("Whether to pause game time when menu is open")]
    [SerializeField] private bool pauseTimeWhenOpen = true;
    
    // State
    private bool isMenuOpen = false;
    private List<GameObject> spawnedButtons = new List<GameObject>();

    void Start()
    {
        // Hide menu at start
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
        
        // Set title text
        if (titleText != null)
        {
            titleText.text = "Fast Travel - Discovered Chairs";
        }
    }

    void Update()
    {
        // Close menu with ESC key
        if (isMenuOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMenu();
        }
    }

    /// <summary>
    /// Toggle the fast travel menu open/closed
    /// </summary>
    public void ToggleMenu()
    {
        if (isMenuOpen)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }
    }

    /// <summary>
    /// Open the fast travel menu
    /// </summary>
    public void OpenMenu()
    {
        if (menuPanel == null)
        {
            Debug.LogError("FastTravelUI: Menu panel not assigned! Cannot open menu.");
            Debug.LogError("FastTravelUI: Please assign the FastTravelMenu GameObject in the Inspector.");
            // Make sure time is not paused if we can't open the menu
            Time.timeScale = 1f;
            return;
        }

        Debug.Log("FastTravelUI: Opening fast travel menu...");
        
        isMenuOpen = true;
        menuPanel.SetActive(true);
        
        // Pause time if enabled
        if (pauseTimeWhenOpen)
        {
            Time.timeScale = 0f;
            Debug.Log("FastTravelUI: Time paused");
        }

        // Refresh the chair list
        RefreshChairList();
    }

    /// <summary>
    /// Close the fast travel menu
    /// </summary>
    public void CloseMenu()
    {
        Debug.Log("FastTravelUI: Closing fast travel menu...");

        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }

        isMenuOpen = false;

        // Resume time if it was paused
        if (pauseTimeWhenOpen)
        {
            Time.timeScale = 1f;
            Debug.Log("FastTravelUI: Time resumed");
        }
    }

    /// <summary>
    /// Check if the menu is currently open
    /// </summary>
    public bool IsMenuOpen()
    {
        return isMenuOpen;
    }

    /// <summary>
    /// Refresh the list of discovered chairs
    /// </summary>
    private void RefreshChairList()
    {
        // Clear existing buttons
        ClearButtonList();

        GameManager gameManager = GameManager.GetInstance();
        
        if (gameManager == null)
        {
            Debug.LogError("FastTravelUI: GameManager not found!");
            return;
        }

        // Get all discovered spawn points (chairs)
        List<GameManager.SpawnPointData> chairs = gameManager.GetDiscoveredSpawnPoints();
        GameManager.SpawnPointData currentSpawn = gameManager.GetCurrentSpawnPoint();

        if (chairs.Count == 0)
        {
            // Show "no chairs" message
            if (noChairsText != null)
            {
                noChairsText.gameObject.SetActive(true);
                noChairsText.text = "No chairs discovered yet.\nSit on a chair to save progress!";
            }
            return;
        }

        // Hide "no chairs" message
        if (noChairsText != null)
        {
            noChairsText.gameObject.SetActive(false);
        }

        // Create buttons for each discovered chair
        foreach (GameManager.SpawnPointData chair in chairs)
        {
            CreateChairButton(chair, chair.spawnPointId == currentSpawn?.spawnPointId);
        }
    }

    /// <summary>
    /// Create a button for a chair
    /// </summary>
    private void CreateChairButton(GameManager.SpawnPointData chair, bool isCurrent)
    {
        if (chairButtonPrefab == null || buttonContainer == null)
        {
            Debug.LogError("FastTravelUI: Button prefab or container not assigned!");
            return;
        }

        // Instantiate button
        GameObject buttonObj = Instantiate(chairButtonPrefab, buttonContainer);
        spawnedButtons.Add(buttonObj);

        // Get button component
        Button button = buttonObj.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("FastTravelUI: Chair button prefab must have a Button component!");
            return;
        }

        // Set button text
        Text buttonText = buttonObj.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            string buttonLabel = chair.displayName;
            if (isCurrent)
            {
                buttonLabel += " (Current)";
            }
            buttonText.text = buttonLabel;
        }

        // Set button interactability
        button.interactable = !isCurrent; // Disable current chair button

        // Add click listener
        button.onClick.AddListener(() => OnChairButtonClicked(chair.spawnPointId));

        Debug.Log($"FastTravelUI: Created button for chair '{chair.displayName}' (Current: {isCurrent})");
    }

    /// <summary>
    /// Handle chair button click
    /// </summary>
    private void OnChairButtonClicked(string chairId)
    {
        Debug.Log($"FastTravelUI: Chair button clicked - {chairId}");

        GameManager gameManager = GameManager.GetInstance();
        if (gameManager == null)
        {
            Debug.LogError("FastTravelUI: GameManager not found!");
            return;
        }

        // Set this chair as the current spawn point
        gameManager.SetCurrentSpawnPoint(chairId);
        
        // Teleport to the chair
        gameManager.TeleportToSpawnPoint(chairId);
        
        // Close the menu
        CloseMenu();
        
        Debug.Log($"FastTravelUI: Set '{chairId}' as current spawn point and teleported");
    }

    /// <summary>
    /// Clear all spawned buttons
    /// </summary>
    private void ClearButtonList()
    {
        foreach (GameObject button in spawnedButtons)
        {
            if (button != null)
            {
                Destroy(button);
            }
        }
        spawnedButtons.Clear();
    }

    /// <summary>
    /// Public method to refresh the chair list (useful for external calls)
    /// </summary>
    public void RefreshMenu()
    {
        if (isMenuOpen)
        {
            RefreshChairList();
        }
    }
}
