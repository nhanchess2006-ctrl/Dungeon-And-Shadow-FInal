using UnityEngine;

public class ItemNotificationUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject avatarButton;
    [SerializeField] private GameObject redDot;
    [SerializeField] private GameObject itemInfoPanel;

    private void Awake()
    {
        Debug.Log("ItemNotificationUI: Awake()");

        if (avatarButton == null)
        {
            Debug.LogError("AvatarButton CHƯA ĐƯỢC GÁN!");
            return;
        }

        if (redDot == null)
        {
            Debug.LogError("RedDot CHƯA ĐƯỢC GÁN!");
            return;
        }

        if (itemInfoPanel == null)
        {
            Debug.LogError("ItemInfoPanel CHƯA ĐƯỢC GÁN!");
            return;
        }

        avatarButton.SetActive(false);
        redDot.SetActive(false);
        itemInfoPanel.SetActive(false);

        Debug.Log("ItemNotificationUI: Setup thành công!");
    }

    public void ShowNotification()
    {
        Debug.Log("ShowNotification() ĐÃ ĐƯỢC GỌI!");

        avatarButton.SetActive(true);
        redDot.SetActive(true);

        Debug.Log("Avatar + RedDot đã được bật!");
    }

    public void OpenItemInfo()
    {
        Debug.Log("OpenItemInfo() ĐÃ ĐƯỢC GỌI!");

        itemInfoPanel.SetActive(true);
    }

    public void ConfirmItem()
    {
        Debug.Log("ConfirmItem() ĐÃ ĐƯỢC GỌI!");

        itemInfoPanel.SetActive(false);
        redDot.SetActive(false);
    }
}