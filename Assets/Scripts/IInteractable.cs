using UnityEngine;

/// <summary>
/// Interface for all interactable objects in the game (doors, chairs, NPCs, etc.)
/// Implementing this interface allows objects to be detected and interacted with by the player
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Called when the player enters the interaction zone
    /// </summary>
    /// <param name="player">Reference to the PlayerController that entered the zone</param>
    void OnPlayerEnterZone(PlayerController player);

    /// <summary>
    /// Called when the player exits the interaction zone
    /// </summary>
    /// <param name="player">Reference to the PlayerController that exited the zone</param>
    void OnPlayerExitZone(PlayerController player);

    /// <summary>
    /// Called when the player presses the interact button while in range
    /// </summary>
    /// <param name="player">Reference to the PlayerController performing the interaction</param>
    /// <returns>True if interaction was successful, false otherwise</returns>
    bool Interact(PlayerController player);

    /// <summary>
    /// Returns the GameObject this interactable belongs to
    /// </summary>
    GameObject GetGameObject();
}

