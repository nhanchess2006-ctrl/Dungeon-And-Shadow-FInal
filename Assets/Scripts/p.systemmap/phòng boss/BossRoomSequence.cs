using UnityEngine;

public class BossRoomSequence : MonoBehaviour
{
    // =====================================================
    // BOSS MAP
    // =====================================================

    [Header("Boss Map")]

    [SerializeField]
    private GameObject bossMap;

    [SerializeField]
    private Transform bossMapSpawnPoint;


    // =====================================================
    // BOSS NPC
    // =====================================================

    [Header("Boss NPC")]

    [SerializeField]
    private NPCTrigger bossNPCTrigger;


    // =====================================================
    // BOSS MAP UI
    // =====================================================

    [Header("Boss Map UI")]

    [SerializeField]
    private GameObject bossMapAvatar;

    [SerializeField]
    private GameObject bossInfoPanel;


    // =====================================================
    // BOSS PORTAL
    // =====================================================

    [Header("Boss Portal")]

    [SerializeField]
    private Object_Portal portal;

    [SerializeField]
    private Transform portalSpawnPoint;


    // =====================================================
    // STATE
    // =====================================================

    private bool sequenceStarted = false;

    private bool mapCollected = false;

    private bool bossInfoCompleted = false;


    // =====================================================
    // START
    // =====================================================

    public void StartBossRoomSequence()
    {
        if (sequenceStarted)
        {
            Debug.LogWarning(
                "BossRoomSequence đã bắt đầu!"
            );

            return;
        }


        sequenceStarted = true;


        Debug.Log(
            "========== BOSS ROOM SEQUENCE START =========="
        );


        // Avatar OFF

        if (bossMapAvatar != null)
        {
            bossMapAvatar.SetActive(false);
        }


        // Info OFF

        if (bossInfoPanel != null)
        {
            bossInfoPanel.SetActive(false);
        }


        // Portal OFF

        if (portal != null)
        {
            portal.gameObject.SetActive(false);
        }


        // Boss Map

        SpawnBossMap();
    }


    // =====================================================
    // SPAWN BOSS MAP
    // =====================================================

    private void SpawnBossMap()
    {
        if (bossMap == null)
        {
            Debug.LogError(
                "BossRoomSequence: Boss Map chưa được gán!"
            );

            return;
        }


        if (bossMapSpawnPoint == null)
        {
            Debug.LogError(
                "BossRoomSequence: Boss Map Spawn Point chưa được gán!"
            );

            return;
        }


        bossMap.transform.position =
            bossMapSpawnPoint.position;


        bossMap.SetActive(true);


        Debug.Log(
            "========== BOSS MAP ĐÃ XUẤT HIỆN =========="
        );
    }


    // =====================================================
    // BOSS MAP PICKUP
    // =====================================================

    public void OnBossMapCollected(
        GameObject player
    )
    {
        if (mapCollected)
            return;


        if (player == null)
        {
            Debug.LogError(
                "BossRoomSequence: Player null!"
            );

            return;
        }


        mapCollected = true;


        Debug.Log(
            "========== PLAYER ĐÃ NHẶT BOSS MAP =========="
        );


        if (bossMap != null)
        {
            bossMap.SetActive(false);
        }


        if (bossNPCTrigger != null)
        {
            bossNPCTrigger
                .SpawnNPCInFrontOfPlayer(player);
        }
        else
        {
            Debug.LogError(
                "BossRoomSequence: Boss NPC Trigger chưa được gán!"
            );
        }
    }


    // =====================================================
    // NPC DIALOGUE FINISHED
    // =====================================================

    private void OnBossDialogueFinished()
    {
        Debug.Log(
            "========== BOSS NPC DIALOGUE HOÀN TẤT =========="
        );


        if (!mapCollected)
        {
            Debug.LogWarning(
                "NPC thoại xong nhưng Boss Map chưa được nhặt!"
            );

            return;
        }


        ShowBossMapAvatar();
    }


    // =====================================================
    // SHOW AVATAR
    // =====================================================

    private void ShowBossMapAvatar()
    {
        if (bossMapAvatar == null)
        {
            Debug.LogError(
                "BossRoomSequence: Boss Map Avatar chưa được gán!"
            );

            return;
        }


        bossMapAvatar.SetActive(true);


        Debug.Log(
            "========== BOSS MAP AVATAR ĐÃ HIỆN =========="
        );
    }


    // =====================================================
    // OPEN BOSS INFO
    // =====================================================

    public void OpenBossInfo()
    {
        Debug.Log(
            "========== OPEN BOSS INFO =========="
        );


        if (!mapCollected)
        {
            Debug.LogWarning(
                "Không thể mở Boss Info: chưa nhặt Boss Map!"
            );

            return;
        }


        if (bossInfoPanel == null)
        {
            Debug.LogError(
                "BossRoomSequence: Boss Info Panel chưa được gán!"
            );

            return;
        }


        bossInfoPanel.SetActive(true);


        Debug.Log(
            "========== BOSS INFO PANEL ĐÃ MỞ =========="
        );
    }


    // =====================================================
    // COMPLETE BOSS INFO
    // =====================================================

    public void CompleteBossInfo()
    {
        Debug.Log(
            "========== COMPLETE BOSS INFO =========="
        );


        if (bossInfoCompleted)
            return;


        if (!mapCollected)
        {
            Debug.LogWarning(
                "Không thể Complete Boss Info: chưa nhặt Boss Map!"
            );

            return;
        }


        bossInfoCompleted = true;


        // Info OFF

        if (bossInfoPanel != null)
        {
            bossInfoPanel.SetActive(false);
        }


        // Avatar OFF

        if (bossMapAvatar != null)
        {
            bossMapAvatar.SetActive(false);
        }


        Debug.Log(
            "========== BOSS INFO ĐÃ HOÀN THÀNH =========="
        );


        // Portal

        OpenPortal();
    }


    // =====================================================
    // OPEN PORTAL
    // =====================================================

    private void OpenPortal()
    {
        Debug.Log(
            "========== ĐANG SPAWN PORTAL =========="
        );


        // =========================================
        // PORTAL
        // =========================================

        if (portal == null)
        {
            Debug.LogError(
                "BossRoomSequence: Portal chưa được gán!"
            );

            return;
        }


        // =========================================
        // SPAWN POINT
        // =========================================

        if (portalSpawnPoint == null)
        {
            Debug.LogError(
                "BossRoomSequence: Portal Spawn Point chưa được gán!"
            );

            return;
        }


        // =========================================
        // POSITION
        // =========================================

        portal.transform.position =
            portalSpawnPoint.position;


        portal.transform.rotation =
            portalSpawnPoint.rotation;


        // =========================================
        // ACTIVE
        // =========================================

        portal.gameObject.SetActive(true);


        // =========================================
        // PORTAL LOGIC
        // =========================================

        portal.ActivatePortal(
            portalSpawnPoint.position
        );


        Debug.Log(
            "Portal Position = "
            + portal.transform.position
        );


        Debug.Log(
            "Portal Spawn Point = "
            + portalSpawnPoint.position
        );


        Debug.Log(
            "========== PORTAL BOSS ĐÃ XUẤT HIỆN =========="
        );
    }


    // =====================================================
    // EVENT
    // =====================================================

    private void OnEnable()
    {
        if (bossNPCTrigger != null)
        {
            bossNPCTrigger.OnBossDialogueFinished
                += OnBossDialogueFinished;
        }
    }


    private void OnDisable()
    {
        if (bossNPCTrigger != null)
        {
            bossNPCTrigger.OnBossDialogueFinished
                -= OnBossDialogueFinished;
        }
    }
}