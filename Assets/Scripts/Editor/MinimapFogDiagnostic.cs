using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

/// <summary>
/// 小地圖迷霧系統診斷工具
/// 用於檢查和修復常見的設置問題
/// </summary>
public class MinimapFogDiagnostic : EditorWindow
{
    [MenuItem("Tools/Minimap Fog Diagnostic")]
    public static void ShowWindow()
    {
        GetWindow<MinimapFogDiagnostic>("Minimap Fog Diagnostic");
    }

    private Vector2 scrollPosition;

    void OnGUI()
    {
        GUILayout.Label("小地圖迷霧系統診斷", EditorStyles.boldLabel);
        GUILayout.Space(10);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        if (GUILayout.Button("檢查迷霧系統設置", GUILayout.Height(30)))
        {
            CheckFogSystem();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("修復 FogOverlay 設置", GUILayout.Height(30)))
        {
            FixFogOverlay();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("創建/更新迷霧材質", GUILayout.Height(30)))
        {
            CreateFogMaterial();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("測試：揭開所有迷霧", GUILayout.Height(30)))
        {
            RevealAllFog();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("測試：重置所有迷霧", GUILayout.Height(30)))
        {
            ResetAllFog();
        }

        EditorGUILayout.EndScrollView();
    }

    private void CheckFogSystem()
    {
        Debug.Log("===== 開始診斷小地圖迷霧系統 =====");

        // 檢查 MinimapFogOfWar
        MinimapFogOfWar fogSystem = FindFirstObjectByType<MinimapFogOfWar>();
        if (fogSystem == null)
        {
            Debug.LogError("❌ 找不到 MinimapFogOfWar 組件！請創建並設置。");
        }
        else
        {
            Debug.Log("✓ 找到 MinimapFogOfWar 組件");
            
            Texture2D fogTexture = fogSystem.GetFogTexture();
            if (fogTexture != null)
            {
                Debug.Log($"✓ 迷霧 Texture 已創建：{fogTexture.width}x{fogTexture.height}");
            }
            else
            {
                if (Application.isPlaying)
                {
                    Debug.LogError("❌ 迷霧 Texture 為 null！系統可能未初始化。");
                }
                else
                {
                    Debug.LogWarning("⚠️ 迷霧 Texture 為 null（編輯器模式）。嘗試手動初始化...");
                    fogSystem.Initialize();
                    
                    fogTexture = fogSystem.GetFogTexture();
                    if (fogTexture != null)
                    {
                        Debug.Log($"✓ 迷霧 Texture 已手動創建：{fogTexture.width}x{fogTexture.height}");
                    }
                    else
                    {
                        Debug.LogError("❌ 無法創建迷霧 Texture！請檢查 Enable Fog 是否勾選。");
                    }
                }
            }
        }

        // 檢查 MinimapFogRenderer
        MinimapFogRenderer fogRenderer = FindFirstObjectByType<MinimapFogRenderer>();
        if (fogRenderer == null)
        {
            Debug.LogError("❌ 找不到 MinimapFogRenderer 組件！請在 FogOverlay 上添加。");
        }
        else
        {
            Debug.Log("✓ 找到 MinimapFogRenderer 組件");
            
            RawImage rawImage = fogRenderer.GetComponent<RawImage>();
            if (rawImage != null)
            {
                Debug.Log($"✓ RawImage 組件存在");
                Debug.Log($"  - Texture: {(rawImage.texture != null ? rawImage.texture.name : "null")}");
                Debug.Log($"  - UV Rect: {rawImage.uvRect}");
                Debug.Log($"  - Color: {rawImage.color}");
                Debug.Log($"  - Size: {rawImage.rectTransform.rect.size}");
                Debug.Log($"  - Anchors: Min={rawImage.rectTransform.anchorMin}, Max={rawImage.rectTransform.anchorMax}");
                Debug.Log($"  - Material: {(rawImage.material != null ? rawImage.material.name : "null")}");
                
                if (rawImage.uvRect != new Rect(0, 0, 1, 1))
                {
                    Debug.LogWarning("⚠️ UV Rect 不是 (0,0,1,1)，可能導致顯示不完整！");
                }
                
                if (rawImage.color != Color.white)
                {
                    Debug.LogWarning($"⚠️ RawImage 顏色不是白色，當前為 {rawImage.color}");
                }
                
                if (rawImage.material == null)
                {
                    Debug.LogWarning("⚠️ 沒有設置材質！建議使用 'UI/MinimapFog' Shader 的材質。");
                }
                else if (rawImage.material.shader.name != "UI/MinimapFog")
                {
                    Debug.LogWarning($"⚠️ 材質使用的 Shader 是 '{rawImage.material.shader.name}'，建議使用 'UI/MinimapFog'。");
                }
            }
            else
            {
                Debug.LogError("❌ RawImage 組件不存在！");
            }
        }

        // 檢查 Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("⚠️ 找不到 Player！請確保 Player GameObject 有 'Player' Tag。");
        }
        else
        {
            Debug.Log($"✓ 找到 Player: {player.name} at {player.transform.position}");
        }

