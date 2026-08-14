using UnityEngine;

public class ElevatorPlatform : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitTime = 3f;

    private bool isActivated = false;
    private bool isReturning = false;
    private float waitTimer = 0f;

    void Start()
    {
        // Đảm bảo platform bắt đầu đúng vị trí startPoint
        if (startPoint != null)
        {
            transform.position = startPoint.position;
        }
    }

    void Update()
    {
        if (!isActivated)
            return;

        // Đang đi lên
        if (!isReturning)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                endPoint.position,
                speed * Time.deltaTime
            );

            // Đã tới endPoint
            if (Vector2.Distance(transform.position, endPoint.position) < 0.01f)
            {
                waitTimer += Time.deltaTime;

                // Đợi 3 giây
                if (waitTimer >= waitTime)
                {
                    isReturning = true;
                    waitTimer = 0f;
                }
            }
        }
        // Đang quay về startPoint
        else
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                startPoint.position,
                speed * Time.deltaTime
            );

            // Đã về startPoint
            if (Vector2.Distance(transform.position, startPoint.position) < 0.01f)
            {
                transform.position = startPoint.position;

                isActivated = false;
                isReturning = false;
                waitTimer = 0f;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isActivated = true;

            // Player đi theo platform
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}