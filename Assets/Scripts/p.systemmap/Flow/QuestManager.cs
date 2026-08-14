using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("Quest Complete")]
    [SerializeField] private GameObject completePanel;
    [SerializeField] private float completeShowTime = 2f;

    [Header("Half Quest")]
    [SerializeField] private GameObject halfQuestPanel;
    [SerializeField] private float halfQuestShowTime = 2f;

    [Header("Portal")]
    [SerializeField] private Object_Portal portal;
    [SerializeField] private Transform portalSpawnPoint;

    [Header("Boss")]
    [SerializeField] private Enemy_Health questBoss;

    [Header("UI")]
    [SerializeField] private QuestUI questUI;

    public static QuestManager Instance;

    public bool bossDefeated = false;

    private bool doorSpawned = false;
    private bool halfQuestShown = false;

    public int currentFlower = 0;
    public int targetFlower = 20;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        questUI.UpdateProgress(currentFlower, targetFlower);

        completePanel.SetActive(false);

        if (halfQuestPanel != null)
        {
            halfQuestPanel.SetActive(false);
        }
    }

    public bool IsQuestCompleted()
    {
        return currentFlower >= targetFlower;
    }

    public void CollectFlower()
    {
        currentFlower++;

        questUI.UpdateProgress(currentFlower, targetFlower);

        Debug.Log($"Đã nhặt: {currentFlower}/{targetFlower}");

        // Mốc 10 hoa
        if (currentFlower >= 10 && !halfQuestShown)
        {
            halfQuestShown = true;

            Debug.Log("Đã thu thập được 10 bông hoa!");

            if (halfQuestPanel != null)
            {
                halfQuestPanel.SetActive(true);
                Invoke(nameof(HideHalfQuestPanel), halfQuestShowTime);
            }
        }

        // Hoàn thành nhiệm vụ
        if (currentFlower >= targetFlower && !doorSpawned)
        {
            doorSpawned = true;

            Debug.Log("Hoàn thành nhiệm vụ!");

            completePanel.SetActive(true);

            Invoke(nameof(HideCompletePanel), completeShowTime);

            if (portal != null)
            {
                portal.ActivatePortal(portalSpawnPoint.position);
            }
        }
    }

    public void NotifyEnemyDeath(Enemy_Health enemy)
    {
        if (enemy == questBoss)
        {
            bossDefeated = true;
            Debug.Log("Boss nhiệm vụ đã bị tiêu diệt!");
        }
    }

    private void HideCompletePanel()
    {
        completePanel.SetActive(false);
    }

    private void HideHalfQuestPanel()
    {
        if (halfQuestPanel != null)
        {
            halfQuestPanel.SetActive(false);
        }
    }
}