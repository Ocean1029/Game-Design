using UnityEngine;
using System.Collections;

public class player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    // 是否拿起鑰匙
    private GameObject carriedKey = null;
    // 拿鑰匙的位置的位置
    public Transform holdpoint;
    // 使否使用cable垂降中
    private bool isRappelling = false;
    // 垂降動畫持續時間 (根據動畫長度調整)
    private float rappellingDuration = 1.0f;
    private SpriteRenderer spriteRenderer;

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
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(moveSpeed * Time.deltaTime, 0, 0);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(-moveSpeed * Time.deltaTime, 0, 0);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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