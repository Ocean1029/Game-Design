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
    [SerializeField] private float jumpForce = 8f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float currentGravityScale = 1f;

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
    /// Make the player jump if grounded
    /// </summary>
    public void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
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

