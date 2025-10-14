using UnityEngine;

public class JumpUITester : MonoBehaviour
{
    private JumpUI ui;
    private int jumpsLeft = 3;

    void Start() => ui = FindObjectOfType<JumpUI>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpsLeft = Mathf.Max(0, jumpsLeft - 1);
            ui.UpdateJumpCount(jumpsLeft);
        }
    }
}