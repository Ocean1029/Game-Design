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

    // private void OnCollisionEnter2D(Collision2D other)
    // {
    //     if (other.gameObject.tag == "key1")
    //     {
    //         Debug.Log("Door is opened!");
    //     }

    // }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        // 1. 嘗試從碰撞物件上取得 player 腳本
        player playerScript = other.gameObject.GetComponent<player>();

        // 2. 檢查是否是 Player 撞到了門
        if (playerScript != null)
        {
            // 3. 詢問 Player 是否攜帶鑰匙
            GameObject key = playerScript.GetCarriedKey();
            
            // 4. 檢查 Player 攜帶的鑰匙是否是我們要的 "key1"
            if (key != null && key.CompareTag("key1"))
            {
                Debug.Log("Door is opened!");

                // 【執行開門動作】
                // 銷毀鑰匙並讓門消失或移動
                playerScript.UseCarriedKey(); // 讓玩家銷毀鑰匙
                Destroy(gameObject); // 銷毀門，達到「開門」效果
                
                // 或者: gameObject.SetActive(false);
            }
        }
    }
}
