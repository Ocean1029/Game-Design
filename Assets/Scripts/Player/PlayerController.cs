using UnityEngine;
using System.Collections;

/// <summary>
/// Main player controller that coordinates all player subsystems
/// Manages input, state, movement, animation, and interactions
/// Implements IInteractor to allow interaction with game objects
/// </summary>
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerStateMachine))]
[RequireComponent(typeof(PlayerAnimationController))]
[RequireComponent(typeof(InteractionHandler))]
public class PlayerController : MonoBehaviour, IInteractor
{
    [Header("Component References")]
    private PlayerMovement movement;
    private PlayerStateMachine stateMachine;
    private PlayerAnimationController animationController;
    private InteractionHandler interactionHandler;
    private PlayerEnergy energySystem;

    [Header("Input Keys")]
    [SerializeField] private KeyCode moveLeftKey = KeyCode.LeftArrow;
    [SerializeField] private KeyCode moveRightKey = KeyCode.RightArrow;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode interactKey = KeyCode.Z;  // 改用 Z 鍵來互動（坐下、起立、使用鑰匙、使用炸彈等）
    [SerializeField] private KeyCode respawnKey = KeyCode.R;
    [SerializeField] private KeyCode fastTravelMenuKey = KeyCode.M;

    [Header("Rappelling Settings")]
    [SerializeField] private float rappellingDuration = 1.0f;

    [Header("Rendering Settings")]
    [Tooltip("Z index (Sorting Order) for player sprite renderers. Higher values render on top")]
    [SerializeField] private int playerZIndex = 1;

    // Chair interaction state
    private chair currentChair = null;

    // Movement state tracking for animation triggers
    private bool wasMoving = false;

    // Sprite renderer references for z index management
    private SpriteRenderer[] spriteRenderers;

    // Track the trip coroutine so we can stop it when pressing R
    private Coroutine tripCoroutine = null;
    private PlayerState lastState = PlayerState.Idle; // For debug logging

    void OnEnable()
    {
        if (energySystem == null)
            energySystem = GetComponent<PlayerEnergy>();
            
        if (energySystem != null)
        {
            energySystem.OnFirstEnergyDepletion += HandleTrip;
        }
    }

    void OnDisable()
    {
        if (energySystem != null)
        {
            energySystem.OnFirstEnergyDepletion -= HandleTrip;
        }
    }

    void Awake()
    {
        // Get all required components
        movement = GetComponent<PlayerMovement>();
        stateMachine = GetComponent<PlayerStateMachine>();
        animationController = GetComponent<PlayerAnimationController>();
        interactionHandler = GetComponent<InteractionHandler>();
        energySystem = GetComponent<PlayerEnergy>();

        if (energySystem == null)
        {
            Debug.LogWarning("PlayerController: PlayerEnergy component not found! Jump energy system will be disabled.");
        }

        // Get all SpriteRenderer components in player and children
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        
        // Apply initial z index setting
        SetPlayerZIndex(playerZIndex);
    }

    void Start()
    {
        // Initialize to idle state
        stateMachine.ChangeState(PlayerState.Idle);
    }

    void Update()
    {
        // Debug current state at start of each update
        if (stateMachine.CurrentState != lastState)
        {
            Debug.Log($"PlayerController: State changed from {lastState} to {stateMachine.CurrentState}");
            lastState = stateMachine.CurrentState;
        }

        // Check R key first (before any input locking)
        if (Input.GetKeyDown(respawnKey))
        {
            Debug.Log($"PlayerController: R key pressed! Current state: {stateMachine.CurrentState}");

            // Allow respawn if tripping
            if (stateMachine.CurrentState == PlayerState.Tripping)
            {
                Debug.Log("PlayerController: R key detected while Tripping! Calling RespawnAtLastChair...");
                RespawnAtLastChair();
                return;
            }
        }

        // Don't process input if in locked state
        if (stateMachine.IsInputLocked())
        {
            return;
        }

        HandleMovementInput();
        HandleJumpInput();
        HandleInteractionInput();
        HandleSystemInput();
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
            // Clear input when movement is not allowed (e.g., when sitting or tripping)
            // This prevents cached input from being applied in FixedUpdate
            if (stateMachine.CurrentState == PlayerState.Sitting || 
                stateMachine.CurrentState == PlayerState.Tripping)
            {
                movement.ClearInput();
            }
            return;
        }

