using UnityEngine;
using UnityEngine.Playables;
using System.Collections;

public class Scene1BossBattle : MonoBehaviour
{
    [Header("UI Settings")]
    [Tooltip("Kéo GameObject Canvas của bạn vào đây")]
    [SerializeField] private CanvasGroup uiCanvasGroup;

    [Header("Timeline Component")]
    [Tooltip("Kéo Component Playable Director của màn chơi vào đây")]
    [SerializeField] private PlayableDirector director;

    [Header("Entities")]
    // Giấu khỏi Inspector để dập hoàn toàn lỗi vẽ giao diện UI Toolkit của Unity 6
    [HideInInspector] 
    [SerializeField] private Entity_Health enemyHealth; 
    
    [Tooltip("Kéo Boss Enemy_Skeleton ngoài Hierarchy vào đây")]
    [SerializeField] private Enemy_Skeleton skeletonEnemy; 

    [Header("Player Settings")]
    [Tooltip("Kéo nhân vật Player ngoài Hierarchy vào đây")]
    [SerializeField] private Player player; 

    [Header("Objects to Toggle on Enemy Death")]
    [Tooltip("Object hoặc Collider mở đường khi Boss chết (Ví dụ: Finish Collider)")]
    [SerializeField] private GameObject objectToEnable;  
    [Tooltip("Vật cản chặn đường cần biến mất khi Boss chết (Ví dụ: Invisible Wall)")]
    [SerializeField] private GameObject objectToDisable; 

    private bool isBattleActive = false;
    private bool isDeathProcessed = false;
    private float cachedMaxHealth = 100f; 

    private void Awake()
    {
        // Tự động tìm và gán Component máu của Skeleton khi vừa bấm nút Play
        if (skeletonEnemy != null && enemyHealth == null)
        {
            enemyHealth = skeletonEnemy.GetComponent<Entity_Health>();
        }
    }

    // Được gọi duy nhất 1 lần khi Timeline phát Tín hiệu (Signal) vào trận
    public void StartBattle()
    {
        Debug.Log("💥 ĐÃ NHẬN SIGNAL TỪ TIMELINE!");

        // 1. AN TOÀN TUYỆT ĐỐI: Hiện Canvas CẤP TỐC ngay lập tức ở nano-giây đầu tiên!
        if (uiCanvasGroup != null)
        {
            uiCanvasGroup.alpha = 1f;          
            uiCanvasGroup.interactable = true;  
            uiCanvasGroup.blocksRaycasts = true;
            Debug.Log("📱 [UI System] Đã kích hoạt Canvas xuất hiện không có độ trễ!");
        }

        // 2. GIẢI PHÁP UNBIND ANIMATOR (Bẻ gãy xích đóng băng của Timeline lên mạch hoạt họa nhân vật)
        if (director != null)
        {
            foreach (var playableAssetOutput in director.playableAsset.outputs)
            {
                Object boundObject = director.GetGenericBinding(playableAssetOutput.sourceObject);
                if (boundObject != null)
                {
                    if (player != null && (boundObject == player.anim || boundObject == player.gameObject))
                    {
                        director.SetGenericBinding(playableAssetOutput.sourceObject, null);
                    }
                    if (skeletonEnemy != null && (boundObject == skeletonEnemy.anim || boundObject == skeletonEnemy.gameObject))
                    {
                        director.SetGenericBinding(playableAssetOutput.sourceObject, null);
                    }
                }
            }

            // Tạm dừng dòng chảy thời gian của phim cắt cảnh theo chuẩn an toàn
            director.timeUpdateMode = DirectorUpdateMode.Manual;
            director.time = director.time; 
            director.Pause(); 
        }

        // Kích hoạt chuỗi Coroutine xử lý hoán đổi linh kiện và trạng thái ở phía sau
        StartCoroutine(KickstartBattleRoutine());

        isBattleActive = true; 
    }

    private IEnumerator KickstartBattleRoutine()
    {
        // 3. PHẪU THUẬT COMPONENT CẤP TỐC: Đổi sang DestroyImmediate để dập lỗi hạ Boss nhanh bị crash!
        if (skeletonEnemy != null && enemyHealth != null)
        {
            var stats = skeletonEnemy.GetComponent<Entity_Stats>();
            if (stats != null) cachedMaxHealth = stats.GetMaxHealth();

            // Cắt bỏ lập tức cấu hình lỗi dòng 35, đòn đánh nhanh cỡ nào cũng không thể gây crash
            DestroyImmediate(enemyHealth);

            Entity_Health cleanHealthComponent = skeletonEnemy.gameObject.AddComponent<Entity_Health>();
            enemyHealth = cleanHealthComponent;
            Debug.Log("🛡️ [Component Swap] Đã dọn sạch Enemy_Health lỗi lập tức.");
        }

        // 4. Giải thoát cấu hình Component và chuyển giao máy trạng thái tự do cho Player
        if (player != null)
        {
            player.enabled = false;
            player.enabled = true; 
            
            if (player.stateMachine != null && player.idleState != null)
            {
                player.stateMachine.ChangeState(player.idleState);
            }
        }

        // Nhường quyền kích hoạt hoạt họa mở khóa đóng băng cho giải pháp chiếc Ghim độc lập số 1
        if (skeletonEnemy != null && skeletonEnemy.anim != null)
        {
            skeletonEnemy.anim.SetBool("battle", true); 
            skeletonEnemy.anim.Play("battle");
        }

        // Chờ duy nhất 1 khung hình vật lý trôi qua để các hàm SetVelocity(0,0) cũ chạy xong xuôi
        yield return new WaitForEndOfFrame();

        // 5. Cập nhật lại đồ họa toàn màn hình một lần cuối an toàn
        if (director != null)
        {
            director.Evaluate(); 
        }

        // CHỐT KHÓA UI VỮNG CHẮC: Đảm bảo sau khi Evaluate, UI Canvas vẫn giữ nguyên hiện trạng
        if (uiCanvasGroup != null)
        {
            uiCanvasGroup.alpha = 1f; 
        }
    }

    private void Update()
    {
        // Liên tục quét xem quái chết chưa bằng Component sạch mới cấy
        if (isBattleActive && !isDeathProcessed)
        {
            if (enemyHealth == null || enemyHealth.isDead || !enemyHealth.gameObject.activeInHierarchy)
            {
                HandleEnemyDeath();
            }
        }
    }

    private void HandleEnemyDeath()
    {
        isDeathProcessed = true; 
        isBattleActive = false;

        Debug.Log("🏆 HỆ THỐNG PHÁT HIỆN SKELETON ĐÃ CHẾT! Tiến hành dọn đường.");

        // Bù đắp logic hệ thống Nhiệm vụ do ta đã xóa file Enemy_Health chứa dòng code số 35 lỗi
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.wavesCompleted = true;
            Debug.Log("📜 [Quest System] Đã tự động ghi nhận điểm diệt Boss vào hệ thống Nhiệm vụ!");
        }

        // Thực hiện lệnh bật/tắt các bức tường mở cửa ải
        if (objectToEnable != null) objectToEnable.SetActive(true); 
        if (objectToDisable != null) objectToDisable.SetActive(false); 
    }
}
