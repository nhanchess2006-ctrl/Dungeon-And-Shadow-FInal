using System.Collections;
using UnityEngine;

public class AreaBarrier : MonoBehaviour
{
    [Header("Barrier")]
    [SerializeField] private Collider2D col;

    [SerializeField] private SpriteRenderer sr;

    [Header("Reveal Effect")]
    [SerializeField] private AreaRevealEffect revealEffect;

    private void Awake()
    {
        if (col == null)
            col = GetComponent<Collider2D>();

        if (sr == null)
            sr = GetComponent<SpriteRenderer>();
    }

    public void ShowGlow()
    {
        Debug.Log("AreaBarrier: ShowGlow");

        if (revealEffect != null)
        {
            revealEffect.Play();
        }
        else
        {
            Debug.LogWarning(
                "AreaBarrier: Chưa gán AreaRevealEffect!"
            );
        }

        StartCoroutine(DisableBarrier());
    }

    private IEnumerator DisableBarrier()
    {
        // Chờ effect chạy
        yield return new WaitForSeconds(2.5f);

        if (col != null)
        {
            col.enabled = false;
        }

        Debug.Log("AreaBarrier: Khu vực đã mở!");
    }

    public void Unlock()
    {
        ShowGlow();
    }
}