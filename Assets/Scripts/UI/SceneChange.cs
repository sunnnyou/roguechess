using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    private void Start()
    {
        // Fade in when scene starts
        if (this.fadeImage != null)
        {
            this.StartCoroutine(this.FadeIn());
        }
    }

    public void LoadSceneWithFade(string sceneName)
    {
        this.StartCoroutine(this.FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        // Fade out
        yield return this.StartCoroutine(this.FadeOut());

        // Load scene
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeIn()
    {
        Color color = this.fadeImage.color;
        for (float t = 1f; t >= 0f; t -= Time.deltaTime / this.fadeDuration)
        {
            color.a = t;
            this.fadeImage.color = color;
            yield return null;
        }
        color.a = 0f;
        this.fadeImage.color = color;
    }

    private IEnumerator FadeOut()
    {
        Color color = this.fadeImage.color;
        for (float t = 0f; t <= 1f; t += Time.deltaTime / this.fadeDuration)
        {
            color.a = t;
            this.fadeImage.color = color;
            yield return null;
        }
        color.a = 1f;
        this.fadeImage.color = color;
    }
}
