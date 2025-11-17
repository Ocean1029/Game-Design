using UnityEngine;

/// <summary>
/// Handles bomb usage from inventory — spawns a bomb object and plays its animation
/// Shows prompt on bomb item in backpack when near stone
/// </summary>
public class PlayerUseBomb : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private InventorySystem inventorySystem;

    [Header("Bomb Settings")]
    [Tooltip("The Item ID for the bomb in the inventory")]
    [SerializeField] private string bombItemId = "bomb_for_rock";

    [Tooltip("The prefab to spawn when bomb is used")]
    [SerializeField] private GameObject bombPrefab;

    [Tooltip("Offset from player position when spawning bomb")]
    [SerializeField] private Vector2 spawnOffset = new Vector2(0f, -0.5f);

    private Stone nearbyStone = null;
    private InventoryPromptManager promptManager;

    private void Start()
    {
        if (inventorySystem == null)
        {
            inventorySystem = GetComponent<InventorySystem>();
        }

        if (inventorySystem == null)
        {
            Debug.LogError("PlayerUseBomb: InventorySystem not found on player!");
        }

        if (bombPrefab == null)
        {
            Debug.LogWarning("PlayerUseBomb: No bomb prefab assigned!");
        }
        
        promptManager = InventoryPromptManager.GetInstance();

        Debug.Log($"[PlayerUseBomb] inventorySystem found? {inventorySystem != null}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (nearbyStone != null)
                TryUseBomb();
        }
    }

    public void SetNearStone(bool isNear, Stone stone)
    {
        // 更新石頭狀態
        bool wasNear = nearbyStone != null;
        nearbyStone = isNear ? stone : null;
        
        // 管理背包炸彈提示的顯示/隱藏
        if (isNear && !wasNear)
        {
            // 玩家進入石頭範圍，檢查是否有炸彈
            if (HasBomb())
            {
                ShowBombPrompt();
            }
        }
        else if (!isNear && wasNear)
        {
            // 玩家離開石頭範圍，隱藏提示
            HideBombPrompt();
        }
    }
    
    /// <summary>
    /// 檢查玩家是否有炸彈
    /// </summary>
    private bool HasBomb()
    {
        if (inventorySystem == null) return false;
        ItemData bombItem = inventorySystem.GetItemById(bombItemId);
        return bombItem != null;
    }
    
    /// <summary>
    /// 在背包炸彈物件上方顯示按鍵提示
    /// </summary>
    private void ShowBombPrompt()
    {
        if (promptManager == null)
        {
            promptManager = InventoryPromptManager.GetInstance();
        }
        
        if (promptManager != null)
        {
            promptManager.ShowPromptForItem(bombItemId);
            Debug.Log($"PlayerUseBomb: 顯示炸彈按鍵提示 (itemId: {bombItemId})");
        }
    }
    
    /// <summary>
    /// 隱藏背包炸彈物件上方的按鍵提示
    /// </summary>
    private void HideBombPrompt()
    {
        if (promptManager != null)
        {
            promptManager.HidePromptForItem(bombItemId);
            Debug.Log($"PlayerUseBomb: 隱藏炸彈按鍵提示 (itemId: {bombItemId})");
        }
    }

    private void TryUseBomb()
    {
        if (inventorySystem == null) return;

        // 1️⃣ 檢查背包內是否有炸彈
        ItemData bombItem = inventorySystem.GetItemById(bombItemId);
        if (bombItem == null)
        {
            Debug.Log("💣 PlayerUseBomb: No bomb in inventory!");
            return;
        }

        // 先隱藏提示（在使用炸彈之前）
        HideBombPrompt();

        // 2️⃣ 生成炸彈物件
        if (bombPrefab != null)
        {
            // 距離可自行調整
            float facingDir = Mathf.Sign(transform.localScale.x);
            Vector3 spawnOffset = new Vector3(0.8f * facingDir, 0f, 0f);
            Vector3 spawnPosition = transform.position + spawnOffset;

            GameObject spawnedBomb = Instantiate(bombPrefab, spawnPosition, Quaternion.identity);

            Debug.Log("💣 PlayerUseBomb: Spawned bomb prefab");

            // 3️⃣ 播放炸彈動畫
            Animator bombAnimator = spawnedBomb.GetComponent<Animator>();
            if (bombAnimator != null)
            {
                bombAnimator.SetTrigger("Explode"); // trigger 名稱請確認 Animator 裡有
            }
            else
            {
                Debug.LogWarning("PlayerUseBomb: Bomb prefab has no Animator component!");
            }

            // 4️⃣ 延遲銷毀炸彈（讓動畫播完）
            Destroy(spawnedBomb, 1.5f);
        }

        // 5️⃣ 從背包移除炸彈
        inventorySystem.RemoveItem(bombItem, 1);
    }
}
