using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Logo : MonoBehaviour
{
    RectTransform rect;
    Image image;

    public void Appear()
    {
        rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        rect.DOAnchorPosY(70f, 2f);
        image.DOFade(1, 2f);
    }

    public void OnClick()
    {
        Debug.LogError("OnClick");
        rect.DOAnchorPos(new Vector3(630f, 270f, 0f), 1f);
        transform.DOScale(1f, 1f);
        DOVirtual.DelayedCall(1f, () =>
        {
            transform.DOScale(1.1f, 1f).SetLoops(-1, LoopType.Yoyo);
        });
    }
}
