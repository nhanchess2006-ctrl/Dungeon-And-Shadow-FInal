using UnityEngine;

public class QuestStartController : MonoBehaviour
{
    
    [Header("Quest System")]
    [SerializeField] private QuestManager questManager;

    [SerializeField] private QuestPopupUI questPopupUI;

    public void StartQuestSystem()
    {
        if (questManager != null)
        {
            questManager.InitializeQuest();
        }

        if (questPopupUI != null)
        {
            questPopupUI.StartQuestPopup();
        }

        Debug.Log("Quest System đã bắt đầu!");
    }
}