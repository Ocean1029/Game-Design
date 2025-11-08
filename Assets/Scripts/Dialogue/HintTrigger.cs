using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HintTrigger : MonoBehaviour
{
    [Header("提示文字設定")]
    [TextArea(2, 4)]
    public string hintMessage = "這裡可以放提示文字。";

    [Header("門狀態檢查")]
    [Tooltip("若指定，門開啟後將不顯示提示；留空則自動在半徑內搜尋")]
    public PhysicalDoor linkedDoor;  // <-- 改成 PhysicalDoor
    public float detectDoorRadius = 2f;

    [Header("顯示控制")]
    public bool showOnlyOnce = false;
    public float reenterCooldown = 0.25f;

    private bool _hasShown;
    private float _lastExitTime;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
        gameObject.name = "HintTrigger";
    }

    private void Start()
    {
        // 嘗試自動尋找最近的 PhysicalDoor
        if (linkedDoor == null && detectDoorRadius > 0f)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectDoorRadius);
            float bestDist = float.MaxValue;
            PhysicalDoor best = null;

            foreach (var h in hits)
            {
                var d = h.GetComponent<PhysicalDoor>();
                if (d == null) continue;

                float dist = Vector2.SqrMagnitude((Vector2)h.transform.position - (Vector2)transform.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = d;
                }
            }

            if (best != null)
            {
                linkedDoor = best;
                Debug.Log($"[HintTrigger] 自動連結到 PhysicalDoor：{linkedDoor.name}");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (Time.time - _lastExitTime < reenterCooldown) return;
        if (showOnlyOnce && _hasShown) return;

        // 🔍 這裡是重點：檢查門是否開啟
        if (linkedDoor != null)
        {
            bool opened = linkedDoor.IsOpened(); // <-- 注意要加括號
            Debug.Log($"[HintTrigger] 檢查門狀態：{linkedDoor.name} IsOpened={opened}");

            if (opened)
            {
                // 門已開啟，不顯示提示
                return;
            }
        }

        _hasShown = true;

        var dm = DialogueManager.Instance;
        if (dm != null)
        {
            dm.ShowDialogue(hintMessage);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        _lastExitTime = Time.time;

        var dm = DialogueManager.Instance;
        if (dm != null)
        {
            dm.HideDialogue();
        }
    }

    // 顯示範圍輔助線
    private void OnDrawGizmos()
    {
        var col = GetComponent<Collider2D>();
        if (col == null) return;

        Gizmos.color = new Color(1f, 0.85f, 0.2f, 0.25f);
        if (col is BoxCollider2D b)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(b.offset, b.size);
            Gizmos.color = new Color(1f, 0.6f, 0f, 0.8f);
            Gizmos.DrawWireCube(b.offset, b.size);
        }
        else if (col is CircleCollider2D c)
        {
            Gizmos.DrawSphere(transform.position + (Vector3)c.offset, c.radius);
        }
        else if (col is CapsuleCollider2D cp)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(cp.offset, cp.size);
            Gizmos.color = new Color(1f, 0.6f, 0f, 0.8f);
            Gizmos.DrawWireCube(cp.offset, cp.size);
        }

        if (linkedDoor == null && detectDoorRadius > 0f)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.35f);
            Gizmos.DrawWireSphere(transform.position, detectDoorRadius);
        }
    }
}
