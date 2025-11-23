using System.Collections;
using UnityEngine;

public class StoneButton : MonoBehaviour
{
    [Header("目標石牆")]
    [Tooltip("要控制的 StoneWall（牆會升起，HintTrigger 用它判斷是否顯示提示）")]
    public StoneWall stoneWall;

    [Header("攝影機設定")]
    [Tooltip("主攝影機，不填會自動抓 Camera.main")]
    public Camera mainCamera;

    [Tooltip("鏡頭移動花費的時間（秒）")]
    public float cameraMoveDuration = 1f;

    private bool isActivated = false; // 避免重複觸發

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 檢查是否玩家
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        if (isActivated) return; // 避免重複啟動
        isActivated = true;

        Debug.Log("StoneButton: 玩家踩到按鈕，開始鏡頭移動至石牆");

        if (mainCamera == null)
        {
            Debug.LogError("StoneButton: mainCamera 未設定且找不到 Camera.main！");
            return;
        }

        if (stoneWall == null)
        {
            Debug.LogError("StoneButton: stoneWall 未指定！");
            return;
        }

        // 開始鏡頭移動流程
        StartCoroutine(MoveCameraToStoneWall());
    }

    private IEnumerator MoveCameraToStoneWall()
    {
        Vector3 startPos = mainCamera.transform.position;
        Vector3 targetPos = stoneWall.transform.position;

        // 鎖住 Z 軸不變（避免鏡頭跑到場景裡）
        targetPos.z = startPos.z;

        float elapsed = 0f;

        while (elapsed < cameraMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / cameraMoveDuration);

            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }

        // 最後對齊一次
        mainCamera.transform.position = targetPos;

        Debug.Log("StoneButton: 鏡頭已移動到石牆位置");

        // === 讓石牆變成「已升起」狀態（HintTrigger 才會停止顯示） ===
        stoneWall.RaiseWall();

        // === 無動畫版：直接把石牆往上移動兩格 ===
        stoneWall.transform.position += new Vector3(0, 2f, 0);

        Debug.Log("StoneWall: 已向上移動 2 格（無動畫版）");

        // 如果之後你要鏡頭回玩家，我可以幫你加在這裡
    }
}
