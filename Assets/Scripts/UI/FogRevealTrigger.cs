using UnityEngine;

/// <summary>
/// 當玩家進入此觸發區域時，自動揭開該區域的迷霧
/// 用於重要地點或特殊區域的迷霧揭露
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class FogRevealTrigger : MonoBehaviour
{
    [Header("Reveal Settings")]
    [Tooltip("揭開的區域中心點（留空則使用此物件的位置）")]
    [SerializeField] private Transform revealCenter;
    
    [Tooltip("揭開的半徑（世界單位）")]
    [SerializeField] private float revealRadius = 10f;
    
    [Tooltip("是否只觸發一次")]
    [SerializeField] private bool triggerOnce = true;
    
    [Tooltip("揭開迷霧的延遲時間（秒）")]
    [SerializeField] private float revealDelay = 0f;

    [Header("Debug")]
    [Tooltip("顯示調試資訊")]
    [SerializeField] private bool showDebugInfo = false;

    private MinimapFogOfWar fogSystem;
    private bool hasTriggered = false;
    private Collider2D triggerCollider;

    void Start()
    {
        // 獲取 Collider2D
        triggerCollider = GetComponent<Collider2D>();
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
        else
        {
            Debug.LogError("FogRevealTrigger: Collider2D component not found!");
        }

        // 獲取迷霧系統
        fogSystem = MinimapFogOfWar.GetInstance();
        
        if (fogSystem == null)
        {
            Debug.LogWarning("FogRevealTrigger: MinimapFogOfWar system not found!");
        }

        // 如果沒有設置中心點，使用此物件的位置
        if (revealCenter == null)
        {
            revealCenter = transform;
        }

        if (showDebugInfo)
        {
            Debug.Log($"FogRevealTrigger: Initialized at {transform.position}, radius: {revealRadius}");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 檢查是否為玩家
        if (other.CompareTag("Player"))
        {
            if (triggerOnce && hasTriggered)
            {
                return;
            }

            if (showDebugInfo)
            {
                Debug.Log($"FogRevealTrigger: Player entered, revealing fog at {revealCenter.position}");
            }

            // 揭開迷霧
            if (revealDelay > 0f)
            {
                Invoke(nameof(RevealFog), revealDelay);
            }
            else
            {
                RevealFog();
            }

            hasTriggered = true;
        }
    }

    /// <summary>
    /// 揭開此區域的迷霧
    /// </summary>
    private void RevealFog()
    {
        if (fogSystem != null && revealCenter != null)
        {
            fogSystem.RevealArea(revealCenter.position, revealRadius);
            
            if (showDebugInfo)
            {
                Debug.Log($"FogRevealTrigger: Revealed fog area at {revealCenter.position}");
            }
        }
    }

    /// <summary>
    /// 手動觸發揭開迷霧
    /// </summary>
    public void ManualReveal()
    {
        RevealFog();
    }

    /// <summary>
    /// 重置觸發狀態
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
    }

    // ==================== 調試功能 ====================

    void OnDrawGizmosSelected()
    {
        if (!showDebugInfo) return;

        Transform center = revealCenter != null ? revealCenter : transform;
        
        // 繪製揭開範圍
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawSphere(center.position, revealRadius);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center.position, revealRadius);
    }
}

