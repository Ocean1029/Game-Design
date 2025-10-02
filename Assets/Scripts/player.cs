using UnityEngine;

public class player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    private GameObject carriedKey = null;
    public Transform holdpoint;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(moveSpeed * Time.deltaTime, 0, 0);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(-moveSpeed * Time.deltaTime, 0, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("key1") && carriedKey == null)
        {
            carriedKey = collision.gameObject;

            if (holdpoint != null)
            {
                carriedKey.transform.SetParent(holdpoint);
                carriedKey.transform.localPosition = Vector3.zero;
            }
            else
            {
                Debug.LogError("HoldPoint is not assigned in the Inspector!");
            }
        }
    }
    // 用於讓門檢查
    public GameObject GetCarriedKey()
    {
        return carriedKey;
    }

    // 如果門成功打開，門可以呼叫此方法銷毀或釋放鑰匙
    public void UseCarriedKey()
    {
        if (carriedKey != null)
        {
            Destroy(carriedKey);
            carriedKey = null;
        }
    }

}