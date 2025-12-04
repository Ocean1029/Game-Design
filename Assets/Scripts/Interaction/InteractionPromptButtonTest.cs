using UnityEngine;

/// <summary>
/// 測試組件：強制顯示 InteractionPromptButton 效果
/// 添加此組件到有 InteractionPromptButton 的物件上來測試是否能正常顯示
/// </summary>
public class InteractionPromptButtonTest : MonoBehaviour
{
    private InteractionPromptButton promptButton;

    void Start()
    {
        promptButton = GetComponent<InteractionPromptButton>();
        if (promptButton != null)
        {
            Debug.Log("InteractionPromptButtonTest: 強制顯示提示按鍵");
            promptButton.ShowPrompt(true);
        }
        else
        {
            Debug.LogError("InteractionPromptButtonTest: 找不到 InteractionPromptButton 組件！");
        }
    }

    void Update()
    {
        // 按 T 鍵切換提示顯示
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (promptButton != null)
            {
                bool isVisible = promptButton.IsPlayerNearby();
                promptButton.ShowPrompt(!isVisible);
                Debug.Log($"InteractionPromptButtonTest: 提示按鍵顯示狀態切換為 {!isVisible}");
            }
        }
    }
}
