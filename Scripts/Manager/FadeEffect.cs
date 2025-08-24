using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
    [SerializeField] private Image fade;

    public void Awake()
    {
        fade.gameObject.SetActive(true);
    }

    public void Fadein()
    {
        StartCoroutine(FadeInCoroutine());
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    public IEnumerator FadeInCoroutine()
    {
        fade.color = new Color(0, 0, 0, 0);
        fade.gameObject.SetActive(true);

        while (fade.color.a < 1)
        {
            fade.color = new Color(0, 0, 0, fade.color.a + Time.deltaTime / 1.5f);
            yield return null;
        }
        fade.gameObject.SetActive(false);
    }

    public IEnumerator FadeOutCoroutine()
    {
        fade.color = new Color(0, 0, 0, 1);
        fade.gameObject.SetActive(true);

        while (fade.color.a > 0)
        {
            fade.color = new Color(0, 0, 0, fade.color.a - Time.deltaTime / 1.5f);
            yield return null;
        }

        fade.gameObject.SetActive(false);
    }
}
