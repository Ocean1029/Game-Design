using UnityEngine;
using System.Collections;

/// <summary>
/// 實體門腳本 - 有碰撞的門，需要鑰匙才能開啟
/// 當玩家碰撞到門時，如果有正確的鑰匙，會消耗鑰匙並將門變成透明
/// </summary>
public class PhysicalDoor : MonoBehaviour
{
    [Header("門的配置")]
    [Tooltip("開啟門所需的鑰匙標籤")]
    public string requiredKeyTag = "key1";
    
    [Tooltip("門開啟時的 Sprite（可選，如果設置則使用此 Sprite 而非透明度動畫）")]
    public Sprite doorOpenedSprite;
    
    [Tooltip("門開啟時的透明度 (0-1)")]
    [Range(0f, 1f)]
    public float openTransparency = 0.5f;
    
    [Tooltip("門開啟動畫的持續時間")]
    public float animationDuration = 1f;
    
    [Header("音效")]
    [Tooltip("門開啟時的音效")]
    public AudioClip openSound;
    
    [Tooltip("門關閉時的音效")]
    public AudioClip closeSound;
    
    [Header("視覺效果")]
    [Tooltip("門開啟時的粒子效果")]
    public GameObject openEffect;
    
    private bool isOpened = false;
    private bool isAnimating = false;
    private SpriteRenderer spriteRenderer;
    private Collider2D doorCollider;
    private AudioSource audioSource;
    private Color originalColor;
    private Color targetColor;
    
    void Start()
    {
        // 獲取組件
        spriteRenderer = GetComponent<SpriteRenderer>();
        doorCollider = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        
        // 如果沒有 AudioSource，添加一個
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // 檢查必要組件
        if (spriteRenderer == null)
        {
            Debug.LogError($"PhysicalDoor: {gameObject.name} 缺少 SpriteRenderer 組件！");
        }
        
        if (doorCollider == null)
        {
            Debug.LogError($"PhysicalDoor: {gameObject.name} 缺少 Collider2D 組件！");
        }
        else
        {
            // 確保碰撞器不是觸發器
            doorCollider.isTrigger = false;
        }
        
        // 儲存原始顏色
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 如果門已經開啟或正在動畫中，忽略碰撞
        if (isOpened || isAnimating) return;
        
        // 檢查碰撞的物件是否是玩家
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player == null) return;
        
