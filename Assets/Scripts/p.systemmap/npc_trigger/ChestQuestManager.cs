using UnityEngine;
using TMPro;
using System.Collections;

public class ChestQuestManager : MonoBehaviour
{
    public static ChestQuestManager Instance;

    // =========================================================
    // QUEST
    // =========================================================

    [Header("Quest")]

    [SerializeField] private int targetChest = 5;

    private int currentChest = 0;

    private bool questStarted = false;
    private bool questCompleted = false;


    // =========================================================
    // QUEST UI
    // =========================================================

    [Header("Quest UI")]

    [SerializeField] private GameObject questPanel;

    [SerializeField] private TMP_Text questText;


    // =========================================================
    // QUEST COMPLETE UI
    // =========================================================

    [Header("Quest Complete UI")]

    [SerializeField] private GameObject questCompletePanel;

    [SerializeField] private CanvasGroup completeCanvasGroup;

    [SerializeField] private RectTransform completeTitle;

    [SerializeField] private float fadeInDuration = 0.3f;

    [SerializeField] private float scaleDuration = 0.45f;

    [SerializeField] private float completeTextDuration = 1.5f;

    [SerializeField] private float fadeOutDuration = 0.4f;


    // =========================================================
    // NPC
    // =========================================================

    [Header("NPC After Quest")]

    [Tooltip("NPC GameObject sẽ xuất hiện sau khi người chơi Confirm Reward.")]
    [SerializeField] private GameObject npcObject;

    [SerializeField] private float npcSpawnDelay = 1f;


    // =========================================================
    // QUEST REWARD
    // =========================================================

    [Header("Quest Reward")]

    [SerializeField]
    private QuestRewardController questRewardController;


    // =========================================================
    // PUBLIC PROPERTY
    // =========================================================

    public int CurrentChest => currentChest;

    public int TargetChest => targetChest;

    public bool IsQuestCompleted => questCompleted;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        // Ẩn UI quest
        if (questPanel != null)
        {
            questPanel.SetActive(false);
        }

        // Ẩn UI hoàn thành
        if (questCompletePanel != null)
        {
            questCompletePanel.SetActive(false);
        }

        // NPC không xuất hiện từ đầu
        if (npcObject != null)
        {
            npcObject.SetActive(false);
        }

