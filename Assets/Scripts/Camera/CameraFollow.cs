using UnityEngine;

/// <summary>
/// Makes the camera follow the player smoothly
/// Supports offset configuration and smooth damping for natural camera movement
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The transform to follow (usually the player)")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [Tooltip("Offset from the target position")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -10f);

    [Tooltip("How quickly the camera follows the target (higher = faster)")]
    [SerializeField] private float smoothSpeed = 0.125f;

    [Header("Follow Axes")]
    [Tooltip("Should the camera follow on X axis?")]
    [SerializeField] private bool followX = true;

    [Tooltip("Should the camera follow on Y axis?")]
    [SerializeField] private bool followY = true;

    [Tooltip("Should the camera follow on Z axis? (Usually false for 2D games)")]
    [SerializeField] private bool followZ = false;

    [Header("Optional Boundaries")]
    [Tooltip("Enable camera boundaries to limit movement")]
    [SerializeField] private bool useBoundaries = false;

    [Tooltip("Minimum X position the camera can move to")]
    [SerializeField] private float minX = -50f;

    [Tooltip("Maximum X position the camera can move to")]
    [SerializeField] private float maxX = 50f;

    [Tooltip("Minimum Y position the camera can move to")]
    [SerializeField] private float minY = -50f;

    [Tooltip("Maximum Y position the camera can move to")]
    [SerializeField] private float maxY = 50f;

    [Header("Debug")]
    [Tooltip("Show debug information in console")]
    [SerializeField] private bool showDebugInfo = false;

    void LateUpdate()
    {
        if (target == null)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("CameraFollow: No target assigned!");
            }
            return;
        }

        FollowTarget();
    }

    /// <summary>
    /// Calculate and apply smooth camera following
    /// </summary>
    private void FollowTarget()
    {
        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;

        // Apply axis restrictions
        if (!followX) desiredPosition.x = transform.position.x;
        if (!followY) desiredPosition.y = transform.position.y;
        if (!followZ) desiredPosition.z = transform.position.z;

        // Apply boundaries if enabled
        if (useBoundaries)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        // Smoothly interpolate between current position and desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Apply the new position
        transform.position = smoothedPosition;

        if (showDebugInfo)
        {
            Debug.Log($"Camera: {transform.position} | Target: {target.position} | Desired: {desiredPosition}");
        }
    }

    /// <summary>
    /// Set a new target for the camera to follow
    /// </summary>
    /// <param name="newTarget">The new target transform</param>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        Debug.Log($"CameraFollow: Target changed to {newTarget.name}");
    }

    /// <summary>
    /// Instantly snap camera to target position without smoothing
    /// Useful for scene transitions or respawning
    /// </summary>
    public void SnapToTarget()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: Cannot snap to target - no target assigned!");
            return;
        }

        Vector3 targetPosition = target.position + offset;

        if (!followX) targetPosition.x = transform.position.x;
        if (!followY) targetPosition.y = transform.position.y;
        if (!followZ) targetPosition.z = transform.position.z;

        if (useBoundaries)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        transform.position = targetPosition;
        Debug.Log("CameraFollow: Snapped to target position");
    }

    /// <summary>
    /// Change the camera offset at runtime
    /// </summary>
    /// <param name="newOffset">New offset value</param>
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }

    /// <summary>
    /// Change the smooth speed at runtime
    /// </summary>
    /// <param name="newSpeed">New smooth speed (0-1 range recommended)</param>
    public void SetSmoothSpeed(float newSpeed)
    {
        smoothSpeed = Mathf.Clamp01(newSpeed);
    }

    // Draw gizmos in the editor to visualize boundaries
    void OnDrawGizmosSelected()
    {
        if (!useBoundaries) return;

        Gizmos.color = Color.yellow;

        // Draw boundary corners
        Vector3 bottomLeft = new Vector3(minX, minY, transform.position.z);
        Vector3 bottomRight = new Vector3(maxX, minY, transform.position.z);
        Vector3 topLeft = new Vector3(minX, maxY, transform.position.z);
        Vector3 topRight = new Vector3(maxX, maxY, transform.position.z);

        // Draw boundary rectangle
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);

        // Draw center cross
        float centerX = (minX + maxX) / 2f;
        float centerY = (minY + maxY) / 2f;
        float crossSize = 1f;

        Gizmos.DrawLine(
            new Vector3(centerX - crossSize, centerY, transform.position.z),
            new Vector3(centerX + crossSize, centerY, transform.position.z)
        );
        Gizmos.DrawLine(
            new Vector3(centerX, centerY - crossSize, transform.position.z),
            new Vector3(centerX, centerY + crossSize, transform.position.z)
        );
    }
}

