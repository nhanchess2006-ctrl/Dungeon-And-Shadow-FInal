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

    // Số lượng rương cần thu thập để hoàn thành nhiệm vụ.
    [SerializeField] private int targetChest = 5;

    // Số rương hiện tại người chơi đã thu thập.
    private int currentChest = 0;

    // Kiểm tra nhiệm vụ đã bắt đầu chưa.
    private bool questStarted = false;

    // Kiểm tra nhiệm vụ đã hoàn thành chưa.
    private bool questCompleted = false;


    // =========================================================
    // QUEST UI
    // =========================================================

    [Header("Quest UI")]

    // Panel hiển thị tiến độ nhiệm vụ.
    // Ví dụ:
    // Lụm rương 3/5
    [SerializeField] private GameObject questPanel;

    // Text hiển thị số lượng rương.
    [SerializeField] private TMP_Text questText;


    // =========================================================
    // QUEST COMPLETE UI
    // =========================================================

    [Header("Quest Complete UI")]

    // Panel hiển thị chữ "HOÀN THÀNH".
    [SerializeField] private GameObject questCompletePanel;

    // CanvasGroup dùng để Fade In / Fade Out.
    [SerializeField] private CanvasGroup completeCanvasGroup;

    // Text hoặc UI chữ hoàn thành.
    // Dùng để tạo hiệu ứng phóng to.
    [SerializeField] private RectTransform completeTitle;

    // Thời gian Fade In.
    [SerializeField] private float fadeInDuration = 0.3f;

    // Thời gian hiệu ứng phóng to.
    [SerializeField] private float scaleDuration = 0.45f;

    // Thời gian giữ chữ HOÀN THÀNH.
    [SerializeField] private float completeTextDuration = 1.5f;

    // Thời gian Fade Out.
    [SerializeField] private float fadeOutDuration = 0.4f;


    // =========================================================
    // NPC
    // =========================================================

    [Header("NPC After Quest")]

    // NPC sẽ xuất hiện sau khi người chơi
    // hoàn thành quest VÀ nhận phần thưởng.
    [SerializeField] private NPCTrigger npcTrigger;

    // Thời gian chờ trước khi NPC xuất hiện.
    [SerializeField] private float npcSpawnDelay = 1f;


    // =========================================================
    // QUEST REWARD
    // =========================================================

    [Header("Quest Reward")]

    // Script quản lý Reward Panel.
    //
    // Khi quest hoàn thành:
    // questRewardController.ShowReward()
    //
    // Khi người chơi bấm Xác nhận:
    // QuestRewardController sẽ cộng EXP
    // và gọi Event OnRewardClaimed.
    [SerializeField]
    private QuestRewardController questRewardController;


    // =========================================================
    // PUBLIC PROPERTY
    // =========================================================

    // Cho phép script khác đọc số rương hiện tại.
    public int CurrentChest => currentChest;

    // Cho phép script khác đọc số rương cần thu thập.
    public int TargetChest => targetChest;

    // Kiểm tra quest đã hoàn thành chưa.
    public bool IsQuestCompleted => questCompleted;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        // Kiểm tra Singleton.
        //
        // Nếu đã có ChestQuestManager khác
        // thì xóa object bị trùng.
        if (Instance != null &&
            Instance != this)
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
        // Ẩn UI nhiệm vụ lúc bắt đầu.
        if (questPanel != null)
        {
            questPanel.SetActive(false);
        }


        // Ẩn UI "HOÀN THÀNH" lúc bắt đầu.
        if (questCompletePanel != null)
        {
            questCompletePanel.SetActive(false);
        }


        // Đảm bảo UI tiến độ đúng.
        UpdateQuestUI();
    }


    // =========================================================
    // START QUEST
    // =========================================================

    public void StartChestQuest()
    {
        // Nếu quest đã bắt đầu thì không bắt đầu lại.
        if (questStarted)
            return;


        // Đánh dấu quest đã bắt đầu.
        questStarted = true;


        // Reset số rương.
        currentChest = 0;


        // Đánh dấu quest chưa hoàn thành.
        questCompleted = false;


        // Hiện UI nhiệm vụ.
        if (questPanel != null)
        {
            questPanel.SetActive(true);
        }


        // Cập nhật UI.
        UpdateQuestUI();


        Debug.Log(
            "Bắt đầu nhiệm vụ: Lụm rương 0/"
            + targetChest
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
                "Quest chưa được bắt đầu!"
            );

            return;
        }


        // -----------------------------------------------------
        // QUEST ĐÃ HOÀN THÀNH
        // -----------------------------------------------------

        if (questCompleted)
        {
            Debug.LogWarning(
                "Quest đã hoàn thành!"
            );

            return;
        }


        // -----------------------------------------------------
        // CỘNG RƯƠNG
        // -----------------------------------------------------

        currentChest++;


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
        // Nếu chưa gán Text thì dừng.
        if (questText == null)
            return;


        // Cập nhật tiến độ.
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
        // Tránh chạy CompleteQuest nhiều lần.
        if (questCompleted)
            return;


        // Đánh dấu nhiệm vụ hoàn thành.
        questCompleted = true;


        Debug.Log(
            "================================"
        );

        Debug.Log(
            "NHIỆM VỤ ĐÃ HOÀN THÀNH!"
        );

        Debug.Log(
            "================================"
        );



        // -----------------------------------------------------
        // ẨN UI TIẾN ĐỘ
        // -----------------------------------------------------

        if (questPanel != null)
        {
            questPanel.SetActive(false);
        }


        // -----------------------------------------------------
        // TÌM PLAYER
        // -----------------------------------------------------

        GameObject player =
            GameObject.FindGameObjectWithTag(
                "Player"
            );


        if (player == null)
        {
            Debug.LogError(
                "ChestQuestManager: Không tìm thấy Player!"
            );

            return;
        }


        // -----------------------------------------------------
        // BẮT ĐẦU CHUỖI HOÀN THÀNH QUEST
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
        // RESET UI
        // =====================================================

        // Hiện Complete Panel
        if (questCompletePanel != null)
        {
            questCompletePanel.SetActive(true);
        }


        // Reset alpha về 0
        if (completeCanvasGroup != null)
        {
            completeCanvasGroup.alpha = 0f;
        }


        // Reset Scale về 0
        if (completeTitle != null)
        {
            completeTitle.localScale =
                Vector3.zero;
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


        // Đảm bảo Scale cuối cùng = 1
        if (completeTitle != null)
        {
            completeTitle.localScale =
                Vector3.one;
        }


        // =====================================================
        // GIỮ CHỮ "HOÀN THÀNH"
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

        if (questRewardController != null)
        {
            questRewardController.ShowReward();

            Debug.Log(
                "ChestQuestManager: "
                + "Đang chờ người chơi nhận phần thưởng."
            );


            // =================================================
            // CHỜ NGƯỜI CHƠI NHẬN REWARD
            // =================================================

            bool rewardReceived = false;


            void OnRewardReceived()
            {
                rewardReceived = true;
            }


            // Đăng ký Event
            questRewardController.OnRewardClaimed
                += OnRewardReceived;


            // Chờ người chơi bấm Confirm
            yield return new WaitUntil(
                () => rewardReceived
            );


            Debug.Log(
                "========== REWARD ĐÃ ĐƯỢC NHẬN =========="
            );


            // Hủy đăng ký Event
            questRewardController.OnRewardClaimed
                -= OnRewardReceived;
        }
        else
        {
            Debug.LogWarning(
                "ChestQuestManager: "
                + "QuestRewardController chưa được gán!"
            );
        }


        // =====================================================
        // KẾT THÚC
        // =====================================================
        //
        // QUAN TRỌNG:
        //
        // KHÔNG SPAWN NPC Ở ĐÂY.
        //
        // NPC chỉ được Spawn khi Player
        // nhặt Boss Map.
        //
        // Luồng mới:
        //
        // 5/5 Hoa
        //      ↓
        // Boss Map xuất hiện
        //      ↓
        // Player nhặt Boss Map
        //      ↓
        // BossMapPickup
        //      ↓
        // BossRoomSequence
        //      ↓
        // NPC xuất hiện
        //
        // =====================================================

        Debug.Log(
            "ChestQuestManager: "
            + "Quest Reward đã hoàn tất. "
            + "Chờ Player nhặt Boss Map."
        );
    }


    // =========================================================
    // EASE OUT BACK
    // =========================================================

    private float EaseOutBack(float t)
    {
        // Giá trị dùng cho hiệu ứng Overshoot.
        float c1 = 1.70158f;

        float c3 =
            c1 + 1f;


        // Tạo hiệu ứng:
        //
        // 
        // ↓
        // phóng lớn hơi quá 1 chút
        // ↓
        // trở về 1
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