/// <summary>
/// Enumeration of all possible player states
/// Used by PlayerStateMachine to manage state transitions and input locking
/// </summary>
public enum PlayerState
{
    Idle,           // Standing still, can receive input
    Moving,         // Walking or running
    Jumping,        // In the air (may be combined with Moving)
    Falling,        // Falling without initial jump
    Sitting,        // Sitting on a chair, limited input
    Rappelling,     // Descending on a cable, input locked
    Interacting,    // Generic interaction state (talking, reading, etc.)
    Cutscene        // During cutscene, all input locked
}

