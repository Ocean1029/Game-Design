using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays player energy as a simple UI with colored blocks
/// Can be upgraded to use sprites or more complex UI later
/// </summary>
public class EnergyUIDisplay : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The player's energy system to monitor")]
    [SerializeField] private PlayerEnergy energySystem;
    
    [Header("UI Setup")]
    [Tooltip("Parent container for energy blocks")]
    [SerializeField] private Transform energyBlockContainer;
    
    [Tooltip("Prefab for a single energy block (should have an Image component)")]
    [SerializeField] private GameObject energyBlockPrefab;

    [Header("Visual Settings")]
    [Tooltip("Size of each energy block (width and height)")]
    [SerializeField] private Vector2 blockSize = new Vector2(10, 10);
    
    [Tooltip("Spacing between energy blocks")]
    [SerializeField] private float blockSpacing = 3f;
    
    [Tooltip("Color when energy block is full")]
    [SerializeField] private Color fullColor = new Color(0.3f, 0.8f, 1f); // Light blue
    
    [Tooltip("Color when energy block is empty")]
    [SerializeField] private Color emptyColor = new Color(0.2f, 0.2f, 0.2f); // Dark gray

    private Image[] energyBlocks;

    void Start()
    {
        if (energySystem == null)
        {
            // Try to find the player energy system
            energySystem = FindFirstObjectByType<PlayerEnergy>();
            
            if (energySystem == null)
            {
                Debug.LogError("EnergyUIDisplay: No PlayerEnergy system found!");
                enabled = false;
                return;
            }
        }

        // Subscribe to energy change events
        energySystem.OnEnergyChanged += UpdateEnergyDisplay;

        // Create initial UI blocks
        CreateEnergyBlocks();
        
        // Initial update
        UpdateEnergyDisplay(energySystem.GetCurrentEnergy(), energySystem.GetMaxEnergy());
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (energySystem != null)
        {
            energySystem.OnEnergyChanged -= UpdateEnergyDisplay;
        }
    }

    /// <summary>
    /// Create UI blocks to represent energy
    /// </summary>
    private void CreateEnergyBlocks()
    {
        int maxEnergy = energySystem.GetMaxEnergy();
        energyBlocks = new Image[maxEnergy];

        // If no prefab provided, create simple blocks
        bool usePrefab = energyBlockPrefab != null;

        for (int i = 0; i < maxEnergy; i++)
        {
            GameObject block;
            
            if (usePrefab)
            {
                block = Instantiate(energyBlockPrefab, energyBlockContainer);
            }
            else
            {
                // Create a simple UI block
                block = new GameObject($"EnergyBlock_{i}");
                block.transform.SetParent(energyBlockContainer);
                
                Image image = block.AddComponent<Image>();
                image.color = fullColor;
                
                // Set size from inspector settings
                RectTransform rectTransform = block.GetComponent<RectTransform>();
                rectTransform.sizeDelta = blockSize;
            }

            // Get the Image component
            energyBlocks[i] = block.GetComponent<Image>();
            
            if (energyBlocks[i] == null)
            {
                Debug.LogError($"EnergyUIDisplay: Block {i} doesn't have an Image component!");
            }
        }

        // Set up horizontal layout if container doesn't have one
        HorizontalLayoutGroup layout = energyBlockContainer.GetComponent<HorizontalLayoutGroup>();
        if (layout == null)
        {
            layout = energyBlockContainer.gameObject.AddComponent<HorizontalLayoutGroup>();
        }
        
        // Configure layout to use fixed spacing regardless of container width
        layout.spacing = blockSpacing;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
        layout.childScaleWidth = false;
        layout.childScaleHeight = false;
    }

    /// <summary>
    /// Update the visual display when energy changes
    /// </summary>
    private void UpdateEnergyDisplay(int currentEnergy, int maxEnergy)
    {
        if (energyBlocks == null || energyBlocks.Length != maxEnergy)
        {
            // Recreate blocks if max energy changed
            ClearEnergyBlocks();
            CreateEnergyBlocks();
        }

        // Update each block's color based on current energy
        for (int i = 0; i < energyBlocks.Length; i++)
        {
            if (energyBlocks[i] != null)
            {
                energyBlocks[i].color = i < currentEnergy ? fullColor : emptyColor;
            }
        }
    }

    /// <summary>
    /// Clear all energy blocks
    /// </summary>
    private void ClearEnergyBlocks()
    {
        if (energyBlocks != null)
        {
            foreach (Image block in energyBlocks)
            {
                if (block != null)
                {
                    Destroy(block.gameObject);
                }
            }
        }
        energyBlocks = null;
    }
}