using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    #region Singleton
    public static ScreenFader Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    #endregion

    public bool isFading;
    public CanvasGroup faderCanvasGroup;
    float fadeDuration;

    protected IEnumerator Fade(float fadeDuration, float finalAlpha, CanvasGroup canvasGroup)
    {
        isFading = true;
        canvasGroup.blocksRaycasts = true;
        float fadeSpeed = Mathf.Abs(canvasGroup.alpha - finalAlpha) / fadeDuration;
        while (!Mathf.Approximately(canvasGroup.alpha, finalAlpha))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, finalAlpha,
                fadeSpeed * Time.unscaledDeltaTime);
            yield return null;
        }
        canvasGroup.alpha = finalAlpha;
        isFading = false;
        canvasGroup.blocksRaycasts = false;
    }


    // changed these three methods from static to "normal"  JT
    // for future reference, setting them to Static eliminates the need to write .Instance...
    public void SetAlpha (float alpha)
    {
        Instance.faderCanvasGroup.alpha = alpha; 
    }

    public IEnumerator FadeSceneIn (float duration)
    {
        yield return Instance.StartCoroutine(Instance.Fade(duration, 0f, Instance.faderCanvasGroup));
        Instance.faderCanvasGroup.gameObject.SetActive(false);
    }

    public IEnumerator FadeSceneOut (float duration)
    {
        Instance.faderCanvasGroup.gameObject.SetActive (true);
        yield return Instance.StartCoroutine(Instance.Fade(duration, 1f, Instance.faderCanvasGroup));
    }
}
