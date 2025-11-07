using UnityEngine;

/// <summary>
/// Camera component for rendering map previews of spawn point locations
/// This camera is used to dynamically render the area around a spawn point
/// and display it in the Fast Travel UI
/// </summary>
[RequireComponent(typeof(Camera))]
public class MapPreviewCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    [Tooltip("Orthographic size for the preview camera (how much area to show)")]
    [SerializeField] private float orthographicSize = 10f;
    
    [Tooltip("Height offset from spawn point position (for 2D games, usually 0)")]
    [SerializeField] private float heightOffset = 0f;
    
    [Tooltip("Z position offset for camera (for 2D games, usually -10)")]
    [SerializeField] private float zOffset = -10f;
    
    [Header("Render Settings")]
    [Tooltip("Render texture width for map preview")]
    [SerializeField] private int renderTextureWidth = 512;
    
    [Tooltip("Render texture height for map preview")]
    [SerializeField] private int renderTextureHeight = 512;
    
    [Tooltip("Layers to render in the preview (which layers should be visible)")]
    [SerializeField] private LayerMask cullingMask = -1;
    
    [Header("Debug")]
    [Tooltip("Show debug information in console")]
    [SerializeField] private bool showDebugInfo = false;

    // Components
    private Camera previewCamera;
    private RenderTexture renderTexture;
    
    // State
    private bool isInitialized = false;
    private string currentSceneName = "";

    void Awake()
    {
        // Get camera component
        previewCamera = GetComponent<Camera>();
        if (previewCamera == null)
        {
            Debug.LogError("MapPreviewCamera: Camera component not found!");
            return;
        }

        // Initialize camera settings
        InitializeCamera();
        
        // Create render texture
        CreateRenderTexture();
        
        isInitialized = true;
        
        if (showDebugInfo)
        {
            Debug.Log("MapPreviewCamera: Initialized");
        }
    }

    void OnDestroy()
    {
        // Clean up render texture
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
    }

    /// <summary>
    /// Initialize camera settings for map preview
    /// </summary>
    private void InitializeCamera()
    {
        previewCamera.orthographic = true;
        previewCamera.orthographicSize = orthographicSize;
        previewCamera.clearFlags = CameraClearFlags.SolidColor;
        previewCamera.backgroundColor = Color.black;
        previewCamera.cullingMask = cullingMask;
        previewCamera.depth = 10; // Higher than main camera
        previewCamera.enabled = false; // Disable by default, enable when needed
    }

    /// <summary>
    /// Create render texture for map preview
    /// </summary>
    private void CreateRenderTexture()
    {
        renderTexture = new RenderTexture(renderTextureWidth, renderTextureHeight, 24);
        renderTexture.name = "MapPreviewRenderTexture";
        
        // Assign render texture to camera
        previewCamera.targetTexture = renderTexture;
        
        if (showDebugInfo)
        {
            Debug.Log($"MapPreviewCamera: Created render texture {renderTextureWidth}x{renderTextureHeight}");
        }
    }

    /// <summary>
    /// Update map preview to show a specific spawn point location
    /// </summary>
    /// <param name="spawnPoint">The spawn point to preview</param>
    /// <returns>RenderTexture containing the preview, or null if failed</returns>
    public RenderTexture UpdatePreview(SpawnPointData spawnPoint)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("MapPreviewCamera: Not initialized, cannot update preview");
            return null;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("MapPreviewCamera: Spawn point is null");
            return null;
        }

        string targetScene = spawnPoint.sceneName;
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // Check if spawn point is in a different scene
        if (targetScene != currentScene)
        {
            if (showDebugInfo)
            {
                Debug.Log($"MapPreviewCamera: Spawn point is in different scene '{targetScene}', current scene is '{currentScene}'. Cannot render preview.");
            }
            // Return black texture or placeholder
            return renderTexture;
        }

        // Position camera at spawn point location
        Vector3 cameraPosition = new Vector3(
            spawnPoint.position.x,
            spawnPoint.position.y + heightOffset,
            spawnPoint.position.z + zOffset
        );
        
        transform.position = cameraPosition;
        transform.rotation = Quaternion.identity;

        if (showDebugInfo)
        {
            Debug.Log($"MapPreviewCamera: Positioning camera at {cameraPosition}, orthographic size: {orthographicSize}, culling mask: {cullingMask.value}");
        }

        // Enable camera and render
        // Note: Camera.Render() works even when Time.timeScale = 0
        previewCamera.enabled = true;
        previewCamera.Render();
        previewCamera.enabled = false;

        currentSceneName = targetScene;

        if (showDebugInfo)
        {
            Debug.Log($"MapPreviewCamera: Updated preview for '{spawnPoint.displayName}' at {spawnPoint.position}, render texture: {renderTexture != null}");
        }

        return renderTexture;
    }

    /// <summary>
    /// Get the render texture (for UI display)
    /// </summary>
    public RenderTexture GetRenderTexture()
    {
        return renderTexture;
    }

    /// <summary>
    /// Set orthographic size (how much area to show)
    /// </summary>
    public void SetOrthographicSize(float size)
    {
        orthographicSize = size;
        if (previewCamera != null)
        {
            previewCamera.orthographicSize = size;
        }
    }

    /// <summary>
    /// Check if camera is ready to render
    /// </summary>
    public bool IsReady()
    {
        return isInitialized && previewCamera != null && renderTexture != null;
    }
}

