using UnityEngine;

/// <summary>
/// Interface for all interactable objects in the game (doors, chairs, NPCs, etc.)
/// Implementing this interface allows objects to be detected and interacted with by any interactor
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Called when an interactor (player, NPC, etc.) enters the interaction zone
    /// </summary>
    /// <param name="interactor">Reference to the IInteractor that entered the zone</param>
    void OnInteractorEnterZone(IInteractor interactor);

    /// <summary>
    /// Called when an interactor exits the interaction zone
    /// </summary>
    /// <param name="interactor">Reference to the IInteractor that exited the zone</param>
    void OnInteractorExitZone(IInteractor interactor);

    /// <summary>
    /// Called when an interactor presses the interact button while in range
    /// </summary>
    /// <param name="interactor">Reference to the IInteractor performing the interaction</param>
    /// <returns>True if interaction was successful, false otherwise</returns>
    bool Interact(IInteractor interactor);

    /// <summary>
    /// Returns the GameObject this interactable belongs to
    /// </summary>
    GameObject GetGameObject();
}

