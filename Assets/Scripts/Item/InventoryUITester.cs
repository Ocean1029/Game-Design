using UnityEngine;

public class InventoryUITester : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            FindObjectOfType<InventoryUI>().SetItemCollected(ItemType.Key, true);
        else if (Input.GetKeyDown(KeyCode.K))
            FindObjectOfType<InventoryUI>().SetItemCollected(ItemType.Rope, true);
        else if (Input.GetKeyDown(KeyCode.L))
            FindObjectOfType<InventoryUI>().SetItemCollected(ItemType.Bottle, true);
    }
}