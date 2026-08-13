using UnityEngine;

public class Flower : MonoBehaviour
{
    [Header("Unlock Condition")]
    [SerializeField] private bool requireWaves = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Hoa này yêu cầu hoàn thành Wave 2
        if (requireWaves && !QuestManager.Instance.wavesCompleted)
        {
            Debug.Log("Phải đánh bại Wave 2 trước!");
            return;
        }

        QuestManager.Instance.CollectFlower();

        Destroy(gameObject);
    }
}