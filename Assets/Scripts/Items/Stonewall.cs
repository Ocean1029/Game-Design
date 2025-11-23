using UnityEngine;

public class StoneWall : MonoBehaviour
{
    [Header("Stone Wall Settings")]
    public Collider2D wallCollider;

    private bool isRaised = false;

    private void Awake()
    {
        if (wallCollider == null)
            wallCollider = GetComponent<Collider2D>();
    }

    /// <summary>
    /// 被 StoneButton 呼叫，讓石牆進入「升起」狀態
    /// </summary>
    public void RaiseWall()
    {
        if (isRaised) return;

        isRaised = true;
        Debug.Log("StoneWall: RaiseWall 被呼叫");

        // 無動畫版，直接關閉 collider
        if (wallCollider != null)
            wallCollider.enabled = false;
    }

    /// <summary>
    /// 給 HintTrigger 使用：石牆是否被升起？
    /// </summary>
    public bool IsRaised()
    {
        return isRaised;
    }
}
