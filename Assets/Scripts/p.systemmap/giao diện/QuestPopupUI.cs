using System.Collections;
using UnityEngine;

public class QuestPopupUI : MonoBehaviour
{
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private GameObject hudPanel;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        // Khi vừa vào Scene:
        // Popup phải được ẩn
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }

        // HUD cũng chưa hiện
        if (hudPanel != null)
        {
            hudPanel.SetActive(false);
        }
    }

    // Hàm này sẽ được QuestStartController gọi
    // SAU KHI MapIntroController hoàn thành
    public void StartQuestPopup()
    {
        if (popupPanel == null)
            return;

        popupPanel.SetActive(true);

        if (hudPanel != null)
        {
            hudPanel.SetActive(false);
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha +=
                Time.deltaTime / fadeDuration;

            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void ConfirmQuest()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -=
                Time.deltaTime / fadeDuration;

            yield return null;
        }

        canvasGroup.alpha = 0f;

        popupPanel.SetActive(false);

        if (hudPanel != null)
        {
            hudPanel.SetActive(true);
        }
    }
}