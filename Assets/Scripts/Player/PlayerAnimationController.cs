using UnityEngine;

/// <summary>
/// Manages player animation parameters and visual effects
/// Provides a clean interface to control animations without directly exposing the Animator
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Animation parameter names (configure these to match your Animator Controller)
    private readonly string PARAM_SPEED = "Speed";
    private readonly string PARAM_IS_GROUNDED = "IsGrounded";
    private readonly string PARAM_IS_SITTING = "IsSitting";
    private readonly string PARAM_JUMP_TRIGGER = "Jump";
    private readonly string PARAM_INTERACT_TRIGGER = "Interact";

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning("PlayerAnimationController: SpriteRenderer not found on player object!");
        }
    }

    /// <summary>
    /// Set the movement speed parameter for blend trees
    /// </summary>
    /// <param name="speed">Absolute speed value (0 = idle, > 0 = moving)</param>
    public void SetSpeed(float speed)
    {
        if (animator != null && HasParameter(PARAM_SPEED))
        {
            animator.SetFloat(PARAM_SPEED, Mathf.Abs(speed));
        }
    }

    /// <summary>
    /// Set whether the player is on the ground
    /// </summary>
    public void SetGrounded(bool grounded)
    {
        if (animator != null && HasParameter(PARAM_IS_GROUNDED))
        {
            animator.SetBool(PARAM_IS_GROUNDED, grounded);
        }
    }

    /// <summary>
    /// Set whether the player is sitting
    /// </summary>
    public void SetSitting(bool sitting)
    {
        if (animator != null && HasParameter(PARAM_IS_SITTING))
        {
            animator.SetBool(PARAM_IS_SITTING, sitting);
        }
    }

    /// <summary>
    /// Trigger the jump animation
    /// </summary>
    public void TriggerJump()
    {
        if (animator != null && HasParameter(PARAM_JUMP_TRIGGER))
        {
            animator.SetTrigger(PARAM_JUMP_TRIGGER);
        }
    }

    /// <summary>
    /// Trigger a generic interact animation
    /// </summary>
    public void TriggerInteract()
    {
        if (animator != null && HasParameter(PARAM_INTERACT_TRIGGER))
        {
            animator.SetTrigger(PARAM_INTERACT_TRIGGER);
        }
    }

    /// <summary>
    /// Flip the sprite to face a specific direction
    /// </summary>
    /// <param name="facingRight">True to face right, false to face left</param>
    public void SetFacingDirection(bool facingRight)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !facingRight;
        }
    }

    /// <summary>
    /// Show or hide the player sprite
    /// </summary>
    /// <param name="visible">True to show, false to hide</param>
    public void SetVisible(bool visible)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = visible;
        }
    }

    /// <summary>
    /// Check if the animator has a specific parameter
    /// </summary>
    private bool HasParameter(string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }

    /// <summary>
    /// Get direct access to the Animator for advanced usage
    /// </summary>
    public Animator GetAnimator()
    {
        return animator;
    }
}