        UpdateQuestUI();
    }


    // =========================================================
    // START QUEST
    // =========================================================

    public void StartChestQuest()
    {
        if (questStarted)
        {
            Debug.LogWarning(
                "ChestQuestManager: Quest đã được bắt đầu."
            );

            return;
        }

        questStarted = true;

        currentChest = 0;

        questCompleted = false;

        if (questPanel != null)
        {
            questPanel.SetActive(true);
        }

        UpdateQuestUI();

        Debug.Log(
            "========================================"
        );

        Debug.Log(
            "CHEST QUEST STARTED: 0/" + targetChest
        );

        Debug.Log(
            "========================================"
        );
    }


    // =========================================================
    // COLLECT CHEST
    // =========================================================

    public void CollectChest()
    {
        Debug.Log(
            "=== COLLECT CHEST CALLED ==="
        );


        // -----------------------------------------------------
        // QUEST CHƯA BẮT ĐẦU
        // -----------------------------------------------------

        if (!questStarted)
        {
            Debug.LogWarning(
                "ChestQuestManager: Quest chưa được bắt đầu!"
            );

            return;
        }


        // -----------------------------------------------------
        // QUEST ĐÃ HOÀN THÀNH
        // -----------------------------------------------------

        if (questCompleted)
        {
            Debug.LogWarning(
                "ChestQuestManager: Quest đã hoàn thành!"
            );

            return;
        }


        // -----------------------------------------------------
        // CỘNG RƯƠNG
        // -----------------------------------------------------

        currentChest++;

        // Không cho vượt quá target
        if (currentChest > targetChest)
        {
            currentChest = targetChest;
        }


        // -----------------------------------------------------
        // UPDATE UI
        // -----------------------------------------------------

        UpdateQuestUI();


        Debug.Log(
            "RƯƠNG: "
            + currentChest
            + "/"
            + targetChest
        );


        // -----------------------------------------------------
        // KIỂM TRA HOÀN THÀNH
        // -----------------------------------------------------

        if (currentChest >= targetChest)
        {
            CompleteQuest();
        }
    }


    // =========================================================
    // UPDATE QUEST UI
    // =========================================================

    private void UpdateQuestUI()
    {
        if (questText == null)
        {
            return;
        }

        questText.text =
            "Lụm rương "
            + currentChest
            + "/"
            + targetChest;
    }


    // =========================================================
    // COMPLETE QUEST
    // =========================================================

    private void CompleteQuest()
    {
        if (questCompleted)
        {
            return;
        }

        questCompleted = true;


        Debug.Log(
            "========================================"
        );

        Debug.Log(
            "NHIỆM VỤ ĐÃ HOÀN THÀNH!"
        );

        Debug.Log(
            "========================================"
        );


        // -----------------------------------------------------
        // ẨN QUEST PANEL
        // -----------------------------------------------------

        if (questPanel != null)
        {
            questPanel.SetActive(false);
        }


        // -----------------------------------------------------
        // TÌM PLAYER
        // -----------------------------------------------------

        GameObject player =
            GameObject.FindGameObjectWithTag("Player");


        if (player == null)
        {
            Debug.LogError(
                "ChestQuestManager: Không tìm thấy Player!"
            );

            return;
        }


        // -----------------------------------------------------
        // BẮT ĐẦU SEQUENCE
        // -----------------------------------------------------

        StartCoroutine(
            QuestCompleteSequence(player)
        );
    }


    // =========================================================
    // QUEST COMPLETE SEQUENCE
    // =========================================================

    private IEnumerator QuestCompleteSequence(
        GameObject player
    )
    {
        // =====================================================
        // COMPLETE PANEL
        // =====================================================

        if (questCompletePanel != null)
        {
            questCompletePanel.SetActive(true);
        }


        // =====================================================
        // RESET CANVAS GROUP
        // =====================================================

        if (completeCanvasGroup != null)
        {
            completeCanvasGroup.alpha = 0f;
        }


        // =====================================================
        // RESET TITLE SCALE
        // =====================================================

        if (completeTitle != null)
        {
            completeTitle.localScale = Vector3.zero;
        }


        // =====================================================
        // FADE IN
        // =====================================================

        float timer = 0f;

        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;

            float t =
                timer / fadeInDuration;

            if (completeCanvasGroup != null)
            {
                completeCanvasGroup.alpha =
                    Mathf.Lerp(
                        0f,
                        1f,
                        t
                    );
            }

            yield return null;
        }


        // =====================================================
        // TITLE POP
        // =====================================================

        timer = 0f;

        while (timer < scaleDuration)
        {
            timer += Time.deltaTime;

            float t =
                timer / scaleDuration;

            float scale =
                EaseOutBack(t);

            if (completeTitle != null)
            {
                completeTitle.localScale =
                    Vector3.one * scale;
            }

            yield return null;
        }


        if (completeTitle != null)
        {
            completeTitle.localScale =
                Vector3.one;
        }


        // =====================================================
        // GIỮ CHỮ HOÀN THÀNH
        // =====================================================

        yield return new WaitForSeconds(
            completeTextDuration
        );


        // =====================================================
        // FADE OUT
        // =====================================================

        timer = 0f;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;

            float t =
                timer / fadeOutDuration;

            if (completeCanvasGroup != null)
            {
                completeCanvasGroup.alpha =
                    Mathf.Lerp(
                        1f,
                        0f,
                        t
                    );
            }

            yield return null;
        }


        // =====================================================
        // ẨN COMPLETE PANEL
        // =====================================================

        if (questCompletePanel != null)
        {
            questCompletePanel.SetActive(false);
        }


        // =====================================================
        // HIỆN REWARD PANEL
        // =====================================================

        if (questRewardController == null)
        {
            Debug.LogError(
                "ChestQuestManager: "
                + "QuestRewardController chưa được gán!"
            );

            // Nếu không có reward controller,
            // vẫn cho NPC xuất hiện.
            yield return StartCoroutine(
                SpawnNPCAfterReward()
            );

            yield break;
        }


        Debug.Log(
            "ChestQuestManager: Hiện Reward Panel."
        );


        questRewardController.ShowReward();


        // =====================================================
        // CHỜ CONFIRM REWARD
        // =====================================================

        bool rewardReceived = false;


        void OnRewardReceived()
        {
            rewardReceived = true;

            Debug.Log(
                "ChestQuestManager: OnRewardClaimed được gọi!"
            );
        }


        // Đăng ký Event
        questRewardController.OnRewardClaimed
            += OnRewardReceived;


        Debug.Log(
            "ChestQuestManager: "
            + "Đang chờ Player bấm CONFIRM..."
        );


        // =====================================================
        // WAIT FOR CONFIRM
        // =====================================================

        yield return new WaitUntil(
            () => rewardReceived
        );


        Debug.Log(
            "========================================"
        );

        Debug.Log(
            "REWARD ĐÃ ĐƯỢC NHẬN!"
        );

        Debug.Log(
            "========================================"
        );


        // =====================================================
        // HỦY EVENT
        // =====================================================

        questRewardController.OnRewardClaimed
            -= OnRewardReceived;


        // =====================================================
        // SPAWN NPC
        // =====================================================

        yield return StartCoroutine(
            SpawnNPCAfterReward()
        );
    }


    // =========================================================
    // SPAWN NPC AFTER REWARD
    // =========================================================

    private IEnumerator SpawnNPCAfterReward()
    {
        Debug.Log(
            "ChestQuestManager: "
            + "Chuẩn bị spawn NPC..."
        );


        // -----------------------------------------------------
        // DELAY
        // -----------------------------------------------------

        if (npcSpawnDelay > 0f)
        {
            yield return new WaitForSeconds(
                npcSpawnDelay
            );
        }


        // -----------------------------------------------------
        // KIỂM TRA NPC
        // -----------------------------------------------------

        if (npcObject == null)
        {
            Debug.LogError(
                "========================================"
            );

            Debug.LogError(
                "ChestQuestManager: NPC OBJECT CHƯA ĐƯỢC GÁN!"
            );

            Debug.LogError(
                "Hãy kéo NPC GameObject vào ô "
                + "'Npc Object' trong Inspector."
            );

            Debug.LogError(
                "========================================"
            );

            yield break;
        }


        // -----------------------------------------------------
        // SPAWN NPC
        // -----------------------------------------------------

        npcObject.SetActive(true);


        Debug.Log(
            "========================================"
        );

        Debug.Log(
            "NPC ĐÃ XUẤT HIỆN!"
        );

        Debug.Log(
            "========================================"
        );
    }


    // =========================================================
    // MANUAL SPAWN NPC
    // =========================================================

    public void SpawnNPC()
    {
        if (npcObject == null)
        {
            Debug.LogError(
                "ChestQuestManager: NPC Object chưa được gán!"
            );

            return;
        }

        npcObject.SetActive(true);

        Debug.Log(
            "ChestQuestManager: NPC đã được bật."
        );
    }


    // =========================================================
    // RESET QUEST
    // =========================================================

    public void ResetQuest()
    {
        currentChest = 0;

        questStarted = false;

        questCompleted = false;


        if (questPanel != null)
        {
            questPanel.SetActive(false);
        }


        if (questCompletePanel != null)
        {
            questCompletePanel.SetActive(false);
        }


        if (npcObject != null)
        {
            npcObject.SetActive(false);
        }


        UpdateQuestUI();


        Debug.Log(
            "ChestQuestManager: Quest đã được reset."
        );
    }


    // =========================================================
    // EASE OUT BACK
    // =========================================================

    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;

        float c3 =
            c1 + 1f;


        return 1f
            + c3 * Mathf.Pow(
                t - 1f,
                3f
            )
            + c1 * Mathf.Pow(
                t - 1f,
                2f
            );
    }
}