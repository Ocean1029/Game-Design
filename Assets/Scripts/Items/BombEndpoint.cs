using UnityEngine;

/// <summary>
/// 炸彈終點 - 當玩家觸碰到此物件時，顯示成功破關訊息
/// </summary>
public class BombEndpoint : MonoBehaviour
{
    [Header("Message Settings")]
    [Tooltip("顯示的成功訊息")]
    [SerializeField] private string successMessage = "成功破關第一階段 prototype";
    
    [Tooltip("訊息顏色")]
    [SerializeField] private Color messageColor = new Color(0f, 1f, 0f); // 綠色
    
    [Tooltip("字體大小")]
    [SerializeField] private float fontSize = 36f;
    
    [Header("Audio & Visual")]
    [Tooltip("觸發時的音效")]
    [SerializeField] private AudioClip triggerSound;
    
    [Tooltip("觸發時的視覺效果")]
    [SerializeField] private GameObject triggerEffect;

    private bool isTriggered = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggered) return;

        // 檢查是否為玩家
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player == null) return;

        // 觸發成功訊息
        TriggerSuccess(player.transform.position);
    }

    /// <summary>
    /// 觸發成功破關訊息
    /// </summary>
    private void TriggerSuccess(Vector3 playerPosition)
    {
        if (isTriggered) return;
        isTriggered = true;

        // 顯示浮現文字
        FloatingTextManager floatingTextManager = FloatingTextManager.GetInstance();
        if (floatingTextManager != null)
        {
            floatingTextManager.ShowFloatingText(
                successMessage, 
                transform.position, 
                messageColor, 
                fontSize
            );
        }
        else
        {
            Debug.LogWarning($"BombEndpoint: FloatingTextManager not found! Message: {successMessage}");
        }

        // 播放音效
        if (triggerSound != null)
        {
            AudioSource.PlayClipAtPoint(triggerSound, transform.position);
        }

        // 生成視覺效果
        if (triggerEffect != null)
        {
            Instantiate(triggerEffect, transform.position, Quaternion.identity);
        }

        Debug.Log($"BombEndpoint: Success message triggered - {successMessage}");
    }

    /// <summary>
    /// 設置成功訊息
    /// </summary>
    public void SetSuccessMessage(string message)
    {
        successMessage = message;
    }

    /// <summary>
    /// 設置訊息顏色
    /// </summary>
    public void SetMessageColor(Color color)
    {
        messageColor = color;
    }
}
