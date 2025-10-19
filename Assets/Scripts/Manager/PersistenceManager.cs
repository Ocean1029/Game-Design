using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages persistent objects across scene transitions
/// Ensures Player, UI, and other critical objects persist using DontDestroyOnLoad
/// </summary>
public class PersistenceManager : MonoBehaviour
{
    [Header("Persistent Objects")]
    [Tooltip("The player that should persist across scenes")]
    [SerializeField] private GameObject player;
    
    [Tooltip("The UI canvas that should persist across scenes")]
    [SerializeField] private Canvas gameCanvas;

    [Header("Optional Persistent Objects")]
    [Tooltip("Additional objects to persist across scenes")]
    [SerializeField] private List<GameObject> additionalPersistentObjects = new List<GameObject>();

    private bool isInitialized = false;

    // ==================== Initialization ====================

    /// <summary>
    /// Initialize all persistent objects
    /// Should be called once at game start
    /// </summary>
    public void Initialize()
    {
        if (isInitialized) return;

        // Make player persistent
        if (player != null)
        {
            DontDestroyOnLoad(player);
            Debug.Log("PersistenceManager: Player set to persist across scenes");
        }
        else
        {
            Debug.LogWarning("PersistenceManager: No player assigned!");
        }

        // Make UI Canvas persistent
        if (gameCanvas != null)
        {
            DontDestroyOnLoad(gameCanvas.gameObject);
            Debug.Log("PersistenceManager: UI Canvas set to persist across scenes");
        }
        else
        {
            Debug.LogWarning("PersistenceManager: No canvas assigned!");
        }

        // Make additional objects persistent
        foreach (GameObject obj in additionalPersistentObjects)
        {
            if (obj != null)
            {
                DontDestroyOnLoad(obj);
                Debug.Log($"PersistenceManager: {obj.name} set to persist across scenes");
            }
        }

        isInitialized = true;
        Debug.Log("PersistenceManager: Initialization complete");
    }

    // ==================== Object Management ====================

    /// <summary>
    /// Add an object to persist across scenes
    /// </summary>
    public void AddPersistentObject(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("PersistenceManager: Cannot add null object!");
            return;
        }

        if (!additionalPersistentObjects.Contains(obj))
        {
            additionalPersistentObjects.Add(obj);
            DontDestroyOnLoad(obj);
            Debug.Log($"PersistenceManager: Added {obj.name} to persistent objects");
        }
    }

    /// <summary>
    /// Remove an object from persistent list (does not destroy it)
    /// </summary>
    public void RemovePersistentObject(GameObject obj)
    {
        if (additionalPersistentObjects.Contains(obj))
        {
            additionalPersistentObjects.Remove(obj);
            Debug.Log($"PersistenceManager: Removed {obj.name} from persistent objects");
        }
    }

    // ==================== Getters ====================

    /// <summary>
    /// Get the persistent player object
    /// </summary>
    public GameObject GetPlayer()
    {
        return player;
    }

    /// <summary>
    /// Get the persistent UI canvas
    /// </summary>
    public Canvas GetCanvas()
    {
        return gameCanvas;
    }

    /// <summary>
    /// Get all additional persistent objects
    /// </summary>
    public List<GameObject> GetAdditionalPersistentObjects()
    {
        return new List<GameObject>(additionalPersistentObjects);
    }

    // ==================== Setters ====================

    /// <summary>
    /// Set the player reference
    /// </summary>
    public void SetPlayer(GameObject playerObject)
    {
        player = playerObject;
        if (isInitialized && player != null)
        {
            DontDestroyOnLoad(player);
            Debug.Log("PersistenceManager: Player reference updated");
        }
    }

    /// <summary>
    /// Set the canvas reference
    /// </summary>
    public void SetCanvas(Canvas canvas)
    {
        gameCanvas = canvas;
        if (isInitialized && gameCanvas != null)
        {
            DontDestroyOnLoad(gameCanvas.gameObject);
            Debug.Log("PersistenceManager: Canvas reference updated");
        }
    }

    // ==================== Status ====================

    /// <summary>
    /// Check if persistence manager is initialized
    /// </summary>
    public bool IsInitialized()
    {
        return isInitialized;
    }

    /// <summary>
    /// Get count of persistent objects
    /// </summary>
    public int GetPersistentObjectCount()
    {
        int count = 0;
        if (player != null) count++;
        if (gameCanvas != null) count++;
        count += additionalPersistentObjects.Count;
        return count;
    }
}

