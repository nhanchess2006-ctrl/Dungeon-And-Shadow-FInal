using UnityEngine;

public class RewardChest : MonoBehaviour
{
    [Header("Quest Settings")]
    [SerializeField] private bool countOnlyWhenQuestActive = true;

    [Header("Chest")]
    [SerializeField] private Animator chestAnimator;

    [SerializeField] private string openParameter = "chestOpen";


    private bool hasCounted = false;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        // Nếu chưa kéo Animator vào Inspector
        // thì tự tìm Animator
        if (chestAnimator == null)
        {
            chestAnimator =
                GetComponentInChildren<Animator>();
        }


        if (chestAnimator == null)
        {
            Debug.LogWarning(
                "RewardChest: Không tìm thấy Animator!"
                + gameObject.name
            );
        }
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (hasCounted)
            return;


        if (chestAnimator == null)
            return;


        // =====================================================
        // KIỂM TRA RƯƠNG ĐÃ MỞ
        // =====================================================

        bool chestOpened =
            chestAnimator.GetBool(
                openParameter
            );


        if (!chestOpened)
            return;


        // =====================================================
        // KIỂM TRA QUEST
        // =====================================================

        if (
            countOnlyWhenQuestActive &&
            ChestQuestManager.Instance == null
        )
        {
            return;
        }


        // =====================================================
        // CỘNG RƯƠNG
        // =====================================================

        if (ChestQuestManager.Instance != null)
        {
            hasCounted = true;


            ChestQuestManager.Instance
                .CollectChest();


            Debug.Log(
                "RewardChest: Rương "
                + gameObject.name
                + " đã được tính vào nhiệm vụ."
            );
        }
    }
}