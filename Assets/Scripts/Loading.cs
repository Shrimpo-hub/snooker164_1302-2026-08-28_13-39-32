using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private float waitSecond = 1f;

    private bool loading = false;

    private void Start()
    {
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (loading)
        {
            return;
        }

        if (waitSecond > 0f)
        {
            waitSecond -= Time.unscaledDeltaTime;
        }
        else
        {
            loading = true;
            StartCoroutine(LoadNewScene());
        }
    }

    private IEnumerator LoadNewScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Scene01");

        while (!operation.isDone)
        {
            slider.value = operation.progress / 0.9f;
            yield return null;
        }
    }
}