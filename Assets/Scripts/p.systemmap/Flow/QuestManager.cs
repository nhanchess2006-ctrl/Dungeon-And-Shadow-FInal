using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class QuestManager : MonoBehaviour
{
    // =====================================================
    // SINGLETON
    // =====================================================

    public static QuestManager Instance;


    // =====================================================
    // BOSS ROOM
    // =====================================================

    [Header("Boss Room")]

    [SerializeField]
    private BossRoomSequence bossRoomSequence;


    // =====================================================
    // CAMERA
    // =====================================================

    [Header("Camera")]

    [SerializeField]
    private CinemachineCamera cinemachineCamera;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private Transform unlockAreaTarget;


    // =====================================================
    // AREA UNLOCK
    // =====================================================

    [Header("Area Unlock")]

    [SerializeField]
    private int unlockAreaFlowerCount = 3;

    private bool areaUnlocked = false;

    [SerializeField]
    private AreaBarrier areaBarrier;


    // =====================================================
    // QUEST UI
    // =====================================================

    [Header("Quest UI")]

    [SerializeField]
    private QuestUI questUI;

    [SerializeField]
    private GameObject completePanel;

    [SerializeField]
    private float completeShowTime = 2f;


    // =====================================================
    // FLOWER QUEST
    // =====================================================

    [Header("Flower Quest")]

    public int currentFlower = 0;

    public int targetFlower = 5;


    // =====================================================
    // WAVE 1
    // =====================================================

    [Header("Wave 1")]

    [SerializeField]
    private GameObject[] firstWaveEnemies;


    // =====================================================
    // WAVE 2
    // =====================================================

    [Header("Wave 2")]

    [SerializeField]
    private GameObject[] secondWaveEnemies;


    // =====================================================
    // PORTAL
    // =====================================================

    [Header("Portal")]

    [SerializeField]
    private Object_Portal portal;

    [SerializeField]
    private Transform portalSpawnPoint;


    // =====================================================
    // STATE
    // =====================================================

    // Giữ public để các script cũ có thể kiểm tra.
    public bool wavesCompleted = false;

    private bool battleStarted = false;

    private bool wave1Completed = false;

    private bool wave2Completed = false;

    private bool questCompleted = false;

    private bool doorSpawned = false;

    private bool bossSequenceStarted = false;


    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (completePanel != null)
        {
            completePanel.SetActive(false);
        }
    }


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        // Không tự động bắt đầu Battle.
        // Battle được gọi bởi Battle Trigger.
    }


    // =====================================================
    // INITIALIZE QUEST
    // =====================================================

    public void InitializeQuest()
    {
        // =================================================
        // RESET STATE
        // =================================================

        currentFlower = 0;

        areaUnlocked = false;

        battleStarted = false;

        wave1Completed = false;

        wave2Completed = false;

        wavesCompleted = false;

        questCompleted = false;

        doorSpawned = false;

        bossSequenceStarted = false;


        // =================================================
        // COMPLETE UI
        // =================================================

        if (completePanel != null)
        {
            completePanel.SetActive(false);
        }


        // =================================================
        // QUEST UI
        // =================================================

        if (questUI != null)
        {
            questUI.UpdateProgress(
                currentFlower,
                targetFlower
            );
        }


        // =================================================
        // WAVE 1 OFF
        // =================================================

        SetWaveActive(
            firstWaveEnemies,
            false
        );


        // =================================================
        // WAVE 2 OFF
        // =================================================

        SetWaveActive(
            secondWaveEnemies,
            false
        );


        // =================================================
        // PORTAL OFF
        // =================================================

        if (portal != null)
        {
            portal.gameObject.SetActive(false);
        }


        // =================================================
        // LOG
        // =================================================

        Debug.Log(
            "QuestManager: Quest đã được khởi tạo 0/"
            + targetFlower
        );
    }


    // =====================================================
    // AREA UNLOCK CHECK
    // =====================================================

    public bool IsAreaUnlocked()
    {
        return areaUnlocked;
    }


    // =====================================================
    // COLLECT FLOWER
    // =====================================================

    public void CollectFlower()
    {
        Debug.Log(
            "========== COLLECT FLOWER =========="
        );


        // =================================================
        // ĐÃ ĐỦ HOA
        // =================================================

        if (currentFlower >= targetFlower)
        {
            Debug.Log(
                "Đã đủ "
                + targetFlower
                + " hoa!"
            );

            return;
        }


        // =================================================
        // CỘNG HOA
        // =================================================

        currentFlower++;


        // =================================================
        // UPDATE UI
        // =================================================

        if (questUI != null)
        {
            questUI.UpdateProgress(
                currentFlower,
                targetFlower
            );
        }


        Debug.Log(
            "Flower: "
            + currentFlower
            + "/"
            + targetFlower
        );


        // =================================================
        // ĐỦ 3 HOA
        // =================================================

        if (!areaUnlocked &&
            currentFlower >= unlockAreaFlowerCount)
        {
            areaUnlocked = true;

            Debug.Log(
                "Đã đủ "
                + unlockAreaFlowerCount
                + " hoa!"
            );

            Debug.Log(
                "Khu vực mới đã được mở khóa."
            );

            UnlockRestrictedArea();
        }


        // =================================================
        // ĐỦ 5 HOA
        // =================================================

        if (currentFlower >= targetFlower)
        {
            Debug.Log(
                "ĐÃ ĐỦ HOA."
            );

            // QUAN TRỌNG:
            // Không CompleteQuest ngay.
            //
            // Phải chờ cả Wave 1 + Wave 2.
            TryCompleteQuest();
        }
    }


    // =====================================================
    // TRY COMPLETE QUEST
    // =====================================================

    private void TryCompleteQuest()
    {
        Debug.Log(
            "========== TRY COMPLETE QUEST =========="
        );

        Debug.Log(
            "Hoa: "
            + currentFlower
            + "/"
            + targetFlower
        );

        Debug.Log(
            "Wave 1: "
            + wave1Completed
        );

        Debug.Log(
            "Wave 2: "
            + wave2Completed
        );


        // =================================================
        // CHƯA ĐỦ HOA
        // =================================================

        if (currentFlower < targetFlower)
        {
            Debug.Log(
                "Chưa đủ hoa."
            );

            return;
        }


        // =================================================
        // WAVE CHƯA XONG
        // =================================================

        if (!wavesCompleted)
        {
            Debug.Log(
                "Đã đủ hoa nhưng Wave chưa hoàn thành."
            );

            Debug.Log(
                "Boss Map CHƯA được spawn."
            );

            return;
        }


        // =================================================
        // HOÀN THÀNH
        // =================================================

        CompleteQuest();
    }


    // =====================================================
    // CAMERA FOCUS
    // =====================================================

    private IEnumerator FocusUnlockedArea()
    {
        if (cinemachineCamera == null)
        {
            Debug.LogWarning(
                "QuestManager: Chưa gán Cinemachine Camera!"
            );

            yield break;
        }


        if (unlockAreaTarget == null)
        {
            Debug.LogWarning(
                "QuestManager: Chưa gán Unlock Area Target!"
            );

            yield break;
        }


        if (player == null)
        {
            Debug.LogWarning(
                "QuestManager: Chưa gán Player!"
            );

            yield break;
        }


        cinemachineCamera.Follow =
            unlockAreaTarget;


        Debug.Log(
            "Camera đang di chuyển tới Area!"
        );


        yield return new WaitForSeconds(2f);


        if (areaBarrier != null)
        {
            areaBarrier.ShowGlow();
        }


        yield return new WaitForSeconds(3.5f);


        cinemachineCamera.Follow =
            player;


        Debug.Log(
            "Camera quay lại Player!"
        );
    }


    // =====================================================
    // UNLOCK AREA
    // =====================================================

    private void UnlockRestrictedArea()
    {
        StartCoroutine(
            FocusUnlockedArea()
        );
    }


    // =====================================================
    // START BATTLE
    // =====================================================

    public void StartBattleFromTrigger()
    {
        if (battleStarted)
        {
            Debug.Log(
                "QuestManager: Battle đã bắt đầu rồi."
            );

            return;
        }


        battleStarted = true;


        Debug.Log(
            "================================"
        );

        Debug.Log(
            "PLAYER ĐÃ KÍCH HOẠT BATTLE TRIGGER!"
        );

        Debug.Log(
            "================================"
        );


        StartCoroutine(
            BattleSequence()
        );
    }


    // =====================================================
    // BATTLE SEQUENCE
    // =====================================================

    private IEnumerator BattleSequence()
    {
        // =================================================
        // WAVE 1
        // =================================================

        Debug.Log(
            "========== WAVE 1 BẮT ĐẦU =========="
        );


        SetWaveActive(
            firstWaveEnemies,
            true
        );


        if (!HasActiveEnemies(firstWaveEnemies))
        {
            Debug.LogError(
                "QuestManager: Wave 1 không có Enemy được gán!"
            );

            yield break;
        }


        // =================================================
        // CHỜ WAVE 1 CHẾT HẾT
        // =================================================

        yield return new WaitUntil(
            () => IsWaveDead(
                firstWaveEnemies
            )
        );


        wave1Completed = true;


        Debug.Log(
            "========== WAVE 1 ĐÃ HOÀN THÀNH =========="
        );


        // =================================================
        // KIỂM TRA
        // =================================================

        Debug.Log(
            "Wave 1 Completed = TRUE"
        );


        // =================================================
        // WAVE 2
        // =================================================

        Debug.Log(
            "========== WAVE 2 BẮT ĐẦU =========="
        );


        SetWaveActive(
            secondWaveEnemies,
            true
        );


        if (!HasActiveEnemies(secondWaveEnemies))
        {
            Debug.LogError(
                "QuestManager: Wave 2 không có Enemy được gán!"
            );

            yield break;
        }


        // =================================================
        // CHỜ WAVE 2 CHẾT HẾT
        // =================================================

        yield return new WaitUntil(
            () => IsWaveDead(
                secondWaveEnemies
            )
        );


        wave2Completed = true;


        Debug.Log(
            "========== WAVE 2 ĐÃ HOÀN THÀNH =========="
        );


        // =================================================
        // TẤT CẢ WAVE HOÀN THÀNH
        // =================================================

        wavesCompleted = true;


        Debug.Log(
            "================================"
        );

        Debug.Log(
            "ĐÃ HOÀN THÀNH TOÀN BỘ WAVE!"
        );

        Debug.Log(
            "Wave 1 = "
            + wave1Completed
        );

        Debug.Log(
            "Wave 2 = "
            + wave2Completed
        );

        Debug.Log(
            "wavesCompleted = TRUE"
        );

        Debug.Log(
            "================================"
        );


        // =================================================
        // QUAN TRỌNG
        // =================================================
        //
        // Nếu Player đã nhặt đủ 5 hoa trước khi Wave 2 chết,
        // thì lúc này mới cho CompleteQuest.
        //

        TryCompleteQuest();
    }


    // =====================================================
    // SET WAVE ACTIVE
    // =====================================================

    private void SetWaveActive(
        GameObject[] wave,
        bool active
    )
    {
        if (wave == null)
            return;


        foreach (GameObject enemy in wave)
        {
            if (enemy != null)
            {
                enemy.SetActive(active);
            }
        }
    }


    // =====================================================
    // HAS ACTIVE ENEMIES
    // =====================================================

    private bool HasActiveEnemies(
        GameObject[] wave
    )
    {
        if (wave == null ||
            wave.Length == 0)
        {
            return false;
        }


        foreach (GameObject enemy in wave)
        {
            if (enemy != null)
            {
                return true;
            }
        }


        return false;
    }


    // =====================================================
    // IS WAVE DEAD
    // =====================================================

    private bool IsWaveDead(
        GameObject[] wave
    )
    {
        if (wave == null ||
            wave.Length == 0)
        {
            return true;
        }


        foreach (GameObject enemy in wave)
        {
            if (enemy != null &&
                enemy.activeInHierarchy)
            {
                return false;
            }
        }


        return true;
    }


    // =====================================================
    // COMPLETE QUEST
    // =====================================================

    private void CompleteQuest()
    {
        // =================================================
        // TRÁNH CHẠY NHIỀU LẦN
        // =================================================

        if (questCompleted)
        {
            return;
        }


        // =================================================
        // KIỂM TRA HOA
        // =================================================

        if (currentFlower < targetFlower)
        {
            Debug.LogWarning(
                "Không thể CompleteQuest: "
                + currentFlower
                + "/"
                + targetFlower
            );

            return;
        }


        // =================================================
        // KIỂM TRA WAVE
        // =================================================

        if (!wavesCompleted)
        {
            Debug.LogWarning(
                "Không thể CompleteQuest: "
                + "Wave chưa hoàn thành!"
            );

            return;
        }


        // =================================================
        // ĐÁNH DẤU
        // =================================================

        questCompleted = true;


        Debug.Log(
            "================================"
        );

        Debug.Log(
            "NHIỆM VỤ HOA ĐÃ HOÀN THÀNH!"
        );

        Debug.Log(
            "HOA: "
            + currentFlower
            + "/"
            + targetFlower
        );

        Debug.Log(
            "WAVE 1: "
            + wave1Completed
        );

        Debug.Log(
            "WAVE 2: "
            + wave2Completed
        );

        Debug.Log(
            "================================"
        );


        // =================================================
        // COMPLETE UI
        // =================================================

        if (completePanel != null)
        {
            completePanel.SetActive(true);

            CancelInvoke(
                nameof(HideCompletePanel)
            );

            Invoke(
                nameof(HideCompletePanel),
                completeShowTime
            );
        }


        // =================================================
        // BOSS ROOM
        // =================================================

        if (bossRoomSequence != null)
        {
            Debug.Log(
                "QuestManager: "
                + "Bắt đầu Boss Room Sequence."
            );


            bossRoomSequence
                .StartBossRoomSequence();
        }
        else
        {
            Debug.LogWarning(
                "QuestManager: "
                + "Chưa gán BossRoomSequence!"
            );
        }
    }


    // =====================================================
    // PORTAL
    // =====================================================

    private void OpenPortal()
    {
        // =================================================
        // ĐÃ MỞ
        // =================================================

        if (doorSpawned)
        {
            return;
        }


        // =================================================
        // KIỂM TRA PORTAL
        // =================================================

        if (portal == null)
        {
            Debug.LogWarning(
                "QuestManager: "
                + "Chưa gán Portal!"
            );

            return;
        }


        // =================================================
        // KIỂM TRA SPAWN POINT
        // =================================================

        if (portalSpawnPoint == null)
        {
            Debug.LogWarning(
                "QuestManager: "
                + "Chưa gán Portal Spawn Point!"
            );

            return;
        }


        // =================================================
        // ĐẶT VỊ TRÍ
        // =================================================

        portal.transform.position =
            portalSpawnPoint.position;


        // =================================================
        // BẬT PORTAL
        // =================================================

        portal.gameObject.SetActive(true);


        // =================================================
        // PORTAL LOGIC
        // =================================================

        portal.ActivatePortal(
            portalSpawnPoint.position
        );


        // =================================================
        // CHỈ ĐÁNH DẤU SAU KHI MỞ THÀNH CÔNG
        // =================================================

        doorSpawned = true;


        Debug.Log(
            "========== PORTAL ĐÃ MỞ =========="
        );
    }


    // =====================================================
    // ẨN COMPLETE PANEL
    // =====================================================

    private void HideCompletePanel()
    {
        if (completePanel != null)
        {
            completePanel.SetActive(false);
        }
    }


    // =====================================================
    // GIỮ COMPATIBILITY VỚI ENEMY HEALTH
    // =====================================================

    public void NotifyEnemyDeath(
        Enemy_Health enemy
    )
    {
        // Không cần xử lý.
        //
        // BattleSequence tự kiểm tra
        // activeInHierarchy của Enemy.
    }


    // =====================================================
    // PUBLIC CHECK
    // =====================================================

    public bool IsQuestCompleted()
    {
        return questCompleted;
    }


    public bool IsFlowerCompleted()
    {
        return currentFlower >= targetFlower;
    }


    public bool IsBattleStarted()
    {
        return battleStarted;
    }


    public bool IsWavesCompleted()
    {
        return wavesCompleted;
    }


    public int GetCurrentFlower()
    {
        return currentFlower;
    }


    public int GetTargetFlower()
    {
        return targetFlower;
    }


    public bool IsPortalOpened()
    {
        return doorSpawned;
    }
}