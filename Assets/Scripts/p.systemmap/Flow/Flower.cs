using UnityEngine;

public class Flower : MonoBehaviour
{
    [Header("Unlock Condition")]
    [SerializeField] private bool requireWaves = false;

    private bool collected = false;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
            return;


        if (!other.CompareTag("Player"))
            return;


        if (QuestManager.Instance == null)
        {
            Debug.LogError(
                "Flower: Không tìm thấy QuestManager!"
            );

            return;
        }


        // =========================================
        // HOA CÓ YÊU CẦU WAVE KHÔNG?
        // =========================================

        if (requireWaves &&
            !QuestManager.Instance.wavesCompleted)
        {
            Debug.Log(
                "Không thể nhặt hoa "
                + gameObject.name
                + ": Wave 2 chưa hoàn thành."
            );

            return;
        }


        // =========================================
        // CỘNG HOA
        // =========================================

        QuestManager.Instance.CollectFlower();


        // =========================================
        // ĐÁNH DẤU ĐÃ NHẶT
        // =========================================

        collected = true;


        Debug.Log(
            "Đã nhặt hoa: "
            + gameObject.name
        );


        // =========================================
        // XÓA HOA
        // =========================================

        Destroy(gameObject);
    }
}