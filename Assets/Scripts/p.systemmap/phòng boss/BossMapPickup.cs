using UnityEngine;

public class BossMapPickup : MonoBehaviour
{
    private bool collected = false;


    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        if (collected)
            return;


        if (!other.CompareTag("Player"))
            return;


        BossRoomSequence sequence =
            FindFirstObjectByType<BossRoomSequence>();


        if (sequence == null)
        {
            Debug.LogError(
                "BossMapPickup: "
                + "Không tìm thấy BossRoomSequence!"
            );

            return;
        }


        collected = true;


        Debug.Log(
            "========== PLAYER ĐÃ NHẶT BOSS MAP =========="
        );


        sequence.OnBossMapCollected(
            other.gameObject
        );


        gameObject.SetActive(false);
    }
}