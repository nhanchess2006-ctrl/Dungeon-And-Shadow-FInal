using UnityEngine;

public class PlayerDialogueLock : MonoBehaviour
{
    private Rigidbody2D rb;

    private bool isLocked;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void LockPlayer()
    {
        isLocked = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    public void UnlockPlayer()
    {
        isLocked = false;

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public bool IsLocked()
    {
        return isLocked;
    }
}