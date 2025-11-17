using UnityEngine;

/// <summary>
/// Represents a rappelling cable that allows an interactor to descend to a lower area
/// Cable starts broken and requires a rope item to repair
/// Broken cables are solid (blocking), repaired cables are passable (trigger)
/// Implements IInteractable to work with the interaction system
/// </summary>
public class cable : MonoBehaviour, IInteractable
{
    [Header("Cable State")]
    [Tooltip("是否預設為壞掉的狀態")]
    [SerializeField] private bool startBroken = true;
    
    [Header("Rope Requirement")]
    [Tooltip("修復 cable 所需的 rope ItemData 的 ID")]
    [SerializeField] private string requiredRopeItemId = "rope";
    
    [Tooltip("修復 cable 所需的 rope tag（用於查找物品）")]
    [SerializeField] private string requiredRopeTag = "rope";
    
    [Header("Rappelling Configuration")]
    [Tooltip("Target position where interactor will land after rappelling")]
    public Transform targetFloorPoint;
    
    [Tooltip("Animator trigger name for cable animation")]
    public string animationTriggerName = "player_enter";

    [Header("UI Prompts (Optional)")]
    [Tooltip("UI element shown when interactor can use the cable")]
    public GameObject usePrompt;
    
    [Header("Visual Feedback")]
    [Tooltip("壞掉狀態的視覺物件（可選）")]
    [SerializeField] private GameObject brokenVisual;
    
    [Tooltip("修復狀態的視覺物件（可選）")]
    [SerializeField] private GameObject repairedVisual;

    private Animator cableAnimator;
    private bool hasBeenUsed = false;
    private bool isBroken = true;
    private bool isPlayerNearby = false;
    private PlayerController nearbyPlayer = null;
    private IInteractor currentInteractor = null;
    private Collider2D cableCollider;
    private InventoryPromptManager promptManager;

    void Start()
    {
        // Get components
        cableAnimator = GetComponent<Animator>();
        cableCollider = GetComponent<Collider2D>();
        promptManager = InventoryPromptManager.GetInstance();
        
        if (cableAnimator == null)
        {
            Debug.LogWarning("Cable object does not have an Animator component. Rappelling will work but without cable animation.");
        }

        if (targetFloorPoint == null)
        {
            Debug.LogError("Cable: targetFloorPoint is not assigned! Interactor will not be able to rappel.");
        }
        
        if (cableCollider == null)
        {
            Debug.LogError("Cable: No Collider2D found! Cable will not work properly.");
        }

        // Hide prompt at start
        if (usePrompt != null)
        {
            usePrompt.SetActive(false);
        }
        
        // 設置初始狀態
        isBroken = startBroken;
        UpdateCableState();
    }
    
