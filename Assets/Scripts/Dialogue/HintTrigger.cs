using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class HintTrigger : MonoBehaviour
{
    [Header("提示文字設定")]
    [TextArea(2, 4)]
    public string hintMessage = "這裡可以放提示文字。";

    [Header("能量補滿提示")]
    [TextArea(2, 4)]
    public string energyFullMessage = "Energy is refilled!! I can jump again.";
    private bool hasShownEnergyFull = false;

    [Header("門狀態檢查")]
    [Tooltip("若指定，門開啟後將不顯示提示；留空則自動在半徑內搜尋")]
    public PhysicalDoor linkedDoor;
    public float detectDoorRadius = 2f;

    [Header("石牆狀態檢查")]
    [Tooltip("若指定石牆，石牆升起後不再顯示提示")]
    public StoneWall linkedStoneWall;

    [Tooltip("自動搜尋石牆的名稱（可選）")]
    public string stoneWallObjectName = "StoneWall";

    [Header("地板狀態檢查")]
    [Tooltip("若指定地板物件，當地板被炸掉或失效時不再顯示提示")]
    public GameObject linkedFloor; // 💣 新增欄位
    public string floorObjectName = "Floor_destroy_by_bomb"; // 預設搜尋名稱

    [Header("顯示控制")]
    public bool showOnlyOnce = false;
    public float reenterCooldown = 0.25f;
    private bool isDialogueTyping = false;

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
        // 監聽能量補滿事件
        var playerEnergy = FindObjectOfType<PlayerEnergy>();
        if (playerEnergy != null)
        {
            playerEnergy.OnEnergyRestored += () =>
            {
                if (!hasShownEnergyFull)
                {
                    hasShownEnergyFull = true;
                    DialogueManager.Instance?.ShowDialogue(energyFullMessage);
                    isDialogueTyping = true;
                    DialogueManager.Instance.OnDialogueFinished += HandleDialogueFinished;
                }
            };
        }

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

        // 🔍 自動尋找 StoneWall
        if (linkedStoneWall == null && !string.IsNullOrEmpty(stoneWallObjectName))
        {
            GameObject sw = GameObject.Find(stoneWallObjectName);
            if (sw != null)
            {
                linkedStoneWall = sw.GetComponent<StoneWall>();
                if (linkedStoneWall != null)
                    Debug.Log($"[HintTrigger] 自動連結 StoneWall：{linkedStoneWall.name}");
            }
        }

        // 💣 自動尋找地板物件
        if (linkedFloor == null && !string.IsNullOrEmpty(floorObjectName))
        {
            linkedFloor = GameObject.Find(floorObjectName);
            if (linkedFloor != null)
            {
                Debug.Log($"[HintTrigger] 自動連結地板物件：{linkedFloor.name}");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (Time.time - _lastExitTime < reenterCooldown) return;
        if (showOnlyOnce && _hasShown) return;

        // 🧱 若地板被炸掉，就不顯示
        if (linkedFloor != null)
        {
            if (!linkedFloor.activeInHierarchy)
            {
                Debug.Log($"[HintTrigger] 地板 {linkedFloor.name} 已被炸掉，不顯示提示。");
                return;
            }
        }

        // 🚪 若門已開啟，也不顯示
        if (linkedDoor != null)
        {
            bool opened = linkedDoor.IsOpened();
            if (opened)
            {
                Debug.Log($"[HintTrigger] 門 {linkedDoor.name} 已開啟，不顯示提示。");
                return;
            }
        }

        // 🧱 石牆升起後不顯示
        if (linkedStoneWall != null)
        {
            if (linkedStoneWall.IsRaised())
            {
                Debug.Log($"[HintTrigger] 石牆 {linkedStoneWall.name} 已升起，不顯示提示。");
                return;
            }
        }
   
        if (showOnlyOnce) _hasShown = true;

        var dm = DialogueManager.Instance;
        if (dm != null)
        {
            dm.ShowDialogue(hintMessage);
            isDialogueTyping = true;
            dm.OnDialogueFinished += HandleDialogueFinished;  // ← 當文字全部出現時回呼
        }
    }

    private IEnumerator HideAfterDelay(DialogueManager dm, float delay)
    {
        yield return new WaitForSeconds(delay);
        dm.HideDialogue();
    }

    private void HandleDialogueFinished()
    {
        isDialogueTyping = false;

        var dm = DialogueManager.Instance;
        if (dm != null)
            dm.OnDialogueFinished -= HandleDialogueFinished; // 取消訂閱避免重複觸發
            StartCoroutine(HideAfterDelay(dm, 2f));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        _lastExitTime = Time.time;

        var dm = DialogueManager.Instance;
        if (dm != null && !isDialogueTyping)
        {
            dm.HideDialogue();
        }
    }

    // Gizmo 可視化
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

    public void OnLinkedFloorDestroyed()
    {
        Debug.Log("[HintTrigger] 收到地板被炸掉的通知，關閉對話");
        var dm = DialogueManager.Instance;
        if (dm != null)
        {
            dm.HideDialogue();
        }
    }

}
