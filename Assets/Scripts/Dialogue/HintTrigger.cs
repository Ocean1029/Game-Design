using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HintTrigger : MonoBehaviour
{
    [TextArea(2, 4)]
    public string hintMessage = "這裡可以放提示文字。";
    
    [Header("選配")]
    public bool showOnlyOnce = false;      // 只顯示一次
    public float reenterCooldown = 0.25f;  // 退出後再進入的冷卻，避免閃爍

    private bool _hasShown;
    private float _lastExitTime;

    private void Reset()
    {
        // 自動把 Collider2D 設成 Trigger，避免忘記
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
        gameObject.name = "HintTrigger";
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // 冷卻保護
        if (Time.time - _lastExitTime < reenterCooldown) return;

        if (showOnlyOnce && _hasShown) return;

        _hasShown = true;
        DialogueManager.Instance?.ShowDialogue(hintMessage);
        Debug.Log($"[HintTrigger] Player 進入：{name}");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        _lastExitTime = Time.time;
        DialogueManager.Instance?.HideDialogue();
        Debug.Log($"[HintTrigger] Player 離開：{name}");
    }

    // 在 Scene 視窗畫出可視化範圍（不會進入遊戲）
    private void OnDrawGizmos()
    {
        var col = GetComponent<Collider2D>();
        if (col == null) return;

        Gizmos.color = new Color(1f, 0.85f, 0.2f, 0.25f); // 淡黃
        var box = col as BoxCollider2D;
        var circle = col as CircleCollider2D;
        var capsule = col as CapsuleCollider2D;

        if (box != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.offset, box.size);
            Gizmos.color = new Color(1f, 0.6f, 0f, 0.8f);
            Gizmos.DrawWireCube(box.offset, box.size);
        }
        else if (circle != null)
        {
            Gizmos.DrawSphere(transform.position + (Vector3)circle.offset, circle.radius);
        }
        else if (capsule != null)
        {
            // 簡化：用盒子近似
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(capsule.offset, capsule.size);
            Gizmos.color = new Color(1f, 0.6f, 0f, 0.8f);
            Gizmos.DrawWireCube(capsule.offset, capsule.size);
        }
    }
}