        Debug.Log("===== 診斷完成 =====");
    }

    private void FixFogOverlay()
    {
        Debug.Log("===== 開始修復 FogOverlay 設置 =====");

        MinimapFogRenderer fogRenderer = FindFirstObjectByType<MinimapFogRenderer>();
        if (fogRenderer == null)
        {
            Debug.LogError("❌ 找不到 MinimapFogRenderer！請先創建 FogOverlay 並添加此組件。");
            return;
        }

        RawImage rawImage = fogRenderer.GetComponent<RawImage>();
        if (rawImage == null)
        {
            Debug.LogError("❌ FogOverlay 上沒有 RawImage 組件！");
            return;
        }

        // 修復 UV Rect
        rawImage.uvRect = new Rect(0, 0, 1, 1);
        Debug.Log("✓ 設置 UV Rect 為 (0, 0, 1, 1)");

        // 修復顏色
        rawImage.color = Color.white;
        Debug.Log("✓ 設置顏色為白色");

        // 修復 RectTransform
        RectTransform rectTransform = rawImage.rectTransform;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        Debug.Log("✓ 設置 RectTransform 為 Stretch 全滿");

        // 確保迷霧 Texture 已設置
        MinimapFogOfWar fogSystem = MinimapFogOfWar.GetInstance();
        if (fogSystem == null)
        {
            fogSystem = FindFirstObjectByType<MinimapFogOfWar>();
        }
        
        if (fogSystem != null)
        {
            // 確保系統已初始化
            fogSystem.Initialize();
            
            Texture2D fogTexture = fogSystem.GetFogTexture();
            if (fogTexture != null)
            {
                rawImage.texture = fogTexture;
                Debug.Log($"✓ 設置迷霧 Texture ({fogTexture.width}x{fogTexture.height})");
            }
            else
            {
                Debug.LogError("❌ 無法獲取迷霧 Texture！請確認 MinimapFogOfWar 的 Enable Fog 已勾選。");
            }
        }
        else
        {
            Debug.LogError("❌ 找不到 MinimapFogOfWar 組件！");
        }

        EditorUtility.SetDirty(rawImage);
        Debug.Log("===== 修復完成！請運行遊戲測試。 =====");
    }

    private void CreateFogMaterial()
    {
        Debug.Log("===== 開始創建/更新迷霧材質 =====");

        // 查找或創建材質
        string materialPath = "Assets/Materials/MinimapFogMaterial.mat";
        Material fogMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        
        // 查找 Shader
        Shader fogShader = Shader.Find("UI/MinimapFog");
        if (fogShader == null)
        {
            Debug.LogError("❌ 找不到 UI/MinimapFog Shader！請確認 Assets/Shaders/MinimapFog.shader 存在。");
            return;
        }
        
        bool isNewMaterial = false;
        if (fogMaterial == null)
        {
            // 確保目錄存在
            if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            {
                AssetDatabase.CreateFolder("Assets", "Materials");
                Debug.Log("✓ 創建 Assets/Materials 目錄");
            }
            
            // 創建新材質
            fogMaterial = new Material(fogShader);
            AssetDatabase.CreateAsset(fogMaterial, materialPath);
            isNewMaterial = true;
            Debug.Log($"✓ 創建新材質: {materialPath}");
        }
        else
        {
            // 更新現有材質
            fogMaterial.shader = fogShader;
            Debug.Log($"✓ 更新現有材質: {materialPath}");
        }
        
        // 設置材質屬性
        fogMaterial.SetColor("_Color", Color.white);
        
        EditorUtility.SetDirty(fogMaterial);
        AssetDatabase.SaveAssets();
        
        if (isNewMaterial)
        {
            Debug.Log("✓ 材質創建成功！");
        }
        else
        {
            Debug.Log("✓ 材質更新成功！");
        }
        
        // 自動應用到 FogOverlay
        MinimapFogRenderer fogRenderer = FindFirstObjectByType<MinimapFogRenderer>();
        if (fogRenderer != null)
        {
            RawImage rawImage = fogRenderer.GetComponent<RawImage>();
            if (rawImage != null)
            {
                rawImage.material = fogMaterial;
                EditorUtility.SetDirty(rawImage);
                Debug.Log("✓ 已將材質應用到 FogOverlay");
            }
        }
        
        Debug.Log("===== 材質設置完成！ =====");
    }

    private void RevealAllFog()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("請在運行模式下使用此功能！");
            return;
        }

        MinimapFogOfWar fogSystem = MinimapFogOfWar.GetInstance();
        if (fogSystem != null)
        {
            fogSystem.RevealAllFog();
            Debug.Log("✓ 已揭開所有迷霧（測試用）");
        }
        else
        {
            Debug.LogError("找不到 MinimapFogOfWar 系統！");
        }
    }

    private void ResetAllFog()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("請在運行模式下使用此功能！");
            return;
        }

        MinimapFogOfWar fogSystem = MinimapFogOfWar.GetInstance();
        if (fogSystem != null)
        {
            fogSystem.ResetFog();
            Debug.Log("✓ 已重置所有迷霧");
        }
        else
        {
            Debug.LogError("找不到 MinimapFogOfWar 系統！");
        }
    }
}

