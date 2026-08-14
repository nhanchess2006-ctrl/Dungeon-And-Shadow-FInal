using UnityEngine;

/// <summary>
/// Gắn script này vào Enemy.
///
/// Khi Enemy chết, Enemy sẽ thưởng EXP.
///
/// Ví dụ:
/// Slime  = 10 EXP
/// Goblin = 25 EXP
/// Boss   = 500 EXP
/// </summary>
public class EnemyExpReward : MonoBehaviour
{
    // =========================================================
    // EXP REWARD
    // =========================================================

    [Header("EXP Reward")]

    // Số EXP người chơi nhận được
    // khi tiêu diệt Enemy này.
    //
    // Bạn có thể chỉnh trực tiếp
    // trong Inspector.
    [SerializeField] private int expReward = 10;


    // =========================================================
    // STATE
    // =========================================================

    // Kiểm tra Enemy này đã trao EXP chưa.
    //
    // Biến này rất quan trọng để tránh:
    //
    // 1 con quái chết
    // nhưng bị gọi nhiều lần
    // → cộng EXP nhiều lần.
    private bool hasGivenReward = false;


    // =========================================================
    // PUBLIC FUNCTION
    // =========================================================

    /// <summary>
    /// Hàm này sẽ được gọi khi Enemy chết.
    /// </summary>
    public void GiveExpReward()
    {
        // -----------------------------------------------------
        // KIỂM TRA ĐÃ TRAO THƯỞNG CHƯA
        // -----------------------------------------------------

        if (hasGivenReward)
        {
            return;
        }


        // Đánh dấu đã trao thưởng.
        hasGivenReward = true;


        // -----------------------------------------------------
        // KIỂM TRA PLAYER PROGRESS MANAGER
        // -----------------------------------------------------

        if (PlayerProgressManager.Instance == null)
        {
            Debug.LogError(
                "EnemyExpReward: "
                + "Không tìm thấy PlayerProgressManager!"
            );

            return;
        }


        // -----------------------------------------------------
        // CỘNG EXP
        // -----------------------------------------------------

        PlayerProgressManager.Instance.AddExp(
            expReward
        );


        Debug.Log(
            "EnemyExpReward: "
            + gameObject.name
            + " đã thưởng "
            + expReward
            + " EXP."
        );
    }
}