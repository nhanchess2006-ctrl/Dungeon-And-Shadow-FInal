using UnityEngine;
using System.Collections;

public class NPCTrigger : MonoBehaviour
{
    // =========================================================
    // NPC MODE
    // =========================================================

    public enum NPCTriggerMode
    {
        ExistingNPC,
        SpawnOnTrigger,
        QuestNPC
    }


    // =========================================================
    // NPC
    // =========================================================

    [Header("NPC")]
    [SerializeField] private GameObject npc;

    [SerializeField]
    private NPCTriggerMode triggerMode =
        NPCTriggerMode.ExistingNPC;


    // =========================================================
    // NPC SETTINGS
    // =========================================================

    [Header("NPC Settings")]
    [SerializeField] private bool hideNPCAfterDialogue = false;


    // =========================================================
    // DIALOGUE
    // =========================================================

    [Header("Dialogue")]
    [SerializeField] private DialogueUI dialogueUI;

    [SerializeField] private string npcName = "NPC";


    // =========================================================
    // FIRST DIALOGUE
    // =========================================================

    [Header("First Dialogue")]

    [TextArea(2, 5)]
    [SerializeField]
    private string firstDialogue =
        "Chào mừng bạn đến với vùng đất này!";


    [TextArea(2, 5)]
    [SerializeField]
    private string secondDialogue =
        "Hãy lại gần người thợ rèn và bấm F để tương tác.";


    // =========================================================
    // SECOND DIALOGUE
    // =========================================================

    [Header("Quest Completed Dialogue")]

    [TextArea(2, 5)]
    [SerializeField]
    private string completedDialogue1 =
        "Tốt lắm! Bạn đã thu thập đủ 5 chiếc rương.";


    [TextArea(2, 5)]
    [SerializeField]
    private string completedDialogue2 =
        "Bây giờ hãy tiếp tục đi về phía trước.";


    // =========================================================
    // TIMING
    // =========================================================

    [Header("Timing")]
    [SerializeField] private float npcAppearDelay = 0.5f;

    [SerializeField] private float dialogueDuration = 3f;


    // =========================================================
    // CHEST QUEST
    // =========================================================

    [Header("Chest Quest")]

    [SerializeField]
    private ChestQuestManager chestQuestManager;


    // =========================================================
    // QUEST LIGHT
    // =========================================================

    [Header("Quest Light Effect")]

    [SerializeField]
    private QuestLightController questLightController;


    // =========================================================
    // INTERNAL
    // =========================================================

    private bool hasTriggered = false;

