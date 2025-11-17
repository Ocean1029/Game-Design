using UnityEngine;

public class Bomb_explode : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // 🔥 這個函式會在爆炸動畫結束時由 Animation Event 呼叫
    public void OnExplosionEnd()
    {
        // 尋找場景中名為 floor(10) 的物件
        GameObject targetFloor = GameObject.Find("Floor_destroy_by_bomb");
        GameObject targetFloor1 = GameObject.Find("grass1");

        if (targetFloor != null)
        {
            // 讓該物件消失
            targetFloor.SetActive(false);
            targetFloor1.SetActive(false);
            var hint = FindObjectOfType<HintTrigger>();
            if (hint != null)
            {
                hint.OnLinkedFloorDestroyed();
            }
            Debug.Log("💥 Floor_destroy_by_bomb 已消失！");
        }
        else
        {
            Debug.LogWarning("⚠️ 找不到名為 Floor_destroy_by_bomb 的物件！");
        }

        // （可選）摧毀炸彈物件本身
        Destroy(gameObject, 0.2f);
    }
}
