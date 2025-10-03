using UnityEngine;

public class cable : MonoBehaviour
{
    // 在 Inspector 設定，這是玩家垂降動畫結束後，應該出現的位置
    public Transform targetFloorPoint;
    private Animator cableAnim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 確保 Cable 物件上有 Animator 組件
        cableAnim = GetComponent<Animator>();
        if (cableAnim == null)
        {
            Debug.LogError("Cable 物件上找不到 Animator 組件！");
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Cable 被觸發了！");
        
        // 檢查是否是玩家進入了觸發區域
        player playerScript = other.GetComponent<player>();

        // 檢查 playerScript 是否成功取得， 如果不是 Player 碰到的，則立即退出，不執行後續邏輯
        if (playerScript == null) 
        {
            return;
        }

        if (targetFloorPoint != null && cableAnim != null)
        {
            cableAnim.SetTrigger("player_enter");

            playerScript.StartRappelling(targetFloorPoint);

            //禁用此觸發器，避免玩家在垂降過程中再次觸發
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = false;
            }
        }
        else
        {
            Debug.LogError("Cable 設定錯誤：TargetFloorPoint 或 Animator 缺失！");
        }
    }
}
