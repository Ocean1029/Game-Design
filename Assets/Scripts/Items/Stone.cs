// using UnityEngine;

// public class Stone : MonoBehaviour
// {
//     void Start()
//     {
//         // 可選：初始化
//     }

//     void Update()
//     {
//         // 可選：更新邏輯
//     }

//     // 當 Stone 與其他物件發生物理碰撞時執行
//     private void OnCollisionEnter2D(Collision2D collision)
//     {
//         if (collision.gameObject.name == "Floor_destroy_by_stone")
//         {   
//             Debug.Log("Stone 碰到了：" + collision.gameObject.name);
//             collision.gameObject.SetActive(false);
//             Debug.Log("🪨 Floor_destroy_by_stone 被石頭砸到後消失！");
//         }
//     }
// }


using UnityEngine;

/// <summary>
/// 可被炸彈破壞的石頭：
/// - 會掉落並與地板碰撞
/// - 可在玩家靠近時使用炸彈
/// - 可砸壞特定地板
/// </summary>
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Stone : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("偵測玩家用的感應區域半徑")]
    [SerializeField] private float detectRadius = 2.0f;

    [Tooltip("是否顯示偵測範圍 (僅用於除錯)")]
    [SerializeField] private bool showGizmo = true;

    private PlayerUseBomb nearbyPlayer;   // 記錄可互動的玩家
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError("Stone: 缺少 Rigidbody2D！");

        // 確保 Collider 不是 Trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && col.isTrigger)
            col.isTrigger = false;
    }

    void Update()
    {
        DetectPlayerNearby();
    }

    /// <summary>
    /// 檢查玩家是否在感應範圍內
    /// </summary>
    private void DetectPlayerNearby()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                if (nearbyPlayer == null)
                {
                    nearbyPlayer = hit.GetComponent<PlayerUseBomb>();
                    if (nearbyPlayer != null)
                    {
                        nearbyPlayer.SetNearStone(true, this);
                        Debug.Log("🪨 玩家進入石頭範圍，可以使用炸彈 (Tag)");
                    }
                }
                return; // 已找到玩家，不用繼續找
            }
        }

        // 如果跑完沒有找到玩家
        if (nearbyPlayer != null)
        {
            nearbyPlayer.SetNearStone(false, null);
            nearbyPlayer = null;
            Debug.Log("🪨 玩家離開石頭範圍 (Tag)");
        }
    }

    /// <summary>
    /// 當石頭撞到地板時
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Floor_destroy_by_stone")
        {
            Debug.Log("🪨 Stone 碰到了：" + collision.gameObject.name);
            collision.gameObject.SetActive(false);
            Debug.Log("💥 Floor_destroy_by_stone 被石頭砸到後消失！");
        }
    }

    /// <summary>
    /// 被炸彈炸毀時呼叫
    /// </summary>
    public void Explode()
    {
        Debug.Log("💣 Stone 被炸毀！");
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    // 顯示偵測範圍 (僅除錯用)
    private void OnDrawGizmosSelected()
    {
        if (showGizmo)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectRadius);
        }
    }
#endif
}

