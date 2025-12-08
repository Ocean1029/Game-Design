using UnityEngine;

/// <summary>
/// Transition Point data structure for Scene Transition system
/// Represents a connection point between scenes in the game world
/// </summary>
[System.Serializable]
public class TransitionPointData
{
    [Header("Basic Information")]
    [Tooltip("Unique identifier for this transition point")]
    public string transitionPointId;
    
    [Tooltip("Display name shown in debug info")]
    public string displayName;
    
    [Tooltip("Description of the transition point")]
    public string description;
    
    [Header("Position & Scene")]
    [Tooltip("World position of the transition point")]
    public Vector3 position;
    
    [Tooltip("Scene name where this transition point is located")]
    public string sceneName;
    
    [Header("Connection Information")]
    [Tooltip("Target scene name for this transition")]
    public string targetSceneName;
    
    [Tooltip("Target transition point ID in the target scene")]
    public string targetTransitionPointId;
    
    [Tooltip("Target transition point index in the target scene's transition list")]
    public int targetTransitionPointIndex = -1;
    
    [Header("Transition Settings")]
    [Tooltip("Type of transition (entrance, exit, bidirectional)")]
    public TransitionType transitionType = TransitionType.Bidirectional;
    
    [Tooltip("Whether this transition requires a key or special condition")]
    public bool requiresKey = false;
    
    [Tooltip("Key ID required for this transition (if requiresKey is true)")]
    public string requiredKeyId = "";
    
    [Tooltip("Whether the player should face a specific direction after transition")]
    public bool setPlayerDirection = false;
    
    [Tooltip("Direction the player should face after transition")]
    public Vector2 playerDirection = Vector2.right;
    
    [Header("Visual & Audio")]
    [Tooltip("Visual effect when transitioning")]
    public GameObject transitionEffect;
    
    [Tooltip("Sound played during transition")]
    public AudioClip transitionSound;
    
    [Tooltip("Transition duration in seconds")]
    public float transitionDuration = 1.0f;

    /// <summary>
    /// Default constructor
    /// </summary>
    public TransitionPointData()
    {
        transitionPointId = "";
        displayName = "";
        description = "";
        position = Vector3.zero;
        sceneName = "";
        targetSceneName = "";
        targetTransitionPointId = "";
        targetTransitionPointIndex = -1;
        transitionType = TransitionType.Bidirectional;
        requiresKey = false;
        requiredKeyId = "";
        setPlayerDirection = false;
        playerDirection = Vector2.right;
        transitionDuration = 1.0f;
    }

    /// <summary>
    /// Constructor with basic parameters
    /// </summary>
    public TransitionPointData(string id, string name, Vector3 pos, string scene, string targetScene, string targetId)
    {
        transitionPointId = id;
        displayName = name;
        position = pos;
        sceneName = scene;
        targetSceneName = targetScene;
        targetTransitionPointId = targetId;
        targetTransitionPointIndex = -1;
        transitionType = TransitionType.Bidirectional;
        requiresKey = false;
        requiredKeyId = "";
        setPlayerDirection = false;
        playerDirection = Vector2.right;
        transitionDuration = 1.0f;
    }

    /// <summary>
    /// Create a copy of this transition point data
    /// </summary>
    public TransitionPointData Clone()
    {
        TransitionPointData clone = new TransitionPointData();
        clone.transitionPointId = this.transitionPointId;
        clone.displayName = this.displayName;
        clone.description = this.description;
        clone.position = this.position;
        clone.sceneName = this.sceneName;
        clone.targetSceneName = this.targetSceneName;
        clone.targetTransitionPointId = this.targetTransitionPointId;
        clone.targetTransitionPointIndex = this.targetTransitionPointIndex;
        clone.transitionType = this.transitionType;
        clone.requiresKey = this.requiresKey;
        clone.requiredKeyId = this.requiredKeyId;
        clone.setPlayerDirection = this.setPlayerDirection;
        clone.playerDirection = this.playerDirection;
        clone.transitionEffect = this.transitionEffect;
        clone.transitionSound = this.transitionSound;
        clone.transitionDuration = this.transitionDuration;
        return clone;
    }

    /// <summary>
    /// Check if this transition point is properly configured
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(transitionPointId) && 
               !string.IsNullOrEmpty(sceneName) && 
               !string.IsNullOrEmpty(targetSceneName) && 
               !string.IsNullOrEmpty(targetTransitionPointId);
    }

    /// <summary>
    /// Check if this transition can be used in the specified direction
    /// </summary>
    public bool CanTransition(TransitionDirection direction)
    {
        switch (direction)
        {
            case TransitionDirection.Exit:
                return transitionType == TransitionType.Exit || transitionType == TransitionType.Bidirectional;
            case TransitionDirection.Enter:
                return transitionType == TransitionType.Entrance || transitionType == TransitionType.Bidirectional;
            default:
                return false;
        }
    }

    /// <summary>
    /// Check if the player has the required key for this transition
    /// </summary>
    public bool HasRequiredKey()
    {
        if (!requiresKey) return true;
        
        // TODO: Implement key checking logic
        // This would check the player's inventory for the required key
        return true; // Placeholder
    }

    /// <summary>
    /// Get formatted string for debug output
    /// </summary>
    public override string ToString()
    {
        return $"TransitionPoint[{transitionPointId}]: {displayName} at {position} in {sceneName} -> {targetSceneName}[{targetTransitionPointId}] (Type: {transitionType})";
    }
}

/// <summary>
/// Types of transition points
/// </summary>
public enum TransitionType
{
    /// <summary>
    /// Can be used to exit this scene
    /// </summary>
    Exit,
    
    /// <summary>
    /// Can be used to enter this scene
    /// </summary>
    Entrance,
    
    /// <summary>
    /// Can be used in both directions
    /// </summary>
    Bidirectional
}

/// <summary>
/// Direction of transition
/// </summary>
public enum TransitionDirection
{
    /// <summary>
    /// Exiting current scene
    /// </summary>
    Exit,
    
    /// <summary>
    /// Entering new scene
    /// </summary>
    Enter
}
