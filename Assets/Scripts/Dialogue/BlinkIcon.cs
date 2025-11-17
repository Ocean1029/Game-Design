using UnityEngine;
using UnityEngine.UI;

public class BlinkIcon : MonoBehaviour
{
    [Header("閃爍速度 (次/秒)")]
    public float speed = 0.5f; // 每秒大約 3 次閃爍

    [Header("閃爍強度 (0~1)")]
    [Range(0f, 1f)]
    public float intensity = 0.1f; // 透明度波動幅度（0.05 = ±5%）

    private Image img;
    private Color baseColor;
    private float baseAlpha;

    void Start()
    {
        img = GetComponent<Image>();
        baseColor = img.color;
        baseAlpha = img.color.a;
    }

    void Update()
    {
        // sin 波範圍 -1~1，轉成 0~1 再偏移中心值
        float offset = Mathf.Sin(Time.time * speed * Mathf.PI) * intensity;
        float alpha = baseAlpha + offset;

        // 限制在 0~1 範圍內
        alpha = Mathf.Clamp01(alpha);

        img.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
    }
}
