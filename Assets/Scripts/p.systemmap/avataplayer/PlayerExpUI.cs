using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Chỉ chịu trách nhiệm hiển thị EXP và Level lên UI.
/// Script này KHÔNG lưu dữ liệu.
/// Dữ liệu thật nằm trong PlayerProgressManager.
/// </summary>
public class PlayerExpUI : MonoBehaviour
{
    [Header("UI References")]

    // Slider hiển thị thanh EXP.
    [SerializeField] private Slider expSlider;

    // Text hiển thị Level.
    [SerializeField] private TextMeshProUGUI levelText;


    [Header("Animation Settings")]

    // Thời gian thanh EXP chạy đến giá trị mới.
    [SerializeField] private float expAnimationDuration = 0.5f;


    // Lưu Coroutine đang chạy.
    //
    // Mục đích:
    // Nếu EXP mới đến khi animation cũ chưa xong,
    // ta có thể dừng animation cũ trước.
    private Coroutine expAnimationCoroutine;


    private void Start()
    {
        // Kiểm tra Manager đã tồn tại chưa.
        if (PlayerProgressManager.Instance == null)
        {
            Debug.LogError(
                "Không tìm thấy PlayerProgressManager!"
            );

            return;
        }


        // Đăng ký Event.
        //
        // Khi EXP thay đổi,
        // hàm HandleExpChanged sẽ được gọi.
        PlayerProgressManager.Instance.OnExpChanged
            += HandleExpChanged;


        // Khi Level thay đổi,
        // hàm HandleLevelUp sẽ được gọi.
        PlayerProgressManager.Instance.OnLevelUp
            += HandleLevelUp;


        // Cập nhật UI ngay khi Scene được load.
        RefreshUIImmediately();
    }


    private void OnDestroy()
    {
        // Hủy đăng ký Event.
        //
        // Điều này rất quan trọng vì UI có thể bị Destroy
        // khi chuyển Scene nhưng PlayerProgressManager vẫn tồn tại.
        if (PlayerProgressManager.Instance != null)
        {
            PlayerProgressManager.Instance.OnExpChanged
                -= HandleExpChanged;

            PlayerProgressManager.Instance.OnLevelUp
                -= HandleLevelUp;
        }
    }


    /// <summary>
    /// Cập nhật UI ngay lập tức.
    /// Dùng khi Scene vừa load.
    /// </summary>
    private void RefreshUIImmediately()
    {
        int currentExp =
            PlayerProgressManager.Instance.CurrentExp;

        int requiredExp =
            PlayerProgressManager.Instance.RequiredExp;

        int currentLevel =
            PlayerProgressManager.Instance.CurrentLevel;


        // Cập nhật Level Text.
        levelText.text = "" + currentLevel;


        // Chuyển EXP thành tỷ lệ 0 -> 1.
        float expPercent =
            (float)currentExp / requiredExp;


        // Cập nhật Slider.
        expSlider.value = expPercent;
    }


    /// <summary>
    /// Được gọi khi EXP thay đổi.
    /// </summary>
    private void HandleExpChanged(
        int currentExp,
        int requiredExp)
    {
        // Nếu đang có animation cũ chạy
        if (expAnimationCoroutine != null)
        {
            // Dừng animation cũ.
            StopCoroutine(expAnimationCoroutine);
        }


        // Bắt đầu animation mới.
        expAnimationCoroutine =
            StartCoroutine(
                AnimateExp(
                    currentExp,
                    requiredExp
                )
            );
    }


    /// <summary>
    /// Được gọi khi Player Level Up.
    /// </summary>
    private void HandleLevelUp(int newLevel)
    {
        // Cập nhật Level Text.
        levelText.text = "" + newLevel;
    }


    /// <summary>
    /// Animation thanh EXP chạy từ giá trị hiện tại
    /// đến giá trị EXP mới.
    /// </summary>
    private IEnumerator AnimateExp(
        int targetExp,
        int requiredExp)
    {
        // Giá trị hiện tại của Slider.
        float startValue =
            expSlider.value;


        // Giá trị Slider cần chạy tới.
        float targetValue =
            (float)targetExp / requiredExp;


        // Thời gian đã chạy.
        float elapsedTime = 0f;


        // Chạy animation.
        while (elapsedTime < expAnimationDuration)
        {
            // Tăng thời gian.
            elapsedTime += Time.deltaTime;


            // Tính tỷ lệ 0 -> 1.
            float progress =
                elapsedTime / expAnimationDuration;


            // Di chuyển Slider từ start đến target.
            expSlider.value =
                Mathf.Lerp(
                    startValue,
                    targetValue,
                    progress
                );


            // Chờ frame tiếp theo.
            yield return null;
        }


        // Đảm bảo kết quả cuối cùng chính xác.
        expSlider.value = targetValue;

        // Coroutine đã hoàn thành.
        expAnimationCoroutine = null;
    }
}