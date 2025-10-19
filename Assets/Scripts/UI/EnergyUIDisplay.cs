using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnergyUIDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerEnergy energySystem;

    [Header("UI Setup")]
    [SerializeField] private Transform energyBlockContainer;
    [SerializeField] private GameObject energyBlockPrefab;

    [Header("Visual Settings")]
    [SerializeField] private Vector2 blockSize = new Vector2(30, 30);
    [SerializeField] private float blockSpacing = 3f;

    [Header("Color Settings")]
    [SerializeField] private Color fullColor = Color.white; // 顯示原圖顏色
    [SerializeField] private Color emptyColor = new Color(1f, 1f, 1f, 0.2f); // 變暗

    [Header("Animation Settings")]
    [SerializeField] private float colorLerpDuration = 0.25f;
    [SerializeField] private float scalePulseDuration = 0.15f;
    [SerializeField] private float scaleMultiplier = 1.15f;

    private Image[] energyBlocks;
    private Coroutine[] animCoroutines;

    void Start()
    {
        if (energySystem == null)
            energySystem = FindFirstObjectByType<PlayerEnergy>();

        if (energySystem == null)
        {
            Debug.LogError("EnergyUIDisplay: No PlayerEnergy system found!");
            enabled = false;
            return;
        }

        energySystem.OnEnergyChanged += UpdateEnergyDisplay;
        CreateEnergyBlocks();
        UpdateEnergyDisplay(energySystem.GetCurrentEnergy(), energySystem.GetMaxEnergy());
    }

    void OnDestroy()
    {
        if (energySystem != null)
            energySystem.OnEnergyChanged -= UpdateEnergyDisplay;
    }

    private void CreateEnergyBlocks()
    {
        int maxEnergy = energySystem.GetMaxEnergy();
        energyBlocks = new Image[maxEnergy];
        animCoroutines = new Coroutine[maxEnergy];

        for (int i = 0; i < maxEnergy; i++)
        {
            GameObject block = Instantiate(energyBlockPrefab, energyBlockContainer);
            RectTransform rt = block.GetComponent<RectTransform>();
            rt.sizeDelta = blockSize;
            energyBlocks[i] = block.GetComponent<Image>();
        }

        HorizontalLayoutGroup layout = energyBlockContainer.GetComponent<HorizontalLayoutGroup>();
        if (layout == null)
            layout = energyBlockContainer.gameObject.AddComponent<HorizontalLayoutGroup>();

        layout.spacing = blockSpacing;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
    }

    private void UpdateEnergyDisplay(int current, int max)
    {
        for (int i = 0; i < energyBlocks.Length; i++)
        {
            bool shouldBeFull = i < current;
            Image img = energyBlocks[i];
            if (img == null) continue;

            Color targetColor = shouldBeFull ? fullColor : emptyColor;

            // 停掉前一個動畫（避免重疊）
            if (animCoroutines[i] != null)
                StopCoroutine(animCoroutines[i]);

            // 啟動顏色漸變動畫
            animCoroutines[i] = StartCoroutine(AnimateBlock(img, targetColor, shouldBeFull));
        }
    }

    private IEnumerator AnimateBlock(Image img, Color targetColor, bool gainedEnergy)
    {
        // 顏色漸變
        Color startColor = img.color;
        float t = 0f;

        // 放大縮小動畫
        RectTransform rt = img.rectTransform;
        Vector3 startScale = rt.localScale;
        Vector3 targetScale = gainedEnergy ? Vector3.one * scaleMultiplier : Vector3.one * 0.9f;

        // 第一段縮放（快速）
        float pulseTime = 0f;
        while (pulseTime < scalePulseDuration)
        {
            pulseTime += Time.deltaTime;
            float p = pulseTime / scalePulseDuration;
            rt.localScale = Vector3.Lerp(Vector3.one, targetScale, p);
            yield return null;
        }

        // 顏色漸變
        while (t < colorLerpDuration)
        {
            t += Time.deltaTime;
            float lerp = t / colorLerpDuration;
            img.color = Color.Lerp(startColor, targetColor, lerp);
            yield return null;
        }

        // 回復正常大小
        pulseTime = 0f;
        while (pulseTime < scalePulseDuration)
        {
            pulseTime += Time.deltaTime;
            float p = pulseTime / scalePulseDuration;
            rt.localScale = Vector3.Lerp(targetScale, Vector3.one, p);
            yield return null;
        }

        img.color = targetColor;
        rt.localScale = Vector3.one;
    }
}