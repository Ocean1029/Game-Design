using UnityEngine;

/// <summary>
/// Spawn Point data structure for Save Point system
/// Represents a checkpoint/save point in the game world
/// </summary>
[System.Serializable]
public class SpawnPointData
{
    [Header("Basic Information")]
    [Tooltip("Unique identifier for this spawn point")]
    public string spawnPointId;
    
    [Tooltip("Display name shown in UI")]
    public string displayName;
    
    [Tooltip("Description of the location")]
    public string locationDescription;
    
    [Header("Position & Scene")]
    [Tooltip("World position of the spawn point")]
    public Vector3 position;
    
    [Tooltip("Scene name where this spawn point is located")]
    public string sceneName;
    
    [Header("Spawn Point Properties")]
    [Tooltip("If true, this spawn point can only be used once and will be replaced by newer spawn points")]
    public bool isOneTimeUse = false;
    
    [Tooltip("If true, player can teleport to this spawn point via fast travel menu")]
    public bool isTransportable = true;
    
    [Tooltip("If true, this spawn point is currently active (last used)")]
    public bool isActive = false;
    
    [Header("Visual & Audio")]
    [Tooltip("Visual indicator when this spawn point is active")]
    public GameObject activeVisual;
    
    [Tooltip("Sound played when player activates this spawn point")]
    public AudioClip activationSound;
    
    [Header("Gameplay Settings")]
    [Tooltip("Whether this spawn point restores player energy")]
    public bool restoresEnergy = true;
    
    [Tooltip("Amount of energy to restore (0 = restore all)")]
    public int energyRestoreAmount = 0;

    /// <summary>
    /// Default constructor
    /// </summary>
    public SpawnPointData()
    {
        spawnPointId = "";
        displayName = "";
        locationDescription = "";
        position = Vector3.zero;
        sceneName = "";
        isOneTimeUse = false;
        isTransportable = true;
        isActive = false;
        restoresEnergy = true;
        energyRestoreAmount = 0;
    }

    /// <summary>
    /// Constructor with basic parameters
    /// </summary>
    public SpawnPointData(string id, string name, string description, Vector3 pos, string scene)
    {
        spawnPointId = id;
        displayName = name;
        locationDescription = description;
        position = pos;
        sceneName = scene;
        isOneTimeUse = false;
        isTransportable = true;
        isActive = false;
        restoresEnergy = true;
        energyRestoreAmount = 0;
    }

    /// <summary>
    /// Create a copy of this spawn point data
    /// </summary>
    public SpawnPointData Clone()
    {
        SpawnPointData clone = new SpawnPointData();
        clone.spawnPointId = this.spawnPointId;
        clone.displayName = this.displayName;
        clone.locationDescription = this.locationDescription;
        clone.position = this.position;
        clone.sceneName = this.sceneName;
        clone.isOneTimeUse = this.isOneTimeUse;
        clone.isTransportable = this.isTransportable;
        clone.isActive = this.isActive;
        clone.activeVisual = this.activeVisual;
        clone.activationSound = this.activationSound;
        clone.restoresEnergy = this.restoresEnergy;
        clone.energyRestoreAmount = this.energyRestoreAmount;
        return clone;
    }

    /// <summary>
    /// Check if this spawn point can be used for teleportation
    /// </summary>
    public bool CanTeleportTo()
    {
        return isTransportable && !string.IsNullOrEmpty(spawnPointId);
    }

    /// <summary>
    /// Check if this spawn point can be activated
    /// </summary>
    public bool CanActivate()
    {
        return !string.IsNullOrEmpty(spawnPointId) && !string.IsNullOrEmpty(sceneName);
    }

    /// <summary>
    /// Get formatted string for debug output
    /// </summary>
    public override string ToString()
    {
        return $"SpawnPoint[{spawnPointId}]: {displayName} at {position} in {sceneName} (OneTime: {isOneTimeUse}, Transportable: {isTransportable}, Active: {isActive})";
    }
}
