using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class InventoryUI : MonoBehaviour
{
    [Serializable]
    public struct ItemIcon
    {
        public ItemType type;
        public Image image;
    }

    [Header("Icon Mapping")]
    public ItemIcon[] icons;

    [Header("Colors")]
    public Color collectedColor = Color.white;
    public Color uncollectedColor = new Color(1f, 1f, 1f, 0.3f);

    [Header("Animation Settings")]
    [Tooltip("放大倍率")]
    public float popScale = 1.2f;
    [Tooltip("動畫時間（秒）")]
    public float popDuration = 0.15f;
    [Tooltip("顏色漸變時間")]
    public float colorLerpDuration = 0.25f;

    private Coroutine[] activeAnimations;

    void Awake()
    {
        activeAnimations = new Coroutine[icons.Length];
        foreach (var ic in icons)
            SetItemCollected(ic.type, false);
    }

    public void SetItemCollected(ItemType type, bool collected)
    {
        for (int i = 0; i < icons.Length; i++)
        {
            var ic = icons[i];
            if (ic.type == type && ic.image != null)
            {
                if (activeAnimations[i] != null)
                    StopCoroutine(activeAnimations[i]);

                activeAnimations[i] = StartCoroutine(AnimateIcon(ic.image, collected));
                return;
            }
        }

        Debug.LogWarning($"[InventoryUI] No icon bound for {type}");
    }

    private IEnumerator AnimateIcon(Image image, bool collected)
    {
        Color startColor = image.color;
        Color targetColor = collected ? collectedColor : uncollectedColor;

        Vector3 originalScale = image.rectTransform.localScale;
        Vector3 targetScale = collected ? Vector3.one * popScale : Vector3.one * 0.9f;

        // --- 第一段：放大/縮小 ---
        float t = 0;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            float p = t / popDuration;
            image.rectTransform.localScale = Vector3.Lerp(Vector3.one, targetScale, p);
            yield return null;
        }

        // --- 第二段：顏色漸變 ---
        float c = 0;
        while (c < colorLerpDuration)
        {
            c += Time.deltaTime;
            float p = c / colorLerpDuration;
            image.color = Color.Lerp(startColor, targetColor, p);
            yield return null;
        }

        // --- 第三段：恢復原大小 ---
        t = 0;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            float p = t / popDuration;
            image.rectTransform.localScale = Vector3.Lerp(targetScale, Vector3.one, p);
            yield return null;
        }

        image.rectTransform.localScale = Vector3.one;
        image.color = targetColor;
    }
}