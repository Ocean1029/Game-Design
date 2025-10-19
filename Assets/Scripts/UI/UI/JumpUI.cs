using UnityEngine;
using TMPro;

public class JumpUI : MonoBehaviour
{
    [Header("UI 元件")]
    public TMP_Text jumpText;

    [Header("最大跳躍次數")]
    public int maxJumps = 3;

    private int remainingJumps;

    void Start()
    {
        remainingJumps = maxJumps;
        UpdateJumpCount(remainingJumps);
    }

    public void UpdateJumpCount(int remaining)
    {
        remainingJumps = Mathf.Clamp(remaining, 0, maxJumps);
        jumpText.text = $"Jump: {remainingJumps}/{maxJumps}";
    }
}