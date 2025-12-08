using UnityEngine;
using System.Collections;

/// <summary>
/// 基礎 SavePoint 類別，提供通用的重生點功能
/// 可以被 Chair 和其他重生點組件繼承或組合使用
/// </summary>
public abstract class BaseSavePoint : MonoBehaviour
{
    [Header("Save Point Configuration")]
    [Tooltip("Unique identifier for this save point")]
    [SerializeField] protected string savePointId = "";
    
    [Tooltip("Display name shown in UI")]
    [SerializeField] protected string displayName = "";
    
    [Tooltip("Description of the save point location")]
    [SerializeField] protected string locationDescription = "";

    [Header("Save Point Properties")]
    [Tooltip("If true, this save point can only be used once")]
    [SerializeField] protected bool isOneTimeUse = false;
    
    [Tooltip("If true, player can teleport to this save point")]
    [SerializeField] protected bool isTransportable = true;
    
    [Tooltip("Whether this save point restores player energy")]
    [SerializeField] protected bool restoresEnergy = true;
    
    [Tooltip("Amount of energy to restore (0 = restore all)")]
    [SerializeField] protected int energyRestoreAmount = 0;

    [Header("Audio & Visual")]
    [Tooltip("Sound played when activating save point")]
    [SerializeField] protected AudioClip activationSound;
    
    [Tooltip("Visual indicator shown when save point is active")]
    [SerializeField] protected GameObject activeVisual;

    [Header("Settings")]
    [Tooltip("Whether to show debug information")]
    [SerializeField] protected bool showDebugInfo = false;

    // Protected references
    protected SpawnPointSystem spawnPointSystem;
    protected GameManager gameManager;

    // Protected state
    protected bool isRegistered = false;

    void Start()
    {
        InitializeSavePoint();
    }

    /// <summary>
    /// Initialize the save point
    /// </summary>
    protected virtual void InitializeSavePoint()
    {
        // Get references
        gameManager = GameManager.GetInstance();
        if (gameManager != null)
        {
            spawnPointSystem = gameManager.GetComponent<SpawnPointSystem>();
        }

        // Generate ID if not set
        if (string.IsNullOrEmpty(savePointId))
        {
            savePointId = GenerateSavePointId();
        }

        // Generate display name if not set
        if (string.IsNullOrEmpty(displayName))
        {
            displayName = GenerateDisplayName();
        }

        // Register with spawn point system
        RegisterSavePoint();

        // Initialize visual state
        UpdateVisualStatus();

        if (showDebugInfo)
        {
            Debug.Log($"BaseSavePoint: Initialized '{savePointId}' - {displayName}");
        }
    }

    /// <summary>
    /// Generate a unique save point ID
    /// Override this method to customize ID generation
    /// </summary>
    protected virtual string GenerateSavePointId()
    {
        return $"savepoint_{gameObject.name}_{transform.position.x:F0}_{transform.position.y:F0}";
    }

    /// <summary>
    /// Generate a display name for the save point
    /// Override this method to customize name generation
    /// </summary>
    protected virtual string GenerateDisplayName()
    {
        return gameObject.name;
    }

    /// <summary>
    /// Register this save point with the spawn point system
    /// </summary>
    protected virtual void RegisterSavePoint()
    {
        if (spawnPointSystem == null)
        {
            Debug.LogWarning($"BaseSavePoint: Cannot register '{savePointId}' - SpawnPointSystem not found");
            return;
        }

        // Create spawn point data for registration
        SpawnPointData spawnPointData = new SpawnPointData(
            savePointId,
            displayName,
            locationDescription,
            GetSpawnPosition(),
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        )
        {
            isOneTimeUse = this.isOneTimeUse,
            isTransportable = this.isTransportable,
            restoresEnergy = this.restoresEnergy,
            energyRestoreAmount = this.energyRestoreAmount,
            activationSound = this.activationSound,
            activeVisual = this.activeVisual
        };

        // Add to spawn point system
        spawnPointSystem.AddSpawnPoint(spawnPointData);
        isRegistered = true;

        if (showDebugInfo)
        {
            Debug.Log($"BaseSavePoint: Registered '{savePointId}' with SpawnPointSystem");
        }
    }

    /// <summary>
    /// Get the spawn position for this save point
    /// Override this method to customize spawn position
    /// </summary>
    protected virtual Vector3 GetSpawnPosition()
    {
        return transform.position;
    }

    /// <summary>
    /// Activate this save point
    /// </summary>
    public virtual void ActivateSavePoint()
    {
        if (gameManager == null)
        {
            Debug.LogWarning($"BaseSavePoint: Cannot activate '{savePointId}' - GameManager not found");
            return;
        }

        gameManager.ActivateSpawnPoint(savePointId);
        
        // Play activation sound
        if (activationSound != null)
        {
            AudioSource.PlayClipAtPoint(activationSound, transform.position);
        }

        // Update visual status
        UpdateVisualStatus();

        if (showDebugInfo)
        {
            Debug.Log($"BaseSavePoint: Activated '{savePointId}' - {displayName}");
        }
    }

    /// <summary>
    /// Update the visual status of the save point
    /// </summary>
    protected virtual void UpdateVisualStatus()
    {
        if (activeVisual == null || gameManager == null || spawnPointSystem == null) return;

        SpawnPointData currentActive = spawnPointSystem.GetCurrentActiveSpawnPoint();
        bool isActive = (currentActive != null && currentActive.spawnPointId == savePointId);
        activeVisual.SetActive(isActive);
    }

    /// <summary>
    /// Check if this save point is currently active
    /// </summary>
    public virtual bool IsActive()
    {
        if (spawnPointSystem == null) return false;
        
        SpawnPointData currentActive = spawnPointSystem.GetCurrentActiveSpawnPoint();
        return currentActive != null && currentActive.spawnPointId == savePointId;
    }

    /// <summary>
    /// Get the save point data for this save point
    /// </summary>
    public virtual SpawnPointData GetSavePointData()
    {
        return new SpawnPointData(
            savePointId,
            displayName,
            locationDescription,
            GetSpawnPosition(),
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        )
        {
            isOneTimeUse = this.isOneTimeUse,
            isTransportable = this.isTransportable,
            restoresEnergy = this.restoresEnergy,
            energyRestoreAmount = this.energyRestoreAmount,
            activationSound = this.activationSound,
            activeVisual = this.activeVisual
        };
    }

    // Public properties for access
    public string SavePointId => savePointId;
    public string DisplayName => displayName;
    public string LocationDescription => locationDescription;
    public bool IsOneTimeUse => isOneTimeUse;
    public bool IsTransportable => isTransportable;
    public bool RestoresEnergy => restoresEnergy;
    public int EnergyRestoreAmount => energyRestoreAmount;
    public AudioClip ActivationSound => activationSound;
    public GameObject ActiveVisual => activeVisual;
}
