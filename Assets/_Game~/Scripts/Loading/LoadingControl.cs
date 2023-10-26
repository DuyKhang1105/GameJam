using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingControl : MonoBehaviour
{
    [SerializeField] private GameObject m_progressBar;
    [SerializeField] private Image m_imageLoading;
    AsyncOperation async;
    public int time;

    private void Start()
    {
        if (time <= 0)
            time = 6;
        Screen.orientation = ScreenOrientation.Portrait;
        LoadScene();
    }
    public void LoadScene()
    {
        StartCoroutine(IELoadScene(1));
    }

    private IEnumerator IELoadScene(int sceneId)
    {
        yield return new WaitForSeconds(0.1f);
        async = SceneManager.LoadSceneAsync(sceneId);
        async.allowSceneActivation = false;

        // slider
        m_imageLoading.DOFillAmount(1f, time);
        yield return new WaitUntil(() => async.progress == 0.9f);
        yield return new WaitForSeconds(time);

        async.allowSceneActivation = true;
    }
}
