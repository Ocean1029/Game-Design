using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI 元件設定")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;

    [Header("打字機效果設定")]
    public float typingSpeed = 0.03f;  // 每個字之間的間隔（秒）
    public AudioSource typeSound;      // 可選：打字音效（可留空）

    private Coroutine typingCoroutine;
    private bool isTyping = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (dialogueBox != null)
            dialogueBox.SetActive(false);
    }

    public void ShowDialogue(string message)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueBox.SetActive(true);
        typingCoroutine = StartCoroutine(TypeText(message));
    }

    private IEnumerator TypeText(string message)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in message)
        {
            dialogueText.text += letter;

            // 可選：播放打字音效
            if (typeSound != null && !typeSound.isPlaying)
                typeSound.Play();

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void HideDialogue()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        dialogueBox.SetActive(false);
    }

    // 提供跳過效果用（如果之後想加“按鍵跳過文字”）
    public void SkipTyping(string fullText)
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = fullText;
            isTyping = false;
        }
    }
}
