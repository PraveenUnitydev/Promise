using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Full-screen black overlay used for death / respawn fades. Created on demand and kept across scenes.
// (Level-to-level fades are still done by LevelChanger's animator.)
public class ScreenFader : MonoBehaviour
{
    static ScreenFader _instance;
    Image _image;

    public static ScreenFader Instance
    {
        get
        {
            if (_instance == null)
                _instance = Create();
            return _instance;
        }
    }

    static ScreenFader Create()
    {
        var go = new GameObject("ScreenFader");
        DontDestroyOnLoad(go);

        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        var imageGo = new GameObject("Black");
        imageGo.transform.SetParent(go.transform, false);
        var rect = imageGo.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var fader = go.AddComponent<ScreenFader>();
        fader._image = imageGo.AddComponent<Image>();
        fader._image.color = new Color(0f, 0f, 0f, 0f);
        fader._image.raycastTarget = false;
        return fader;
    }

    public float Alpha => _image.color.a;

    public IEnumerator FadeTo(float alpha, float duration)
    {
        float start = _image.color.a;
        for (float t = 0f; t < duration; t += Time.unscaledDeltaTime)
        {
            SetAlpha(Mathf.Lerp(start, alpha, t / duration));
            yield return null;
        }
        SetAlpha(alpha);
    }

    void SetAlpha(float a)
    {
        var c = _image.color;
        c.a = a;
        _image.color = c;
    }
}
