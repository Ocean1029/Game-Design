using UnityEngine;
using System.Collections;

/// <summary>
/// Main player controller that coordinates all player subsystems
/// Manages input, state, movement, animation, and interactions
/// </summary>
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerStateMachine))]
[RequireComponent(typeof(PlayerAnimationController))]
[RequireComponent(typeof(InteractionHandler))]
public class PlayerController : MonoBehaviour
{
    [Header("Component References")]
    private PlayerMovement movement;
    private PlayerStateMachine stateMachine;
    private PlayerAnimationController animationController;
    private InteractionHandler interactionHandler;

    [Header("Input Keys")]
    [SerializeField] private KeyCode moveLeftKey = KeyCode.LeftArrow;
    [SerializeField] private KeyCode moveRightKey = KeyCode.RightArrow;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode interactKey = KeyCode.U;
    [SerializeField] private KeyCode exitInteractionKey = KeyCode.D;

    [Header("Rappelling Settings")]
    [SerializeField] private float rappellingDuration = 1.0f;

    // Chair interaction state
    private chair currentChair = null;

    void Awake()
    {
        // Get all required components
        movement = GetComponent<PlayerMovement>();
        stateMachine = GetComponent<PlayerStateMachine>();
        animationController = GetComponent<PlayerAnimationController>();
        interactionHandler = GetComponent<InteractionHandler>();
    }

    void Start()
    {
        // Initialize to idle state
        stateMachine.ChangeState(PlayerState.Idle);
    }

    void Update()
    {
        // Don't process input if in locked state
        if (stateMachine.IsInputLocked())
        {
            return;
        }

        HandleMovementInput();
        HandleJumpInput();
        HandleInteractionInput();
        UpdateAnimations();
        UpdateMovementState();
    }

    /// <summary>
    /// Handle horizontal movement input
    /// </summary>
    private void HandleMovementInput()
    {
        if (!stateMachine.CanMove())
        {
            return;
        }

        float horizontal = 0f;

        if (Input.GetKey(moveRightKey))
        {
            horizontal = 1f;
            animationController.SetFacingDirection(true);
        }
        else if (Input.GetKey(moveLeftKey))
        {
            horizontal = -1f;
            animationController.SetFacingDirection(false);
        }

        movement.Move(horizontal);
    }

    /// <summary>
    /// Handle jump input - supports variable height jumping
    /// </summary>
    private void HandleJumpInput()
    {
        // Start jump when button is first pressed
        if (stateMachine.CanJump() && Input.GetKeyDown(jumpKey))
        {
            movement.StartJump();
            animationController.TriggerJump();
            stateMachine.ChangeState(PlayerState.Jumping);
        }
        
        // Continue applying upward force while button is held
        if (Input.GetKey(jumpKey))
        {
            movement.ContinueJump();
        }
        
        // Stop jump early when button is released
        if (Input.GetKeyUp(jumpKey))
        {
            movement.StopJump();
        }
    }

    /// <summary>
    /// Handle interaction input (sitting on chairs, using objects, etc.)
    /// </summary>
    private void HandleInteractionInput()
    {
        if (!stateMachine.CanInteract())
        {
            Debug.Log("Cannot interact - current state: " + stateMachine.CurrentState);
            return;
        }

        // Interact with nearby objects
        if (Input.GetKeyDown(interactKey))
        {
            Debug.Log("U key pressed - attempting interaction");
            // Try standard interaction first
            if (interactionHandler.TryInteract(this))
            {
                Debug.Log("Interaction successful!");
                animationController.TriggerInteract();
            }
            else
            {
                Debug.Log("No interactable object nearby");
            }
        }

        // Exit interaction (e.g., stand up from chair)
        if (Input.GetKeyDown(exitInteractionKey))
        {
            if (stateMachine.CurrentState == PlayerState.Sitting)
            {
                LeaveChair();
            }
        }
    }

    /// <summary>
    /// Update animation parameters based on current movement
    /// </summary>
    private void UpdateAnimations()
    {
        Vector2 velocity = movement.GetVelocity();
        animationController.SetSpeed(Mathf.Abs(velocity.x));
        animationController.SetGrounded(movement.IsGrounded());
        animationController.SetSitting(stateMachine.CurrentState == PlayerState.Sitting);
    }

