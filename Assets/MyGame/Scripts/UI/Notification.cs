using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Notification : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI notiTxt;
    public static Notification Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowNoti(string text)
    {
        notiTxt.transform.localPosition = Vector3.up * -200f;
        notiTxt.text = text;
        DOTween.Kill(this);
        Sequence seq = DOTween.Sequence();
        seq.Append(notiTxt.transform.DOLocalMoveY(0, 0.1f).SetEase(Ease.OutBack));
        seq.AppendInterval(1f);
        seq.Append(notiTxt.transform.DOLocalMoveY(-200, 0.1f));
        seq.SetId(this);
    }
}
