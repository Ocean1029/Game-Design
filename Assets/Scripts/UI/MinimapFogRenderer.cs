using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 在小地圖 UI 上渲染迷霧效果
/// 這個腳本應該附加到小地圖的 UI Image 上，用來顯示迷霧遮罩
/// </summary>
[RequireComponent(typeof(RawImage))]
public class MinimapFogRenderer : MonoBehaviour
{
    [Header("References")]
    [Tooltip("迷霧系統參考")]
    [SerializeField] private MinimapFogOfWar fogSystem;

    [Header("Rendering")]
    [Tooltip("迷霧圖層（在小地圖上方顯示）")]
    [SerializeField] private RawImage fogImage;
    
    [Tooltip("迷霧材質")]
    [SerializeField] private Material fogMaterial;

    [Header("Debug")]
    [Tooltip("顯示調試資訊")]
    [SerializeField] private bool showDebugInfo = false;

    private bool isInitialized = false;

    void Start()
    {
        InitializeRenderer();
    }

    void Update()
    {
        // 確保迷霧 Texture 始終更新
        if (isInitialized && fogSystem != null && fogImage != null)
        {
            Texture2D fogTexture = fogSystem.GetFogTexture();
            if (fogTexture != null && fogImage.texture != fogTexture)
            {
                fogImage.texture = fogTexture;
            }
        }
    }

    /// <summary>
    /// 初始化渲染器
    /// </summary>
    private void InitializeRenderer()
    {
        // 獲取 RawImage 組件
        if (fogImage == null)
        {
            fogImage = GetComponent<RawImage>();
        }

        if (fogImage == null)
        {
            Debug.LogError("MinimapFogRenderer: RawImage component not found!");
            return;
        }

        // 自動尋找迷霧系統
        if (fogSystem == null)
        {
            fogSystem = MinimapFogOfWar.GetInstance();
            
            if (fogSystem == null)
            {
                fogSystem = FindFirstObjectByType<MinimapFogOfWar>();
            }

            if (fogSystem == null)
            {
                Debug.LogWarning("MinimapFogRenderer: MinimapFogOfWar system not found!");
                return;
            }
        }

        // 確保迷霧系統已初始化
        fogSystem.Initialize();

        // 設置迷霧 Texture
        Texture2D fogTexture = fogSystem.GetFogTexture();
        if (fogTexture != null)
        {
            fogImage.texture = fogTexture;
            
            // 重要：設置 UV Rect 確保顯示整個 Texture
            fogImage.uvRect = new Rect(0, 0, 1, 1);
            
            // 設置材質（如果有）
            if (fogMaterial != null)
            {
                fogImage.material = fogMaterial;
            }
            else
            {
                // 如果沒有材質，確保使用預設的 UI/Default 材質
                fogImage.material = null;
            }
            
            // 確保顏色是白色（不要影響 Texture 顏色）
            fogImage.color = Color.white;
            
            if (showDebugInfo)
            {
                Debug.Log($"MinimapFogRenderer: Initialized with fog texture {fogTexture.width}x{fogTexture.height}");
                Debug.Log($"MinimapFogRenderer: RawImage size: {fogImage.rectTransform.rect.size}");
                Debug.Log($"MinimapFogRenderer: UV Rect: {fogImage.uvRect}");
            }
        }
        else
        {
            Debug.LogError("MinimapFogRenderer: Fog texture is null after initialization! Check MinimapFogOfWar Enable Fog setting.");
        }

        isInitialized = true;
    }

    /// <summary>
    /// 設置迷霧系統參考
    /// </summary>
    public void SetFogSystem(MinimapFogOfWar system)
    {
        fogSystem = system;
        
        if (isInitialized && fogImage != null && fogSystem != null)
        {
            fogImage.texture = fogSystem.GetFogTexture();
        }
    }

    /// <summary>
    /// 顯示/隱藏迷霧
    /// </summary>
    public void SetFogVisible(bool visible)
    {
        if (fogImage != null)
        {
            fogImage.enabled = visible;
        }
    }
}

