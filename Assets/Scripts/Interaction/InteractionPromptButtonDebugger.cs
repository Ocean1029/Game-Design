using UnityEngine;

/// <summary>
/// 除錯工具：幫助檢查 InteractionPromptButton 的設置是否正確
/// 將此組件添加到有 InteractionPromptButton 的物件上來診斷問題
/// </summary>
public class InteractionPromptButtonDebugger : MonoBehaviour
{
    private InteractionPromptButton promptButton;
    private Collider2D collider2D;

    void Start()
    {
        promptButton = GetComponent<InteractionPromptButton>();
        collider2D = GetComponent<Collider2D>();

        Debug.Log($"=== InteractionPromptButton 診斷報告 ===");
        Debug.Log($"物件名稱: {gameObject.name}");
        Debug.Log($"位置: {transform.position}");
        Debug.Log($"層級: {gameObject.layer}");

        // 檢查 Collider2D
        if (collider2D == null)
        {
            Debug.LogError("❌ 缺少 Collider2D 組件！");
        }
        else
        {
            Debug.Log($"✅ Collider2D 類型: {collider2D.GetType().Name}");
            Debug.Log($"   - Is Trigger: {collider2D.isTrigger}");

            // 檢查碰撞器大小
            if (collider2D is BoxCollider2D box)
            {
                Debug.Log($"   - 大小: {box.size}");
                Debug.Log($"   - 偏移: {box.offset}");
            }
            else if (collider2D is CircleCollider2D circle)
            {
                Debug.Log($"   - 半徑: {circle.radius}");
                Debug.Log($"   - 偏移: {circle.offset}");
            }
        }

        // 檢查 InteractionPromptButton
        if (promptButton == null)
        {
            Debug.LogError("❌ 缺少 InteractionPromptButton 組件！");
        }
        else
        {
            Debug.Log("✅ InteractionPromptButton 組件存在");

            // 檢查按鍵圖片
            var spriteField = promptButton.GetType().GetField("buttonSprite", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (spriteField != null)
            {
                Sprite buttonSprite = (Sprite)spriteField.GetValue(promptButton);
                if (buttonSprite == null)
                {
                    Debug.LogWarning("⚠️ 按鍵圖片未設置，將嘗試自動載入");
                }
                else
                {
                    Debug.Log($"✅ 按鍵圖片已設置: {buttonSprite.name}");
                }
            }

            // 檢查檢測設定
            var detectionField = promptButton.GetType().GetField("useDistanceDetection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (detectionField != null)
            {
                bool useDistance = (bool)detectionField.GetValue(promptButton);
                Debug.Log($"   - 使用距離檢測: {useDistance}");
            }
        }

        // 檢查玩家物件
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("❌ 找不到標籤為 'Player' 的物件！");
        }
        else
        {
            Debug.Log($"✅ 找到玩家物件: {player.name}");
            Debug.Log($"   - 玩家位置: {player.transform.position}");
            float distance = Vector2.Distance(transform.position, player.transform.position);
            Debug.Log($"   - 距離: {distance}");

            // 檢查玩家是否有 PlayerController
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("❌ 玩家物件缺少 PlayerController 組件！");
            }
            else
            {
                Debug.Log("✅ 玩家有 PlayerController 組件");
            }
        }

        // 檢查渲染層級
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Debug.Log($"   - SpriteRenderer 排序順序: {spriteRenderer.sortingOrder}");
            Debug.Log($"   - SpriteRenderer 排序層級: {spriteRenderer.sortingLayerName}");
        }

        Debug.Log("=== 診斷完成 ===");
    }

    void Update()
    {
        // 每秒檢查一次玩家距離（避免過多日誌）
        if (Time.frameCount % 60 == 0) // 每秒檢查一次
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distance = Vector2.Distance(transform.position, player.transform.position);
                bool inRange = distance <= 2f; // 假設檢測範圍為 2

                if (inRange)
                {
                    Debug.Log($"玩家在範圍內，距離: {distance:F2}");
                }
            }
        }
    }
}
