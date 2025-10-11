using UnityEngine;
using System.Collections;

public class player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    private SpriteRenderer spriteRenderer;

    // === 拿鑰匙 ===
    private GameObject carriedKey = null;
    public Transform holdpoint;

    // === 垂降 ===
    private bool isRappelling = false;
    private float rappellingDuration = 1.0f;

    // === 坐椅子 ===
    private chair nearbyChair = null;
    private bool isSitting = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("Player 物件上找不到 SpriteRenderer 組件！");
        }
    }

    void Update()
    {
        if (isRappelling == false)
        {
            if (!isSitting)
            {
                // 左右移動
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    transform.Translate(moveSpeed * Time.deltaTime, 0, 0);
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    transform.Translate(-moveSpeed * Time.deltaTime, 0, 0);
                }
            }

            // 坐椅子互動
            if (nearbyChair != null && Input.GetKeyDown(KeyCode.U))
            {
                SitOnChair();
            }
            if (isSitting && Input.GetKeyDown(KeyCode.D))
            {
                LeaveChair();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 撿鑰匙
        if (collision.CompareTag("key1") && carriedKey == null)
        {
            carriedKey = collision.gameObject;

            if (holdpoint != null)
            {
                carriedKey.transform.SetParent(holdpoint);
                carriedKey.transform.localPosition = Vector3.zero;
            }
            else
            {
                Debug.LogError("HoldPoint is not assigned in the Inspector!");
            }
        }

        // 椅子互動
        chair chair = collision.GetComponent<chair>();
        if (chair != null)
        {
            nearbyChair = chair;
            chair.ShowPromptA(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        chair chair = collision.GetComponent<chair>();
        if (chair != null && chair == nearbyChair)
        {
            if (!isSitting)
            {
                chair.ShowPromptA(false);
                chair.ShowPromptD(false);
                nearbyChair = null;
            }
        }
    }

    // === 坐下功能 ===
    void SitOnChair()
    {
        if (nearbyChair == null) return;

        transform.position = nearbyChair.sitpoint.position;
        isSitting = true;
        Debug.Log("Player 坐下了！");

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero;
        }
        
        nearbyChair.ShowPromptA(false);
        nearbyChair.ShowPromptD(true);
    }

    // === 站起來功能 ===
    void LeaveChair()
    {
        if (!isSitting) return;

        transform.position += new Vector3(0f, 0.5f, 0f);
        isSitting = false;
        Debug.Log("Player 站起來了！");

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 1;
        }

        if (nearbyChair != null)
        {
            nearbyChair.ShowPromptD(false);
            nearbyChair.ShowPromptA(true);
        }
    }

    // 用於讓門檢查
    public GameObject GetCarriedKey()
    {
        return carriedKey;
    }

    // 如果門成功打開，門呼叫此方法銷毀或釋放鑰匙
    public void UseCarriedKey()
    {
        if (carriedKey != null)
        {
            Destroy(carriedKey);
            carriedKey = null;
        }
    }

    // === 垂降功能 ===
    public void StartRappelling(Transform target)
    {
        if (isRappelling) return; // 避免重複啟動

        isRappelling = true;

        // 隱藏 Player
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // 啟動協程來處理時間延遲和位置傳送
        StartCoroutine(RappelSequence(target));
    }

    // 垂降協程
    IEnumerator RappelSequence(Transform target)
    {
        // 動畫處理，duration須與動畫長度一致
        yield return new WaitForSeconds(rappellingDuration);

        // 位置傳送
        transform.position = target.position;

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true; // <--- 確保這行程式碼有寫
        }

        // 結束垂降狀態，允許玩家重新輸入移動
        isRappelling = false;
    }
}