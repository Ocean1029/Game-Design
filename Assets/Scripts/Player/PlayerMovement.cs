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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentGravityScale = rb.gravityScale;
    }

    void FixedUpdate()
    {
        CheckGrounded();
    }

    /// <summary>
    /// Move the player horizontally
    /// </summary>
    /// <param name="horizontal">Input value (-1 for left, 1 for right, 0 for no movement)</param>
    public void Move(float horizontal)
    {
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
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
            
            Debug.Log("Jump started");
        }
    }

    /// <summary>
    /// Continue applying upward force while jump button is held
    /// Call this every frame while the button is held down
    /// </summary>
    public void ContinueJump()
    {
        if (isJumping)
        {
            // Continue only if within max hold time and still moving upward
            if (jumpTimeCounter < maxJumpHoldTime && rb.linearVelocity.y > 0)
            {
                // Apply continuous upward acceleration
                rb.linearVelocity += Vector2.up * jumpHoldAcceleration * Time.deltaTime;
                jumpTimeCounter += Time.deltaTime;
            }
            else
            {
                // Max time reached or started falling, stop jump boost
                isJumping = false;
            }
        }
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

