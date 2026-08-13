using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Typewriter")]
    [SerializeField] private float typingSpeed = 0.04f;

    private Coroutine typingCoroutine;

    private void Start()
    {
        Hide();
    }

    public void ShowDialogue(string npcName, string message)
    {
        dialoguePanel.SetActive(true);

        nameText.text = npcName;

        // Nếu đang có hiệu ứng chữ cũ
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(message));
    }

    private IEnumerator TypeText(string message)
    {
        dialogueText.text = "";

        foreach (char letter in message)
        {
            dialogueText.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }

        typingCoroutine = null;
    }

    public void Hide()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.text = "";

        dialoguePanel.SetActive(false);
    }
}