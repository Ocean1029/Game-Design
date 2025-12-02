using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 在背包物件 slot 上方顯示閃動的按鍵提示
/// 用於提示玩家可以使用特定物品（鑰匙、炸彈等）
/// </summary>
public class InventorySlotPrompt : MonoBehaviour
{
    [Header("按鍵圖片設定")]
    [Tooltip("要顯示的按鍵圖片（例如 zbutton.png）")]
    [SerializeField] private Sprite buttonSprite;
    
    [Tooltip("按鍵圖片在 slot 上方的偏移位置")]
    [SerializeField] private Vector2 offset = new Vector2(0f, 60f);
    
    [Tooltip("按鍵圖片的尺寸")]
    [SerializeField] private Vector2 buttonSize = new Vector2(40f, 40f);
    
    [Header("閃動動畫設定")]
    [Tooltip("閃動動畫的速度（每秒閃動次數）")]
    [SerializeField] private float pulseSpeed = 1.2f;
    
    [Tooltip("閃動動畫的最小透明度")]
    [Range(0f, 1f)]
    [SerializeField] private float minAlpha = 0.7f;
    
    [Tooltip("閃動動畫的最大透明度")]
    [Range(0f, 1f)]
    [SerializeField] private float maxAlpha = 1f;
    
    [Tooltip("是否啟用上下浮動動畫")]
    [SerializeField] private bool enableFloatAnimation = true;
    
    [Tooltip("上下浮動的距離")]
    [SerializeField] private float floatDistance = 5f;
    
    [Tooltip("上下浮動的速度")]
    [SerializeField] private float floatSpeed = 2f;
    
    // 內部變數
    private GameObject promptObject;
    private Image promptImage;
    private RectTransform promptRect;
    private bool isPromptVisible = false;
    private Vector2 basePosition;
    private float floatTimer = 0f;
    private Canvas parentCanvas;
    
    void Start()
    {
        // 找到父 Canvas
        parentCanvas = GetComponentInParent<Canvas>();
        
        // 載入按鍵圖片（如果未在 Inspector 中設置）
        if (buttonSprite == null)
        {
            LoadButtonSprite();
        }
        
        // 創建提示物件
        CreatePromptObject();
        
        // 初始隱藏提示
        SetPromptVisible(false);
    }
    
    void Update()
    {
        // 更新閃動動畫
        if (isPromptVisible && promptImage != null)
        {
            UpdatePulseAnimation();
            
            if (enableFloatAnimation)
            {
                UpdateFloatAnimation();
            }
        }
    }
    
    /// <summary>
    /// 載入按鍵圖片
    /// </summary>
    private void LoadButtonSprite()
    {
        // 嘗試從 Resources 載入新的 Z_but 圖片
        buttonSprite = Resources.Load<Sprite>("UI/Z_but");
        
        // 如果還是沒有，嘗試使用 UnityEditor 的 AssetDatabase（僅在編輯器中）
        #if UNITY_EDITOR
        if (buttonSprite == null)
        {
            buttonSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Image/UI/Z_but.png");
        }
        #endif
    }
    
    /// <summary>
    /// 創建提示物件
    /// </summary>
    private void CreatePromptObject()
    {
        // 創建新的 GameObject 用於顯示按鍵圖片
        promptObject = new GameObject("InventorySlotPrompt_Button");
        promptObject.transform.SetParent(transform, false);
        
        // 添加 RectTransform
        promptRect = promptObject.AddComponent<RectTransform>();
        promptRect.anchorMin = new Vector2(0.5f, 0.5f);
        promptRect.anchorMax = new Vector2(0.5f, 0.5f);
        promptRect.pivot = new Vector2(0.5f, 0.5f);
        promptRect.anchoredPosition = offset;
        promptRect.sizeDelta = buttonSize;
        basePosition = offset;
        
        // 添加 Image
        promptImage = promptObject.AddComponent<Image>();
        promptImage.sprite = buttonSprite;
        promptImage.raycastTarget = false; // 不攔截點擊
        
        // 初始隱藏
        promptObject.SetActive(false);
    }
    
    /// <summary>
    /// 設置提示是否可見
    /// </summary>
    public void SetPromptVisible(bool visible)
    {
        isPromptVisible = visible;
        
        if (promptObject != null)
        {
            promptObject.SetActive(visible);
            
            if (visible)
            {
                // 重置動畫狀態
                floatTimer = 0f;
                if (promptRect != null)
                {
                    promptRect.anchoredPosition = basePosition;
                }
            }
        }
    }
    
    /// <summary>
    /// 更新閃動動畫
    /// </summary>
    private void UpdatePulseAnimation()
    {
        if (promptImage == null) return;
        
        // 計算閃動透明度
        float pulse = Mathf.Sin(Time.time * pulseSpeed * Mathf.PI * 2f) * 0.5f + 0.5f;
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, pulse);
        
        // 應用透明度
        Color color = promptImage.color;
        color.a = alpha;
        promptImage.color = color;
    }
    
    /// <summary>
    /// 更新上下浮動動畫
    /// </summary>
    private void UpdateFloatAnimation()
    {
        if (promptRect == null) return;
        
        floatTimer += Time.deltaTime * floatSpeed;
        float yOffset = Mathf.Sin(floatTimer) * floatDistance;
        
        Vector2 newPosition = basePosition + Vector2.up * yOffset;
        promptRect.anchoredPosition = newPosition;
    }
    
    /// <summary>
    /// 手動設置按鍵圖片
    /// </summary>
    public void SetButtonSprite(Sprite sprite)
    {
        buttonSprite = sprite;
        if (promptImage != null)
        {
            promptImage.sprite = sprite;
        }
    }
    
    /// <summary>
    /// 手動設置偏移位置
    /// </summary>
    public void SetOffset(Vector2 newOffset)
    {
        offset = newOffset;
        basePosition = newOffset;
        if (promptRect != null)
        {
            promptRect.anchoredPosition = newOffset;
        }
    }
    
    /// <summary>
    /// 顯示提示
    /// </summary>
    public void ShowPrompt()
    {
        SetPromptVisible(true);
    }
    
    /// <summary>
    /// 隱藏提示
    /// </summary>
    public void HidePrompt()
    {
        SetPromptVisible(false);
    }
    
    /// <summary>
    /// 檢查提示是否正在顯示
    /// </summary>
    public bool IsPromptVisible()
    {
        return isPromptVisible;
    }
    
    void OnDestroy()
    {
        // 清理提示物件
        if (promptObject != null)
        {
            Destroy(promptObject);
        }
    }
}

