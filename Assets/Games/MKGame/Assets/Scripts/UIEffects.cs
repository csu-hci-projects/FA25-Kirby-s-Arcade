using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIEffects : MonoBehaviour
{
    [SerializeField] private CanvasGroup blinkPanel;
    [SerializeField] private float blinkSpeed = 0.15f;

    [Header("Ground Reveal Settings")]
    [SerializeField] private float revealTime = 0.5f;
    [SerializeField] private AnimationCurve revealCurve;
    [SerializeField] private RectTransform groundMask;

    [Header("Winner UI")]
    public TextMeshProUGUI winnerText;

    public IEnumerator Blink(float duration, float blinkInterval)
    {
        float timePassed = 0f;

        while (timePassed < duration)
        {
            blinkPanel.alpha = 1f;
            yield return new WaitForSeconds(blinkInterval);

            blinkPanel.alpha = 0f;
            yield return new WaitForSeconds(blinkInterval);

            timePassed += blinkInterval * 2f;
        }

        blinkPanel.alpha = 0f;
    }

    public IEnumerator GroundUpReveal(float time)
    {
        if (groundMask == null)
            yield break;

        float elapsed = 0f;
        float startY = -groundMask.rect.height;
        float endY = 0f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / time);

            float newY = Mathf.Lerp(startY, endY, revealCurve.Evaluate(t));
            groundMask.anchoredPosition = new Vector2(groundMask.anchoredPosition.x, newY);

            yield return null;
        }

        groundMask.anchoredPosition = new Vector2(groundMask.anchoredPosition.x, endY);
    }

    public void ShowWinner(string text)
    {
        if (winnerText != null)
        {
            winnerText.text = text;
            winnerText.gameObject.SetActive(true);
        }
    }
}
