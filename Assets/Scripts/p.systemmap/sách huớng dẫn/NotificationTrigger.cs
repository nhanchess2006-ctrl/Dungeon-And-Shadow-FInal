using UnityEngine;

public class NotificationTrigger : MonoBehaviour
{
    [SerializeField] private ItemNotificationUI notificationUI;

    private bool triggered = false;

    private void Start()
    {
        Debug.Log("NotificationTrigger đã hoạt động.");

        if (notificationUI == null)
        {
            Debug.LogError("NotificationTrigger: NotificationUI CHƯA ĐƯỢC GÁN!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Có Object đi vào NotificationTrigger: " + other.name);

        if (triggered)
        {
            Debug.Log("Trigger này đã được kích hoạt.");
            return;
        }

        if (!other.CompareTag("Player"))
        {
            Debug.Log("Object không có Tag Player.");
            return;
        }

        Debug.Log("PLAYER ĐÃ VÀO NOTIFICATION TRIGGER!");

        if (notificationUI == null)
        {
            Debug.LogError("notificationUI đang NULL!");
            return;
        }

        triggered = true;

        notificationUI.ShowNotification();
    }
}