        float horizontal = 0f;
        bool isMoving = false;

        if (Input.GetKey(moveRightKey))
        {
            horizontal = 1f;
            animationController.SetFacingDirection(true);
            isMoving = true;
        }
        else if (Input.GetKey(moveLeftKey))
        {
            horizontal = -1f;
            animationController.SetFacingDirection(false);
            isMoving = true;
        }

        // Trigger walk animation when movement starts (transition from idle to moving)
        if (isMoving && !wasMoving && movement.IsGrounded() && animationController.IsAnimationSystemReady())
        {
            animationController.TriggerWalk();
        }

        // Update movement state tracking
        wasMoving = isMoving;

        movement.Move(horizontal);
    }

    /// <summary>
    /// Handle jump input - supports variable height jumping with energy cost
    /// </summary>
    private void HandleJumpInput()
    {
        // Start jump when button is first pressed
        if (stateMachine.CanJump() && Input.GetKeyDown(jumpKey))
        {
            // Check if player has enough energy to jump
            if (energySystem != null)
            {
                if (!energySystem.HasEnergyToJump())
                {
                    Debug.Log("Cannot jump - not enough energy!");
                    return;
                }

                // Consume energy for the jump
                if (!energySystem.ConsumeJumpEnergy())
                {
                    return;
                }
            }

            movement.StartJump();
            if (animationController.IsAnimationSystemReady())
            {
                animationController.TriggerJump();
            }
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
            Debug.Log("Z key pressed - attempting interaction");
            // Try standard interaction first
            if (interactionHandler.TryInteract(this))
            {
                Debug.Log("Interaction successful!");
                if (animationController.IsAnimationSystemReady())
                {
                    animationController.TriggerInteract();
                }
            }
            else
            {
                Debug.Log("No interactable object nearby");
            }
        }

        // Exit interaction is now handled by the interact key (Z)
        // No longer need a separate exit key
    }

    /// <summary>
    /// Handle system input (R key for respawn, M key for fast travel menu, Z for chair interaction)
    /// </summary>
    private void HandleSystemInput()
    {
        // Respawn at last chair
        if (Input.GetKeyDown(respawnKey))
        {
            RespawnAtLastChair();
        }

        // Toggle fast travel menu
        if (Input.GetKeyDown(fastTravelMenuKey))
        {
            ToggleFastTravelMenu();
        }

        // Handle chair interaction with Z key
        // Note: This is now handled by the interaction system (InteractionHandler)
        // The Z key is mapped to the interact key
    }

    /// <summary>
    /// Update animation parameters based on current movement
    /// </summary>
    private void UpdateAnimations()
    {
        // Only update animations if the animation system is properly configured
        if (!animationController.IsAnimationSystemReady())
        {
            return;
        }

        Vector2 velocity = movement.GetVelocity();
        bool isGrounded = movement.IsGrounded();

        animationController.SetSpeed(Mathf.Abs(velocity.x));
        animationController.SetGrounded(isGrounded);
        animationController.SetSitting(stateMachine.CurrentState == PlayerState.Sitting);

        // Update jump animation dynamically based on vertical velocity
        if (!isGrounded)
        {
            // Get height above ground for better animation timing
            float heightAboveGround = CalculateHeightAboveGround();

            // Get gravity scale from rigidbody
            Rigidbody2D rb = movement.GetComponent<Rigidbody2D>();
            float gravityScale = rb != null ? rb.gravityScale : 1f;

            animationController.UpdateJumpAnimation(velocity.y, isGrounded, heightAboveGround, gravityScale);
        }
    }

    /// <summary>
    /// Update movement-related state based on velocity and ground status
    /// </summary>
    private void UpdateMovementState()
    {
        // Don't update if in special states
        if (stateMachine.CurrentState == PlayerState.Sitting ||
            stateMachine.CurrentState == PlayerState.Rappelling ||
            stateMachine.CurrentState == PlayerState.Cutscene ||
            stateMachine.CurrentState == PlayerState.Tripping)
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

    // ==================== HELPER METHODS ====================

    /// <summary>
    /// Calculate height above ground using raycast
    /// </summary>
    private float CalculateHeightAboveGround()
    {
        // Cast ray downward to detect ground
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 50f, LayerMask.GetMask("Ground"));

        if (hit.collider != null)
        {
            return transform.position.y - hit.point.y;
        }

        // Fallback: assume reasonable height if no ground detected
        return 2f;
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


        // Stop movement and lock it to prevent any input from being applied
        movement.StopMovement();
        movement.SetMovementLocked(true);
        movement.SetGravityEnabled(false);

        stateMachine.ChangeState(PlayerState.Sitting);

        // Start restoring energy while sitting
        if (energySystem != null)
        {
            energySystem.StartEnergyRestore();
        }

        Debug.Log("Player sat down on chair - energy restoring");
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

        // Stop restoring energy when leaving chair
        if (energySystem != null)
        {
            energySystem.StopEnergyRestore();
        }

        // Unlock movement before changing position
        movement.SetMovementLocked(false);

        // Move slightly upward to avoid re-triggering the chair
        transform.position += new Vector3(0f, 0.5f, 0f);

        movement.SetGravityEnabled(true);
        stateMachine.ChangeState(PlayerState.Idle);

        currentChair = null;

        FastTravelUI fastTravelUI = FindFirstObjectByType<FastTravelUI>();
        if (fastTravelUI != null && fastTravelUI.IsOpen())
        {
            fastTravelUI.CloseFastTravelUI();
        }

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
        movement.SetMovementLocked(true);
        movement.SetGravityEnabled(false);

        Debug.Log("Rappelling started");

        // Wait for rappelling animation duration
        yield return new WaitForSeconds(rappellingDuration);

        // Teleport to target position
        movement.Teleport(targetPosition.position);
        movement.SetMovementLocked(false);
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

        // Key pickup is now handled by Key component itself (automatic collection)

        // Handle interactable objects
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable != null)
        {
            Debug.Log("Found interactable: " + collision.gameObject.name);
            interactionHandler.RegisterInteractable(interactable);
            interactable.OnInteractorEnterZone(this);
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
            interactable.OnInteractorExitZone(this);
        }
    }

    // ==================== SYSTEM METHODS ====================

    /// <summary>
    /// Respawn player at the last chair they sat on
    /// Uses GameManager
    /// </summary>
    private void RespawnAtLastChair()
    {
        Debug.Log("PlayerController: RespawnAtLastChair called");

        // Force state reset if we were tripping
        if (stateMachine.CurrentState == PlayerState.Tripping)
        {
            Debug.Log("PlayerController: Recovering from trip via respawn - starting recovery...");
            
            // Stop the trip coroutine if it's running
            if (tripCoroutine != null)
            {
                Debug.Log("PlayerController: Stopping trip coroutine");
                StopCoroutine(tripCoroutine);
                tripCoroutine = null;
            }
            
            Debug.Log("PlayerController: Changing state to Idle");
            stateMachine.ChangeState(PlayerState.Idle);
            
            Debug.Log("PlayerController: Unlocking movement");
            movement.SetMovementLocked(false);
            movement.SetGravityEnabled(true);

            // Trigger Recover animation to transition out of Trip
            if (animationController.IsAnimationSystemReady())
            {
                Debug.Log("PlayerController: Triggering Recover animation");
                animationController.TriggerRecover();
            }
            else
            {
                Debug.LogWarning("PlayerController: Animation system not ready!");
            }
            
            // Restore energy since we are respawning (presumably at a save point/chair)
            if (energySystem != null)
            {
                Debug.Log("PlayerController: Restoring all energy");
                energySystem.RestoreAllEnergy();
            }
            
            Debug.Log("PlayerController: Recovery from trip complete!");
        }

        Debug.Log("PlayerController: Getting GameManager for respawn");
        GameManager gameManager = GameManager.GetInstance();
        if (gameManager == null)
        {
            Debug.LogError("PlayerController: GameManager not found! Cannot respawn.");
            return;
        }

        Debug.Log("PlayerController: Calling GameManager.RespawnPlayer()");
        gameManager.RespawnPlayer();
    }

    /// <summary>
    /// Toggle the fast travel menu
    /// </summary>
    private void ToggleFastTravelMenu()
    {
        Debug.Log("PlayerController: M key pressed - attempting to toggle fast travel menu");

        if (!IsSitting())
        {
            Debug.Log("PlayerController: Fast travel menu unavailable - player is not sitting");
            return;
        }

        // Fallback to original FastTravelUI
        FastTravelUI fastTravelUI = FindFirstObjectByType<FastTravelUI>();

        if (fastTravelUI != null)
        {
            Debug.Log("PlayerController: FastTravelUI found, toggling menu...");
            if (fastTravelUI.IsOpen())
            {
                fastTravelUI.CloseFastTravelUI();
            }
            else
            {
                fastTravelUI.OpenFastTravelUI();
            }
            return;
        }

        Debug.LogWarning("PlayerController: No Fast Travel UI found in scene!");
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

    /// <summary>
    /// Get the energy system component
    /// </summary>
    public PlayerEnergy GetEnergySystem()
    {
        return energySystem;
    }

    // ==================== IInteractor IMPLEMENTATION ====================

    /// <summary>
    /// Get the transform of this interactor
    /// </summary>
    Transform IInteractor.GetTransform()
    {
        return transform;
    }

    /// <summary>
    /// Get the GameObject of this interactor
    /// </summary>
    GameObject IInteractor.GetGameObject()
    {
        return gameObject;
    }

    // ==================== Z INDEX MANAGEMENT ====================

    /// <summary>
    /// Set the z index (Sorting Order) for all player sprite renderers
    /// Higher values render on top of lower values
    /// </summary>
    /// <param name="zIndex">The sorting order value to set</param>
    public void SetPlayerZIndex(int zIndex)
    {
        playerZIndex = zIndex;

        // Refresh sprite renderer references if needed
        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        }

        // Apply z index to all sprite renderers
        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            if (renderer != null)
            {
                renderer.sortingOrder = zIndex;
            }
        }
    }

    /// <summary>
    /// Get the current z index (Sorting Order) of the player
    /// </summary>
    /// <returns>The current sorting order value</returns>
    public int GetPlayerZIndex()
    {
        return playerZIndex;
    }

    // ==================== TRIP LOGIC ====================

    private void HandleTrip()
    {
        // Only trip if not already in a critical state
        if (stateMachine.CurrentState != PlayerState.Cutscene && 
            stateMachine.CurrentState != PlayerState.Rappelling &&
            stateMachine.CurrentState != PlayerState.Tripping)
        {
            tripCoroutine = StartCoroutine(TripRoutine());
        }
    }

    private IEnumerator TripRoutine()
    {
        Debug.Log("PlayerController: TripRoutine started! Setting state to Tripping...");

        stateMachine.ChangeState(PlayerState.Tripping);
        Debug.Log($"PlayerController: State changed. Current state: {stateMachine.CurrentState}");

        movement.StopMovement(); // Ensure physics stop
        movement.SetMovementLocked(true); // Lock movement to prevent player from moving
        Debug.Log("PlayerController: Movement locked");

        if (animationController.IsAnimationSystemReady())
        {
            animationController.TriggerTrip();
            Debug.Log("PlayerController: Trip animation triggered");
        }

        Debug.Log("PlayerController: Waiting 2 seconds...");
        // Wait for animation or fixed time
        yield return new WaitForSeconds(2.0f); // 2 seconds trip time

        Debug.Log($"PlayerController: 2 seconds elapsed. Current state: {stateMachine.CurrentState}");

        // Restore to idle if we are still tripping (haven't been interrupted by cutscene etc)
        if (stateMachine.CurrentState == PlayerState.Tripping)
        {
            Debug.Log("PlayerController: Auto-recovering from trip (2 seconds passed)");
            movement.SetMovementLocked(false); // Unlock movement
            stateMachine.ChangeState(PlayerState.Idle);
        }
        else
        {
            Debug.Log($"PlayerController: Not auto-recovering - state changed to: {stateMachine.CurrentState}");
        }

        // Clear the coroutine reference
        tripCoroutine = null;
        Debug.Log("PlayerController: TripRoutine ended");
    }
}

