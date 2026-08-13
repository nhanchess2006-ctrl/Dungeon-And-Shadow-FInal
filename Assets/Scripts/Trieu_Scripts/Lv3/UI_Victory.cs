using System.Collections;
using UnityEngine;

public class UI_FinishScreen : MonoBehaviour
{
    [Header("Cấu hình Boss")]
    [Tooltip("Kéo thả GameObject của Boss trong Scene vào đây")]
    [SerializeField] private GameObject bossGameObject;

    [Tooltip("Đặt là 0 để UI hiện ngay lập tức. Nếu muốn chờ animation chết thì chỉnh số giây tương ứng.")]
    [SerializeField] private float delayBeforeShowUI = 0f;

    [Header("Cấu hình UI Panel")]
    [Tooltip("Kéo thả Panel UI Victory vào đây")]
    [SerializeField] private GameObject victoryPanel;

    private bool isGameEnded = false;
    
    // Biến lưu trữ cho quá trình "Phẫu thuật Component"
    private Entity_Health activeBossHealth;
    private string cachedQuestTargetId = "";
    private Player_QuestManager playerQuestManager;

    private void Start()
    {
        Time.timeScale = 1f;

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        // Lưu trữ Player Quest Manager an toàn
        if (Player.instance != null)
            playerQuestManager = Player.instance.questManager;

        // TIẾN HÀNH "PHẪU THUẬT" COMPONENT NGAY KHI BẮT ĐẦU GAME
        if (bossGameObject != null)
        {
            SwapBossHealthComponent();
        }
    }

    private void SwapBossHealthComponent()
    {
        // 1. Tìm Component Enemy_Health chứa dòng số 35 gây lỗi
        Enemy_Health badHealth = bossGameObject.GetComponent<Enemy_Health>();
        
        // 2. Trích xuất ID Nhiệm vụ từ Component Enemy trước khi xóa máu cũ
        Enemy enemyComponent = bossGameObject.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            cachedQuestTargetId = enemyComponent.questTargetId;
        }

        if (badHealth != null)
        {
            // 3. Tiêu diệt triệt để Component gây lỗi
            Destroy(badHealth);

            // 4. Cấy ghép Component Entity_Health thuần khiết (không chứa code lỗi)
            activeBossHealth = bossGameObject.AddComponent<Entity_Health>();
            Debug.Log("🛡️ [UI_FinishScreen] Đã thay thế Enemy_Health bằng Entity_Health thuần khiết thành công!");
        }
        else
        {
            // Nếu Boss đã dùng Entity_Health sẵn thì lấy luôn
            activeBossHealth = bossGameObject.GetComponent<Entity_Health>();
        }
    }

    private void Update()
    {
        if (isGameEnded)
            return;

        // LIÊN TỤC QUÉT TRẠNG THÁI CHẾT CỦA BOSS DỰA TRÊN COMPONENT MỚI
        bool isBossDead = false;

        // Nếu GameObject bị ẩn/xóa HOẶC Component máu mới báo tín hiệu isDead = true
        if (bossGameObject == null || !bossGameObject.activeInHierarchy)
        {
            isBossDead = true;
        }
        // Tham chiếu từ cách xử lý trong file Scene1BossBattle của bạn
        else if (activeBossHealth != null && activeBossHealth.isDead) 
        {
            isBossDead = true;
        }

        if (isBossDead)
        {
            TriggerVictory();
        }
    }

    public void TriggerVictory()
    {
        if (isGameEnded)
            return;

        isGameEnded = true;

        // 🔥 BÙ ĐẮP LOGIC NHIỆM VỤ: Do ta đã xóa dòng 35 bị lỗi của quái, ta tự tay cộng tiến trình tại đây!
        CompleteBossQuest();

        if (delayBeforeShowUI <= 0f)
        {
            ShowVictoryUI();
        }
        else
        {
            StartCoroutine(VictorySequenceCo());
        }
    }

    private void CompleteBossQuest()
    {
        // Cập nhật biến bossDefeated
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance. wavesCompleted = true;
            Debug.Log("📜 [Quest System] Đã tự động ghi nhận Boss bị tiêu diệt vào hệ thống!");
        }

        // Tự động cộng Quest Progress dựa trên ID đã trích xuất ở hàm Start
        if (playerQuestManager != null && !string.IsNullOrEmpty(cachedQuestTargetId))
        {
            playerQuestManager.AddProgress(cachedQuestTargetId);
            Debug.Log($"📜 [Quest System] Đã cộng tiến trình cho Quest ID: {cachedQuestTargetId}");
        }
    }

    private IEnumerator VictorySequenceCo()
    {
        yield return new WaitForSecondsRealtime(delayBeforeShowUI);
        ShowVictoryUI();
    }

    private void ShowVictoryUI()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        Time.timeScale = 0f; // Dừng thời gian game
    }

    #region Các nút bấm trên UI (UI Buttons)

    public void GoToMainMenuBTN()
    {
        Time.timeScale = 1f;
        GameManager.instance.ChangeScene("MainMenu", RespawnType.NonSpecific);
    }

    #endregion
}