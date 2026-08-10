using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AreaRevealEffect : MonoBehaviour
{
    [Header("Main Light")]
    [SerializeField] private Light2D mainLight;

    [SerializeField] private float maxIntensity = 1.5f;

    [Header("Reveal Wave")]
    [SerializeField] private Transform revealWave;

    [SerializeField] private float waveStartX = -8f;
    [SerializeField] private float waveEndX = 8f;

    [SerializeField] private float waveDuration = 2.5f;

    [Header("Reveal Glow")]
    [SerializeField] private Transform revealGlow;

    [SerializeField] private float glowStartScale = 0.2f;
    [SerializeField] private float glowEndScale = 1f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem particles;

    [Header("Barrier")]
    [SerializeField] private SpriteRenderer barrierRenderer;

    [SerializeField] private float barrierFadeDuration = 2f;

    private Color originalBarrierColor;

    private void Awake()
    {
        if (barrierRenderer != null)
        {
            originalBarrierColor = barrierRenderer.color;
        }

        ResetEffect();
    }

    public void Play()
    {
        StopAllCoroutines();

        StartCoroutine(RevealSequence());
    }

    private IEnumerator RevealSequence()
    {
        Debug.Log("AREA REVEAL START");

        // ==========================================
        // 1. RESET
        // ==========================================

        ResetEffect();

        // ==========================================
        // 2. BẬT PARTICLE
        // ==========================================

        if (particles != null)
        {
            particles.Play();
        }

        // ==========================================
        // 3. BẬT WAVE
        // ==========================================

        if (revealWave != null)
        {
            revealWave.gameObject.SetActive(true);

            Vector3 pos =
                revealWave.localPosition;

            pos.x = waveStartX;

            revealWave.localPosition = pos;
        }

        // ==========================================
        // 4. BẬT GLOW
        // ==========================================

        if (revealGlow != null)
        {
            revealGlow.gameObject.SetActive(true);

            revealGlow.localScale =
                Vector3.one * glowStartScale;
        }

        // ==========================================
        // 5. WAVE CHẠY TỪ -X → +X
        // ==========================================

        float timer = 0f;

        while (timer < waveDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / waveDuration
                );

            // Smooth
            float smoothT =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );

            // ======================================
            // WAVE MOVE
            // ======================================

            if (revealWave != null)
            {
                Vector3 pos =
                    revealWave.localPosition;

                pos.x =
                    Mathf.Lerp(
                        waveStartX,
                        waveEndX,
                        smoothT
                    );

                revealWave.localPosition = pos;
            }

            // ======================================
            // GLOW SCALE
            // ======================================

            if (revealGlow != null)
            {
                float scale =
                    Mathf.Lerp(
                        glowStartScale,
                        glowEndScale,
                        smoothT
                    );

                revealGlow.localScale =
                    Vector3.one * scale;
            }

            // ======================================
            // LIGHT FADE
            // ======================================

            if (mainLight != null)
            {
                mainLight.intensity =
                    Mathf.Lerp(
                        0f,
                        maxIntensity,
                        smoothT
                    );
            }

            // ======================================
            // BARRIER FADE
            // ======================================

            if (barrierRenderer != null)
            {
                Color color =
                    barrierRenderer.color;

                color.a =
                    Mathf.Lerp(
                        originalBarrierColor.a,
                        0f,
                        smoothT
                    );

                barrierRenderer.color = color;
            }

            yield return null;
        }

        // ==========================================
        // 6. FINAL
        // ==========================================

        if (mainLight != null)
        {
            mainLight.intensity =
                maxIntensity;
        }

        if (barrierRenderer != null)
        {
            Color color =
                barrierRenderer.color;

            color.a = 0f;

            barrierRenderer.color = color;
        }

        Debug.Log("AREA REVEAL COMPLETE");

        // ==========================================
        // 7. GIỮ ÁNH SÁNG
        // ==========================================

        yield return new WaitForSeconds(1f);

        // Particle dừng phát
        if (particles != null)
        {
            particles.Stop();
        }
    }

    public void ResetEffect()
    {
        if (mainLight != null)
        {
            mainLight.intensity = 0f;
        }

        if (revealWave != null)
        {
            revealWave.gameObject.SetActive(false);

            Vector3 pos =
                revealWave.localPosition;

            pos.x = waveStartX;

            revealWave.localPosition = pos;
        }

        if (revealGlow != null)
        {
            revealGlow.gameObject.SetActive(false);

            revealGlow.localScale =
                Vector3.one * glowStartScale;
        }

        if (particles != null)
        {
            particles.Stop(
                true,
                ParticleSystemStopBehavior
                    .StopEmittingAndClear
            );
        }

        if (barrierRenderer != null)
        {
            barrierRenderer.color =
                originalBarrierColor;
        }
    }
}