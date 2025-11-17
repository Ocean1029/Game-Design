using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 管理小地圖的迷霧系統（Fog of War）
/// 追蹤玩家探索過的區域，未探索的區域會被迷霧遮蔽
/// </summary>
public class MinimapFogOfWar : MonoBehaviour
{
    [Header("Fog Settings")]
    [Tooltip("迷霧解析度（越高越精細但性能消耗越大）")]
    [SerializeField] private int fogResolution = 512;
    
    [Tooltip("玩家視野範圍（世界單位）")]
    [SerializeField] private float visionRadius = 15f;
    
    [Tooltip("迷霧顏色")]
    [SerializeField] private Color fogColor = new Color(0f, 0f, 0f, 0.8f);
    
    [Tooltip("已探索區域的透明度（0=完全透明，1=完全不透明）")]
    [SerializeField] private float exploredAlpha = 0.3f;
    
    [Tooltip("迷霧漸變平滑度")]
    [SerializeField] private float smoothness = 3f;

    [Header("World Bounds")]
    [Tooltip("地圖世界邊界 - 最小點")]
    [SerializeField] private Vector2 worldMin = new Vector2(-50f, -50f);
    
    [Tooltip("地圖世界邊界 - 最大點")]
    [SerializeField] private Vector2 worldMax = new Vector2(50f, 50f);

    [Header("References")]
    [Tooltip("玩家 Transform")]
    [SerializeField] private Transform player;

    [Header("Performance")]
    [Tooltip("更新頻率（秒）- 較高的值可以提升性能")]
    [SerializeField] private float updateInterval = 0.1f;
    
    [Tooltip("是否啟用迷霧系統")]
    [SerializeField] private bool enableFog = true;

    [Header("Debug")]
    [Tooltip("顯示調試資訊")]
    [SerializeField] private bool showDebugInfo = false;

    // Private fields
    private Texture2D fogTexture;
    private Color[] fogData;
    private float nextUpdateTime = 0f;
    private Vector2 worldSize;
    private float pixelsPerUnit;
    
    // Singleton
    private static MinimapFogOfWar instance;
    
