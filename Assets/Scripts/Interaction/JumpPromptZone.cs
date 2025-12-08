using UnityEngine;

/// <summary>
/// 跳躍提示區域 - 當玩家靠近時顯示跳躍按鍵提示
/// </summary>
public class JumpPromptZone : MonoBehaviour
{
    private InteractionPromptButton promptButton;

    void Awake()
    {
        Debug.Log($"JumpPromptZone: 初始化物件 {gameObject.name}");
    }

    void Start()
    {
        Debug.Log($"JumpPromptZone: Start() 開始執行");

        // 檢查是否已有 InteractionPromptButton
        promptButton = GetComponent<InteractionPromptButton>();
        Debug.Log($"JumpPromptZone: 檢查 InteractionPromptButton - {(promptButton != null ? "找到" : "未找到")}");

        if (promptButton == null)
        {
            // 如果沒有，自動添加一個
            Debug.Log("JumpPromptZone: 正在添加 InteractionPromptButton 組件...");
            promptButton = gameObject.AddComponent<InteractionPromptButton>();
            Debug.Log("JumpPromptZone: 已自動添加 InteractionPromptButton 組件");

            // 設置按鍵為 Z 鍵
            Debug.Log("JumpPromptZone: 正在載入按鍵圖片...");
            Sprite jumpSprite = Resources.Load<Sprite>("UI/Z_but");
            if (jumpSprite != null)
            {
                promptButton.SetButtonSprite(jumpSprite);
                Debug.Log($"JumpPromptZone: 已設置跳躍按鍵圖片: {jumpSprite.name}");
            }
            else
            {
                Debug.LogWarning("JumpPromptZone: 找不到跳躍按鍵圖片，請手動設置");
                Debug.LogWarning("JumpPromptZone: 檢查 Assets/Image/UI/Z_but.png 是否存在");
            }
        }
        else
        {
            Debug.Log("JumpPromptZone: 找到現有的 InteractionPromptButton 組件");
        }

        // 確保有碰撞器
        Collider2D collider = GetComponent<Collider2D>();
        Debug.Log($"JumpPromptZone: 檢查碰撞器 - {(collider != null ? $"找到 {collider.GetType().Name}" : "未找到")}");

        if (collider == null)
        {
            // 添加圓形碰撞器
            Debug.Log("JumpPromptZone: 正在添加 CircleCollider2D...");
            CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
            circleCollider.radius = 2f;
            circleCollider.isTrigger = true;
            Debug.Log("JumpPromptZone: 已自動添加 CircleCollider2D (半徑: 2, 觸發器: true)");
        }
        else
        {
            // 確保是觸發器
            collider.isTrigger = true;
            Debug.Log("JumpPromptZone: 碰撞器已存在，確保設為觸發器");
        }

        // 檢查玩家物件
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log($"JumpPromptZone: 找到玩家物件: {player.name} 在位置 {player.transform.position}");
            float distance = Vector2.Distance(transform.position, player.transform.position);
            Debug.Log($"JumpPromptZone: 當前距離玩家: {distance}");
        }
        else
        {
            Debug.LogError("JumpPromptZone: 未找到玩家物件！請確保玩家有 'Player' 標籤");
        }

        Debug.Log("JumpPromptZone: 初始化完成");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"JumpPromptZone: 物件 {collision.gameObject.name} 進入觸發區域");

        if (collision.CompareTag("Player"))
        {
            Debug.Log("JumpPromptZone: 玩家進入跳躍提示區域");

            // 強制顯示提示（用於測試）
            if (promptButton != null)
            {
                promptButton.ShowPrompt(true);
                Debug.Log("JumpPromptZone: 已強制顯示跳躍提示");
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("JumpPromptZone: 玩家離開跳躍提示區域");

            // 隱藏提示
            if (promptButton != null)
            {
                promptButton.ShowPrompt(false);
                Debug.Log("JumpPromptZone: 已隱藏跳躍提示");
            }
        }
    }

    void Update()
    {
        // 每秒檢查一次距離（用於除錯）
        if (Time.frameCount % 60 == 0)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distance = Vector2.Distance(transform.position, player.transform.position);
                bool shouldShow = distance <= 2f;
                // Debug.Log($"JumpPromptZone: 玩家距離: {distance:F2}, 應該顯示: {shouldShow}");
            }
        }
    }
}
