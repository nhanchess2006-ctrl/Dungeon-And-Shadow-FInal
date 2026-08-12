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
    [SerializeField] private NPCTrigger npcTrigger;

    [SerializeField] private float npcSpawnDelay = 1f;


    // =========================================================
    // PUBLIC PROPERTY
    // =========================================================

    public int CurrentChest => currentChest;

    public int TargetChest => targetChest;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
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
        // Ẩn quest UI
        if (questPanel != null)
        {
            questPanel.SetActive(false);
        }

        // Ẩn complete UI
        if (questCompletePanel != null)
        {
            questCompletePanel.SetActive(false);
        }

        UpdateQuestUI();
    }


    // =========================================================
    // START QUEST
    // =========================================================

    public void StartChestQuest()
    {
        if (questStarted)
            return;

        questStarted = true;

        currentChest = 0;

        questCompleted = false;


        // Hiện UI nhiệm vụ
        if (questPanel != null)
        {
            questPanel.SetActive(true);
        }


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
        if (questText == null)
            return;

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
            return;


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
        // ẨN UI 0/5
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
        // CHẠY HIỆU ỨNG HOÀN THÀNH
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

        if (questCompletePanel != null)
        {
            questCompletePanel.SetActive(true);
        }


        if (completeCanvasGroup != null)
        {
            completeCanvasGroup.alpha = 0f;
        }


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
                    Mathf.Lerp(0f, 1f, t);
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
        // GIỮ CHỮ
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
                    Mathf.Lerp(1f, 0f, t);
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
        // DELAY TRƯỚC NPC
        // =====================================================

        yield return new WaitForSeconds(
            npcSpawnDelay
        );


        // =====================================================
        // NPC XUẤT HIỆN
        // =====================================================

        if (npcTrigger != null)
        {
            npcTrigger.SpawnNPCInFrontOfPlayer(
                player
            );
        }
        else
        {
            Debug.LogError(
                "ChestQuestManager: NPCTrigger chưa được gán!"
            );
        }
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