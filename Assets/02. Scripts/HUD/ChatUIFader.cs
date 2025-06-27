using System.Collections;
using UnityEngine;

public class ChatUIFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float visibleDuration = 5f;

    private Coroutine fadeCoroutine;
    private Coroutine autoHideCoroutine;

    private void OnEnable()
    {
        FadeIn();
    }

    public void FadeIn()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeTo(1f));

        if (autoHideCoroutine != null) StopCoroutine(autoHideCoroutine);
        autoHideCoroutine = StartCoroutine(AutoHide());
    }

    private IEnumerator AutoHide()
    {
        yield return new WaitForSeconds(visibleDuration);
        FadeOut();
    }

    public void FadeOut()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeTo(0f));
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        float startAlpha = _canvasGroup.alpha;
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            yield return null;
        }
        _canvasGroup.alpha = targetAlpha;
    }
}
