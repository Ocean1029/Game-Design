using UnityEngine;

public class chair : MonoBehaviour
{
    [Tooltip("玩家坐下時要移動到的位置 (子物件)")]
    public Transform sitpoint;

    [Header("提示 UI")]
    public GameObject pressAPrompt;
    public GameObject pressDPrompt;

    // 顯示提示
    public void ShowPromptA(bool show)
    {
        if (pressAPrompt != null)
            pressAPrompt.SetActive(show);
    }

    public void ShowPromptD(bool show)
    {
        if (pressDPrompt != null)
            pressDPrompt.SetActive(show);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
