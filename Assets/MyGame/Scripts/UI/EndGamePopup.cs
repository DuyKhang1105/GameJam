using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGamePopup : MonoBehaviour
{
    [SerializeField] private GameObject tittle;
    [SerializeField] private GameObject tittleWin;
    [SerializeField] private GameObject tittleLose;
    [SerializeField] private GameObject decorLeft;
    [SerializeField] private GameObject decorRight;
    [SerializeField] private GameObject replayBtn;

    [Header("Sounds")]
    [SerializeField] private AudioClip winSnd;
    [SerializeField] private AudioClip loseSnd;

    private void Awake()
    {
        replayBtn.GetComponent<Button>().onClick.AddListener(OnClickReplay);
    }

    [ContextMenu("Test lose")]
    private void TestLose()
    {
        Show(false);
    }

    [ContextMenu("Test win")]
    private void TestWin()
    {
        Show(true);
    }

    public void Show(bool isWin)
    {
        tittleWin.transform.localScale = isWin? Vector3.one : Vector3.zero;
        tittleLose.transform.localScale = isWin ? Vector3.zero : Vector3.one;

        Sequence seq = DOTween.Sequence();
        tittle.transform.localScale = Vector3.zero;
        tittle.transform.localPosition = Vector3.up * 900;

        float time = 1f;
        seq.Join(tittle.transform.DOLocalMoveY(90, time).SetEase(Ease.OutBack));
        seq.Join(tittle.transform.DOScale(Vector3.one, time).SetEase(Ease.OutBack));

        replayBtn.transform.localPosition = Vector3.up * -900;
        replayBtn.transform.localScale = Vector3.zero;
        seq.Join(replayBtn.transform.DOLocalMoveY(-300, time).SetEase(Ease.OutBack));
        seq.Join(replayBtn.transform.DOScale(Vector3.one, time).SetEase(Ease.OutBack));

        decorLeft.transform.localPosition = new Vector3(-1200, 600);
        decorRight.transform.localPosition = new Vector3(1200, 600);
        if (isWin)
        {
            seq.Join(decorLeft.transform.DOLocalMove(new Vector3(-710, 440), time/2f));
            seq.Join(decorRight.transform.DOLocalMove(new Vector3(710, 440), time/2f));
        }

        SoundManager.Instance.PlayOneShot(isWin ? winSnd : loseSnd);
    }

    private void OnClickReplay()
    {
        SoundManager.Instance.PlayButtonSound();
        SceneManager.LoadScene("MainScene");
    }
}
