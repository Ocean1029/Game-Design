using UnityEngine;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 字體管理器 - 統一管理遊戲中所有的字體
/// </summary>
public class FontManager : MonoBehaviour
{
    public static FontManager Instance { get; private set; }

    [SerializeField]
    private TMP_FontAsset defaultFont;

    [SerializeField]
    private TMP_FontAsset pixellariFontAsset;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 如果 Inspector 中沒有設置字體，則嘗試動態加載
        if (pixellariFontAsset == null)
        {
            LoadPixellariFontDynamic();
        }
    }

    private void LoadPixellariFontDynamic()
    {
        #if UNITY_EDITOR
        // Editor 模式下使用 AssetDatabase
        string[] guids = AssetDatabase.FindAssets("Pixellari t:TMP_FontAsset");
        if (guids.Length > 0)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            pixellariFontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
            Debug.Log($"[FontManager] 從編輯器加載字體: {assetPath}");
        }
        else
        {
            Debug.LogWarning("[FontManager] 未找到 Pixellari TMP_FontAsset！");
        }
        #else
        // 運行時使用 Resources.Load
        pixellariFontAsset = Resources.Load<TMP_FontAsset>("Fonts/pixellari");
        if (pixellariFontAsset == null)
        {
            Debug.LogWarning("[FontManager] 未找到 Pixellari 字體資源！");
        }
        #endif
    }

    /// <summary>
    /// 獲取 Pixellari 字體
    /// </summary>
    public TMP_FontAsset GetPixellariFontAsset()
    {
        return pixellariFontAsset;
    }

    /// <summary>
    /// 為指定的 TextMeshProUGUI 設置 Pixellari 字體
    /// </summary>
    public void ApplyPixellariFont(TextMeshProUGUI textComponent)
    {
        if (textComponent != null && pixellariFontAsset != null)
        {
            textComponent.font = pixellariFontAsset;
        }
    }

    /// <summary>
    /// 為場景中所有的 TextMeshProUGUI 設置 Pixellari 字體
    /// </summary>
    public void ApplyPixellariFontToAll()
    {
        TextMeshProUGUI[] allTextComponents = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach (TextMeshProUGUI text in allTextComponents)
        {
            ApplyPixellariFont(text);
        }
        
        Debug.Log($"[FontManager] 已為 {allTextComponents.Length} 個文本組件設置 Pixellari 字體");
    }
}

#if UNITY_EDITOR
public class FontManagerEditor
{
    [MenuItem("Tools/Font Manager/Apply Pixellari To All Texts")]
    public static void ApplyPixellariFontToAllEditor()
    {
        FontManager fontManager = Object.FindAnyObjectByType<FontManager>();
        
        if (fontManager == null)
        {
            EditorUtility.DisplayDialog("錯誤", "場景中未找到 FontManager", "確定");
            return;
        }

        fontManager.ApplyPixellariFontToAll();
        EditorUtility.DisplayDialog("完成", "已為所有文本設置 Pixellari 字體", "確定");
    }

    [MenuItem("Tools/Font Manager/Create Font Manager")]
    public static void CreateFontManager()
    {
        GameObject fontManagerObj = new GameObject("FontManager");
        FontManager fontManager = fontManagerObj.AddComponent<FontManager>();
        
        // 使用 AssetDatabase 查找字體
        string[] guids = AssetDatabase.FindAssets("Pixellari t:TMP_FontAsset");
        
        if (guids.Length > 0)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            TMP_FontAsset pixellariFontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
            
            fontManager.GetType().GetField("pixellariFontAsset", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(fontManager, pixellariFontAsset);
            
            EditorUtility.DisplayDialog("成功", $"FontManager 已創建\n加載字體: {assetPath}", "確定");
        }
        else
        {
            EditorUtility.DisplayDialog("警告", 
                "FontManager 已創建，但未找到 Pixellari TMP_FontAsset\n\n" +
                "可能需要手動設置：\n" +
                "1. 在 Inspector 中找到 FontManager\n" +
                "2. 將 Pixellari 字體拖到 'Pixellari Font Asset' 欄位", 
                "確定");
        }
        
        Selection.activeGameObject = fontManagerObj;
    }
}
#endif

