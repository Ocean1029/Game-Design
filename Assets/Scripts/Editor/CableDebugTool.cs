using UnityEngine;
using UnityEditor;

/// <summary>
/// Cable 系統診斷工具
/// 幫助檢查 Cable 的設置是否正確
/// </summary>
public class CableDebugTool : EditorWindow
{
    private cable targetCable;
    private Vector2 scrollPosition;

    [MenuItem("Window/Cable Debug Tool")]
    public static void ShowWindow()
    {
        GetWindow<CableDebugTool>("Cable Debug");
    }

    private void OnGUI()
    {
        GUILayout.Label("Cable 系統診斷工具", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.Label("選擇 Cable GameObject：", EditorStyles.label);
        targetCable = (cable)EditorGUILayout.ObjectField(targetCable, typeof(cable), true);

        if (targetCable == null)
        {
            GUILayout.Label("請在 Hierarchy 中選擇一個有 cable 組件的 GameObject", EditorStyles.helpBox);
            return;
        }

        GUILayout.Space(10);
        GUILayout.Label("診斷結果：", EditorStyles.boldLabel);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        // 檢查 1: Collider
        CheckCollider();

        GUILayout.Space(5);

        // 檢查 2: IInteractable
        CheckIInteractable();

        GUILayout.Space(5);

        // 檢查 3: 其他組件
        CheckOtherComponents();

        GUILayout.EndScrollView();

        GUILayout.Space(10);

        // 快速修復按鈕
        if (GUILayout.Button("快速修復：設置 Collider 為 Trigger", GUILayout.Height(30)))
        {
            FixColliderTrigger();
        }

        if (GUILayout.Button("快速修復：確保 Cable 有 IInteractable", GUILayout.Height(30)))
        {
            FixIInteractable();
        }

        if (GUILayout.Button("在 Play Mode 顯示調試信息", GUILayout.Height(30)))
        {
            EnableCableDebugInfo();
        }
    }

    private void CheckCollider()
    {
        GUILayout.Label("✓ Collider 檢查", EditorStyles.boldLabel);

        Collider2D collider = targetCable.GetComponent<Collider2D>();
        if (collider == null)
        {
            GUILayout.Label("❌ 沒有 Collider2D 組件！", EditorStyles.helpBox);
            return;
        }

        GUILayout.Label($"✓ Collider 類型: {collider.GetType().Name}");

        // 檢查 Is Trigger
        if (collider.isTrigger)
        {
            GUILayout.Label("✓ Collider.isTrigger = true");
        }
        else
        {
            GUILayout.Label("⚠️ Collider.isTrigger = false", EditorStyles.helpBox);
            GUILayout.Label("問題：Cable 壞掉時 isTrigger 應為 false（solid），但在玩家進入時需要觸發 OnTriggerEnter2D");
            GUILayout.Label("解決：Cable 需要有一個 Trigger Collider 用於檢測玩家進入");
        }

        // 檢查碰撞體大小
        if (collider.bounds.size.magnitude < 0.1f)
        {
            GUILayout.Label("⚠️ Collider 太小了！", EditorStyles.helpBox);
        }
        else
        {
            GUILayout.Label($"✓ Collider 大小: {collider.bounds.size}");
        }

        // 檢查 enabled
        if (!collider.enabled)
        {
            GUILayout.Label("❌ Collider.enabled = false", EditorStyles.helpBox);
        }
        else
        {
            GUILayout.Label("✓ Collider.enabled = true");
        }
    }

    private void CheckIInteractable()
    {
        GUILayout.Label("✓ IInteractable 檢查", EditorStyles.boldLabel);

        if (targetCable is IInteractable)
        {
            GUILayout.Label("✓ Cable 實現了 IInteractable 介面");
        }
        else
        {
            GUILayout.Label("❌ Cable 沒有實現 IInteractable 介面！", EditorStyles.helpBox);
        }

        // 檢查所需的字段
        var hasRequiredFields = CheckPrivateFields();
        if (!hasRequiredFields)
        {
            GUILayout.Label("⚠️ 缺少一些必要的字段設置", EditorStyles.helpBox);
        }
    }

    private void CheckOtherComponents()
    {
        GUILayout.Label("✓ 其他組件檢查", EditorStyles.boldLabel);

        // Animator
        Animator animator = targetCable.GetComponent<Animator>();
        if (animator != null)
        {
            GUILayout.Label("✓ 有 Animator 組件");
        }
        else
        {
            GUILayout.Label("⚠️ 沒有 Animator 組件（可選但推薦）");
        }

        // Transform
        if (targetCable.transform.parent != null)
        {
            GUILayout.Label($"✓ 父物件: {targetCable.transform.parent.name}");
        }

        GUILayout.Label($"✓ 位置: {targetCable.transform.position}");
    }

    private bool CheckPrivateFields()
    {
        // 這個方法通過反射檢查一些私有字段
        var fieldInfo = targetCable.GetType().GetField("startBroken", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return fieldInfo != null;
    }

    private void FixColliderTrigger()
    {
        Collider2D collider = targetCable.GetComponent<Collider2D>();
        if (collider == null)
        {
            EditorUtility.DisplayDialog("錯誤", "Cable 沒有 Collider2D 組件！", "確定");
            return;
        }

        collider.isTrigger = true;
        EditorUtility.DisplayDialog("完成", "已設置 Collider.isTrigger = true\n注意：如果 cable 預設壞掉，需要手動改回 false", "確定");
    }

    private void FixIInteractable()
    {
        if (!(targetCable is IInteractable))
        {
            EditorUtility.DisplayDialog("錯誤", "Cable 類別定義有問題，無法修復\n請檢查 cable.cs 是否正確實現 IInteractable", "確定");
            return;
        }

        EditorUtility.DisplayDialog("檢查", "Cable 已正確實現 IInteractable", "確定");
    }

    private void EnableCableDebugInfo()
    {
        // 通過反射設置調試信息
        var fieldInfo = targetCable.GetType().GetField("showDebugInfo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (fieldInfo != null)
        {
            fieldInfo.SetValue(targetCable, true);
            EditorUtility.DisplayDialog("完成", "已啟用 Cable 調試信息\n在 Play Mode 中查看 Console", "確定");
        }
        else
        {
            EditorUtility.DisplayDialog("注意", "無法自動啟用調試信息，請在 Inspector 中手動設置", "確定");
        }
    }
}

