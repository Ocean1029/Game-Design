using UnityEngine;

/// <summary>
/// Handles bomb usage from inventory — spawns a bomb object and plays its animation
/// </summary>
public class PlayerUseBomb : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private InventorySystem inventorySystem;

    [Header("Bomb Settings")]
    [Tooltip("The Item ID for the bomb in the inventory")]
    [SerializeField] private string bombItemId = "bomb_for_rock";

    [Tooltip("The prefab to spawn when bomb is used")]
    [SerializeField] private GameObject bombPrefab;

    [Tooltip("Offset from player position when spawning bomb")]
    [SerializeField] private Vector2 spawnOffset = new Vector2(0f, -0.5f);

    private void Start()
    {
        if (inventorySystem == null)
        {
            inventorySystem = GetComponent<InventorySystem>();
        }

        if (inventorySystem == null)
        {
            Debug.LogError("PlayerUseBomb: InventorySystem not found on player!");
        }

        if (bombPrefab == null)
        {
            Debug.LogWarning("PlayerUseBomb: No bomb prefab assigned!");
        }

        Debug.Log($"[PlayerUseBomb] inventorySystem found? {inventorySystem != null}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("💥 Z pressed – TryUseBomb called");  // <— 新增這行
            TryUseBomb();
        }
    }

    private void TryUseBomb()
    {
        if (inventorySystem == null) return;

        // 1️⃣ 檢查背包內是否有炸彈
        ItemData bombItem = inventorySystem.GetItemById(bombItemId);
        if (bombItem == null)
        {
            Debug.Log("💣 PlayerUseBomb: No bomb in inventory!");
            return;
        }

        // 2️⃣ 生成炸彈物件
        if (bombPrefab != null)
        {
            // 距離可自行調整
            float facingDir = Mathf.Sign(transform.localScale.x);
            Vector3 spawnOffset = new Vector3(0.8f * facingDir, 0f, 0f);
            Vector3 spawnPosition = transform.position + spawnOffset;

            GameObject spawnedBomb = Instantiate(bombPrefab, spawnPosition, Quaternion.identity);

            Debug.Log("💣 PlayerUseBomb: Spawned bomb prefab");

            // 3️⃣ 播放炸彈動畫
            Animator bombAnimator = spawnedBomb.GetComponent<Animator>();
            if (bombAnimator != null)
            {
                bombAnimator.SetTrigger("Explode"); // trigger 名稱請確認 Animator 裡有
            }
            else
            {
                Debug.LogWarning("PlayerUseBomb: Bomb prefab has no Animator component!");
            }

            // 4️⃣ 延遲銷毀炸彈（讓動畫播完）
            Destroy(spawnedBomb, 1.5f);
        }

        // 5️⃣ 從背包移除炸彈
        inventorySystem.RemoveItem(bombItem, 1);
    }
}