    public static MinimapFogOfWar GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        Initialize();
    }

    /// <summary>
    /// 初始化迷霧系統（可在編輯器或運行時調用）
    /// </summary>
    public void Initialize()
    {
        // 如果已初始化，不重複初始化
        if (fogTexture != null)
        {
            return;
        }

        if (!enableFog)
        {
            Debug.Log("MinimapFogOfWar: Fog system disabled");
            return;
        }

        // 計算世界大小
        worldSize = worldMax - worldMin;
        pixelsPerUnit = fogResolution / Mathf.Max(worldSize.x, worldSize.y);

        // 初始化迷霧
        InitializeFog();

        // 自動尋找玩家
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                if (showDebugInfo)
                {
                    Debug.Log("MinimapFogOfWar: Auto-found player");
                }
            }
            else
            {
                Debug.LogWarning("MinimapFogOfWar: No player found! Please assign player transform.");
            }
        }

        if (showDebugInfo)
        {
            Debug.Log($"MinimapFogOfWar: Initialized with resolution {fogResolution}x{fogResolution}");
            Debug.Log($"World bounds: {worldMin} to {worldMax}, size: {worldSize}");
            Debug.Log($"Pixels per unit: {pixelsPerUnit}");
        }
    }

    void Update()
    {
        if (!enableFog || player == null)
        {
            return;
        }

        // 定期更新迷霧
        if (Time.time >= nextUpdateTime)
        {
            RevealAreaAroundPlayer();
            UpdateFogTexture();
            nextUpdateTime = Time.time + updateInterval;
        }
    }

    /// <summary>
    /// 初始化迷霧 Texture
    /// </summary>
    private void InitializeFog()
    {
        // 創建迷霧 Texture
        fogTexture = new Texture2D(fogResolution, fogResolution, TextureFormat.RGBA32, false);
        fogTexture.filterMode = FilterMode.Bilinear;
        fogTexture.wrapMode = TextureWrapMode.Clamp;

        // 初始化迷霧數據（全部為黑色不透明）
        fogData = new Color[fogResolution * fogResolution];
        for (int i = 0; i < fogData.Length; i++)
        {
            fogData[i] = fogColor;
        }

        // 應用到 Texture
        fogTexture.SetPixels(fogData);
        fogTexture.Apply();

        if (showDebugInfo)
        {
            Debug.Log("MinimapFogOfWar: Fog texture created and initialized");
        }
    }

    /// <summary>
    /// 揭開玩家周圍的迷霧
    /// </summary>
    private void RevealAreaAroundPlayer()
    {
        if (player == null) return;

        Vector2 playerPos = player.position;
        RevealArea(playerPos, visionRadius);
    }

    /// <summary>
    /// 揭開指定位置周圍的迷霧
    /// </summary>
    /// <param name="worldPosition">世界座標位置</param>
    /// <param name="radius">揭開的半徑（世界單位）</param>
    public void RevealArea(Vector2 worldPosition, float radius)
    {
        // 轉換世界座標到 Texture 座標
        Vector2Int centerPixel = WorldToTexture(worldPosition);
        int radiusInPixels = Mathf.CeilToInt(radius * pixelsPerUnit);

        // 遍歷圓形區域內的像素
        for (int y = -radiusInPixels; y <= radiusInPixels; y++)
        {
            for (int x = -radiusInPixels; x <= radiusInPixels; x++)
            {
                // 檢查是否在圓形範圍內
                float distSqr = x * x + y * y;
                if (distSqr > radiusInPixels * radiusInPixels)
                {
                    continue;
                }

                int pixelX = centerPixel.x + x;
                int pixelY = centerPixel.y + y;

                // 邊界檢查
                if (pixelX < 0 || pixelX >= fogResolution || pixelY < 0 || pixelY >= fogResolution)
                {
                    continue;
                }

                int index = pixelY * fogResolution + pixelX;

                // 計算距離衰減
                float dist = Mathf.Sqrt(distSqr);
                float normalizedDist = dist / radiusInPixels;
                
                // 平滑衰減（中心完全透明，邊緣保持已探索的透明度）
                float targetAlpha = Mathf.Lerp(0f, exploredAlpha, Mathf.Pow(normalizedDist, smoothness));
                
                // 只更新比當前更透明的區域（不會讓已探索的區域變暗）
                if (targetAlpha < fogData[index].a)
                {
                    Color newColor = fogColor;
                    newColor.a = targetAlpha;
                    fogData[index] = newColor;
                }
            }
        }
    }

    /// <summary>
    /// 更新迷霧 Texture
    /// </summary>
    private void UpdateFogTexture()
    {
        if (fogTexture != null)
        {
            fogTexture.SetPixels(fogData);
            fogTexture.Apply();
        }
    }

    /// <summary>
    /// 世界座標轉換為 Texture 座標
    /// </summary>
    private Vector2Int WorldToTexture(Vector2 worldPos)
    {
        // 正規化到 0-1 範圍
        Vector2 normalized = new Vector2(
            (worldPos.x - worldMin.x) / worldSize.x,
            (worldPos.y - worldMin.y) / worldSize.y
        );

        // 轉換為像素座標
        int x = Mathf.Clamp(Mathf.RoundToInt(normalized.x * fogResolution), 0, fogResolution - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt(normalized.y * fogResolution), 0, fogResolution - 1);

        return new Vector2Int(x, y);
    }

    /// <summary>
    /// 重置所有迷霧（用於新遊戲或測試）
    /// </summary>
    public void ResetFog()
    {
        if (fogData != null)
        {
            for (int i = 0; i < fogData.Length; i++)
            {
                fogData[i] = fogColor;
            }
            UpdateFogTexture();
            
            if (showDebugInfo)
            {
                Debug.Log("MinimapFogOfWar: Fog reset");
            }
        }
    }

    /// <summary>
    /// 完全揭開所有迷霧（用於測試或作弊）
    /// </summary>
    public void RevealAllFog()
    {
        if (fogData != null)
        {
            Color clearColor = fogColor;
            clearColor.a = 0f;
            
            for (int i = 0; i < fogData.Length; i++)
            {
                fogData[i] = clearColor;
            }
            UpdateFogTexture();
            
            if (showDebugInfo)
            {
                Debug.Log("MinimapFogOfWar: All fog revealed");
            }
        }
    }

    /// <summary>
    /// 設置玩家 Transform
    /// </summary>
    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    /// <summary>
    /// 啟用/禁用迷霧系統
    /// </summary>
    public void SetFogEnabled(bool enabled)
    {
        enableFog = enabled;
        
        // 迷霧的顯示/隱藏由 MinimapFogRenderer 控制
        // 這裡只設置系統是否更新迷霧數據
    }

    /// <summary>
    /// 獲取迷霧 Texture（用於其他系統）
    /// </summary>
    public Texture2D GetFogTexture()
    {
        return fogTexture;
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }

        // 清理 Texture
        if (fogTexture != null)
        {
            Destroy(fogTexture);
        }
    }

    // ==================== 調試功能 ====================

    void OnDrawGizmosSelected()
    {
        if (!showDebugInfo) return;

        // 繪製世界邊界
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((worldMin.x + worldMax.x) / 2f, (worldMin.y + worldMax.y) / 2f, 0f);
        Vector3 size = new Vector3(worldMax.x - worldMin.x, worldMax.y - worldMin.y, 0.1f);
        Gizmos.DrawWireCube(center, size);

        // 繪製玩家視野範圍
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.position, visionRadius);
        }
    }
}

