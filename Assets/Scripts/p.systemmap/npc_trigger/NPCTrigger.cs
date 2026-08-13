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

    [SerializeField]
    private GameObject npc;

    [SerializeField]
    private NPCTriggerMode triggerMode =
        NPCTriggerMode.ExistingNPC;


    // =========================================================
    // NPC SETTINGS
    // =========================================================

    [Header("NPC Settings")]

    [SerializeField]
    private bool hideNPCAfterDialogue = false;


    // =========================================================
    // DIALOGUE
    // =========================================================

    [Header("Dialogue")]

    [SerializeField]
    private DialogueUI dialogueUI;

    [SerializeField]
    private string npcName = "NPC";


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
    // QUEST COMPLETED DIALOGUE
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

    // Thời gian chờ trước khi NPC xuất hiện.
    [SerializeField]
    private float npcAppearDelay = 0.5f;


    // Thời gian hiển thị mỗi câu thoại.
    [SerializeField]
    private float dialogueDuration = 3f;


    // =========================================================
    // NPC GROUND SPAWN
    // =========================================================

    [Header("NPC Ground Spawn")]

    // Layer của mặt đất.
    //
    // Ví dụ:
    // Ground
    // Platform
    [SerializeField]
    private LayerMask groundLayer;


    // Khoảng cách NPC xuất hiện
    // so với Player.
    //
    // Có thể chỉnh trực tiếp trong Inspector.
    [SerializeField]
    private float npcSpawnDistance = 1.2f;


    // Raycast bắt đầu từ vị trí cao hơn Player.
    [SerializeField]
    private float groundCheckHeight = 10f;


    // Khoảng cách Raycast bắn xuống.
    [SerializeField]
    private float groundCheckDistance = 20f;


    // Khoảng cách nâng NPC lên khỏi mặt đất.
    //
    // Nếu chân NPC bị chui xuống đất,
    // hãy tăng giá trị này.
    [SerializeField]
    private float groundOffset = 0f;


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


        // NPC đã có sẵn trong Scene.
        if (
            triggerMode ==
            NPCTriggerMode.ExistingNPC
        )
        {
            npc.SetActive(true);
        }


        // NPC chỉ xuất hiện khi Player chạm Trigger.
        else if (
            triggerMode ==
            NPCTriggerMode.SpawnOnTrigger
        )
        {
            npc.SetActive(false);
        }


        // NPC Quest ban đầu bị ẩn.
        else if (
            triggerMode ==
            NPCTriggerMode.QuestNPC
        )
        {
            npc.SetActive(false);
        }
    }


    // =========================================================
    // PLAYER ENTER TRIGGER
    // =========================================================

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        // Tránh chạy nhiều lần.
        if (hasTriggered)
            return;


        // Chỉ Player mới kích hoạt.
        if (!other.CompareTag("Player"))
            return;


        hasTriggered = true;


        StartCoroutine(
            HandleNPC(
                other.gameObject
            )
        );
    }


    // =========================================================
    // HANDLE NPC
    // =========================================================

    private IEnumerator HandleNPC(
        GameObject player
    )
    {
        // =========================================
        // 1. KHÓA PLAYER
        // =========================================

        PlayerDialogueLock playerLock =
            player.GetComponent<PlayerDialogueLock>();

        if (playerLock != null)
        {
            playerLock.LockPlayer();

            Debug.Log(
                "Player đã bị khóa."
            );
        }


        // =========================================
        // 2. NPC SPAWN
        // =========================================

        if (
            triggerMode ==
            NPCTriggerMode.SpawnOnTrigger
            ||
            triggerMode ==
            NPCTriggerMode.QuestNPC
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


        // =========================================
        // 3. LƯU VỊ TRÍ NPC
        // =========================================

        Vector3 npcPosition =
            npc != null
            ? npc.transform.position
            : transform.position;


        // =========================================
        // 4. FIRST DIALOGUE
        // =========================================

        yield return StartCoroutine(
            PlayFirstDialogue()
        );


        // =========================================
        // 5. QUEST NPC
        // =========================================

        if (
            triggerMode ==
            NPCTriggerMode.QuestNPC
        )
        {
            // NPC biến mất nếu được bật.
            if (hideNPCAfterDialogue)
            {
                if (npc != null)
                {
                    npc.SetActive(false);
                }
            }


            // Bắt đầu Quest.
            StartChestQuest(
                npcPosition
            );


            // Mở khóa Player.
            if (playerLock != null)
            {
                playerLock.UnlockPlayer();

                Debug.Log(
                    "QuestNPC: Player đã được mở khóa."
                );
            }
        }


        // =========================================
        // 6. NORMAL NPC
        // =========================================

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
        // -----------------------------------------
        // CÂU THOẠI 1
        // -----------------------------------------

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


        // -----------------------------------------
        // CÂU THOẠI 2
        // -----------------------------------------

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


        // -----------------------------------------
        // ẨN DIALOGUE
        // -----------------------------------------

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
        // =========================================
        // NPC HIDE
        // =========================================

        if (hideNPCAfterDialogue)
        {
            if (npc != null)
            {
                npc.SetActive(false);
            }
        }


        // =========================================
        // LIGHT EFFECT
        // =========================================

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


        // =========================================
        // START QUEST
        // =========================================

        if (chestQuestManager != null)
        {
            chestQuestManager.StartChestQuest();
        }
        else if (
            ChestQuestManager.Instance != null
        )
        {
            ChestQuestManager.Instance
                .StartChestQuest();
        }


        Debug.Log(
            "Quest nhặt rương đã bắt đầu!"
        );
    }


    // =========================================================
    // CALLED WHEN CHESTS = 5
    // =========================================================

    public void StartSecondDialogue(
        GameObject player
    )
    {
        if (secondDialogueStarted)
            return;


        if (player == null)
        {
            Debug.LogError(
                "NPCTrigger: Player không tồn tại!"
            );

            return;
        }


        secondDialogueStarted = true;


        StartCoroutine(
            PlaySecondDialogue(
                player
            )
        );
    }


    // =========================================================
    // SECOND DIALOGUE
    // =========================================================

    private IEnumerator PlaySecondDialogue(
        GameObject player
    )
    {
        // =========================================
        // KHÓA PLAYER
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
        // CÂU THOẠI SAU KHI HOÀN THÀNH QUEST
        // =========================================

        if (dialogueUI != null)
        {
            dialogueUI.ShowDialogue(
                npcName,
                completedDialogue1
            );
        }


        yield return new WaitForSeconds(
            dialogueDuration
        );


        // =========================================
        // CÂU THOẠI THỨ 2
        // =========================================

        if (dialogueUI != null)
        {
            dialogueUI.ShowDialogue(
                npcName,
                completedDialogue2
            );
        }


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
        // MỞ KHÓA PLAYER
        // =========================================

        if (playerLock != null)
        {
            playerLock.UnlockPlayer();
        }
    }


    // =========================================================
    // SPAWN NPC AFTER QUEST REWARD
    // =========================================================

    public void SpawnNPCInFrontOfPlayer(
        GameObject player
    )
    {
        // Tránh NPC xuất hiện nhiều lần.
        if (secondDialogueStarted)
            return;


        if (player == null)
        {
            Debug.LogError(
                "NPCTrigger: Player không tồn tại!"
            );

            return;
        }


        if (npc == null)
        {
            Debug.LogError(
                "NPCTrigger: Chưa gán NPC!"
            );

            return;
        }


        // Đánh dấu NPC sau Quest
        // đã bắt đầu xuất hiện.
        secondDialogueStarted = true;


        StartCoroutine(
            SpawnNPCAfterDelay(
                player
            )
        );
    }


    // =========================================================
    // FIND GROUND POSITION
    // =========================================================

    private bool TryFindGround(
        float targetX,
        float playerY,
        out RaycastHit2D groundHit
    )
    {
        // Điểm bắt đầu Raycast.
        Vector2 rayStart =
            new Vector2(
                targetX,
                playerY + groundCheckHeight
            );


        // Raycast từ trên xuống.
        groundHit =
            Physics2D.Raycast(
                rayStart,
                Vector2.down,
                groundCheckDistance,
                groundLayer
            );


        // Trả về true nếu tìm thấy Ground.
        return groundHit.collider != null;
    }


    // =========================================================
    // SPAWN NPC ON GROUND
    // =========================================================

    private IEnumerator SpawnNPCAfterDelay(
        GameObject player
    )
    {
        // =========================================
        // DELAY
        // =========================================

        yield return new WaitForSeconds(
            npcAppearDelay
        );


        // =========================================
        // KIỂM TRA PLAYER
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
        // XÁC ĐỊNH HƯỚNG PLAYER
        // =========================================

        float playerDirection =
            Mathf.Sign(
                player.transform.localScale.x
            );


        // =========================================
        // THỬ SPAWN PHÍA TRƯỚC
        // =========================================

        float targetX =
            player.transform.position.x
            +
            playerDirection
            *
            npcSpawnDistance;


        RaycastHit2D groundHit;


        bool foundGround =
            TryFindGround(
                targetX,
                player.transform.position.y,
                out groundHit
            );


        // =========================================
        // NẾU PHÍA TRƯỚC KHÔNG CÓ ĐẤT
        // THỬ PHÍA SAU PLAYER
        // =========================================

        if (!foundGround)
        {
            Debug.Log(
                "Không có mặt đất phía trước. "
                + "Thử Spawn NPC phía sau Player."
            );


            targetX =
                player.transform.position.x
                -
                playerDirection
                *
                npcSpawnDistance;


            foundGround =
                TryFindGround(
                    targetX,
                    player.transform.position.y,
                    out groundHit
                );
        }


        // =========================================
        // KHÔNG TÌM THẤY GROUND
        // =========================================

        if (!foundGround)
        {
            Debug.LogError(
                "NPCTrigger: Không tìm thấy "
                + "mặt đất để Spawn NPC!"
            );

            yield break;
        }


        // =========================================
        // TÍNH VỊ TRÍ NPC
        // =========================================

        Vector3 spawnPosition =
            new Vector3(
                targetX,

                // Lấy đúng vị trí mặt đất
                // mà Raycast chạm vào.
                groundHit.point.y
                + groundOffset,

                // Giữ nguyên Z của NPC.
                npc.transform.position.z
            );


        // =========================================
        // ĐẶT NPC XUỐNG MẶT ĐẤT
        // =========================================

        npc.transform.position =
            spawnPosition;


        // =========================================
        // HIỆN NPC
        // =========================================

        npc.SetActive(true);


        // =========================================
        // NPC QUAY MẶT VỀ PLAYER
        // =========================================

        FacePlayer(player);


        // =========================================
        // BẮT ĐẦU DIALOGUE HOÀN THÀNH QUEST
        // =========================================

        StartCoroutine(
            PlaySecondDialogue(
                player
            )
        );
    }


    // =========================================================
    // FACE PLAYER
    // =========================================================

    private void FacePlayer(
        GameObject player
    )
    {
        if (npc == null || player == null)
            return;


        float direction =
            player.transform.position.x
            -
            npc.transform.position.x;


        // =========================================
        // PLAYER Ở BÊN PHẢI NPC
        // =========================================

        if (direction > 0)
        {
            npc.transform.localScale =
                new Vector3(
                    Mathf.Abs(
                        npc.transform.localScale.x
                    ),

                    npc.transform.localScale.y,

                    npc.transform.localScale.z
                );
        }


        // =========================================
        // PLAYER Ở BÊN TRÁI NPC
        // =========================================

        else
        {
            npc.transform.localScale =
                new Vector3(
                    -Mathf.Abs(
                        npc.transform.localScale.x
                    ),

                    npc.transform.localScale.y,

                    npc.transform.localScale.z
                );
        }
    }


    // =========================================================
    // DEBUG GROUND RAY
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        // Hiển thị vị trí Raycast trong Scene View.
        //
        // Chỉ để debug.
        // Không ảnh hưởng gameplay.

        if (Application.isPlaying == false)
            return;


        if (
            Player.instance == null
        )
            return;


        Vector3 playerPosition =
            Player.instance.transform.position;


        float direction =
            Mathf.Sign(
                Player.instance.transform.localScale.x
            );


        float frontX =
            playerPosition.x
            +
            direction
            *
            npcSpawnDistance;


        Vector3 rayStart =
            new Vector3(
                frontX,
                playerPosition.y
                + groundCheckHeight,
                0f
            );


        Gizmos.DrawLine(
            rayStart,
            rayStart
            +
            Vector3.down
            *
            groundCheckDistance
        );
    }
}