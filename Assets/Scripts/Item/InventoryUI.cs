using UnityEngine;
using UnityEngine.UI;
using System;

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

    void Awake()
    {
        // 啟動時先全部設成未取得（可依需求）
        foreach (var ic in icons)
            SetItemCollected(ic.type, false);
    }

    public void SetItemCollected(ItemType type, bool collected)
    {
        foreach (var ic in icons)
        {
            if (ic.type == type && ic.image != null)
            {
                ic.image.color = collected ? collectedColor : uncollectedColor;
                return;
            }
        }
        Debug.LogWarning($"[InventoryUI] No icon bound for {type}");
    }
}