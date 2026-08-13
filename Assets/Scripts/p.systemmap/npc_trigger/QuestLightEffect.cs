using UnityEngine;
using System.Collections;

public class QuestLightEffect : MonoBehaviour
{
    [SerializeField] private float moveDuration = 1f;

    public void FlyTo(Vector2 targetPosition)
    {
        StartCoroutine(
            FlyCoroutine(targetPosition)
        );
    }

    private IEnumerator FlyCoroutine(
        Vector2 targetPosition
    )
    {
        RectTransform rect =
            GetComponent<RectTransform>();

        if (rect == null)
        {
            Debug.LogError(
                "QuestLightEffect phải nằm trên GameObject có RectTransform!"
            );

            yield break;
        }

        Vector2 startPosition =
            rect.anchoredPosition;

        float timer = 0f;

        while (timer < moveDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / moveDuration
                );

            t = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            rect.anchoredPosition =
                Vector2.Lerp(
                    startPosition,
                    targetPosition,
                    t
                );

            yield return null;
        }

        rect.anchoredPosition =
            targetPosition;

        Destroy(gameObject);
    }
}