using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(
            "BattleTrigger nhận va chạm với: " +
            other.name +
            " | Tag: " +
            other.tag
        );

        if (triggered)
            return;

        if (!other.CompareTag("Player"))
        {
            Debug.Log("Đối tượng chạm Trigger không phải Player!");
            return;
        }

        triggered = true;

        Debug.Log("========== PLAYER ĐÃ CHẠM BATTLE TRIGGER ==========");

        if (QuestManager.Instance == null)
        {
            Debug.LogError("BattleTrigger: QuestManager.Instance đang NULL!");
            return;
        }

        Debug.Log("Đã tìm thấy QuestManager!");

        QuestManager.Instance.StartBattleFromTrigger();
    }
}