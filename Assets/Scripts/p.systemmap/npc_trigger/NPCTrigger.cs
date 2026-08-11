using UnityEngine;
using System.Collections;

public class NPCTrigger : MonoBehaviour
{
    [Header("NPC")]
    [SerializeField] private GameObject npc;

    [Header("NPC Settings")]
    [SerializeField] private bool activateNPCOnTrigger = true;
    [SerializeField] private bool hideNPCAfterDialogue = true;

    [Header("Dialogue")]
    [SerializeField] private DialogueUI dialogueUI;

    [SerializeField] private string npcName = "NPC";

    [TextArea(2, 5)]
    [SerializeField] private string dialogueText =
        "Chào mừng bạn đến với vùng đất này!";

    [Header("Timing")]
    [SerializeField] private float npcAppearDelay = 3f;
    [SerializeField] private float dialogueDuration = 5f;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        hasTriggered = true;

        StartCoroutine(PlayNPCDialogue(other.gameObject));
    }

    private IEnumerator PlayNPCDialogue(GameObject player)
    {
        // =========================
        // 1. Khóa Player
        // =========================

        PlayerDialogueLock playerLock =
            player.GetComponent<PlayerDialogueLock>();

        if (playerLock != null)
        {
            playerLock.LockPlayer();
        }

        // =========================
        // 2. NPC xuất hiện nếu được phép
        // =========================

        if (activateNPCOnTrigger)
        {
            if (npc != null)
            {
                npc.SetActive(true);
            }

            // Chờ NPC xuất hiện
            yield return new WaitForSeconds(npcAppearDelay);
        }

        // =========================
        // 3. Hiện Dialogue
        // =========================

        if (dialogueUI != null)
        {
            dialogueUI.ShowDialogue(
                npcName,
                dialogueText
            );
        }

        // =========================
        // 4. Chờ thoại
        // =========================

        yield return new WaitForSeconds(dialogueDuration);

        // =========================
        // 5. Ẩn Dialogue
        // =========================

        if (dialogueUI != null)
        {
            dialogueUI.Hide();
        }

        // =========================
        // 6. NPC biến mất nếu được phép
        // =========================

        if (hideNPCAfterDialogue)
        {
            if (npc != null)
            {
                npc.SetActive(false);
            }
        }

        // =========================
        // 7. Mở khóa Player
        // =========================

        if (playerLock != null)
        {
            playerLock.UnlockPlayer();
        }

        // =========================
        // 8. Tắt Trigger
        // =========================

        gameObject.SetActive(false);
    }
}