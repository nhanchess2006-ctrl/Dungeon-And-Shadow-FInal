using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class MapIntroController : MonoBehaviour
{
    [Header("Quest Start")]
    [SerializeField] private QuestStartController questStartController;

    [Header("Player")]
    [SerializeField] private PlayerDialogueLock playerLock;

    [Header("Camera")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private Transform mapCameraTarget;

    [SerializeField] private float introZoom = 15f;
    [SerializeField] private float zoomDuration = 2f;

    [Header("Enemy")]
    [SerializeField] private GameObject[] enemies;

    [SerializeField] private float spawnDelay = 0.3f;

    [Header("Sequence")]
    [SerializeField] private float cameraHoldTime = 2f;

    [SerializeField] private bool playOnStart = true;

    private float normalZoom;
    private bool introPlaying = false;

    private void Start()
    {
        if (playOnStart)
        {
            PlayIntro();
        }
    }

    public void PlayIntro()
    {
        if (introPlaying)
            return;

        StartCoroutine(IntroSequence());
    }

    private IEnumerator IntroSequence()
    {
        introPlaying = true;

        // =========================================
        // 1. KHÓA PLAYER
        // =========================================

        if (playerLock != null)
        {
            playerLock.LockPlayer();
        }

        // =========================================
        // 2. LƯU CAMERA
        // =========================================

        if (cinemachineCamera != null)
        {
            normalZoom =
                cinemachineCamera.Lens.OrthographicSize;
        }

        // =========================================
        // 3. CAMERA NHÌN TOÀN MAP
        // =========================================

        if (cinemachineCamera != null &&
            mapCameraTarget != null)
        {
            cinemachineCamera.Follow =
                mapCameraTarget;
        }

        // =========================================
        // 4. ZOOM OUT
        // =========================================

        yield return StartCoroutine(
            ZoomCamera(
                normalZoom,
                introZoom
            )
        );

        // =========================================
        // 5. SPAWN ENEMY
        // =========================================

        yield return StartCoroutine(
            SpawnEnemies()
        );

        // =========================================
        // 6. GIỮ CAMERA
        // =========================================

        yield return new WaitForSeconds(
            cameraHoldTime
        );

        // =========================================
        // 7. CAMERA VỀ PLAYER
        // =========================================

        if (cinemachineCamera != null &&
            playerLock != null)
        {
            cinemachineCamera.Follow =
                playerLock.transform;
        }

        // =========================================
        // 8. ZOOM VỀ LẠI
        // =========================================

        yield return StartCoroutine(
            ZoomCamera(
                introZoom,
                normalZoom
            )
        );

        // =========================================
        // 9. MỞ KHÓA PLAYER
        // =========================================

        if (playerLock != null)
        {
            playerLock.UnlockPlayer();
        }

        // =========================================
        // 10. BẮT ĐẦU QUEST SYSTEM
        // =========================================

        if (questStartController != null)
        {
            questStartController.StartQuestSystem();
        }

        // =========================================
        // 11. HOÀN THÀNH INTRO
        // =========================================

        introPlaying = false;

        Debug.Log(
            "Map Intro đã hoàn thành!"
        );
    }

    private IEnumerator ZoomCamera(
        float startZoom,
        float targetZoom)
    {
        if (cinemachineCamera == null)
            yield break;

        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                elapsed / zoomDuration;

            cinemachineCamera
                .Lens
                .OrthographicSize =
                Mathf.Lerp(
                    startZoom,
                    targetZoom,
                    t
                );

            yield return null;
        }

        cinemachineCamera
            .Lens
            .OrthographicSize =
            targetZoom;
    }

    private IEnumerator SpawnEnemies()
    {
        if (enemies == null ||
            enemies.Length == 0)
        {
            Debug.LogWarning(
                "MapIntroController: Chưa có Enemy!"
            );

            yield break;
        }

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null)
                continue;

            enemy.SetActive(true);

            yield return new WaitForSeconds(
                spawnDelay
            );
        }
    }
}