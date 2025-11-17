using UnityEngine;

/// <summary>
/// Handles player movement physics including walking, jumping, and gravity
/// This component should be controlled by PlayerController
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump Settings")]
    [Tooltip("Initial upward force when jump starts")]
    [SerializeField] private float jumpForce = 8f;
    
    [Tooltip("Additional upward acceleration while holding jump button")]
    [SerializeField] private float jumpHoldAcceleration = 15f;
    
    [Tooltip("Maximum time the player can hold jump button (in seconds)")]
    [SerializeField] private float maxJumpHoldTime = 0.3f;
    
    [Tooltip("Multiplier for downward velocity when jump button is released early")]
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    [Header("Jump Sound")]
    [Tooltip("Sound played when player jumps (optional)")]
    [SerializeField] private AudioClip jumpSound;
    
    [Tooltip("Volume of jump sound (0.0 to 1.0)")]
    [SerializeField, Range(0f, 1f)] private float jumpSoundVolume = 0.6f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float currentGravityScale = 1f;
    
    // Jump state tracking
    private bool isJumping = false;
    private float jumpTimeCounter = 0f;

    // Movement input caching (set in Update, applied in FixedUpdate)
    private float horizontalInput = 0f;
    private bool shouldApplyMovement = false;
    
    // Jump input caching (set in Update, applied in FixedUpdate)
    private bool shouldContinueJump = false;
    
    // Flag to lock movement (used when sitting, rappelling, etc.)
    private bool isMovementLocked = false;

    // Sound manager reference (cached for performance)
    private SoundManager soundManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentGravityScale = rb.gravityScale;
        
        // Initialize sound manager reference
        soundManager = SoundManager.GetInstance();
    }

    void FixedUpdate()
    {
        CheckGrounded();
        
        // Don't apply movement if movement is locked (e.g., when sitting)
        if (isMovementLocked)
        {
            // Clear any pending movement input
            shouldApplyMovement = false;
            horizontalInput = 0f;
            // Keep velocity at zero to prevent any movement
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }
        
        // Apply movement in FixedUpdate to ensure physics consistency
        if (shouldApplyMovement)
        {
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
            shouldApplyMovement = false;
        }
        
        // Apply jump continuation in FixedUpdate for physics consistency
        if (shouldContinueJump)
        {
            if (isJumping)
            {
                // Continue only if within max hold time and still moving upward
                if (jumpTimeCounter < maxJumpHoldTime && rb.linearVelocity.y > 0)
                {
                    // Apply continuous upward acceleration using fixedDeltaTime
                    rb.linearVelocity += Vector2.up * jumpHoldAcceleration * Time.fixedDeltaTime;
                    jumpTimeCounter += Time.fixedDeltaTime;
                }
                else
                {
                    // Max time reached or started falling, stop jump boost
                    isJumping = false;
                }
            }
            shouldContinueJump = false;
        }
    }

    /// <summary>
    /// Set horizontal movement input (call from Update)
    /// The actual velocity will be applied in FixedUpdate for physics consistency
    /// </summary>
    /// <param name="horizontal">Input value (-1 for left, 1 for right, 0 for no movement)</param>
    public void Move(float horizontal)
    {
        horizontalInput = horizontal;
        shouldApplyMovement = true;
    }

    /// <summary>
    /// Start the jump - called when jump button is first pressed
    /// </summary>
    public void StartJump()
    {
        if (isGrounded)
        {
            // Apply initial jump force
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            
            // Begin tracking jump hold time
            isJumping = true;
            jumpTimeCounter = 0f;
            
            // Play jump sound
            PlayJumpSound();
            
            Debug.Log("Jump started");
        }
    }

    /// <summary>
    /// Continue applying upward force while jump button is held
    /// Call this every frame while the button is held down (from Update)
    /// The actual force will be applied in FixedUpdate for physics consistency
    /// </summary>
    public void ContinueJump()
    {
        shouldContinueJump = true;
    }

    /// <summary>
    /// Stop jump early when button is released
    /// This allows for variable jump height
    /// </summary>
    public void StopJump()
    {
        if (isJumping && rb.linearVelocity.y > 0)
        {
            // Cut the upward velocity when button is released
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            isJumping = false;
            
            Debug.Log("Jump stopped early");
        }
    }

    /// <summary>
    /// Check if player is currently in a jump
    /// </summary>
    public bool IsJumping()
    {
        return isJumping;
    }

    /// <summary>
    /// Stop all movement immediately
    /// </summary>
    public void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
        // Clear input cache to prevent movement in next FixedUpdate
        ClearInput();
    }
    
    /// <summary>
    /// Clear all cached input (movement and jump)
    /// Useful when transitioning to states that should not accept input
    /// </summary>
    public void ClearInput()
    {
        horizontalInput = 0f;
        shouldApplyMovement = false;
        shouldContinueJump = false;
    }

    /// <summary>
    /// Enable or disable gravity (useful for sitting, rappelling, etc.)
    /// </summary>
    /// <param name="enabled">Whether gravity should be enabled</param>
    public void SetGravityEnabled(bool enabled)
    {
        rb.gravityScale = enabled ? currentGravityScale : 0f;
    }
    
    /// <summary>
    /// Lock or unlock movement (prevents any movement input from being applied)
    /// When locked, movement input is ignored and velocity is kept at zero
    /// </summary>
    /// <param name="locked">Whether movement should be locked</param>
    public void SetMovementLocked(bool locked)
    {
        isMovementLocked = locked;
        if (locked)
        {
            // Clear input and stop movement when locking
            ClearInput();
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    /// <summary>
    /// Teleport the player to a specific position
    /// </summary>
    /// <param name="position">Target position</param>
    public void Teleport(Vector3 position)
    {
        transform.position = position;
        StopMovement();
    }

    /// <summary>
    /// Check if the player is on the ground
    /// </summary>
    private void CheckGrounded()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
    }

    /// <summary>
    /// Get whether the player is currently grounded
    /// </summary>
    public bool IsGrounded()
    {
        return isGrounded;
    }

    /// <summary>
    /// Get the current velocity
    /// </summary>
    public Vector2 GetVelocity()
    {
        return rb.linearVelocity;
    }

    // ==================== Jump Sound ====================

    /// <summary>
    /// Play jump sound using SoundManager with backward compatibility fallback
    /// </summary>
    private void PlayJumpSound()
    {
        if (jumpSound == null)
        {
            return;
        }

        // Get sound position (player's position)
        Vector3 soundPosition = transform.position;

        // Play sound through SoundManager with fallback
        if (soundManager != null)
        {
            soundManager.PlaySound(jumpSound, soundPosition, jumpSoundVolume);
        }
        else
        {
            // Fallback to direct playback if SoundManager is not available
            AudioSource.PlayClipAtPoint(jumpSound, soundPosition, jumpSoundVolume);
        }
    }

    /// <summary>
    /// Set the jump sound clip
    /// </summary>
    public void SetJumpSound(AudioClip clip)
    {
        jumpSound = clip;
    }

    /// <summary>
    /// Get the current jump sound clip
    /// </summary>
    public AudioClip GetJumpSound()
    {
        return jumpSound;
    }

    /// <summary>
    /// Set the jump sound volume
    /// </summary>
    public void SetJumpSoundVolume(float volume)
    {
        jumpSoundVolume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// Get the current jump sound volume
    /// </summary>
    public float GetJumpSoundVolume()
    {
        return jumpSoundVolume;
    }

    // Draw ground check gizmo in editor
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}

