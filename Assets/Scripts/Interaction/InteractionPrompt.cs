using UnityEngine;
using System.Collections;

/// <summary>
/// 互動提示組件 - 在可互動物件上方顯示閃動的按鍵圖片
/// 當玩家靠近時自動顯示，離開時自動隱藏
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class InteractionPrompt : MonoBehaviour
{
    [Header("按鍵圖片設定")]
    [Tooltip("要顯示的按鍵圖片（例如 zbutton.png）")]
    [SerializeField] private Sprite buttonSprite;
    
    [Tooltip("按鍵圖片在世界空間中的偏移位置（相對於物件中心）")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1f, 0f);
    
    [Tooltip("按鍵圖片的尺寸")]
    [SerializeField] private Vector2 buttonSize = new Vector2(5f, 5f);
    
    [Header("閃動動畫設定")]
    [Tooltip("閃動動畫的速度（每秒閃動次數）")]
    [SerializeField] private float pulseSpeed = 1.0f;
    
    [Tooltip("閃動動畫的最小透明度")]
    [Range(0f, 1f)]
    [SerializeField] private float minAlpha = 0.7f;
    
    [Tooltip("閃動動畫的最大透明度")]
    [Range(0f, 1f)]
    [SerializeField] private float maxAlpha = 1f;
    
    [Tooltip("是否啟用上下浮動動畫")]
    [SerializeField] private bool enableFloatAnimation = true;
    
    [Tooltip("上下浮動的距離")]
    [SerializeField] private float floatDistance = 0.1f;
    
    [Tooltip("上下浮動的速度")]
    [SerializeField] private float floatSpeed = 2f;
    
    [Header("檢測設定")]
    [Tooltip("檢測範圍（如果使用距離檢測而非 Trigger）")]
    [SerializeField] private float detectionRadius = 2f;
    
    [Tooltip("使用距離檢測而非 Trigger（如果物件沒有設置 Trigger）")]
    [SerializeField] private bool useDistanceDetection = false;
    
    [Header("除錯設定")]
    [Tooltip("是否顯示除錯資訊")]
    [SerializeField] private bool showDebugInfo = false;
    
    // 內部變數
    private GameObject promptObject;
    private SpriteRenderer promptRenderer;
    private bool isPlayerNearby = false;
    private bool isPromptVisible = false;
    private PlayerController playerController;
    private Collider2D detectionCollider;
    private Vector3 basePosition;
    private float floatTimer = 0f;
    
    void Start()
    {
        // 獲取或創建檢測碰撞器
        detectionCollider = GetComponent<Collider2D>();
        if (detectionCollider == null)
        {
            Debug.LogError($"InteractionPrompt: {gameObject.name} 缺少 Collider2D 組件！");
            return;
        }
        
        // 如果使用距離檢測，確保碰撞器不是觸發器
        if (useDistanceDetection)
        {
            detectionCollider.isTrigger = false;
        }
        else
        {
            // 使用 Trigger 檢測，確保碰撞器是觸發器
            detectionCollider.isTrigger = true;
        }
        
        // 載入按鍵圖片（如果未在 Inspector 中設置）
        if (buttonSprite == null)
        {
            LoadButtonSprite();
        }
        
        // 創建提示物件
        CreatePromptObject();
        
        // 初始隱藏提示
        SetPromptVisible(false);
        
        // 查找玩家控制器
        playerController = FindFirstObjectByType<PlayerController>();
        
        if (showDebugInfo)
        {
            Debug.Log($"InteractionPrompt: 初始化完成 - {gameObject.name}");
        }
    }
    
    void Update()
    {
        // 如果使用距離檢測，每幀檢查玩家距離
        if (useDistanceDetection && playerController != null)
        {
            float distance = Vector2.Distance(transform.position, playerController.transform.position);
            bool shouldShow = distance <= detectionRadius;
            
            if (shouldShow != isPlayerNearby)
            {
                isPlayerNearby = shouldShow;
                SetPromptVisible(shouldShow);
            }
        }
        
        // 更新閃動動畫
        if (isPromptVisible && promptRenderer != null)
        {
            UpdatePulseAnimation();
            
            if (enableFloatAnimation)
            {
                UpdateFloatAnimation();
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!useDistanceDetection && IsPlayer(collision))
        {
            isPlayerNearby = true;
            SetPromptVisible(true);
            
            if (showDebugInfo)
            {
                Debug.Log($"InteractionPrompt: 玩家進入範圍 - {gameObject.name}");
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D collision)
    {
        if (!useDistanceDetection && IsPlayer(collision))
        {
            isPlayerNearby = false;
            SetPromptVisible(false);
            
            if (showDebugInfo)
            {
                Debug.Log($"InteractionPrompt: 玩家離開範圍 - {gameObject.name}");
            }
        }
    }
    
    /// <summary>
    /// 檢查碰撞物件是否為玩家
    /// </summary>
    private bool IsPlayer(Collider2D collision)
    {
        return collision.CompareTag("Player") || 
               collision.GetComponent<PlayerController>() != null;
    }
    
    /// <summary>
    /// 載入按鍵圖片
    /// </summary>
    private void LoadButtonSprite()
    {
        // 嘗試從 Resources 載入（如果圖片在 Resources 資料夾中）
        buttonSprite = Resources.Load<Sprite>("UI/Z_but");
        
        // 如果還是沒有，嘗試使用 UnityEditor 的 AssetDatabase（僅在編輯器中）
        #if UNITY_EDITOR
        if (buttonSprite == null)
        {
            // 嘗試從 Assets/Image/UI/ 路徑載入新的 Z_but 圖片
            buttonSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Image/UI/Z_but.png");
        }
        #endif
        
        // 如果還是沒有，顯示警告
        if (buttonSprite == null)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning($"InteractionPrompt: 無法載入按鍵圖片，請在 Inspector 中手動設置！路徑：Assets/Image/UI/Z_but.png");
            }
        }
    }
    
    /// <summary>
    /// 創建提示物件
    /// </summary>
    private void CreatePromptObject()
    {
        // 創建新的 GameObject 用於顯示按鍵圖片
        promptObject = new GameObject("InteractionPrompt_Button");
        promptObject.transform.SetParent(transform);
        promptObject.transform.localPosition = worldOffset;
        basePosition = promptObject.transform.position;
        
        // 添加 SpriteRenderer
        promptRenderer = promptObject.AddComponent<SpriteRenderer>();
        promptRenderer.sprite = buttonSprite;
        promptRenderer.sortingOrder = 100; // 確保顯示在最上層
        promptRenderer.sortingLayerName = "UI"; // 如果有的話
        
        // 設置尺寸
        if (buttonSprite != null)
        {
            promptObject.transform.localScale = new Vector3(
                buttonSize.x / buttonSprite.bounds.size.x,
                buttonSize.y / buttonSprite.bounds.size.y,
                1f
            );
        }
        
        // 初始隱藏
        promptRenderer.enabled = false;
    }
    
    /// <summary>
    /// 設置提示是否可見
    /// </summary>
    private void SetPromptVisible(bool visible)
    {
        isPromptVisible = visible;
        
        if (promptRenderer != null)
        {
            promptRenderer.enabled = visible;
            
            if (visible)
            {
                // 重置動畫狀態
                floatTimer = 0f;
                if (promptObject != null)
                {
                    basePosition = transform.position + worldOffset;
                    promptObject.transform.position = basePosition;
                }
            }
        }
    }
    
    /// <summary>
    /// 更新閃動動畫
    /// </summary>
    private void UpdatePulseAnimation()
    {
        if (promptRenderer == null) return;
        
        // 計算閃動透明度
        float pulse = Mathf.Sin(Time.time * pulseSpeed * Mathf.PI * 2f) * 0.5f + 0.5f;
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, pulse);
        
        // 應用透明度
        Color color = promptRenderer.color;
        color.a = alpha;
        promptRenderer.color = color;
    }
    
    /// <summary>
    /// 更新上下浮動動畫
    /// </summary>
    private void UpdateFloatAnimation()
    {
        if (promptObject == null) return;
        
        floatTimer += Time.deltaTime * floatSpeed;
        float yOffset = Mathf.Sin(floatTimer) * floatDistance;
        
        Vector3 newPosition = basePosition + Vector3.up * yOffset;
        promptObject.transform.position = newPosition;
    }
    
    /// <summary>
    /// 手動設置按鍵圖片
    /// </summary>
    public void SetButtonSprite(Sprite sprite)
    {
        buttonSprite = sprite;
        if (promptRenderer != null)
        {
            promptRenderer.sprite = sprite;
            
            // 更新尺寸
            if (sprite != null)
            {
                promptObject.transform.localScale = new Vector3(
                    buttonSize.x / sprite.bounds.size.x,
                    buttonSize.y / sprite.bounds.size.y,
                    1f
                );
            }
        }
    }
    
    /// <summary>
    /// 手動設置偏移位置
    /// </summary>
    public void SetWorldOffset(Vector3 offset)
    {
        worldOffset = offset;
        if (promptObject != null)
        {
            promptObject.transform.localPosition = offset;
            basePosition = transform.position + offset;
        }
    }
    
    /// <summary>
    /// 手動顯示/隱藏提示
    /// </summary>
    public void ShowPrompt(bool show)
    {
        SetPromptVisible(show);
    }
    
    /// <summary>
    /// 檢查玩家是否在範圍內
    /// </summary>
    public bool IsPlayerNearby()
    {
        return isPlayerNearby;
    }
    
    void OnDestroy()
    {
        // 清理提示物件
        if (promptObject != null)
        {
            Destroy(promptObject);
        }
    }
    
    /// <summary>
    /// 在 Scene 視圖中繪製檢測範圍（僅用於除錯）
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (useDistanceDetection)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
        
        // 繪製提示位置
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + worldOffset, 0.2f);
    }
}

