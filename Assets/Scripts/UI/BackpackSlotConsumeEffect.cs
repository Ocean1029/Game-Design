using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 背包物品消耗效果
/// 當物品被使用/移除時播放強調動畫
/// </summary>
public class BackpackSlotConsumeEffect : MonoBehaviour
{
    [Header("動畫設定")]
    [Tooltip("閃爍次數")]
    [SerializeField] private int flashCount = 3;
    
    [Tooltip("每次閃爍的持續時間")]
    [SerializeField] private float flashDuration = 0.15f;
    
    [Tooltip("閃爍時的顏色")]
    [SerializeField] private Color flashColor = new Color(1f, 0.5f, 0f, 1f); // 橙色
    
    [Tooltip("縮放動畫的最大比例")]
    [SerializeField] private float scaleMultiplier = 1.2f;
    
    [Tooltip("縮放動畫的持續時間")]
    [SerializeField] private float scaleDuration = 0.3f;
    
    private Image iconImage;
    private RectTransform rectTransform;
    private Color originalColor;
    private Vector3 originalScale;
    private bool isAnimating = false;
    
    void Awake()
    {
        iconImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        
        if (iconImage != null)
        {
            originalColor = iconImage.color;
        }
        
        if (rectTransform != null)
        {
            originalScale = rectTransform.localScale;
        }
    }
    
    /// <summary>
    /// 播放消耗動畫
    /// </summary>
    public void PlayConsumeAnimation()
    {
        if (!isAnimating)
        {
            StartCoroutine(ConsumeAnimationCoroutine());
        }
    }
    
    /// <summary>
    /// 消耗動畫協程
    /// </summary>
    private IEnumerator ConsumeAnimationCoroutine()
    {
        isAnimating = true;
        
        // 儲存原始狀態
        Color startColor = iconImage != null ? iconImage.color : Color.white;
        Vector3 startScale = rectTransform != null ? rectTransform.localScale : Vector3.one;
        
        // 1. 縮放放大動畫
        if (rectTransform != null)
        {
            float elapsedTime = 0f;
            while (elapsedTime < scaleDuration / 2f)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / (scaleDuration / 2f);
                float scale = Mathf.Lerp(1f, scaleMultiplier, progress);
                rectTransform.localScale = startScale * scale;
                yield return null;
            }
        }
        
        // 2. 閃爍動畫
        for (int i = 0; i < flashCount; i++)
        {
            // 閃亮
            if (iconImage != null)
            {
                iconImage.color = flashColor;
            }
            yield return new WaitForSeconds(flashDuration);
            
            // 恢復
            if (iconImage != null)
            {
                iconImage.color = startColor;
            }
            yield return new WaitForSeconds(flashDuration);
        }
        
        // 3. 縮放縮小回原大小
        if (rectTransform != null)
        {
            float elapsedTime = 0f;
            while (elapsedTime < scaleDuration / 2f)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / (scaleDuration / 2f);
                float scale = Mathf.Lerp(scaleMultiplier, 1f, progress);
                rectTransform.localScale = startScale * scale;
                yield return null;
            }
            
            // 確保恢復到原始大小
            rectTransform.localScale = startScale;
        }
        
        isAnimating = false;
        
        // 動畫結束後，通知 BackpackSlotUI 更新最終顯示狀態
        BackpackSlotUI slotUI = GetComponentInParent<BackpackSlotUI>();
        if (slotUI != null)
        {
            slotUI.RefreshDisplayAfterAnimation();
        }
    }
    
    /// <summary>
    /// 設置原始顏色
    /// </summary>
    public void SetOriginalColor(Color color)
    {
        originalColor = color;
    }
    
    /// <summary>
    /// 檢查是否正在播放動畫
    /// </summary>
    public bool IsAnimating()
    {
        return isAnimating;
    }
}

