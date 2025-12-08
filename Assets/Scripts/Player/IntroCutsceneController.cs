// using UnityEngine;
// using System.Collections;

// public class IntroCutsceneController : MonoBehaviour
// {
//     [SerializeField] private Transform walkEndPoint;
//     [SerializeField] private float walkSpeed = 1f;

//     private PlayerController player;
//     private PlayerMovement movement;
//     private PlayerStateMachine stateMachine;


//     private bool cutscenePlaying = false;

//     private void Start()
//     {
//         player = FindFirstObjectByType<PlayerController>();
//         movement = player.GetComponent<PlayerMovement>();
//         stateMachine = player.GetComponent<PlayerStateMachine>();

//         StartCoroutine(PlayIntroCutscene());
//     }

//     private IEnumerator PlayIntroCutscene()
//     {
//         // 若已經有存檔 → 不播開場動畫
//         if (SaveSystem.HasSaveData())
//             yield break;

//         Debug.Log("Intro Cutscene Started");

//         cutscenePlaying = true;

//         // 方向判斷
//         float direction = (walkEndPoint.position.x > player.transform.position.x) ? 1f : -1f;

//         // ⭐ 切換到 Cutscene 狀態 → 自動鎖輸入
//         stateMachine.ChangeState(PlayerState.Cutscene);

//         // ⭐ 自動走路
//         while (Mathf.Abs(player.transform.position.x - walkEndPoint.position.x) > 0.1f)
//         {
//             movement.Move(direction * 0.5f); // 走路動畫自動播放
//             yield return null;
//         }

//         // ⭐ 停止移動
//         movement.Move(0f);
//         movement.StopMovement();


//         // ⭐ 切回 Idle → 恢復輸入
//         stateMachine.ChangeState(PlayerState.Idle);


//         cutscenePlaying = false;

//         Debug.Log("Intro Cutscene Finished");
//     }

// }


using UnityEngine;
using System.Collections;

public class IntroCutsceneController : MonoBehaviour
{
    [SerializeField] private Transform walkEndPoint;

    private PlayerController player;
    private PlayerMovement movement;
    private PlayerStateMachine stateMachine;

    private Animator animator;
    private bool cutscenePlaying = false;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        movement = player.GetComponent<PlayerMovement>();
        stateMachine = player.GetComponent<PlayerStateMachine>();

        // ⭐ 取得 Visual 子物件的 Animator
        animator = player.transform.Find("Visual").GetComponent<Animator>();

        StartCoroutine(PlayIntroCutscene());
    }

    private IEnumerator PlayIntroCutscene()
    {
        if (SaveSystem.HasSaveData())
            yield break;

        cutscenePlaying = true;

        float direction = (walkEndPoint.position.x > player.transform.position.x) ? 1f : -1f;

        // ⭐ 鎖定輸入
        stateMachine.ChangeState(PlayerState.Cutscene);

        while (Mathf.Abs(player.transform.position.x - walkEndPoint.position.x) > 0.1f)
        {
            animator.SetFloat("Speed", 1f);      // ⭐ 播走路動畫
            movement.Move(direction * 0.5f);     // ⭐ 自動走路
            yield return null;
        }

        movement.Move(0f);
        movement.StopMovement();
        animator.SetFloat("Speed", 0f);          // ⭐ 停止走路動畫

        // ⭐ 恢復輸入
        stateMachine.ChangeState(PlayerState.Idle);
        cutscenePlaying = false;
    }
}
