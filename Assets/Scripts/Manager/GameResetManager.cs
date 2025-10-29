using UnityEngine;

/// <summary>
/// 遊戲重置管理器 - 在遊戲開始時清除所有遊戲紀錄
/// 包括存檔點、背包、收集的物品等
/// </summary>
public class GameResetManager : MonoBehaviour
{
    [Header("重置設置")]
    [Tooltip("是否在遊戲開始時自動清除所有存檔")]
    [SerializeField] private bool clearSaveOnStart = true;
    
    [Tooltip("是否清除背包")]
    [SerializeField] private bool clearInventory = true;
    
    [Tooltip("是否清除鑰匙")]
    [SerializeField] private bool clearKeys = true;
    
    [Tooltip("是否顯示調試訊息")]
    [SerializeField] private bool showDebugMessages = true;

    void Awake()
    {
        if (clearSaveOnStart)
        {
            ResetGameData();
        }
    }

    /// <summary>
    /// 重置所有遊戲資料
    /// </summary>
    public void ResetGameData()
    {
        if (showDebugMessages)
        {
            Debug.Log("==================================================");
            Debug.Log("🔄 GameResetManager: 開始清除遊戲紀錄...");
            Debug.Log("==================================================");
        }

        // 1. 清除存檔系統
        SaveSystem.ClearSaveData();
        if (showDebugMessages)
        {
            Debug.Log("✓ 已清除存檔點和收集物品紀錄");
        }

        // 2. 清除玩家背包
        if (clearInventory)
        {
            ClearPlayerInventory();
        }

        // 3. 清除玩家鑰匙
        if (clearKeys)
        {
            ClearPlayerKeys();
        }

        if (showDebugMessages)
        {
            Debug.Log("==================================================");
            Debug.Log("✅ GameResetManager: 遊戲紀錄清除完成！");
            Debug.Log("==================================================");
        }
    }

    /// <summary>
    /// 清除玩家背包
    /// </summary>
    private void ClearPlayerInventory()
    {
        // 尋找玩家物件
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player == null)
        {
            if (showDebugMessages)
            {
                Debug.LogWarning("⚠ 找不到玩家物件，無法清除背包");
            }
            return;
        }

        // 清除 InventorySystem
        InventorySystem inventory = player.GetComponent<InventorySystem>();
        if (inventory != null)
        {
            inventory.ClearInventory();
            if (showDebugMessages)
            {
                Debug.Log("✓ 已清除玩家背包 (InventorySystem)");
            }
        }
    }

    /// <summary>
    /// 清除玩家鑰匙
    /// </summary>
    private void ClearPlayerKeys()
    {
        // 尋找玩家物件
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player == null)
        {
            if (showDebugMessages)
            {
                Debug.LogWarning("⚠ 找不到玩家物件，無法清除鑰匙");
            }
            return;
        }

        // 清除 KeyInventory
        KeyInventory keyInventory = player.GetComponent<KeyInventory>();
        if (keyInventory != null)
        {
            keyInventory.ClearAllKeys();
            if (showDebugMessages)
            {
                Debug.Log("✓ 已清除玩家鑰匙 (KeyInventory)");
            }
        }
    }

    /// <summary>
    /// 手動清除遊戲資料（可在 Inspector 中調用）
    /// </summary>
    [ContextMenu("清除所有遊戲紀錄")]
    public void ManualResetGameData()
    {
        ResetGameData();
    }

    /// <summary>
    /// 僅清除存檔系統
    /// </summary>
    [ContextMenu("僅清除存檔系統")]
    public void ResetSaveSystemOnly()
    {
        SaveSystem.ClearSaveData();
        if (showDebugMessages)
        {
            Debug.Log("✓ 已清除存檔系統");
        }
    }

    /// <summary>
    /// 僅清除背包
    /// </summary>
    [ContextMenu("僅清除背包")]
    public void ResetInventoryOnly()
    {
        ClearPlayerInventory();
    }

    /// <summary>
    /// 僅清除鑰匙
    /// </summary>
    [ContextMenu("僅清除鑰匙")]
    public void ResetKeysOnly()
    {
        ClearPlayerKeys();
    }
}

