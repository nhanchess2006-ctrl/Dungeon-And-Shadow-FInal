using UnityEngine;

public class PlayerDialogueLock : MonoBehaviour
{
    private Player player;

    private Rigidbody2D rb;
    private Animator anim;

    private void Awake()
    {
        player = GetComponent<Player>();

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    public void LockPlayer()
    {
        if (player == null)
            return;

        // Khóa toàn bộ input
        player.input.Player.Disable();

        // Dừng Player
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // Idle
        if (anim != null)
        {
            anim.Play("idle");
        }
    }

    public void UnlockPlayer()
    {
        if (player == null)
            return;

        // Mở input
        player.input.Player.Enable();

        // Dừng lại
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Idle
        if (anim != null)
        {
            anim.Play("idle");
        }
    }
}