    void Update()
    {
        // 如果玩家在附近且 cable 壞掉且有 rope，按 Z 鍵修復
        if (isPlayerNearby && isBroken && !hasBeenUsed && nearbyPlayer != null)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (HasRequiredRope(nearbyPlayer))
                {
                    RepairCable(nearbyPlayer);
                }
            }
        }
    }

    // ==================== IInteractable Implementation ====================

    /// <summary>
    /// Called when an interactor enters the cable's interaction zone
    /// </summary>
    public void OnInteractorEnterZone(IInteractor interactor)
    {
        // Don't allow interaction if already used
        if (hasBeenUsed)
        {
            return;
        }

        Debug.Log("Interactor entered cable zone");
        currentInteractor = interactor;

        // 獲取玩家
        PlayerController player = interactor.GetGameObject().GetComponent<PlayerController>();
        if (player != null)
        {
            isPlayerNearby = true;
            nearbyPlayer = player;
        }

        // 如果 cable 壞掉，檢查玩家是否有 rope
        if (isBroken)
        {
            if (player != null && HasRequiredRope(player))
            {
                // 顯示背包 rope 物件上方的按鍵提示
                ShowRopePrompt(player);
            }
            else
            {
                // 顯示需要 rope 的訊息
                ShowRopeRequiredMessage();
            }
        }
        else
        {
            // Cable 已修復，顯示使用提示
        if (usePrompt != null)
        {
            usePrompt.SetActive(true);
            Debug.Log("Showing rappel prompt");
            }
        }
    }

    /// <summary>
    /// Called when an interactor exits the cable's interaction zone
    /// </summary>
    public void OnInteractorExitZone(IInteractor interactor)
    {
        // Hide prompt when interactor leaves
        if (usePrompt != null)
        {
            usePrompt.SetActive(false);
        }

        // 隱藏 rope 提示
        HideRopePrompt();
        
        isPlayerNearby = false;
        nearbyPlayer = null;
        currentInteractor = null;
    }

    /// <summary>
    /// Called when an interactor presses the interact button while near the cable
    /// </summary>
    public bool Interact(IInteractor interactor)
    {
        // 如果 cable 壞掉，不能直接使用（需要先修復）
        if (isBroken)
        {
            Debug.Log("Cable is broken! Requires rope to repair.");
            
            // 檢查玩家是否有 rope
            PlayerController tempPlayer = interactor.GetGameObject().GetComponent<PlayerController>();
            if (tempPlayer != null && HasRequiredRope(tempPlayer))
            {
                // 有 rope，嘗試修復（由 Update 中的 Z 鍵處理）
                ShowRopeRequiredMessage();
            }
            else
            {
                ShowRopeRequiredMessage();
            }
            return false;
        }
        
        // Prevent multiple uses
        if (hasBeenUsed)
        {
            Debug.Log("Cable has already been used");
            return false;
        }

        if (targetFloorPoint == null)
        {
            Debug.LogError("Cable: Cannot start rappelling - targetFloorPoint is missing!");
            return false;
        }

        Debug.Log("Cable Interact() called - starting rappelling");

        // Play cable animation if available
        if (cableAnimator != null)
        {
            cableAnimator.SetTrigger(animationTriggerName);
        }

        // Start rappelling sequence (player-specific for now)
        // Note: For full decoupling, consider using an IRappelable interface
        if (interactor is PlayerController player)
        {
            player.StartRappelling(targetFloorPoint);
            
            // Hide prompt
            if (usePrompt != null)
            {
                usePrompt.SetActive(false);
            }

            // Disable the trigger to prevent repeated activation
            if (cableCollider != null)
            {
                cableCollider.enabled = false;
            }

            hasBeenUsed = true;
            return true;
        }

        Debug.LogWarning("Cable: Interactor is not a PlayerController, cannot rappel");
        return false;
    }

    /// <summary>
    /// Get the GameObject this interactable belongs to
    /// </summary>
    public GameObject GetGameObject()
    {
        return gameObject;
    }
    
    // ==================== Cable State Management ====================
    
    /// <summary>
    /// 更新 cable 的狀態（壞掉/修復）
    /// </summary>
    private void UpdateCableState()
    {
        if (cableCollider != null)
        {
            if (isBroken)
            {
                // 壞掉：不可穿透（solid collider）
                cableCollider.isTrigger = false;
                Debug.Log("Cable: Set to BROKEN state (solid collider)");
            }
            else
            {
                // 修復：可穿透（trigger）
                cableCollider.isTrigger = true;
                Debug.Log("Cable: Set to REPAIRED state (trigger)");
            }
        }
        
        // 更新視覺效果
        if (brokenVisual != null)
        {
            brokenVisual.SetActive(isBroken);
        }
        
        if (repairedVisual != null)
        {
            repairedVisual.SetActive(!isBroken);
        }
    }
    
    /// <summary>
    /// 檢查玩家是否有所需的 rope
    /// </summary>
    private bool HasRequiredRope(PlayerController player)
    {
        InventorySystem inventory = player.GetComponent<InventorySystem>();
        if (inventory != null)
        {
            // 方法1: 通過 itemId 檢查
            ItemData ropeItem = inventory.GetItemById(requiredRopeItemId);
            if (ropeItem != null)
            {
                return true;
            }
            
            // 方法2: 通過 tag 檢查
            ItemData ropeByTag = inventory.GetItemForTag(requiredRopeTag);
            if (ropeByTag != null)
            {
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// 修復 cable（消耗 rope）
    /// </summary>
    private void RepairCable(PlayerController player)
    {
        if (!isBroken)
        {
            Debug.Log("Cable is already repaired!");
            return;
        }
        
        Debug.Log($"Cable: Attempting to repair with rope");
        
        // 先隱藏提示
        HideRopePrompt();
        
        // 消耗 rope
        ConsumeRope(player);
        
        // 修復 cable
        isBroken = false;
        UpdateCableState();
        
        Debug.Log("Cable: Successfully repaired!");
        
        // Show floating text
        FloatingTextManager floatingTextManager = FloatingTextManager.GetInstance();
        if (floatingTextManager != null)
        {
            floatingTextManager.ShowFloatingText("Cable repaired!", transform.position, Color.green);
        }
    }
    
    /// <summary>
    /// 消耗 rope
    /// </summary>
    private void ConsumeRope(PlayerController player)
    {
        InventorySystem inventory = player.GetComponent<InventorySystem>();
        if (inventory != null)
        {
            // 方法1: 通過 itemId 消耗
            ItemData ropeItem = inventory.GetItemById(requiredRopeItemId);
            if (ropeItem != null)
            {
                inventory.RemoveItem(ropeItem, 1);
                Debug.Log($"Cable: Consumed rope (ID: {requiredRopeItemId})");
                return;
            }
            
            // 方法2: 通過 tag 消耗
            ItemData ropeByTag = inventory.GetItemForTag(requiredRopeTag);
            if (ropeByTag != null)
            {
                inventory.RemoveItem(ropeByTag, 1);
                Debug.Log($"Cable: Consumed rope (Tag: {requiredRopeTag})");
            }
        }
    }
    
    /// <summary>
    /// Show message that rope is required to repair
    /// </summary>
    private void ShowRopeRequiredMessage()
    {
        Debug.Log($"Cable is broken! Need rope to repair.");
        
        FloatingTextManager floatingTextManager = FloatingTextManager.GetInstance();
        if (floatingTextManager != null)
        {
            floatingTextManager.ShowFloatingText("Rope required to repair!", transform.position, Color.red);
        }
    }
    
    /// <summary>
    /// 在背包 rope 物件上方顯示按鍵提示
    /// </summary>
    private void ShowRopePrompt(PlayerController player)
    {
        if (promptManager == null)
        {
            promptManager = InventoryPromptManager.GetInstance();
        }
        
        if (promptManager != null)
        {
            InventorySystem inventory = player.GetComponent<InventorySystem>();
            if (inventory != null)
            {
                // 先嘗試通過 itemId 顯示
                promptManager.ShowPromptForItem(requiredRopeItemId);
                
                // 如果找不到，嘗試通過 tag 顯示
                ItemData ropeByTag = inventory.GetItemForTag(requiredRopeTag);
                if (ropeByTag != null)
                {
                    promptManager.ShowPromptForItem(ropeByTag.itemId);
                }
                
                Debug.Log($"Cable: Showing rope prompt (ID: {requiredRopeItemId})");
            }
        }
    }
    
    /// <summary>
    /// 隱藏背包 rope 物件上方的按鍵提示
    /// </summary>
    private void HideRopePrompt()
    {
        if (promptManager != null && nearbyPlayer != null)
        {
            InventorySystem inventory = nearbyPlayer.GetComponent<InventorySystem>();
            if (inventory != null)
            {
                // 隱藏 itemId 提示
                promptManager.HidePromptForItem(requiredRopeItemId);
                
                // 隱藏 tag 提示
                ItemData ropeByTag = inventory.GetItemForTag(requiredRopeTag);
                if (ropeByTag != null)
                {
                    promptManager.HidePromptForItem(ropeByTag.itemId);
                }
                
                Debug.Log($"Cable: Hiding rope prompt");
            }
        }
    }
    
    /// <summary>
    /// 檢查 cable 是否壞掉
    /// </summary>
    public bool IsBroken()
    {
        return isBroken;
    }
    
    /// <summary>
    /// 手動設置 cable 狀態（用於測試或特殊情況）
    /// </summary>
    public void SetBroken(bool broken)
    {
        isBroken = broken;
        UpdateCableState();
    }
}
