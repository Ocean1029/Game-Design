using UnityEngine;

/// <summary>
/// Debug tool to measure player jump height and horizontal distance
/// Attach this to the player to see exact measurements in the Console
/// Remove or disable this script when done tuning jumps
/// </summary>
public class JumpMeasureTool : MonoBehaviour
{
    [Header("Measurement Settings")]
    [Tooltip("Key to start measuring (should match your jump key)")]
    [SerializeField] private KeyCode measureKey = KeyCode.Space;
    
    [Tooltip("Show continuous updates during jump")]
    [SerializeField] private bool showContinuousUpdates = false;

    private Rigidbody2D rb;
    private bool measuring = false;
    
    // Measurement data
    private float startHeight = 0f;
    private float maxHeight = 0f;
    private float startX = 0f;
    private float endX = 0f;
    private float jumpDuration = 0f;
    private float jumpStartTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (rb == null)
        {
            Debug.LogError("JumpMeasureTool: No Rigidbody2D found on player!");
            enabled = false;
        }

        Debug.Log("<color=cyan>JumpMeasureTool: Ready! Press " + measureKey + " to measure jump.</color>");
    }

    void Update()
    {
        // Start measuring when jump key is pressed
        if (Input.GetKeyDown(measureKey))
        {
            StartMeasurement();
        }

        // Continue measuring during jump
        if (measuring)
        {
            UpdateMeasurement();
        }
    }

    /// <summary>
    /// Start a new jump measurement
    /// </summary>
    private void StartMeasurement()
    {
        startHeight = transform.position.y;
        maxHeight = startHeight;
        startX = transform.position.x;
        jumpStartTime = Time.time;
        measuring = true;

        Debug.Log($"<color=yellow>--- Jump Measurement Started ---</color>");
        Debug.Log($"Start Position: ({transform.position.x:F2}, {transform.position.y:F2})");
    }

    /// <summary>
    /// Update measurements during the jump
    /// </summary>
    private void UpdateMeasurement()
    {
        // Track maximum height reached
        if (transform.position.y > maxHeight)
        {
            maxHeight = transform.position.y;
        }

        // Show continuous updates if enabled
        if (showContinuousUpdates)
        {
            float currentHeight = transform.position.y - startHeight;
            Debug.Log($"Current height: {currentHeight:F2}, Velocity Y: {rb.linearVelocity.y:F2}");
        }

        // Check if jump is complete (started falling and returned near start height)
        if (rb.linearVelocity.y < 0 && transform.position.y <= startHeight + 0.1f)
        {
            EndMeasurement();
        }
    }

    /// <summary>
    /// End measurement and display results
    /// </summary>
    private void EndMeasurement()
    {
        measuring = false;
        endX = transform.position.x;
        jumpDuration = Time.time - jumpStartTime;

        // Calculate measurements
        float jumpHeight = maxHeight - startHeight;
        float horizontalDistance = Mathf.Abs(endX - startX);

        // Display results
        Debug.Log($"<color=green>--- Jump Measurement Complete ---</color>");
        Debug.Log($"<color=white>Jump Height: <b>{jumpHeight:F2}</b> units</color>");
        Debug.Log($"<color=white>Horizontal Distance: <b>{horizontalDistance:F2}</b> units</color>");
        Debug.Log($"<color=white>Jump Duration: {jumpDuration:F2} seconds</color>");
        Debug.Log($"<color=white>Max Height Position: {maxHeight:F2}</color>");
        
        // Compare to target
        Debug.Log($"<color=cyan>Target: Height = 3.0, Distance = 4.0</color>");
        
        float heightDiff = jumpHeight - 3.0f;
        float distDiff = horizontalDistance - 4.0f;
        
        string heightStatus = heightDiff > 0.1f ? "TOO HIGH" : heightDiff < -0.1f ? "TOO LOW" : "GOOD";
        string distStatus = distDiff > 0.2f ? "TOO FAR" : distDiff < -0.2f ? "TOO SHORT" : "GOOD";
        
        Debug.Log($"<color=white>Height Status: {heightStatus} ({heightDiff:+0.00;-0.00})</color>");
        Debug.Log($"<color=white>Distance Status: {distStatus} ({distDiff:+0.00;-0.00})</color>");
        
        // Provide tuning suggestions
        if (heightDiff > 0.1f)
        {
            Debug.Log("<color=yellow>Suggestion: Decrease Jump Force or Jump Hold Acceleration</color>");
        }
        else if (heightDiff < -0.1f)
        {
            Debug.Log("<color=yellow>Suggestion: Increase Jump Force or Jump Hold Acceleration</color>");
        }
        
        if (distDiff > 0.2f)
        {
            Debug.Log("<color=yellow>Suggestion: Decrease Move Speed or increase Gravity Scale</color>");
        }
        else if (distDiff < -0.2f)
        {
            Debug.Log("<color=yellow>Suggestion: Increase Move Speed or decrease Gravity Scale</color>");
        }
        
        Debug.Log($"<color=green>-------------------------------</color>\n");
    }

    // Draw gizmos to show measurement
    void OnDrawGizmos()
    {
        if (measuring)
        {
            // Draw start position
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(new Vector3(startX, startHeight, 0), 0.2f);
            
            // Draw max height reached
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, maxHeight, 0), 0.2f);
            
            // Draw line from start to max
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(
                new Vector3(startX, startHeight, 0),
                new Vector3(startX, maxHeight, 0)
            );
        }
    }
}

