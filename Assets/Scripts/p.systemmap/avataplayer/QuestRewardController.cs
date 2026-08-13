using TMPro;
using UnityEngine;
using System;
/// <summary>
/// Quản lý phần thưởng của một Quest.
///
/// Script này hoàn toàn độc lập với:
/// - ChestQuestManager
/// - RewardChest
/// - PlayerProgressManager
///
/// Nó chỉ nhận lệnh:
///
/// ShowReward();
///
/// Sau đó hiển thị Reward Panel.
///
/// EXP chỉ được cộng khi người chơi
/// bấm nút Xác nhận.
/// </summary>
public class QuestRewardController : MonoBehaviour
{
    public event Action OnRewardClaimed;
    // =========================================================
    // REWARD SETTINGS
    // =========================================================


    [Header("EXP Reward")]

    // Số EXP người chơi nhận khi xác nhận.
    [SerializeField]
    private int expReward = 100;


    // =========================================================
    // REWARD UI
    // =========================================================

    [Header("Reward Panel")]

    // Panel chính hiển thị phần thưởng.
    [SerializeField]
    private GameObject rewardPanel;


    // Text hiển thị số EXP.
    //
    // Ví dụ:
    //
    // +100 EXP
    [SerializeField]
    private TMP_Text expRewardText;


    // =========================================================
    // STATE
    // =========================================================

    // Kiểm tra phần thưởng đã được nhận chưa.
    //
    // Mục đích:
    // Người chơi không thể nhận EXP nhiều lần
    // bằng cách spam nút Confirm.
    private bool rewardClaimed = false;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        // Khi bắt đầu Scene,
        // đảm bảo Reward Panel đang tắt.
        if (rewardPanel != null)
        {
            rewardPanel.SetActive(false);
        }
    }


    // =========================================================
    // SHOW REWARD
    // =========================================================

    /// <summary>
    /// Hiển thị Reward Panel.
    ///
    /// Hàm này sẽ được gọi từ ChestQuestManager
    /// sau khi chữ "Hoàn thành" biến mất.
    /// </summary>
    public void ShowReward()
    {
        // Nếu đã nhận thưởng rồi
        // thì không hiện lại.
        if (rewardClaimed)
        {
            Debug.Log(
                "QuestRewardController: "
                + "Phần thưởng đã được nhận."
            );

            return;
        }


        // Kiểm tra Panel.
        if (rewardPanel == null)
        {
            Debug.LogError(
                "QuestRewardController: "
                + "Reward Panel chưa được gán!"
            );

            return;
        }


        // Hiển thị số EXP.
        if (expRewardText != null)
        {
            expRewardText.text =
                "+" + expReward + " EXP";
        }


        // Hiện Panel.
        rewardPanel.SetActive(true);


        Debug.Log(
            "QuestRewardController: "
            + "Hiện phần thưởng +"
            + expReward
            + " EXP"
        );
    }


    // =========================================================
    // CLAIM REWARD
    // =========================================================

    /// <summary>
    /// Hàm này được gọi khi người chơi
    /// bấm nút Xác nhận.
    /// </summary>
    public void ClaimReward()
{
    // Nếu đã nhận rồi
    if (rewardClaimed)
        return;


    // Kiểm tra PlayerProgressManager
    if (PlayerProgressManager.Instance == null)
    {
        Debug.LogError(
            "Không tìm thấy PlayerProgressManager!"
        );

        return;
    }


    // =====================================================
    // CỘNG EXP
    // =====================================================

    PlayerProgressManager.Instance.AddExp(
        expReward
    );


    // Đánh dấu đã nhận thưởng
    rewardClaimed = true;


    // =====================================================
    // ĐÓNG PANEL
    // =====================================================

    if (rewardPanel != null)
    {
        rewardPanel.SetActive(false);
    }


    // =====================================================
    // THÔNG BÁO ĐÃ NHẬN REWARD
    // =====================================================

    OnRewardClaimed?.Invoke();


    Debug.Log(
        "QuestRewardController: "
        + "Đã nhận "
        + expReward
        + " EXP."
    );
}
}