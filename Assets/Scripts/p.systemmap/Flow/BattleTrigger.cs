using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Không phải Player thì bỏ qua ngay.
        if (!other.CompareTag("Player"))
            return;

        Debug.Log(
            "BattleTrigger nhận Player."
        );

        // Đã kích hoạt rồi thì không kích hoạt lại.
        if (hasTriggered)
        {
            Debug.Log(
                "BattleTrigger: Đã kích hoạt trước đó, bỏ qua."
            );

            return;
        }

        // Khóa ngay lập tức.
        hasTriggered = true;

        Debug.Log(
            "========== BATTLE TRIGGER ACTIVATED =========="
        );

        if (QuestManager.Instance == null)
        {
            Debug.LogError(
                "BattleTrigger: Không tìm thấy QuestManager!"
            );

            return;
        }

        QuestManager.Instance.StartBattleFromTrigger();
    }
}