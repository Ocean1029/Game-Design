using UnityEngine;

/// <summary>
/// 自動為可互動物件添加 InteractionPrompt 的輔助組件
/// 這個組件會自動檢查物件是否為可互動物件，並自動添加 InteractionPrompt
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class AutoInteractionPrompt : MonoBehaviour
{
    [Header("自動設置選項")]
    [Tooltip("是否在 Start 時自動添加 InteractionPrompt")]
    [SerializeField] private bool autoAddOnStart = true;
    
    [Tooltip("是否覆蓋已存在的 InteractionPrompt")]
    [SerializeField] private bool overrideExisting = false;
    
    [Header("InteractionPrompt 設定（僅在自動添加時使用）")]
    [Tooltip("按鍵圖片（留空則使用預設）")]
    [SerializeField] private Sprite buttonSprite;
    
    [Tooltip("提示位置偏移")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1f, 0f);
    
    [Tooltip("提示尺寸")]
    [SerializeField] private Vector2 buttonSize = new Vector2(5f, 5f);
    
    [Tooltip("檢測範圍（如果使用距離檢測）")]
    [SerializeField] private float detectionRadius = 2f;
    
    [Tooltip("使用距離檢測而非 Trigger")]
    [SerializeField] private bool useDistanceDetection = false;
    
    private InteractionPrompt interactionPrompt;
    
    void Start()
    {
        if (autoAddOnStart)
        {
            AddInteractionPromptIfNeeded();
        }
    }
    
    /// <summary>
    /// 檢查並添加 InteractionPrompt（如果需要）
    /// </summary>
    public void AddInteractionPromptIfNeeded()
    {
        // 檢查是否已經有 InteractionPrompt
        interactionPrompt = GetComponent<InteractionPrompt>();
        
        if (interactionPrompt != null && !overrideExisting)
        {
            if (showDebugInfo)
            {
                Debug.Log($"AutoInteractionPrompt: {gameObject.name} 已經有 InteractionPrompt，跳過自動添加");
            }
            return;
        }
        
        // 檢查物件是否為可互動物件
        bool isInteractable = GetComponent<IInteractable>() != null;
        bool isCollectable = GetComponent<CollectableItem>() != null;
        
        if (!isInteractable && !isCollectable)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning($"AutoInteractionPrompt: {gameObject.name} 不是可互動物件（沒有 IInteractable 或 CollectableItem），跳過自動添加");
            }
            return;
        }
        
        // 添加或更新 InteractionPrompt
        if (interactionPrompt == null)
        {
            interactionPrompt = gameObject.AddComponent<InteractionPrompt>();
        }
        
        // 設置 InteractionPrompt 的屬性
        if (buttonSprite != null)
        {
            interactionPrompt.SetButtonSprite(buttonSprite);
        }
        
        interactionPrompt.SetWorldOffset(worldOffset);
        
        // 使用反射設置私有欄位（因為這些是序列化欄位）
        SetPrivateField(interactionPrompt, "buttonSize", buttonSize);
        SetPrivateField(interactionPrompt, "detectionRadius", detectionRadius);
        SetPrivateField(interactionPrompt, "useDistanceDetection", useDistanceDetection);
        
        if (showDebugInfo)
        {
            Debug.Log($"AutoInteractionPrompt: 已為 {gameObject.name} 添加 InteractionPrompt");
        }
    }
    
    /// <summary>
    /// 使用反射設置私有欄位
    /// </summary>
    private void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName, 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            field.SetValue(obj, value);
        }
    }
    
    [Header("除錯設定")]
    [Tooltip("是否顯示除錯資訊")]
    [SerializeField] private bool showDebugInfo = false;
}