        // 檢查玩家是否有鑰匙
        if (HasRequiredKey(player))
        {
            // 消耗鑰匙並開啟門
            ConsumeKeyAndOpenDoor(player);
        }
        else
        {
            // 玩家沒有鑰匙，顯示提示
            ShowKeyRequiredMessage();
        }
    }
    
    /// <summary>
    /// 檢查玩家是否有所需的鑰匙
    /// </summary>
    private bool HasRequiredKey(PlayerController player)
    {
        // 方法1: 檢查 InventorySystem (如果使用物品系統)
        InventorySystem inventory = player.GetComponent<InventorySystem>();
        if (inventory != null)
        {
            ItemData keyItem = inventory.GetItemForTag(requiredKeyTag);
            if (keyItem != null)
            {
                return true;
            }
        }
        
        // 方法2: 檢查 KeyInventory (如果使用鑰匙系統)
        KeyInventory keyInventory = player.GetComponent<KeyInventory>();
        if (keyInventory != null)
        {
            return keyInventory.HasKey(requiredKeyTag);
        }
        
        return false;
    }
    
    /// <summary>
    /// 消耗鑰匙並開啟門
    /// </summary>
    private void ConsumeKeyAndOpenDoor(PlayerController player)
    {
        if (isOpened || isAnimating) return;
        
        Debug.Log($"門被開啟！消耗鑰匙: {requiredKeyTag}");
        
        // 消耗鑰匙
        ConsumeKey(player);
        
        // 開始開啟動畫
        StartCoroutine(OpenDoorAnimation());
    }
    
    /// <summary>
    /// 消耗鑰匙
    /// </summary>
    private void ConsumeKey(PlayerController player)
    {
        // 方法1: 從 InventorySystem 消耗物品
        InventorySystem inventory = player.GetComponent<InventorySystem>();
        if (inventory != null)
        {
            ItemData keyItem = inventory.GetItemForTag(requiredKeyTag);
            if (keyItem != null)
            {
                inventory.RemoveItem(keyItem, 1);
                Debug.Log($"從物品欄消耗鑰匙: {keyItem.itemName}");
                return;
            }
        }
        
        // 方法2: 從 KeyInventory 消耗鑰匙
        KeyInventory keyInventory = player.GetComponent<KeyInventory>();
        if (keyInventory != null)
        {
            if (keyInventory.HasKey(requiredKeyTag))
            {
                keyInventory.RemoveKey(requiredKeyTag);
                Debug.Log($"從鑰匙欄消耗鑰匙: {requiredKeyTag}");
            }
        }
    }
    
    /// <summary>
    /// 顯示需要鑰匙的訊息
    /// </summary>
    private void ShowKeyRequiredMessage()
    {
        Debug.Log($"需要鑰匙才能開啟這扇門: {requiredKeyTag}");
        
        // 可以在這裡添加 UI 提示
        // 例如顯示浮動文字或 UI 提示
        FloatingTextManager floatingTextManager = FloatingTextManager.GetInstance();
        if (floatingTextManager != null)
        {
            floatingTextManager.ShowFloatingText("需要鑰匙！", transform.position, Color.red);
        }
    }
    
    /// <summary>
    /// 開啟門的動畫協程
    /// </summary>
    private IEnumerator OpenDoorAnimation()
    {
        isAnimating = true;
        isOpened = true;
        
        // 播放開啟音效
        if (openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }
        
        // 播放粒子效果
        if (openEffect != null)
        {
            GameObject effect = Instantiate(openEffect, transform.position, Quaternion.identity);
            Destroy(effect, 5f); // 5秒後銷毀粒子效果
        }
        
        // 如果有設置開啟 Sprite，直接切換 Sprite
        if (doorOpenedSprite != null && spriteRenderer != null)
        {
            Debug.Log("PhysicalDoor: 切換到開啟 Sprite");
            spriteRenderer.sprite = doorOpenedSprite;
            // 確保顏色完全不透明
            spriteRenderer.color = Color.white;
        }
        else
        {
            // 計算目標顏色（透明）
            targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, openTransparency);
            
            // 動畫參數
            float elapsedTime = 0f;
            Color startColor = originalColor;
            
            // 透明度動畫
            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / animationDuration;
                
                // 使用平滑插值
                Color currentColor = Color.Lerp(startColor, targetColor, progress);
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = currentColor;
                }
                
                yield return null;
            }
            
            // 確保最終顏色正確
            if (spriteRenderer != null)
            {
                spriteRenderer.color = targetColor;
            }
        }
        
        // 禁用碰撞器，讓玩家可以穿過
        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }
        
        isAnimating = false;
        
        Debug.Log("門已完全開啟！");
    }
    
    /// <summary>
    /// 關閉門（可選功能）
    /// </summary>
    public void CloseDoor()
    {
        if (!isOpened || isAnimating) return;
        
        StartCoroutine(CloseDoorAnimation());
    }
    
    /// <summary>
    /// 關閉門的動畫協程
    /// </summary>
    private IEnumerator CloseDoorAnimation()
    {
        isAnimating = true;
        
        // 播放關閉音效
        if (closeSound != null)
        {
            audioSource.PlayOneShot(closeSound);
        }
        
        // 重新啟用碰撞器
        if (doorCollider != null)
        {
            doorCollider.enabled = true;
        }
        
        // 計算目標顏色（不透明）
        Color startColor = spriteRenderer != null ? spriteRenderer.color : originalColor;
        
        // 動畫參數
        float elapsedTime = 0f;
        
        // 透明度動畫
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationDuration;
            
            // 使用平滑插值
            Color currentColor = Color.Lerp(startColor, originalColor, progress);
            if (spriteRenderer != null)
            {
                spriteRenderer.color = currentColor;
            }
            
            yield return null;
        }
        
        // 確保最終顏色正確
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        
        isOpened = false;
        isAnimating = false;
        
        Debug.Log("門已關閉！");
    }
    
    /// <summary>
    /// 檢查門是否已開啟
    /// </summary>
    public bool IsOpened()
    {
        return isOpened;
    }
    
    /// <summary>
    /// 重置門的狀態（用於測試或重新開始）
    /// </summary>
    public void ResetDoor()
    {
        isOpened = false;
        isAnimating = false;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        
        if (doorCollider != null)
        {
            doorCollider.enabled = true;
        }
        
        Debug.Log("門已重置！");
    }
}
