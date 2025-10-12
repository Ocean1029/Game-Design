using UnityEngine;

/// <summary>
/// Manages player state transitions and determines when input should be locked
/// </summary>
public class PlayerStateMachine : MonoBehaviour
{
    private PlayerState currentState = PlayerState.Idle;
    private PlayerState previousState = PlayerState.Idle;

    /// <summary>
    /// Get the current player state
    /// </summary>
    public PlayerState CurrentState => currentState;

    /// <summary>
    /// Get the previous player state
    /// </summary>
    public PlayerState PreviousState => previousState;

    /// <summary>
    /// Change to a new state
    /// </summary>
    /// <param name="newState">The state to transition to</param>
    public void ChangeState(PlayerState newState)
    {
        if (currentState == newState) return;

        previousState = currentState;
        currentState = newState;

        Debug.Log($"Player state changed: {previousState} -> {currentState}");
    }

    /// <summary>
    /// Check if player can receive movement input in current state
    /// </summary>
    public bool CanMove()
    {
        return currentState == PlayerState.Idle || 
               currentState == PlayerState.Moving || 
               currentState == PlayerState.Jumping || 
               currentState == PlayerState.Falling;
    }

    /// <summary>
    /// Check if player can jump in current state
    /// </summary>
    public bool CanJump()
    {
        return currentState == PlayerState.Idle || 
               currentState == PlayerState.Moving;
    }

    /// <summary>
    /// Check if player can interact with objects in current state
    /// </summary>
    public bool CanInteract()
    {
        return currentState == PlayerState.Idle || 
               currentState == PlayerState.Moving || 
               currentState == PlayerState.Jumping ||
               currentState == PlayerState.Falling ||
               currentState == PlayerState.Sitting;
    }

    /// <summary>
    /// Check if the player is in a state where input is completely locked
    /// </summary>
    public bool IsInputLocked()
    {
        return currentState == PlayerState.Rappelling || 
               currentState == PlayerState.Cutscene;
    }
}

