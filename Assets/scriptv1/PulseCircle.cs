using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PulseCircle : MonoBehaviour
{
    [Header("refs")]
    public Image baseImage;
    public Image pulseImage;

    [Header("base")]
    [Range(0f, 1f)] public float baseAlpha = 0.9f;

    [Header("pulse")]
    public float pulseScale = 1.5f;
    public float pulseDuration = 0.2f;
    [Range(0f, 1f)] public float pulseStartAlpha = 0.9f;
    [Range(0f, 1f)] public float pulseEndAlpha = 0f;

    [Header("Colors")]
    public Color perfectColor = new Color32(255, 220, 0, 255);
    public Color niceColor = new Color32(0, 220, 80, 255);
    public Color okColor = new Color32(50, 140, 255, 255);
    public Color badColor = new Color32(255, 60, 60, 255);
    Vector3 baseScalePulse;
    Coroutine routine;
    void Awake()
    {
        if (pulseImage != null)
        {
            baseScalePulse = pulseImage.rectTransform.localScale;
            var c = pulseImage.color;
            c.a = 0f;
            pulseImage.color = c;
        }

        if (baseImage != null)
        {
            var c = baseImage.color;
            c.a = baseAlpha;
            baseImage.color = c;
        }
    }

    public void PulsePerfect() => Pulse(perfectColor);
    public void PulseNice() => Pulse(niceColor);
    public void PulseOk() => Pulse(okColor);
    public void PulseBad() => Pulse(badColor);

    public void Pulse(Color c)
    {
        if (pulseImage == null) return;
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(PulseRoutine(c));
    }

    IEnumerator PulseRoutine(Color c)
    {
        pulseImage.rectTransform.localScale = baseScalePulse;
        c.a = pulseStartAlpha;
        pulseImage.color = c;
        float t = 0f;
        float half = pulseDuration * 0.5f;
        while (t < half)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / half);
            pulseImage.rectTransform.localScale = Vector3.Lerp(baseScalePulse, baseScalePulse * pulseScale, k);
            var col = pulseImage.color;
            col.a = Mathf.Lerp(pulseStartAlpha, pulseStartAlpha * 0.7f, k);
            pulseImage.color = col;
            yield return null;
        }
        // scale down et idle
        t = 0f;
        while (t < half)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / half);
            pulseImage.rectTransform.localScale = Vector3.Lerp(baseScalePulse * pulseScale, baseScalePulse, k);
            var col = pulseImage.color;
            col.a = Mathf.Lerp(pulseStartAlpha * 0.7f, pulseEndAlpha, k);
            pulseImage.color = col;
            yield return null;
        }
        var end = pulseImage.color; 
        end.a = 0f;
        pulseImage.color = end;
        routine = null;
    }
}
