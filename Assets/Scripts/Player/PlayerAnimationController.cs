using UnityEngine;

/// <summary>
/// Manages player animation parameters and visual effects
/// Provides a clean interface to control animations without directly exposing the Animator
/// </summary>
public class PlayerAnimationController : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float animationSpeed = 0.6f;
    [Header("Component References")]
    [Tooltip("Animator component. If not assigned, will search in children (e.g., Visual child).")]
    public Animator animator;
    [Tooltip("The effects child GameObject that contains the teleport Animator.")]
    [SerializeField] private GameObject effects;
    [Header("Teleport Animation Settings")]
    [Tooltip("The animator layer index where the teleport animation is located. Set to -1 to auto-detect.")]
    [SerializeField] private int teleportLayerIndex = -1;

    private SpriteRenderer spriteRenderer;

    // Animation parameter names (configure these to match your Animator Controller)
    private readonly string PARAM_SPEED = "Speed";
    private readonly string PARAM_IS_GROUNDED = "IsGrounded";
    private readonly string PARAM_IS_SITTING = "IsSitting";
    private readonly string PARAM_JUMP_TRIGGER = "Jump";
    private readonly string PARAM_WALK_TRIGGER = "Walk";
    private readonly string PARAM_INTERACT_TRIGGER = "Interact";
    private readonly string PARAM_TELEPORT_TRIGGER = "Teleport";
    private readonly string PARAM_TRIP_TRIGGER = "Trip";
    private readonly string PARAM_RECOVER_TRIGGER = "Recover";

    // Jump animation control
    private const int JUMP_TOTAL_FRAMES = 8;
    private const int JUMP_HOLD_FRAME = 4; // Frame to hold at peak (0-indexed: frame 4 = 50% of animation)
    private bool isJumpingUp = false;
    private bool isAtPeak = false;
    private bool isFalling = false;
    private bool hasSetPeakPosition = false; // Track if we've already set the peak position
    private float jumpAnimationHoldNormalizedTime = 0.5f; // 4/8 = 0.5 (50% through animation)

    void Awake()
    {
        // Try to get animator from current GameObject first
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // If still not found, search for Visual child specifically
        if (animator == null)
        {
            Transform visualChild = transform.Find("Visual");
            if (visualChild != null)
            {
                animator = visualChild.GetComponent<Animator>();
            }
        }

        // Fallback: search all children (but avoid effects animator)
        if (animator == null)
        {
            Animator[] allAnimators = GetComponentsInChildren<Animator>();
            foreach (Animator anim in allAnimators)
            {
                // Skip the effects animator, we want the main player animator
                if (anim.gameObject.name != "effects")
                {
                    animator = anim;
                    break;
                }
            }
        }

        // Get sprite renderer from current GameObject first
        spriteRenderer = GetComponent<SpriteRenderer>();

        // If not found, look specifically in Visual child
        if (spriteRenderer == null)
        {
            Transform visualChild = transform.Find("Visual");
            if (visualChild != null)
            {
                spriteRenderer = visualChild.GetComponent<SpriteRenderer>();
            }
        }

        // Fallback: search all children (but avoid effects sprite renderer)
        if (spriteRenderer == null)
        {
            SpriteRenderer[] allSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in allSpriteRenderers)
            {
                // Skip the effects sprite renderer, we want the main player sprite
                if (sr.gameObject.name != "effects")
                {
                    spriteRenderer = sr;
                    break;
                }
            }
        }

        if (spriteRenderer != null)
        {
            Debug.Log($"PlayerAnimationController: Found SpriteRenderer on GameObject '{spriteRenderer.gameObject.name}'");
        }
        else
        {
            Debug.LogWarning("PlayerAnimationController: SpriteRenderer not found on player object or children!");
        }

        // Get effects child if not assigned
        if (effects == null)
        {
            effects = transform.Find("effects")?.gameObject;
            Debug.Log($"PlayerAnimationController: Looking for effects child - {(effects != null ? "FOUND" : "NOT FOUND")}");
        }

        if (effects == null)
        {
            Debug.LogWarning("PlayerAnimationController: effects GameObject not found! Teleport animation will not show.");
        }
        else
        {
            Debug.Log("PlayerAnimationController: effects child found and ready");
        }

        // Check if animator has a valid controller
        if (animator != null)
        {
            Debug.Log($"PlayerAnimationController: Found animator on GameObject '{animator.gameObject.name}' with controller '{animator.runtimeAnimatorController?.name ?? "None"}'");
            if (animator.runtimeAnimatorController == null)
            {
                Debug.LogWarning("PlayerAnimationController: Animator found but no AnimatorController assigned. Animation features will be disabled.");
            }
        }
        else
        {
            Debug.LogWarning("PlayerAnimationController: No Animator component found! Please assign one or ensure there's an Animator in a child GameObject.");
        }

        // Set animation speed
        if (animator != null)
        {
            animator.speed = animationSpeed;
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
            float absSpeed = Mathf.Abs(speed);
            animator.SetFloat(PARAM_SPEED, absSpeed);
        }
        else
        {
            Debug.LogWarning($"PlayerAnimationController: Cannot set Speed parameter - animator: {(animator != null ? "found" : "null")}, has parameter: {HasParameter(PARAM_SPEED)}");
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
        else
        {
            Debug.LogWarning($"PlayerAnimationController: Cannot set IsGrounded parameter - animator: {(animator != null ? "found" : "null")}, has parameter: {HasParameter(PARAM_IS_GROUNDED)}");
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
            // Reset jump animation state
            isJumpingUp = true;
            isAtPeak = false;
            isFalling = false;
            hasSetPeakPosition = false;
        }
    }

    /// <summary>
    /// Update jump animation based on vertical velocity
    /// Holds frame 4 at peak, then plays frames 5-8 during fall
    /// </summary>
    /// <param name="verticalVelocity">Current Y velocity</param>
    /// <param name="isGrounded">Whether player is on ground</param>
    /// <param name="currentHeight">Current height above ground (optional, for better timing)</param>
    /// <param name="gravityScale">Gravity scale for physics calculation</param>
    public void UpdateJumpAnimation(float verticalVelocity, bool isGrounded, float currentHeight = 0f, float gravityScale = 1f)
    {
        if (animator == null) return;

        // Reset jump state when grounded
        if (isGrounded)
        {
            isJumpingUp = false;
            isAtPeak = false;
            isFalling = false;
            hasSetPeakPosition = false;
            animator.speed = animationSpeed; // Ensure normal speed when grounded
            return;
        }

        // Get current animation state info
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        // Check if we're in the jump animation (you may need to adjust the state name)
        bool isInJumpAnimation = stateInfo.IsName("jump") || stateInfo.IsName("Jump");
        
        if (!isInJumpAnimation) return;

        // Phase 1: Rising (frames 1-4, normalized time 0.0 to 0.5)
        if (isJumpingUp && verticalVelocity > 0.1f)
        {
            // Let animation play normally up to frame 4
            
            if (stateInfo.normalizedTime % 1 >= jumpAnimationHoldNormalizedTime)
            {
                // Reached frame 4, transition to peak hold
                isJumpingUp = false;
                isAtPeak = true;
                hasSetPeakPosition = false;
            }
        }
        // Phase 2: At Peak (hold frame 4)
        else if (verticalVelocity >= 0f)
        {
            // Only set the position once when entering peak state
            if (!hasSetPeakPosition)
            {
                animator.Play(stateInfo.fullPathHash, 0, jumpAnimationHoldNormalizedTime);
                animator.speed = 0; // Freeze animation
                hasSetPeakPosition = true;
            }
            
            // Transition to falling when velocity becomes negative
            if (verticalVelocity < -0.1f)
            {
                isAtPeak = false;
                isFalling = true;
            }
        }
        // Phase 3: Falling (frames 5-8, normalized time 0.5 to 1.0)
        else if (verticalVelocity < 0f)
        {
            if (!isFalling)
            {
                isFalling = true;
                // ALWAYS start from frame 5 (normalized time 0.5) when falling
                // This works for both jumping and falling off cliffs
                animator.Play(stateInfo.fullPathHash, 0, jumpAnimationHoldNormalizedTime);
            }
            else
            {
                // If already falling, ensure we're not before frame 5
                float currentNormalizedTime = stateInfo.normalizedTime % 1;
                if (currentNormalizedTime < jumpAnimationHoldNormalizedTime)
                {
                    // Force to frame 5 if somehow we're before it
                    animator.Play(stateInfo.fullPathHash, 0, jumpAnimationHoldNormalizedTime);
                }
            }
            
            // Calculate estimated time to hit ground using physics
            float estimatedTimeToGround = CalculateTimeToGround(verticalVelocity, currentHeight, gravityScale);
            
            // Calculate animation speed to complete remaining 50% (0.5 to 1.0) before landing
            // Remaining normalized time = 0.5
            float remainingNormalizedTime = 1.0f - jumpAnimationHoldNormalizedTime;
            
            if (estimatedTimeToGround > 0.01f)
            {
                // Animation speed = how much animation to play / time available
                float targetAnimSpeed = remainingNormalizedTime / estimatedTimeToGround;
                
                // Clamp to reasonable values (don't go too fast or slow)
                targetAnimSpeed = Mathf.Clamp(targetAnimSpeed, animationSpeed * 0.3f, animationSpeed * 3f);
                
                animator.speed = targetAnimSpeed;
            }
            else
            {
                // Fallback if calculation fails
                animator.speed = animationSpeed;
            }
        }
    }

    /// <summary>
    /// Calculate estimated time until player hits the ground based on physics
    /// Uses the formula: t = (sqrt(v^2 + 2*g*h) - v) / g
    /// </summary>
    private float CalculateTimeToGround(float verticalVelocity, float currentHeight, float gravityScale)
    {
        // Get actual gravity magnitude
        float gravity = Mathf.Abs(Physics2D.gravity.y) * gravityScale;
        
        if (gravity < 0.01f) return 1f; // Avoid division by zero
        
        // If we have height info, use it for accurate calculation
        if (currentHeight > 0.1f)
        {
            // Using kinematic equation: h = v*t + 0.5*g*t^2
            // Solve for t: t = (sqrt(v^2 + 2*g*h) - v) / g
            float discriminant = verticalVelocity * verticalVelocity + 2f * gravity * currentHeight;
            
            if (discriminant >= 0)
            {
                float timeToGround = (Mathf.Sqrt(discriminant) - verticalVelocity) / gravity;
                return Mathf.Max(0.1f, timeToGround);
            }
        }
        
        // Fallback: estimate based on velocity alone
        // Assuming typical fall from reasonable height
        float estimatedHeight = 2f; // Assume ~2 units above ground if no height provided
        float discriminant2 = verticalVelocity * verticalVelocity + 2f * gravity * estimatedHeight;
        
        if (discriminant2 >= 0)
        {
            float timeToGround = (Mathf.Sqrt(discriminant2) - verticalVelocity) / gravity;
            return Mathf.Max(0.1f, timeToGround);
        }
        
        // Final fallback
        return Mathf.Abs(verticalVelocity) / gravity;
    }

    /// <summary>
    /// Trigger the walk animation
    /// </summary>
    public void TriggerWalk()
    {
        if (animator != null && HasParameter(PARAM_WALK_TRIGGER))
        {
            animator.SetTrigger(PARAM_WALK_TRIGGER);
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
    /// Trigger the teleport animation
    /// Uses the effects child's Animator to play the teleport animation
    /// </summary>
    public void TriggerTeleport()
    {
        Debug.Log($"PlayerAnimationController: TriggerTeleport called. effects: {(effects != null ? "found" : "null")}");

        // Enable the effects GameObject
        if (effects != null)
        {
            effects.SetActive(true);
            Debug.Log("PlayerAnimationController: effects enabled");

            // Get the Animator from the effects child
            Animator effectsAnimator = effects.GetComponent<Animator>();
            if (effectsAnimator != null)
            {
                Debug.Log($"PlayerAnimationController: Found effects Animator on '{effectsAnimator.gameObject.name}' with controller '{effectsAnimator.runtimeAnimatorController?.name ?? "None"}'");

                // Start the teleport animation immediately (no trigger delay)
                Debug.Log("PlayerAnimationController: Starting teleport animation immediately");
                effectsAnimator.Play("teleport", 0, 0f); // Play from start of animation
                Debug.Log("PlayerAnimationController: Teleport animation started immediately on effects Animator");

                // Start coroutine to handle post-animation logic
                StartCoroutine(DisableTeleportEffectAfterAnimation());
            }
            else
            {
                Debug.LogWarning("PlayerAnimationController: effects GameObject has no Animator component!");
            }
        }
        else
        {
            Debug.LogWarning("PlayerAnimationController: effects GameObject is null!");
        }
    }

    /// <summary>
    /// Trigger the trip/fall animation
    /// </summary>
    public void TriggerTrip()
    {
        if (animator != null)
        {
            if (HasParameter(PARAM_TRIP_TRIGGER))
            {
                animator.SetTrigger(PARAM_TRIP_TRIGGER);
            }
            else
            {
                Debug.LogWarning("PlayerAnimationController: 'Trip' parameter not found in Animator!");
            }
        }
    }

    /// <summary>
    /// Trigger the recover animation (transition from Trip to Idle)
    /// </summary>
    public void TriggerRecover()
    {
        if (animator != null && HasParameter(PARAM_RECOVER_TRIGGER))
        {
            animator.SetTrigger(PARAM_RECOVER_TRIGGER);
        }
    }

    /// <summary>
    /// Disable the teleport effect after the animation completes
    /// Comment out the disabling line if you want the effect to stay visible
    /// </summary>
    private System.Collections.IEnumerator DisableTeleportEffectAfterAnimation()
    {
        // Wait for the animation to complete (0.83 seconds based on the clip)
        yield return new WaitForSeconds(0.9f); // Slightly longer than animation duration

        // Disable the effects GameObject (commented out - keeping effect always visible)
        // if (effects != null)
        // {
        //     effects.SetActive(false);
        //     Debug.Log("PlayerAnimationController: effects disabled after animation");
        // }
    }

    /// <summary>
    /// Get the index of the Teleport layer in the Animator
    /// </summary>
    private int GetTeleportLayerIndex()
    {
        if (animator == null) return -1;

        for (int i = 0; i < animator.layerCount; i++)
        {
            if (animator.GetLayerName(i) == "Teleport")
            {
                return i;
            }
        }

        Debug.LogWarning("PlayerAnimationController: Could not find Teleport layer in Animator!");
        return -1;
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
            Debug.Log($"PlayerAnimationController: Set facing direction - facingRight: {facingRight}, flipX: {!facingRight} on '{spriteRenderer.gameObject.name}'");
        }
        else
        {
            Debug.LogWarning("PlayerAnimationController: Cannot set facing direction - spriteRenderer is null!");
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
        return HasParameterOnAnimator(animator, paramName);
    }

    /// <summary>
    /// Check if a specific Animator has a specific parameter
    /// </summary>
    private bool HasParameterOnAnimator(Animator targetAnimator, string paramName)
    {
        // Check if animator exists and has a valid controller
        if (targetAnimator == null || targetAnimator.runtimeAnimatorController == null)
        {
            return false;
        }

        try
        {
            foreach (AnimatorControllerParameter param in targetAnimator.parameters)
            {
                if (param.name == paramName) return true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"PlayerAnimationController: Error accessing animator parameters: {e.Message}");
            return false;
        }

        return false;
    }

    /// <summary>
    /// Check if the animation system is properly configured and ready to use
    /// </summary>
    public bool IsAnimationSystemReady()
    {
        return animator != null && animator.runtimeAnimatorController != null;
    }

    /// <summary>
    /// Get direct access to the Animator for advanced usage
    /// </summary>
    public Animator GetAnimator()
    {
        return animator;
    }

    /// <summary>
    /// Set the global animation speed multiplier
    /// </summary>
    /// <param name="speed">Animation speed multiplier (1.0 = normal speed, 0.5 = half speed, etc.)</param>
    public void SetAnimationSpeed(float speed)
    {
        if (animator != null)
        {
            animator.speed = speed;
            animationSpeed = speed;
        }
    }

    /// <summary>
    /// Get the current animation speed multiplier
    /// </summary>
    public float GetAnimationSpeed()
    {
        return animationSpeed;
    }
}

