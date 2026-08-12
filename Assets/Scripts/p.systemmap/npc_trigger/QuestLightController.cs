using UnityEngine;

public class QuestLightController : MonoBehaviour
{
    public static QuestLightController Instance;

    [Header("Light Effect")]
    [SerializeField] private GameObject lightPrefab;

    [Header("Target")]
    [SerializeField] private RectTransform questTarget;

    [Header("Canvas")]
    [SerializeField] private Canvas canvas;

    private RectTransform canvasRect;

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (canvas != null)
        {
            canvasRect =
                canvas.GetComponent<RectTransform>();
        }
    }

    public void PlayEffect(
        Vector3 worldStartPosition
    )
    {
        // ==========================================
        // CHECK
        // ==========================================

        if (lightPrefab == null)
        {
            Debug.LogError(
                "QuestLightController: Chưa gán Light Prefab!"
            );

            return;
        }

        if (questTarget == null)
        {
            Debug.LogError(
                "QuestLightController: Chưa gán Quest Target!"
            );

            return;
        }

        if (canvas == null)
        {
            Debug.LogError(
                "QuestLightController: Chưa gán Canvas!"
            );

            return;
        }

        if (Camera.main == null)
        {
            Debug.LogError(
                "QuestLightController: Không tìm thấy Main Camera!"
            );

            return;
        }


        // ==========================================
        // UI CAMERA
        // ==========================================

        Camera uiCamera =
            canvas.renderMode ==
            RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera;


        // ==========================================
        // NPC WORLD → SCREEN
        // ==========================================

        Vector2 startScreenPosition =
            Camera.main.WorldToScreenPoint(
                worldStartPosition
            );


        // ==========================================
        // QUEST TARGET → SCREEN
        // ==========================================

        Vector2 targetScreenPosition =
            RectTransformUtility.WorldToScreenPoint(
                uiCamera,
                questTarget.position
            );


        // ==========================================
        // SCREEN → CANVAS
        // ==========================================

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            startScreenPosition,
            uiCamera,
            out Vector2 startLocalPosition
        );

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            targetScreenPosition,
            uiCamera,
            out Vector2 targetLocalPosition
        );


        // ==========================================
        // CREATE LIGHT EFFECT
        // ==========================================

        GameObject effect =
            Instantiate(
                lightPrefab,
                canvas.transform
            );


        // ==========================================
        // GET RECT TRANSFORM
        // ==========================================

        RectTransform effectRect =
            effect.GetComponent<RectTransform>();

        if (effectRect == null)
        {
            Debug.LogError(
                "LightEffect ROOT phải có RectTransform!"
            );

            Destroy(effect);

            return;
        }


        // ==========================================
        // START POSITION
        // ==========================================

        effectRect.anchoredPosition =
            startLocalPosition;


        // ==========================================
        // PLAY EFFECT
        // ==========================================

        QuestLightEffect lightEffect =
            effect.GetComponent<QuestLightEffect>();

        if (lightEffect == null)
        {
            Debug.LogError(
                "LightEffect ROOT chưa có QuestLightEffect.cs!"
            );

            Destroy(effect);

            return;
        }

        lightEffect.FlyTo(
            targetLocalPosition
        );
    }
}