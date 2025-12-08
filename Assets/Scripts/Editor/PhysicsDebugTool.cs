using UnityEngine;
using UnityEditor;

/// <summary>
/// 物理碰撞層診斷工具
/// 幫助檢查玩家和 Cable 的碰撞設置
/// </summary>
public class PhysicsDebugTool : EditorWindow
{
    private GameObject playerObj;
    private GameObject cableObj;
    private Vector2 scrollPosition;

    [MenuItem("Window/Physics Debug Tool")]
    public static void ShowWindow()
    {
        GetWindow<PhysicsDebugTool>("Physics Debug");
    }

    private void OnGUI()
    {
        GUILayout.Label("物理碰撞層診斷工具", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.Label("選擇物體：", EditorStyles.label);
        playerObj = (GameObject)EditorGUILayout.ObjectField("Player GameObject:", playerObj, typeof(GameObject), true);
        cableObj = (GameObject)EditorGUILayout.ObjectField("Cable GameObject:", cableObj, typeof(GameObject), true);

        GUILayout.Space(10);

        if (GUILayout.Button("檢查碰撞設置", GUILayout.Height(40)))
        {
            CheckCollisionSetup();
        }

        GUILayout.Space(10);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        if (playerObj != null)
        {
            GUILayout.Label("Player 檢查結果：", EditorStyles.boldLabel);
            CheckGameObject(playerObj, "Player");
            GUILayout.Space(10);
        }

        if (cableObj != null)
        {
            GUILayout.Label("Cable 檢查結果：", EditorStyles.boldLabel);
            CheckGameObject(cableObj, "Cable");
            GUILayout.Space(10);
        }

        GUILayout.EndScrollView();

        // 快速修復按鈕
        GUILayout.Space(10);
        GUILayout.Label("快速修復：", EditorStyles.boldLabel);

        if (GUILayout.Button("設置 Player 的 Rigidbody2D 為 Dynamic", GUILayout.Height(30)))
        {
            FixPlayerRigidbody();
        }

        if (GUILayout.Button("設置 Cable 的 Rigidbody2D 為 Kinematic", GUILayout.Height(30)))
        {
            FixCableRigidbody();
        }

        if (GUILayout.Button("確保兩者在相同的 Layer", GUILayout.Height(30)))
        {
            FixLayers();
        }
    }

    private void CheckGameObject(GameObject obj, string name)
    {
        // 檢查 Collider
        Collider2D collider = obj.GetComponent<Collider2D>();
        if (collider == null)
        {
            GUILayout.Label($"❌ {name} 沒有 Collider2D", EditorStyles.helpBox);
            return;
        }

        GUILayout.Label($"✓ {name} 有 Collider2D ({collider.GetType().Name})");

        if (!collider.enabled)
        {
            GUILayout.Label($"❌ {name} 的 Collider 被禁用", EditorStyles.helpBox);
        }
        else
        {
            GUILayout.Label($"✓ {name} 的 Collider 已啟用");
        }

        GUILayout.Label($"  Is Trigger: {(collider.isTrigger ? "✓ true" : "❌ false")}");

        // 檢查 Rigidbody2D
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            GUILayout.Label($"❌ {name} 沒有 Rigidbody2D", EditorStyles.helpBox);
            GUILayout.Label("  提示：Trigger Collider 需要 Rigidbody2D（Body Type: Dynamic 或 Kinematic）");
        }
        else
        {
            GUILayout.Label($"✓ {name} 有 Rigidbody2D");
            GUILayout.Label($"  Body Type: {rb.bodyType}");
            GUILayout.Label($"  Gravity Scale: {rb.gravityScale}");
            GUILayout.Label($"  Collision Detection: {rb.collisionDetectionMode}");
        }

        // 檢查 Layer
        int layer = obj.layer;
        string layerName = LayerMask.LayerToName(layer);
        GUILayout.Label($"  Layer: {layerName} (ID: {layer})");
    }

    private void CheckCollisionSetup()
    {
        if (playerObj == null || cableObj == null)
        {
            EditorUtility.DisplayDialog("提示", "請選擇 Player 和 Cable", "確定");
            return;
        }

        Collider2D playerCollider = playerObj.GetComponent<Collider2D>();
        Collider2D cableCollider = cableObj.GetComponent<Collider2D>();
        Rigidbody2D playerRb = playerObj.GetComponent<Rigidbody2D>();
        Rigidbody2D cableRb = cableObj.GetComponent<Rigidbody2D>();

        string result = "碰撞檢查結果:\n\n";

        if (playerCollider == null)
            result += "❌ Player 沒有 Collider2D\n";
        else
            result += "✓ Player 有 Collider2D\n";

        if (cableCollider == null)
            result += "❌ Cable 沒有 Collider2D\n";
        else
            result += "✓ Cable 有 Collider2D\n";

        if (playerRb == null)
            result += "❌ Player 沒有 Rigidbody2D (需要!)\n";
        else
            result += $"✓ Player 有 Rigidbody2D ({playerRb.bodyType})\n";

        if (cableRb == null)
            result += "❌ Cable 沒有 Rigidbody2D (需要!)\n";
        else
            result += $"✓ Cable 有 Rigidbody2D ({cableRb.bodyType})\n";

        if (cableCollider != null && !cableCollider.isTrigger)
            result += "⚠️ Cable 的 Collider 不是 Trigger\n";
        else
            result += "✓ Cable 的 Collider 是 Trigger\n";

        EditorUtility.DisplayDialog("檢查結果", result, "確定");
    }

    private void FixPlayerRigidbody()
    {
        if (playerObj == null)
        {
            EditorUtility.DisplayDialog("錯誤", "請選擇 Player", "確定");
            return;
        }

        Rigidbody2D rb = playerObj.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = playerObj.AddComponent<Rigidbody2D>();
        }

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        EditorUtility.DisplayDialog("完成", "已設置 Player Rigidbody2D 為 Dynamic", "確定");
    }

    private void FixCableRigidbody()
    {
        if (cableObj == null)
        {
            EditorUtility.DisplayDialog("錯誤", "請選擇 Cable", "確定");
            return;
        }

        Rigidbody2D rb = cableObj.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = cableObj.AddComponent<Rigidbody2D>();
        }

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        EditorUtility.DisplayDialog("完成", "已設置 Cable Rigidbody2D 為 Kinematic", "確定");
    }

    private void FixLayers()
    {
        if (playerObj == null || cableObj == null)
        {
            EditorUtility.DisplayDialog("錯誤", "請選擇 Player 和 Cable", "確定");
            return;
        }

        playerObj.layer = LayerMask.NameToLayer("Default");
        cableObj.layer = LayerMask.NameToLayer("Default");

        EditorUtility.DisplayDialog("完成", "已將兩者設置到 Default Layer", "確定");
    }
}

