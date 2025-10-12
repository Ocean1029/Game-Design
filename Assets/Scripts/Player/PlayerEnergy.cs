using UnityEngine;
using System;

/// <summary>
/// Manages player's energy system for jump limitations
/// Energy is consumed when jumping and restored when resting on chairs
/// </summary>
public class PlayerEnergy : MonoBehaviour
{
    [Header("Energy Settings")]
    [Tooltip("Maximum energy capacity")]
    [SerializeField] private int maxEnergy = 4;
    
    [Tooltip("Starting energy amount")]
    [SerializeField] private int startingEnergy = 4;
    
    [Tooltip("Energy cost per jump")]
    [SerializeField] private int jumpCost = 1;
    
    [Tooltip("Energy restored per second while sitting")]
    [SerializeField] private float energyRestoreRate = 1f;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;

    private int currentEnergy;
    private bool isRestoring = false;
    private float restoreTimer = 0f;

    // Events for UI updates
    public event Action<int, int> OnEnergyChanged; // (currentEnergy, maxEnergy)
    public event Action OnEnergyDepleted;
    public event Action OnEnergyRestored;

    void Start()
    {
        currentEnergy = startingEnergy;
        
        if (showDebugInfo)
        {
            Debug.Log($"<color=cyan>PlayerEnergy: Initialized with {currentEnergy}/{maxEnergy} energy</color>");
        }
        
        // Notify initial energy state
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
    }

    void Update()
    {
        // Handle gradual energy restoration while sitting
        if (isRestoring)
        {
            restoreTimer += Time.deltaTime;
            
            if (restoreTimer >= (1f / energyRestoreRate))
            {
                RestoreSingleEnergy();
                restoreTimer = 0f;
            }
        }
    }

    /// <summary>
    /// Check if player has enough energy to jump
    /// </summary>
    public bool HasEnergyToJump()
    {
        return currentEnergy >= jumpCost;
    }

    /// <summary>
    /// Consume energy for a jump
    /// </summary>
    /// <returns>True if energy was consumed, false if not enough energy</returns>
    public bool ConsumeJumpEnergy()
    {
        if (!HasEnergyToJump())
        {
            if (showDebugInfo)
            {
                Debug.Log("<color=red>PlayerEnergy: Not enough energy to jump!</color>");
            }
            
            OnEnergyDepleted?.Invoke();
            return false;
        }

        currentEnergy -= jumpCost;
        
        if (showDebugInfo)
        {
            Debug.Log($"<color=yellow>PlayerEnergy: Jump consumed {jumpCost} energy. Remaining: {currentEnergy}/{maxEnergy}</color>");
        }
        
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
        return true;
    }

    /// <summary>
    /// Start restoring energy (called when sitting on chair)
    /// </summary>
    public void StartEnergyRestore()
    {
        isRestoring = true;
        restoreTimer = 0f;
        
        if (showDebugInfo)
        {
            Debug.Log("<color=green>PlayerEnergy: Started energy restoration</color>");
        }
    }

    /// <summary>
    /// Stop restoring energy (called when leaving chair)
    /// </summary>
    public void StopEnergyRestore()
    {
        isRestoring = false;
        restoreTimer = 0f;
        
        if (showDebugInfo)
        {
            Debug.Log("<color=green>PlayerEnergy: Stopped energy restoration</color>");
        }
    }

    /// <summary>
    /// Restore a single point of energy
    /// </summary>
    private void RestoreSingleEnergy()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy++;
            
            if (showDebugInfo)
            {
                Debug.Log($"<color=green>PlayerEnergy: Restored 1 energy. Current: {currentEnergy}/{maxEnergy}</color>");
            }
            
            OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
            
            if (currentEnergy == maxEnergy)
            {
                OnEnergyRestored?.Invoke();
                
                if (showDebugInfo)
                {
                    Debug.Log("<color=green>PlayerEnergy: Fully restored!</color>");
                }
            }
        }
    }

    /// <summary>
    /// Instantly restore all energy
    /// </summary>
    public void RestoreAllEnergy()
    {
        currentEnergy = maxEnergy;
        
        if (showDebugInfo)
        {
            Debug.Log($"<color=green>PlayerEnergy: Instantly restored to full! {currentEnergy}/{maxEnergy}</color>");
        }
        
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
        OnEnergyRestored?.Invoke();
    }

    /// <summary>
    /// Add energy (useful for pickups or other mechanics)
    /// </summary>
    /// <param name="amount">Amount of energy to add</param>
    public void AddEnergy(int amount)
    {
        int oldEnergy = currentEnergy;
        currentEnergy = Mathf.Min(currentEnergy + amount, maxEnergy);
        
        if (currentEnergy > oldEnergy)
        {
            if (showDebugInfo)
            {
                Debug.Log($"<color=green>PlayerEnergy: Added {currentEnergy - oldEnergy} energy. Current: {currentEnergy}/{maxEnergy}</color>");
            }
            
            OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
        }
    }

    /// <summary>
    /// Get current energy amount
    /// </summary>
    public int GetCurrentEnergy()
    {
        return currentEnergy;
    }

    /// <summary>
    /// Get maximum energy capacity
    /// </summary>
    public int GetMaxEnergy()
    {
        return maxEnergy;
    }

    /// <summary>
    /// Get energy percentage (0-1)
    /// </summary>
    public float GetEnergyPercentage()
    {
        return (float)currentEnergy / maxEnergy;
    }

    /// <summary>
    /// Check if energy is full
    /// </summary>
    public bool IsEnergyFull()
    {
        return currentEnergy >= maxEnergy;
    }

    /// <summary>
    /// Check if energy is empty
    /// </summary>
    public bool IsEnergyEmpty()
    {
        return currentEnergy <= 0;
    }

    /// <summary>
    /// Set max energy (useful for upgrades)
    /// </summary>
    public void SetMaxEnergy(int newMax)
    {
        maxEnergy = newMax;
        currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
        
        if (showDebugInfo)
        {
            Debug.Log($"<color=cyan>PlayerEnergy: Max energy changed to {maxEnergy}</color>");
        }
        
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
    }
}

