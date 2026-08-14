using System;
using UnityEngine;

/// <summary>
/// Quản lý toàn bộ Level và EXP của người chơi.
/// Object chứa script này sẽ tồn tại khi chuyển Scene.
/// </summary>
public class PlayerProgressManager : MonoBehaviour
{
    // Singleton:
    // Giúp chúng ta có thể gọi hệ thống này ở bất kỳ script nào.
    //
    // Ví dụ:
    // PlayerProgressManager.Instance.AddExp(100);
    public static PlayerProgressManager Instance { get; private set; }


    [Header("Player Progress")]

    // Level hiện tại của người chơi.
    [SerializeField] private int currentLevel = 1;

    // EXP hiện tại trong Level.
    [SerializeField] private int currentExp = 0;

    // EXP cần để lên Level tiếp theo.
    //
    // Hiện tại chúng ta để số cố định để test.
    // Sau này sẽ chuyển sang ScriptableObject Database.
    [SerializeField] private int requiredExp = 100;


    // Các property chỉ cho phép script bên ngoài ĐỌC dữ liệu.
    // Script khác không thể tự ý:
    // currentLevel = 999;
    //
    // Muốn thay đổi EXP phải thông qua AddExp().
    public int CurrentLevel => currentLevel;

    public int CurrentExp => currentExp;

    public int RequiredExp => requiredExp;


    // Event được gọi khi EXP thay đổi.
    //
    // int thứ nhất = Current EXP
    // int thứ hai = Required EXP
    public event Action<int, int> OnExpChanged;


    // Event được gọi khi người chơi lên cấp.
    //
    // int = Level mới.
    public event Action<int> OnLevelUp;


    private void Awake()
    {
        // Nếu chưa có Instance nào tồn tại
        if (Instance == null)
        {
            // Gán object hiện tại làm Instance chính.
            Instance = this;

            // Không destroy object này khi chuyển Scene.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Nếu đã có một PlayerProgressManager khác tồn tại
            // thì destroy object bị trùng.
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Hàm dùng để cộng EXP cho người chơi.
    /// Ví dụ:
    /// AddExp(50);
    /// </summary>
    public void AddExp(int amount)
    {
        // Không cho phép cộng EXP <= 0.
        if (amount <= 0)
            return;

        // Cộng EXP.
        currentExp += amount;

        // Kiểm tra xem EXP có đủ để lên cấp không.
        CheckLevelUp();

        // Báo cho UI biết EXP đã thay đổi.
        OnExpChanged?.Invoke(currentExp, requiredExp);
    }


    /// <summary>
    /// Kiểm tra Level Up.
    /// </summary>
    private void CheckLevelUp()
    {
        // Dùng while thay vì if.
        //
        // Vì sau này người chơi có thể nhận rất nhiều EXP.
        //
        // Ví dụ:
        // Level 1 cần 100 EXP
        // Player đang có 90 EXP
        // Nhận 250 EXP
        //
        // Người chơi có thể lên nhiều cấp liên tiếp.
        while (currentExp >= requiredExp)
        {
            // Trừ EXP đã dùng để lên cấp.
            currentExp -= requiredExp;

            // Tăng Level.
            currentLevel++;

            // Tạm thời mỗi Level tăng thêm 50 EXP yêu cầu.
            //
            // Ví dụ:
            // Level 1: 100
            // Level 2: 150
            // Level 3: 200
            requiredExp += 50;

            // Gửi sự kiện Level Up.
            OnLevelUp?.Invoke(currentLevel);
        }
    }


    /// <summary>
    /// Hàm dùng để debug / test.
    /// Sau này có thể bỏ.
    /// </summary>
    [ContextMenu("Test Add 50 EXP")]
    private void TestAddExp()
    {
        AddExp(50);
    }
}