using System.Collections;
using UnityEngine;

public class StoneButton : MonoBehaviour
{
    [Header("Stone Wall")]
    public StoneWall stoneWall;

    [Header("Camera")]
    public Camera mainCamera;
    public float cameraMoveDuration = 1f;

    [Header("Camera Follow Script")]
    public CameraFollow followScript;

    private bool isActivated = false;

    private Vector3 originalCameraPos;
    private PlayerController cachedPlayer;
    private PlayerMovement cachedMovement; // ⭐ movement system

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (followScript == null)
            followScript = mainCamera.GetComponent<CameraFollow>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        if (isActivated) return;
        isActivated = true;

        cachedPlayer = player;
        cachedMovement = player.GetComponent<PlayerMovement>(); // ⭐ 取得真正控制移動的腳本

        // ⭐ 完全鎖住玩家移動
        cachedMovement.StopMovement();
        cachedMovement.SetMovementLocked(true);
        cachedMovement.SetGravityEnabled(false);

        originalCameraPos = mainCamera.transform.position;

        StartCoroutine(CameraFlow());
    }

    private IEnumerator CameraFlow()
    {
        // 停用鏡頭跟隨
        if (followScript != null) followScript.enabled = false;

        yield return MoveCamera(mainCamera.transform.position, stoneWall.transform.position);

        yield return stoneWall.PlayRaiseAnimation();

        yield return MoveCamera(mainCamera.transform.position, originalCameraPos);

        // ⭐ 解鎖玩家移動
        if (cachedMovement != null)
        {
            cachedMovement.SetMovementLocked(false);
            cachedMovement.SetGravityEnabled(true);
        }

        // 恢復 CameraFollow
        if (followScript != null) followScript.enabled = true;
    }

    private IEnumerator MoveCamera(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;
        Vector3 start = from;
        Vector3 end = to;
        end.z = start.z;

        while (elapsed < cameraMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / cameraMoveDuration);
            mainCamera.transform.position = Vector3.Lerp(start, end, t);

            yield return null;
        }
        mainCamera.transform.position = end;
    }
}

