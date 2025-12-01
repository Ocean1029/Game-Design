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
    [SerializeField] private float runSpeedMultiplier = 1.8f;

    [Header("Jump Settings")]
    [Tooltip("Initial upward force when jump starts")]
    [SerializeField] private float jumpForce = 8.5f;
    
    [Tooltip("Additional upward acceleration while holding jump button")]
    [SerializeField] private float jumpHoldAcceleration = 8f;
    
    [Tooltip("Maximum time the player can hold jump button (in seconds)")]
    [SerializeField] private float maxJumpHoldTime = 0.3f;
    
    [Tooltip("Multiplier for downward velocity when jump button is released early")]
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    [Header("Jump Sound")]
    [Tooltip("Sound played when player jumps (optional)")]
    [SerializeField] private AudioClip jumpSound;
    
    [Tooltip("Volume of jump sound (0.0 to 1.0)")]
    [SerializeField, Range(0f, 1f)] private float jumpSoundVolume = 0.6f;

    [Header("Landing Sound")]
    [Tooltip("Sound played when player lands on the ground (optional)")]
    [SerializeField] private AudioClip landingSound;
    
    [Tooltip("Volume of landing sounds (0.0 to 10.0, can exceed 1.0 for louder sounds)")]
    [SerializeField, Range(0f, 10f)] private float landingSoundVolume = 7.5f;

    [Header("Gravity Settings")]
    [Tooltip("Gravity scale multiplier for the Rigidbody2D")]
    [SerializeField] private float gravityScale = 2f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Step Up Settings")]
    [Tooltip("Maximum height the player can automatically step over (in units)")]
    [SerializeField] private float maxStepHeight = 0.4f;
    
    [Tooltip("Distance ahead of player to check for obstacles")]
    [SerializeField] private float stepCheckDistance = 0.4f;
    
    [Tooltip("Upward force applied when stepping over obstacles")]
    [SerializeField] private float stepUpForce = 4f;
    
    [Tooltip("Vertical offset from ground check for step detection (should be slightly above ground)")]
    [SerializeField] private float stepCheckVerticalOffset = 0.1f;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private bool isGrounded;
    private bool wasGroundedPreviousFrame; // Track previous frame's grounded state for landing detection
    private float currentGravityScale = 1f;
    
    // Jump state tracking
    private bool isJumping = false;
    private float jumpTimeCounter = 0f;

    // Movement input caching (set in Update, applied in FixedUpdate)
    private float horizontalInput = 0f;
    private bool shouldApplyMovement = false;
    private bool isRunning = false;
    
    // Jump input caching (set in Update, applied in FixedUpdate)
    private bool shouldContinueJump = false;
    
    // Flag to lock movement (used when sitting, rappelling, etc.)
    private bool isMovementLocked = false;

    // Sound manager reference (cached for performance)
    private SoundManager soundManager;
    
    // AudioSource for landing sound playback (similar to footstep sound implementation)
    private AudioSource landingAudioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Set gravity scale from serialized field
        rb.gravityScale = gravityScale;
        currentGravityScale = gravityScale;
        
        // Get player collider for step detection
        playerCollider = GetComponent<Collider2D>();
        if (playerCollider == null)
        {
            Debug.LogWarning("PlayerMovement: No Collider2D found on player. Step up feature may not work correctly.");
        }
        
        // Initialize sound manager reference
        soundManager = SoundManager.GetInstance();
        
        // Initialize AudioSource component for landing sounds
        InitializeLandingAudioSource();
    }
    
    /// <summary>
    /// Initialize the AudioSource component for landing sound playback
    /// Configured as 2D sound to avoid distance attenuation issues
    /// Uses same approach as PlayerFootstepSound for consistent volume control
    /// </summary>
    private void InitializeLandingAudioSource()
    {
        landingAudioSource = GetComponent<AudioSource>();
        if (landingAudioSource == null)
        {
            landingAudioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configure AudioSource for 2D playback (no spatialization)
        // This ensures landing sounds are not affected by distance attenuation
        landingAudioSource.spatialBlend = 0f; // 0 = 2D sound, 1 = 3D sound
        landingAudioSource.playOnAwake = false;
        landingAudioSource.loop = false;
        landingAudioSource.volume = landingSoundVolume;
    }

    void FixedUpdate()
    {
        // Store previous frame's grounded state before checking
        wasGroundedPreviousFrame = isGrounded;
        
        CheckGrounded();
        
        // Detect landing event: transition from air to ground
        if (!wasGroundedPreviousFrame && isGrounded)
        {
            PlayLandingSound();
        }
        
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
            // Check for step up before applying movement
            if (isGrounded && !isJumping && Mathf.Abs(horizontalInput) > 0.01f)
            {
                TryStepUp(horizontalInput);
            }
            
            // Apply run speed multiplier if running
            float currentSpeed = isRunning ? moveSpeed * runSpeedMultiplier : moveSpeed;
            rb.linearVelocity = new Vector2(horizontalInput * currentSpeed, rb.linearVelocity.y);
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
    /// <param name="running">Whether the player is running (applies speed multiplier)</param>
    public void Move(float horizontal, bool running = false)
    {
        horizontalInput = horizontal;
        isRunning = running;
        shouldApplyMovement = true;
    }
    
    /// <summary>
    /// Get whether the player is currently running
    /// </summary>
    public bool IsRunning()
    {
        return isRunning;
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

    // ==================== Landing Sound ====================

    /// <summary>
    /// Play landing sound using dedicated AudioSource component
    /// Uses same approach as PlayerFootstepSound for consistent volume control
    /// Called automatically when player transitions from air to ground
    /// </summary>
    private void PlayLandingSound()
    {
        if (landingSound == null)
        {
            return;
        }

        // Ensure AudioSource is initialized
        if (landingAudioSource == null)
        {
            InitializeLandingAudioSource();
        }

        // Set AudioSource volume to landingSoundVolume (can be 0.0 to 10.0)
        // AudioSource.volume can exceed 1.0 to amplify sound beyond normal range
        landingAudioSource.volume = landingSoundVolume;

        // Play sound using AudioSource (2D sound, no distance attenuation)
        // Use volume scale of 1.0 since AudioSource.volume already controls the amplification
        landingAudioSource.PlayOneShot(landingSound, 1f);
    }

    /// <summary>
    /// Set the landing sound clip
    /// </summary>
    public void SetLandingSound(AudioClip clip)
    {
        landingSound = clip;
    }

    /// <summary>
    /// Get the current landing sound clip
    /// </summary>
    public AudioClip GetLandingSound()
    {
        return landingSound;
    }

    /// <summary>
    /// Set the landing sound volume (0.0 to 10.0)
    /// Updates the AudioSource volume immediately if it exists
    /// </summary>
    public void SetLandingSoundVolume(float volume)
    {
        landingSoundVolume = Mathf.Clamp(volume, 0f, 10f);
        
        // Update AudioSource volume if it exists
        if (landingAudioSource != null)
        {
            landingAudioSource.volume = landingSoundVolume;
        }
    }

    /// <summary>
    /// Get the current landing sound volume
    /// </summary>
    public float GetLandingSoundVolume()
    {
        return landingSoundVolume;
    }

    /// <summary>
    /// Attempts to step over small obstacles in the movement direction
    /// Uses raycast to detect obstacles and their height
    /// </summary>
    /// <param name="moveDirection">Direction of movement (-1 for left, 1 for right)</param>
    private void TryStepUp(float moveDirection)
    {
        if (groundCheck == null || playerCollider == null)
        {
            return;
        }

        // Calculate detection point position at player's feet level, offset forward
        Vector2 detectionStart = groundCheck.position;
        detectionStart.x += moveDirection * stepCheckDistance;

        // Raycast forward horizontally to detect if there's an obstacle blocking movement
        Vector2 rayDirection = Vector2.right * moveDirection;
        float rayDistance = 0.2f; // Short distance to detect immediate obstacles
        RaycastHit2D forwardHit = Physics2D.Raycast(detectionStart, rayDirection, rayDistance, groundLayer);

        // If we hit something, it means there's an obstacle in front
        if (forwardHit.collider != null)
        {
            // Get the obstacle's bounds to find its top
            Bounds obstacleBounds = forwardHit.collider.bounds;
            float obstacleTop = obstacleBounds.max.y;

            // Get the player's current ground level (at ground check position)
            float playerGroundLevel = groundCheck.position.y;

            // Calculate height difference between obstacle top and player ground level
            float heightDifference = obstacleTop - playerGroundLevel;

            // Check if the obstacle is within step-up range
            // Minimum 0.05f to avoid stepping over tiny bumps, maximum is maxStepHeight
            if (heightDifference > 0.05f && heightDifference <= maxStepHeight)
            {
                // Check if there's enough space above the obstacle for the player to move into
                // Raycast upward from the top of the obstacle to check for ceiling
                Vector2 topCheckStart = new Vector2(forwardHit.point.x, obstacleTop + 0.01f);
                float playerHeight = playerCollider.bounds.size.y;
                float clearanceCheck = playerHeight + 0.1f; // Extra clearance for safety
                RaycastHit2D ceilingCheck = Physics2D.Raycast(topCheckStart, Vector2.up, clearanceCheck, groundLayer);

                // If there's no ceiling blocking us, apply step-up force
                if (ceilingCheck.collider == null)
                {
                    // Apply upward velocity to step over the obstacle
                    // Only step up if player is not already moving up significantly (to avoid interfering with jumps)
                    if (rb.linearVelocity.y <= 0.1f)
                    {
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, stepUpForce);
                    }
                }
            }
        }
    }

    // Draw ground check gizmo in editor
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Draw step detection gizmos for debugging
        if (groundCheck != null && Application.isPlaying && Mathf.Abs(horizontalInput) > 0.01f)
        {
            float moveDirection = Mathf.Sign(horizontalInput);
            Vector2 detectionStart = groundCheck.position;
            detectionStart.x += moveDirection * stepCheckDistance;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(detectionStart, 0.05f);
            Gizmos.DrawLine(detectionStart, detectionStart + Vector2.right * moveDirection * 0.2f);
        }
    }
}

