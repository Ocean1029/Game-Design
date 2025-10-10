using UnityEngine;

public class door : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // 在 Inspector 面板中拖曳進來的「門打開」圖片
    public Sprite doorOpenedSprite;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 嘗試從碰撞物件上取得 player 腳本
        player playerScript = other.gameObject.GetComponent<player>();

        // 檢查是否是 Player 撞到了門
        if (playerScript != null)
        {
            // 檢查 Player 是否攜帶鑰匙
            GameObject key = playerScript.GetCarriedKey();
            
            // 只有在 key 不為 null 且 Tag 正確時才執行開門邏輯
            if (key != null && key.CompareTag("key1"))
            {
                Debug.Log("Door is opened!");

                // 獲取 Door 物件的 Sprite Renderer 組件
                SpriteRenderer sr = GetComponent<SpriteRenderer>();

                if (sr != null && doorOpenedSprite != null)
                {
                    // 換成「門打開」的圖片
                    sr.sprite = doorOpenedSprite;
                }
                else
                {
                    Debug.LogError("Door 缺少 SpriteRenderer 組件或 doorOpenedSprite 尚未設定！");
                }

                // 讓鑰匙消失 
                playerScript.UseCarriedKey(); 
                
                // 禁用碰撞器，會連偵測都停止
                Collider2D col = GetComponent<Collider2D>();
                if (col != null)
                {
                    col.enabled = false;
                }
            }
        }
    }
}
