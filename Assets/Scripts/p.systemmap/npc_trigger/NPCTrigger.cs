using UnityEngine;
using System.Collections;
using System;

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

    public event Action OnBossDialogueFinished;

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
        "Tốt lắm! Bạn đã hoàn thành nhiệm vụ.";


    [TextArea(2, 5)]
    [SerializeField]
    private string completedDialogue2 =
        "Bây giờ hãy tiếp tục đi về phía trước.";


    // =========================================================
    // TIMING
    // =========================================================

    [Header("Timing")]

    [SerializeField]
    private float npcAppearDelay = 0.5f;

    [SerializeField]
    private float dialogueDuration = 3f;


    // =========================================================
    // QUEST NPC GROUND SPAWN
    // =========================================================
    //
    // QUAN TRỌNG:
    //
    // Đây là cơ chế RIÊNG cho NPC xuất hiện sau Quest.
    //
    // Các NPC cũ không bị ảnh hưởng.
    //
    // false = dùng vị trí cũ của NPC.
    // true  = tìm mặt đất bằng Raycast.
    //

    [Header("Quest NPC Ground Spawn")]

    [SerializeField]
    private bool useGroundSpawnForQuestNPC = false;


    [SerializeField]
    private LayerMask groundLayer;


    [SerializeField]
    private float npcSpawnDistance = 1.2f;


    [SerializeField]
    private float groundCheckHeight = 10f;


    [SerializeField]
    private float groundCheckDistance = 20f;


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


        // =====================================================
        // EXISTING NPC
        // =====================================================

        if (
            triggerMode ==
            NPCTriggerMode.ExistingNPC
        )
        {
            npc.SetActive(true);
        }


        // =====================================================
        // SPAWN ON TRIGGER
        // =====================================================

        else if (
            triggerMode ==
            NPCTriggerMode.SpawnOnTrigger
        )
        {
            npc.SetActive(false);
        }


        // =====================================================
        // QUEST NPC
        // =====================================================

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


        // Chỉ Player.
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
        // =====================================================
        // 1. KHÓA PLAYER
        // =====================================================

        PlayerDialogueLock playerLock =
            player.GetComponent<PlayerDialogueLock>();


        if (playerLock != null)
        {
            playerLock.LockPlayer();

            Debug.Log(
                "Player đã bị khóa."
            );
        }


        // =====================================================
        // 2. NPC SPAWN
        // =====================================================

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


        // =====================================================
        // 3. LƯU VỊ TRÍ NPC
        // =====================================================

        Vector3 npcPosition =
            npc != null
            ? npc.transform.position
            : transform.position;


        // =====================================================
        // 4. FIRST DIALOGUE
        // =====================================================

        yield return StartCoroutine(
            PlayFirstDialogue()
        );


        // =====================================================
        // 5. QUEST NPC
        // =====================================================

        if (
            triggerMode ==
            NPCTriggerMode.QuestNPC
        )
        {
            if (hideNPCAfterDialogue)
            {
                if (npc != null)
                {
                    npc.SetActive(false);
                }
            }


            StartChestQuest(
                npcPosition
            );


            if (playerLock != null)
            {
                playerLock.UnlockPlayer();

                Debug.Log(
                    "QuestNPC: Player đã được mở khóa."
                );
            }
        }


        // =====================================================
        // 6. NORMAL NPC
        // =====================================================

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
        // =====================================================
        // CÂU 1
        // =====================================================

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


        // =====================================================
        // CÂU 2
        // =====================================================

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


        // =====================================================
        // ẨN DIALOGUE
        // =====================================================

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
        // =====================================================
        // NPC HIDE
        // =====================================================

        if (hideNPCAfterDialogue)
        {
            if (npc != null)
            {
                npc.SetActive(false);
            }
        }


        // =====================================================
        // LIGHT EFFECT
        // =====================================================

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


        // =====================================================
        // START QUEST
        // =====================================================

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
    // START SECOND DIALOGUE
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
        // =====================================================
        // KHÓA PLAYER
        // =====================================================

        PlayerDialogueLock playerLock =
            player.GetComponent<PlayerDialogueLock>();


        if (playerLock != null)
        {
            playerLock.LockPlayer();

            Debug.Log(
                "Player đã bị khóa để nói chuyện với NPC."
            );
        }


        // =====================================================
        // NPC QUAY VỀ PLAYER
        // =====================================================

        FacePlayer(player);


        // =====================================================
        // CÂU 1
        // =====================================================

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


        // =====================================================
        // CÂU 2
        // =====================================================

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


        // =====================================================
        // ẨN DIALOGUE
        // =====================================================

        if (dialogueUI != null)
        {
            dialogueUI.Hide();
        }


        // =====================================================
        // NPC BIẾN MẤT
        // =====================================================

        if (hideNPCAfterDialogue)
        {
            if (npc != null)
            {
                npc.SetActive(false);
            }
        }


        // =====================================================
        // MỞ KHÓA PLAYER
        // =====================================================

        if (playerLock != null)
        {
            playerLock.UnlockPlayer();

            Debug.Log(
                "NPC Dialogue đã kết thúc. Player được mở khóa."
            );
        }


        // =====================================================
        // BÁO BOSS ROOM
        // =====================================================

        OnBossDialogueFinished?.Invoke();

        Debug.Log(
            "NPCTrigger: OnBossDialogueFinished đã được gọi."
        );
    }


    // =========================================================
    // SPAWN NPC AFTER QUEST REWARD
    // =========================================================
    //
    // HÀM NÀY CHỈ ĐƯỢC GỌI KHI QUEST ĐÃ THỰC SỰ HOÀN THÀNH
    // VÀ PLAYER ĐÃ NHẬN REWARD.
    //

    public void SpawnNPCInFrontOfPlayer(
        GameObject player
    )
    {
        Debug.Log(
            "========== SpawnNPCInFrontOfPlayer ĐƯỢC GỌI =========="
        );

        Debug.Log(
            "NPCTrigger: " + gameObject.name
        );


        // =====================================================
        // TRÁNH SPAWN NHIỀU LẦN
        // =====================================================

        if (secondDialogueStarted)
        {
            Debug.Log(
                "NPCTrigger: NPC sau Quest đã được xử lý."
            );

            return;
        }


        // =====================================================
        // PLAYER
        // =====================================================

        if (player == null)
        {
            Debug.LogError(
                "NPCTrigger: Player không tồn tại!"
            );

            return;
        }


        // =====================================================
        // NPC
        // =====================================================

        if (npc == null)
        {
            Debug.LogError(
                "NPCTrigger: Chưa gán NPC!"
            );

            return;
        }


        // =====================================================
        // ĐÁNH DẤU ĐÃ BẮT ĐẦU
        // =====================================================

        secondDialogueStarted = true;


        StartCoroutine(
            SpawnNPCAfterDelay(
                player
            )
        );
    }


    // =========================================================
    // FIND GROUND
    // =========================================================

    private bool TryFindGround(
        float targetX,
        float playerY,
        out RaycastHit2D groundHit
    )
    {
        Vector2 rayStart =
            new Vector2(
                targetX,
                playerY + groundCheckHeight
            );


        groundHit =
            Physics2D.Raycast(
                rayStart,
                Vector2.down,
                groundCheckDistance,
                groundLayer
            );


        return groundHit.collider != null;
    }


    // =========================================================
    // SPAWN NPC AFTER QUEST
    // =========================================================

    private IEnumerator SpawnNPCAfterDelay(
        GameObject player
    )
    {
        // =====================================================
        // DELAY
        // =====================================================

        yield return new WaitForSeconds(
            npcAppearDelay
        );


        // =====================================================
        // PLAYER
        // =====================================================

        if (player == null)
        {
            Debug.LogError(
                "NPCTrigger: Player không tồn tại!"
            );

            yield break;
        }


        // =====================================================
        // NPC
        // =====================================================

        if (npc == null)
        {
            Debug.LogError(
                "NPCTrigger: NPC chưa được gán!"
            );

            yield break;
        }


        // =====================================================
        // CƠ CHẾ CŨ
        // =====================================================
        //
        // Nếu useGroundSpawnForQuestNPC = FALSE
        //
        // NPC sẽ xuất hiện tại vị trí hiện tại của nó.
        //
        // Không Raycast.
        // Không cần Ground Layer.
        // Không ảnh hưởng Scene cũ.
        //

        if (!useGroundSpawnForQuestNPC)
        {
            Debug.Log(
                "NPCTrigger: Đang dùng cơ chế Spawn cũ."
            );


            npc.SetActive(true);


            FacePlayer(player);


            StartCoroutine(
                PlaySecondDialogue(
                    player
                )
            );


            yield break;
        }


        // =====================================================
        // CƠ CHẾ GROUND SPAWN MỚI
        // =====================================================

        Debug.Log(
            "NPCTrigger: Đang dùng Ground Spawn."
        );


        // =====================================================
        // HƯỚNG PLAYER
        // =====================================================

        float playerDirection =
            Mathf.Sign(
                player.transform.localScale.x
            );


        if (Mathf.Approximately(
            playerDirection,
            0f))
        {
            playerDirection = 1f;
        }


        // =====================================================
        // SPAWN PHÍA TRƯỚC
        // =====================================================

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


        // =====================================================
        // THỬ PHÍA SAU
        // =====================================================

        if (!foundGround)
        {
            Debug.Log(
                "NPCTrigger: Không có Ground phía trước. "
                +
                "Thử phía sau Player."
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


        // =====================================================
        // KHÔNG TÌM THẤY GROUND
        // =====================================================

        if (!foundGround)
        {
            Debug.LogError(
                "NPCTrigger: Không tìm thấy mặt đất "
                +
                "để Spawn NPC!"
            );


            // =================================================
            // QUAN TRỌNG:
            // Không để Quest bị kẹt hoàn toàn.
            //
            // Nếu Raycast thất bại,
            // Spawn NPC tại vị trí hiện tại.
            // =================================================

            npc.SetActive(true);


            FacePlayer(player);


            StartCoroutine(
                PlaySecondDialogue(
                    player
                )
            );


            yield break;
        }


        // =====================================================
        // TÍNH VỊ TRÍ SPAWN
        // =====================================================

        Vector3 spawnPosition =
            new Vector3(
                targetX,
                groundHit.point.y
                +
                groundOffset,
                npc.transform.position.z
            );


        // =====================================================
        // ĐẶT NPC
        // =====================================================

        npc.transform.position =
            spawnPosition;


        // =====================================================
        // HIỆN NPC
        // =====================================================

        npc.SetActive(true);


        // =====================================================
        // QUAY VỀ PLAYER
        // =====================================================

        FacePlayer(player);


        // =====================================================
        // DIALOGUE
        // =====================================================

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


        // =====================================================
        // PLAYER BÊN PHẢI
        // =====================================================

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


        // =====================================================
        // PLAYER BÊN TRÁI
        // =====================================================

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
        if (!Application.isPlaying)
            return;


        if (Player.instance == null)
            return;


        Vector3 playerPosition =
            Player.instance.transform.position;


        float direction =
            Mathf.Sign(
                Player.instance.transform.localScale.x
            );


        if (Mathf.Approximately(
            direction,
            0f))
        {
            direction = 1f;
        }


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
                +
                groundCheckHeight,
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