    private bool secondDialogueStarted = false;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        SetupNPC();
    }


    // =========================================================
    // SETUP NPC
    // =========================================================

    private void SetupNPC()
    {
        if (npc == null)
        {
            Debug.LogWarning(
                "NPCTrigger: Chưa gán NPC!"
            );

            return;
        }


        // -----------------------------------------
        // NPC ĐÃ CÓ SẴN
        // -----------------------------------------

        if (triggerMode ==
            NPCTriggerMode.ExistingNPC)
        {
            npc.SetActive(true);
        }


        // -----------------------------------------
        // NPC XUẤT HIỆN KHI CHẠM
        // -----------------------------------------

        else if (
            triggerMode ==
            NPCTriggerMode.SpawnOnTrigger)
        {
            npc.SetActive(false);
        }


        // -----------------------------------------
        // NPC QUEST
        // -----------------------------------------

        else if (
            triggerMode ==
            NPCTriggerMode.QuestNPC)
        {
            npc.SetActive(false);
        }
    }


    // =========================================================
    // PLAYER ENTER
    // =========================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered)
            return;


        if (!other.CompareTag("Player"))
            return;


        hasTriggered = true;


        StartCoroutine(
            HandleNPC(other.gameObject)
        );
    }


    // =========================================================
    // HANDLE NPC
    // =========================================================

    private IEnumerator HandleNPC(GameObject player)
{
    // -----------------------------------------
    // 1. LOCK PLAYER
    // -----------------------------------------

    PlayerDialogueLock playerLock =
        player.GetComponent<PlayerDialogueLock>();

    if (playerLock != null)
    {
        playerLock.LockPlayer();

        Debug.Log("Player đã bị khóa.");
    }


    // -----------------------------------------
    // 2. NPC SPAWN
    // -----------------------------------------

    if (
        triggerMode == NPCTriggerMode.SpawnOnTrigger
        ||
        triggerMode == NPCTriggerMode.QuestNPC
    )
    {
        if (npc != null)
        {
            npc.SetActive(true);
        }

        yield return new WaitForSeconds(
            npcAppearDelay
        );
    }


    // -----------------------------------------
    // 3. SAVE NPC POSITION
    // -----------------------------------------

    Vector3 npcPosition =
        npc != null
            ? npc.transform.position
            : transform.position;


    // -----------------------------------------
    // 4. FIRST DIALOGUE
    // -----------------------------------------

    yield return StartCoroutine(
        PlayFirstDialogue()
    );


    // -----------------------------------------
    // 5. QUEST NPC
    // -----------------------------------------

    if (
        triggerMode == NPCTriggerMode.QuestNPC
    )
    {
        // NPC biến mất
        if (hideNPCAfterDialogue)
        {
            if (npc != null)
            {
                npc.SetActive(false);
            }
        }


        // Bắt đầu hiệu ứng + quest
        StartChestQuest(
            npcPosition
        );


        // -----------------------------------------
        // QUAN TRỌNG:
        // MỞ KHÓA PLAYER
        // -----------------------------------------

        if (playerLock != null)
        {
            playerLock.UnlockPlayer();

            Debug.Log(
                "QuestNPC: Player đã được mở khóa."
            );
        }
    }


    // -----------------------------------------
    // 6. NORMAL NPC
    // -----------------------------------------

    else
    {
        if (hideNPCAfterDialogue)
        {
            if (npc != null)
            {
                npc.SetActive(false);
            }
        }


        if (playerLock != null)
        {
            playerLock.UnlockPlayer();

            Debug.Log(
                "Normal NPC: Player đã được mở khóa."
            );
        }
    }
}


    // =========================================================
    // FIRST DIALOGUE
    // =========================================================

    private IEnumerator PlayFirstDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.ShowDialogue(
                npcName,
                firstDialogue
            );
        }


        yield return new WaitForSeconds(
            dialogueDuration
        );


        if (dialogueUI != null)
        {
            dialogueUI.ShowDialogue(
                npcName,
                secondDialogue
            );
        }


        yield return new WaitForSeconds(
            dialogueDuration
        );


        if (dialogueUI != null)
        {
            dialogueUI.Hide();
        }
    }


    // =========================================================
    // START CHEST QUEST
    // =========================================================

    private void StartChestQuest(
        Vector3 npcPosition
    )
    {
        // -----------------------------------------
        // NPC HIDE
        // -----------------------------------------

        if (hideNPCAfterDialogue)
        {
            if (npc != null)
            {
                npc.SetActive(false);
            }
        }


        // -----------------------------------------
        // LIGHT EFFECT
        // -----------------------------------------

        if (questLightController != null)
        {
            questLightController.PlayEffect(
                npcPosition
            );
        }
        else if (
            QuestLightController.Instance != null
        )
        {
            QuestLightController.Instance.PlayEffect(
                npcPosition
            );
        }


        // -----------------------------------------
        // START QUEST
        // -----------------------------------------

        if (chestQuestManager != null)
        {
            chestQuestManager.StartChestQuest();
        }
        else if (
            ChestQuestManager.Instance != null
        )
        {
            ChestQuestManager.Instance.StartChestQuest();
        }


        Debug.Log(
            "Quest nhặt rương đã bắt đầu!"
        );
    }


    // =========================================================
    // CALLED WHEN CHESTS = 5
    // =========================================================

    public void StartSecondDialogue(GameObject player)
{
    if (secondDialogueStarted)
        return;

    if (player == null)
    {
        Debug.LogError("NPCTrigger: Player không tồn tại!");
        return;
    }

    secondDialogueStarted = true;

    StartCoroutine(
        PlaySecondDialogue(player)
    );
}


    // =========================================================
    // SECOND DIALOGUE
    // =========================================================

    private IEnumerator PlaySecondDialogue(GameObject player)
{
    // =========================================
    // LOCK PLAYER
    // =========================================

    PlayerDialogueLock playerLock =
        player.GetComponent<PlayerDialogueLock>();

    if (playerLock != null)
    {
        playerLock.LockPlayer();
    }


    // =========================================
    // NPC QUAY MẶT VỀ PLAYER
    // =========================================

    FacePlayer(player);


    // =========================================
    // THOẠI LẦN 2
    // =========================================

    if (dialogueUI != null)
    {
        dialogueUI.ShowDialogue(
            npcName,
            secondDialogue
        );
    }


    // Chờ thoại
    yield return new WaitForSeconds(
        dialogueDuration
    );


    // =========================================
    // ẨN DIALOGUE
    // =========================================

    if (dialogueUI != null)
    {
        dialogueUI.Hide();
    }


    // =========================================
    // NPC BIẾN MẤT
    // =========================================

    if (hideNPCAfterDialogue)
    {
        if (npc != null)
        {
            npc.SetActive(false);
        }
    }


    // =========================================
    // UNLOCK PLAYER
    // =========================================

    if (playerLock != null)
    {
        playerLock.UnlockPlayer();
    }
}
    public void SpawnNPCInFrontOfPlayer(GameObject player)
{
    if (secondDialogueStarted)
        return;

    if (player == null)
    {
        Debug.LogError("NPCTrigger: Player không tồn tại!");
        return;
    }

    if (npc == null)
    {
        Debug.LogError("NPCTrigger: Chưa gán NPC!");
        return;
    }

    secondDialogueStarted = true;

    StartCoroutine(
        SpawnNPCAfterDelay(player)
    );
}
private IEnumerator SpawnNPCAfterDelay(GameObject player)
{
    // =========================================
    // DELAY SAU KHI ĐỦ 5 BÔNG
    // =========================================

    yield return new WaitForSeconds(0.5f);


    // =========================================
    // KIỂM TRA LẠI PLAYER
    // =========================================

    if (player == null)
    {
        Debug.LogError(
            "NPCTrigger: Player không tồn tại!"
        );

        yield break;
    }


    // =========================================
    // KIỂM TRA NPC
    // =========================================

    if (npc == null)
    {
        Debug.LogError(
            "NPCTrigger: NPC chưa được gán!"
        );

        yield break;
    }


    // =========================================
    // LẤY HƯỚNG PLAYER
    // =========================================

    float direction =
        Mathf.Sign(
            player.transform.localScale.x
        );


    // =========================================
    // TÍNH VỊ TRÍ NPC
    // =========================================

    Vector3 spawnPosition =
        player.transform.position;


    spawnPosition.x +=
        direction * 1.2f;


    spawnPosition.y =
        player.transform.position.y;


    spawnPosition.z =
        npc.transform.position.z;


    // =========================================
    // ĐẶT NPC
    // =========================================

    npc.transform.position =
        spawnPosition;


    // =========================================
    // HIỆN NPC
    // =========================================

    npc.SetActive(true);


    // =========================================
    // NPC NHÌN PLAYER
    // =========================================

    FacePlayer(player);


    // =========================================
    // BẮT ĐẦU THOẠI
    // =========================================

    StartCoroutine(
        PlaySecondDialogue(player)
    );
}
private void FacePlayer(GameObject player)
{
    if (npc == null || player == null)
        return;

    float direction =
        player.transform.position.x
        - npc.transform.position.x;

    if (direction > 0)
    {
        npc.transform.localScale =
            new Vector3(
                Mathf.Abs(npc.transform.localScale.x),
                npc.transform.localScale.y,
                npc.transform.localScale.z
            );
    }
    else
    {
        npc.transform.localScale =
            new Vector3(
                -Mathf.Abs(npc.transform.localScale.x),
                npc.transform.localScale.y,
                npc.transform.localScale.z
            );
    }
}
    
}