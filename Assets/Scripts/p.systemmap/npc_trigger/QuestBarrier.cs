using UnityEngine;

public class QuestBarrier : MonoBehaviour
{
    [Header("Barrier")]
    [SerializeField] private Collider2D barrierCollider;

    [Header("Optional Visual")]
    [SerializeField] private GameObject barrierVisual;

    private bool barrierOpened = false;

    private void Start()
    {
        // Nếu chưa kéo Collider vào Inspector
        if (barrierCollider == null)
        {
            barrierCollider = GetComponent<Collider2D>();
        }

        // Khi bắt đầu game -> vách vẫn chặn
        SetBarrier(true);
    }

    private void Update()
    {
        // Không tìm thấy QuestManager
        if (ChestQuestManager.Instance == null)
            return;

        // Đã hoàn thành nhiệm vụ
        if (ChestQuestManager.Instance.IsQuestCompleted)
        {
            OpenBarrier();
        }
    }

    private void OpenBarrier()
    {
        if (barrierOpened)
            return;

        barrierOpened = true;

        Debug.Log("🚧 Đã đủ rương - mở vách ngăn!");

        // Tắt va chạm
        if (barrierCollider != null)
        {
            barrierCollider.enabled = false;
        }

        // Ẩn hình ảnh vách
        if (barrierVisual != null)
        {
            barrierVisual.SetActive(false);
        }
    }

    private void SetBarrier(bool blocked)
    {
        if (barrierCollider != null)
        {
            barrierCollider.enabled = blocked;
        }

        if (barrierVisual != null)
        {
            barrierVisual.SetActive(blocked);
        }
    }
}