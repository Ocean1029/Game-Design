using UnityEngine;

/// <summary>
/// Interface for any entity that can interact with objects in the game world
/// This could be the player, NPCs, Objects, etc.
/// </summary>
public interface IInteractor
{
    /// <summary>
    /// Get the transform of this interactor
    /// </summary>
    Transform GetTransform();

    /// <summary>
    /// Get the GameObject of this interactor
    /// Used to access specific components when needed
    /// </summary>
    GameObject GetGameObject();
}

