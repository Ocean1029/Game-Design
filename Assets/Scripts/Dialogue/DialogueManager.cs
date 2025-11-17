using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI 元件")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;

    [Header("打字機")]
    public float typingSpeed = 0.03f;
    public AudioSource typeSound;

    private Coroutine typingCoroutine;
    private bool isTyping;

    private void Awake()
    {
        // 單例防重複
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 若要跨場景常駐，解除註解
        // DontDestroyOnLoad(gameObject);

        if (dialogueBox != null) dialogueBox.SetActive(false);
    }

    private void OnDestroy()
    {
        // 重要：避免外部還握著舊的靜態引用
        if (Instance == this) Instance = null;
    }

    public void ShowDialogue(string message)
    {
        // 防呆：物件被銷毀/停用時不做事
        if (!this || !gameObject) return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (dialogueBox != null) dialogueBox.SetActive(true);
        typingCoroutine = StartCoroutine(TypeText(message));
    }

    private IEnumerator TypeText(string message)
    {
        isTyping = true;
        if (dialogueText != null) dialogueText.text = "";

        foreach (char ch in message)
        {
            if (!this || !gameObject) yield break; // 途中被銷毀時安全退出

            if (dialogueText != null) dialogueText.text += ch;
            if (typeSound != null && !typeSound.isPlaying) typeSound.Play();

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void HideDialogue()
    {
        if (!this || !gameObject) return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (dialogueBox != null) dialogueBox.SetActive(false);
    }
}