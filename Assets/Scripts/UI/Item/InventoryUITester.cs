using UnityEngine;

public class InventoryUITester : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            FindFirstObjectByType<InventoryUI>().SetItemCollected(ItemType.Key, true);
        else if (Input.GetKeyDown(KeyCode.K))
            FindFirstObjectByType<InventoryUI>().SetItemCollected(ItemType.Rope, true);
        else if (Input.GetKeyDown(KeyCode.L))
            FindFirstObjectByType<InventoryUI>().SetItemCollected(ItemType.Bottle, true);
    }
}