    /// <summary>
    /// Update movement-related state based on velocity and ground status
    /// </summary>
    private void UpdateMovementState()
    {
        // Don't update if in special states
        if (stateMachine.CurrentState == PlayerState.Sitting ||
            stateMachine.CurrentState == PlayerState.Rappelling ||
            stateMachine.CurrentState == PlayerState.Cutscene)
        {
            return;
        }

        Vector2 velocity = movement.GetVelocity();
        bool isGrounded = movement.IsGrounded();

        if (!isGrounded)
        {
            if (velocity.y > 0.1f)
            {
                stateMachine.ChangeState(PlayerState.Jumping);
            }
            else
            {
                stateMachine.ChangeState(PlayerState.Falling);
            }
        }
        else if (Mathf.Abs(velocity.x) > 0.1f)
        {
            stateMachine.ChangeState(PlayerState.Moving);
        }
        else
        {
            stateMachine.ChangeState(PlayerState.Idle);
        }
    }

    // ==================== PUBLIC API FOR INTERACTIONS ====================

    /// <summary>
    /// Sit down on a chair
    /// </summary>
    public void SitOnChair(chair chairToSit)
    {
        if (stateMachine.CurrentState == PlayerState.Sitting)
        {
            Debug.LogWarning("Already sitting!");
            return;
        }

        currentChair = chairToSit;
        transform.position = chairToSit.sitpoint.position;
        
        movement.StopMovement();
        movement.SetGravityEnabled(false);
        
        stateMachine.ChangeState(PlayerState.Sitting);
        
        Debug.Log("Player sat down on chair");
    }

    /// <summary>
    /// Leave the current chair and stand up
    /// </summary>
    public void LeaveChair()
    {
        if (stateMachine.CurrentState != PlayerState.Sitting)
        {
            return;
        }

        // Move slightly upward to avoid re-triggering the chair
        transform.position += new Vector3(0f, 0.5f, 0f);
        
        movement.SetGravityEnabled(true);
        stateMachine.ChangeState(PlayerState.Idle);
        
        currentChair = null;
        
        Debug.Log("Player left the chair");
    }

    /// <summary>
    /// Start rappelling down a cable
    /// </summary>
    public void StartRappelling(Transform targetPosition)
    {
        if (stateMachine.CurrentState == PlayerState.Rappelling)
        {
            Debug.LogWarning("Already rappelling!");
            return;
        }

        stateMachine.ChangeState(PlayerState.Rappelling);
        StartCoroutine(RappelSequence(targetPosition));
    }

    /// <summary>
    /// Coroutine that handles the rappelling sequence
    /// </summary>
    private IEnumerator RappelSequence(Transform targetPosition)
    {
        // Hide player sprite during rappelling animation
        animationController.SetVisible(false);
        movement.StopMovement();
        movement.SetGravityEnabled(false);

        Debug.Log("Rappelling started");

        // Wait for rappelling animation duration
        yield return new WaitForSeconds(rappellingDuration);

        // Teleport to target position
        movement.Teleport(targetPosition.position);
        movement.SetGravityEnabled(true);

        // Show player sprite again
        animationController.SetVisible(true);

        // Return to normal state
        stateMachine.ChangeState(PlayerState.Idle);

        Debug.Log("Rappelling completed");
    }

    // ==================== TRIGGER DETECTION ====================

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player triggered: " + collision.gameObject.name);

        // Handle key pickup
        if (collision.CompareTag("key1") && interactionHandler.GetCarriedItem() == null)
        {
            interactionHandler.PickUpItem(collision.gameObject);
        }

        // Handle interactable objects
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable != null)
        {
            Debug.Log("Found interactable: " + collision.gameObject.name);
            interactionHandler.RegisterInteractable(interactable);
            interactable.OnPlayerEnterZone(this);
        }
        else
        {
            Debug.Log("No IInteractable component found on: " + collision.gameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Handle interactable objects
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactionHandler.UnregisterInteractable(interactable);
            interactable.OnPlayerExitZone(this);
        }
    }

    // ==================== PUBLIC ACCESSORS ====================

    /// <summary>
    /// Get the carried item (for door checking, etc.)
    /// </summary>
    public GameObject GetCarriedItem()
    {
        return interactionHandler.GetCarriedItem();
    }

    /// <summary>
    /// Use the currently carried item (called by doors, etc.)
    /// </summary>
    public void UseCarriedItem()
    {
        interactionHandler.UseCarriedItem();
    }

    /// <summary>
    /// Get the current player state
    /// </summary>
    public PlayerState GetCurrentState()
    {
        return stateMachine.CurrentState;
    }

    /// <summary>
    /// Check if player is currently sitting
    /// </summary>
    public bool IsSitting()
    {
        return stateMachine.CurrentState == PlayerState.Sitting;
    }

    /// <summary>
    /// Get the interaction handler component
    /// </summary>
    public InteractionHandler GetInteractionHandler()
    {
        return interactionHandler;
    }
}

