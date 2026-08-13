using UnityEngine;

/// <summary>
/// Script chỉ dùng để test EXP.
/// Sau khi hệ thống hoàn chỉnh có thể xóa.
/// </summary>
public class PlayerProgressTester : MonoBehaviour
{
    private void Update()
    {
        // Khi nhấn phím E
        if (Input.GetKeyDown(KeyCode.K))
        {
            // Kiểm tra PlayerProgressManager có tồn tại không.
            if (PlayerProgressManager.Instance != null)
            {
                // Cộng 50 EXP.
                PlayerProgressManager.Instance.AddExp(50);

                Debug.Log(
                    "Current Level: "
                    + PlayerProgressManager.Instance.CurrentLevel
                    + " | Current EXP: "
                    + PlayerProgressManager.Instance.CurrentExp
                    + " / "
                    + PlayerProgressManager.Instance.RequiredExp
                );
            }
        }
    }
}