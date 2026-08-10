using UnityEngine;

public class KillZoneQuest : MonoBehaviour
{
    [Header("Quest Condition")]
    [SerializeField] private int requiredFlower = 3;

    [Header("Kill")]
    [SerializeField] private float killDamage = 999999f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Khu vực đã được mở
        if (QuestManager.Instance.IsAreaUnlocked())
        {
            Debug.Log("Khu vực đã được mở. Người chơi được phép đi qua.");
            return;
        }

        // Chưa đủ điều kiện
        Debug.Log(
            "Chưa thu thập đủ " +
            requiredFlower +
            " bông hoa!"
        );

        Entity_Health health =
            other.GetComponent<Entity_Health>();

        if (health != null)
        {
            health.TakeDamage(
                killDamage,
                0,
                ElementType.None,
                transform
            );
        }
    }
}