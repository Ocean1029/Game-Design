using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 管理背包物件提示的中央系統
/// 負責在特定 slot 上顯示/隱藏按鍵提示
/// </summary>
public class InventoryPromptManager : MonoBehaviour
{
    private static InventoryPromptManager instance;
    
    [Header("設定")]
    [Tooltip("是否顯示除錯資訊")]
    [SerializeField] private bool showDebugInfo = false;
    
    // 儲存所有 slot 的提示組件
    private Dictionary<string, InventorySlotPrompt> slotPrompts = new Dictionary<string, InventorySlotPrompt>();
    
    void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    
    /// <summary>
    /// 獲取單例實例
    /// </summary>
    public static InventoryPromptManager GetInstance()
    {
        return instance;
    }
    
    /// <summary>
    /// 註冊一個 slot 的提示組件
    /// </summary>
    public void RegisterSlotPrompt(string itemId, InventorySlotPrompt prompt)
    {
        if (slotPrompts.ContainsKey(itemId))
        {
            if (showDebugInfo)
            {
                Debug.LogWarning($"InventoryPromptManager: Slot prompt for '{itemId}' already registered. Replacing.");
            }
        }
        
        slotPrompts[itemId] = prompt;
        
        if (showDebugInfo)
        {
            Debug.Log($"InventoryPromptManager: Registered slot prompt for '{itemId}'");
        }
    }
    
    /// <summary>
    /// 取消註冊 slot 提示組件
    /// </summary>
    public void UnregisterSlotPrompt(string itemId)
    {
        if (slotPrompts.ContainsKey(itemId))
        {
            slotPrompts.Remove(itemId);
            
            if (showDebugInfo)
            {
                Debug.Log($"InventoryPromptManager: Unregistered slot prompt for '{itemId}'");
            }
        }
    }
    
    /// <summary>
    /// 在指定的 item slot 上顯示按鍵提示
    /// </summary>
    /// <param name="itemId">物品的 ID（例如：key1、bomb_for_rock）</param>
    public void ShowPromptForItem(string itemId)
    {
        if (slotPrompts.ContainsKey(itemId))
        {
            slotPrompts[itemId].ShowPrompt();
            
            if (showDebugInfo)
            {
                Debug.Log($"InventoryPromptManager: Showing prompt for '{itemId}'");
            }
        }
        else
        {
            if (showDebugInfo)
            {
                Debug.LogWarning($"InventoryPromptManager: No slot prompt registered for '{itemId}'");
            }
        }
    }
    
    /// <summary>
    /// 隱藏指定的 item slot 上的按鍵提示
    /// </summary>
    /// <param name="itemId">物品的 ID</param>
    public void HidePromptForItem(string itemId)
    {
        if (slotPrompts.ContainsKey(itemId))
        {
            slotPrompts[itemId].HidePrompt();
            
            if (showDebugInfo)
            {
                Debug.Log($"InventoryPromptManager: Hiding prompt for '{itemId}'");
            }
        }
    }
    
    /// <summary>
    /// 顯示多個物品的提示（用於同一 tag 對應多個物品的情況）
    /// </summary>
    /// <param name="itemIds">物品 ID 陣列</param>
    public void ShowPromptsForItems(params string[] itemIds)
    {
        foreach (string itemId in itemIds)
        {
            ShowPromptForItem(itemId);
        }
    }
    
    /// <summary>
    /// 隱藏多個物品的提示
    /// </summary>
    /// <param name="itemIds">物品 ID 陣列</param>
    public void HidePromptsForItems(params string[] itemIds)
    {
        foreach (string itemId in itemIds)
        {
            HidePromptForItem(itemId);
        }
    }
    
    /// <summary>
    /// 隱藏所有提示
    /// </summary>
    public void HideAllPrompts()
    {
        foreach (var prompt in slotPrompts.Values)
        {
            prompt.HidePrompt();
        }
        
        if (showDebugInfo)
        {
            Debug.Log("InventoryPromptManager: Hiding all prompts");
        }
    }
    
    /// <summary>
    /// 檢查指定物品的提示是否正在顯示
    /// </summary>
    public bool IsPromptVisibleForItem(string itemId)
    {
        if (slotPrompts.ContainsKey(itemId))
        {
            return slotPrompts[itemId].IsPromptVisible();
        }
        return false;
    }
    
    /// <summary>
    /// 根據 tag 顯示提示（自動查找對應的 itemId）
    /// </summary>
    public void ShowPromptForTag(string tag, InventorySystem inventorySystem)
    {
        ItemData item = inventorySystem.GetItemForTag(tag);
        if (item != null)
        {
            ShowPromptForItem(item.itemId);
        }
        else
        {
            if (showDebugInfo)
            {
                Debug.LogWarning($"InventoryPromptManager: No item found with tag '{tag}'");
            }
        }
    }
    
    /// <summary>
    /// 根據 tag 隱藏提示
    /// </summary>
    public void HidePromptForTag(string tag, InventorySystem inventorySystem)
    {
        ItemData item = inventorySystem.GetItemForTag(tag);
        if (item != null)
        {
            HidePromptForItem(item.itemId);
        }
    }
}

