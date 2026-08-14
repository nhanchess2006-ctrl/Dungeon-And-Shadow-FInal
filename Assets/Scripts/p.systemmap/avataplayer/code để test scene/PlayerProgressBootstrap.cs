using UnityEngine;

/// <summary>
/// Tự động tạo PlayerProgressManager
/// khi bắt đầu game.
///
/// Mục đích:
/// - Có thể Play trực tiếp ở bất kỳ Scene nào.
/// - Không cần đặt PlayerProgressManager
///   thủ công trong từng Scene.
/// - Không tạo Manager trùng lặp.
/// </summary>
public static class PlayerProgressBootstrap
{
    // Hàm này chạy trước khi Scene được load.
    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.BeforeSceneLoad
    )]
    private static void CreatePlayerProgressManager()
    {
        // Nếu PlayerProgressManager đã tồn tại
        // thì không tạo thêm.
        if (PlayerProgressManager.Instance != null)
        {
            return;
        }


        // Tạo GameObject mới.
        GameObject managerObject =
            new GameObject(
                "PlayerProgressManager"
            );


        // Gắn script PlayerProgressManager vào.
        managerObject.AddComponent<
            PlayerProgressManager
        >();


        Debug.Log(
            "PlayerProgressBootstrap: "
            + "Đã tự tạo PlayerProgressManager."
        );
    }
}