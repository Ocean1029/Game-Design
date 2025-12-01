using UnityEngine;

/// <summary>
/// 視差背景層腳本（每幀位移 + 第一幀不移動版本）
/// </summary>
public class ParallaxLayer : MonoBehaviour
{
    [Header("視差設定")]
    [Tooltip("視差移動倍率 (0-1)。數值越小，背景看起來越遠")]
    [Range(0f, 1f)]
    public float parallaxMultiplier = 0.1f; // 遠景建議 0.02~0.08，中景 0.1~0.25

    [Header("限制設定")]
    [Tooltip("啟用位置限制以防止背景移動過度露出空白區域")]
    public bool enableClamp = false;

    [Tooltip("背景可移動的最小偏移量（相對於起始位置）")]
    public Vector2 minOffset = new Vector2(-5f, -5f);

    [Tooltip("背景可移動的最大偏移量（相對於起始位置）")]
    public Vector2 maxOffset = new Vector2(5f, 5f);

    [Header("相機參考")]
    [Tooltip("目標相機（留空則自動使用主相機）")]
    public Camera targetCamera;

    [Header("調試設定")]
    [Tooltip("啟用調試訊息（會在 Console 顯示移動資訊）")]
    public bool enableDebug = false;

    [Tooltip("等待幾幀後才開始視差計算（讓相機 follow 先完成）")]
    public int waitFrames = 3;

    // ===== 私有變數 =====
    private Vector3 startPosition;           // 背景的起始位置（你在 Scene 放的那個位置）
    private Vector3 previousCameraPosition;  // 上一幀相機位置
    private bool isInitialized = false;      // 是否已完成第一次對齊（第一幀不移動）
    private int frameCounter = 0;            // 幀計數器

    private void Awake()
    {
        // 一開始就記住「我在場景裡被擺在哪」
        startPosition = transform.position;
    }

    void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null)
            {
                Debug.LogError("ParallaxLayer: 找不到主相機！請確保場景有 MainCamera。");
                enabled = false;
                return;
            }
        }
    }

    void LateUpdate()
    {
        if (targetCamera == null) return;

        Vector3 currentCamPos = targetCamera.transform.position;

        // ✅ 等待幾幀，讓相機的 follow 腳本先完成
        if (!isInitialized)
        {
            frameCounter++;

            if (enableDebug)
            {
                Debug.Log($"[{gameObject.name}] 等待中... 第 {frameCounter}/{waitFrames} 幀，相機位置 = {currentCamPos}");
            }

            // 還沒等夠幀數，保持原位不動
            if (frameCounter < waitFrames)
            {
                return;
            }

            // 等夠了！現在才記錄相機的「真正起始位置」
            previousCameraPosition = currentCamPos;
            isInitialized = true;

            if (enableDebug)
            {
                Debug.Log($"[{gameObject.name}] ✓ 初始化完成！背景起始位置 = {startPosition}, 相機起始位置 = {currentCamPos}");
            }

            // 這一幀也不移動，從下一幀開始才做視差
            return;
        }

        // 之後每一幀才開始算「這一幀相機動了多少」
        Vector3 delta = currentCamPos - previousCameraPosition;

        // 只吃一小部分的位移（視差）
        Vector3 move = new Vector3(
            delta.x * parallaxMultiplier,
            delta.y * parallaxMultiplier,
            0f
        );

        // 嘗試新位置：在目前位置上加上這一幀的位移
        Vector3 tentativePos = transform.position + move;

        if (enableClamp)
        {
            // 算出「相對起始位置」偏移量
            Vector3 offsetFromStart = tentativePos - startPosition;

            // Clamp 偏移量，確保只在你設定的範圍內飄
            offsetFromStart.x = Mathf.Clamp(offsetFromStart.x, minOffset.x, maxOffset.x);
            offsetFromStart.y = Mathf.Clamp(offsetFromStart.y, minOffset.y, maxOffset.y);

            tentativePos = startPosition + offsetFromStart;
        }

        // 最終位置：X/Y 按 tentative，Z 固定不動
        transform.position = new Vector3(
            tentativePos.x,
            tentativePos.y,
            startPosition.z
        );

        if (enableDebug && delta.sqrMagnitude > 0f)
        {
            Debug.Log($"[{gameObject.name}] delta={delta}, move={move}, pos={transform.position}");
        }

        // 更新上一幀相機位置
        previousCameraPosition = currentCamPos;
    }

    private void OnDrawGizmosSelected()
    {
        if (!enableClamp) return;

        Vector3 center = Application.isPlaying ? startPosition : transform.position;

        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);

        Vector3 minCorner = center + new Vector3(minOffset.x, minOffset.y, 0f);
        Vector3 maxCorner = center + new Vector3(maxOffset.x, maxOffset.y, 0f);

        Vector3 topLeft = new Vector3(minCorner.x, maxCorner.y, center.z);
        Vector3 topRight = new Vector3(maxCorner.x, maxCorner.y, center.z);
        Vector3 bottomLeft = new Vector3(minCorner.x, minCorner.y, center.z);
        Vector3 bottomRight = new Vector3(maxCorner.x, minCorner.y, center.z);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center, 0.2f);
    }
}