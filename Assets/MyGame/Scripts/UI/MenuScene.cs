using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScene : MonoBehaviour
{
    public bool isCanClick;
    public Logo logo;

    void Start()
    {
        isCanClick = false;
        logo.Appear();

        DOVirtual.DelayedCall(2f, () =>
        {
            isCanClick = true;
        });
    }

    public void OnFirstClick()
    {
        if (!isCanClick)
            return;

        isCanClick = false;
        logo.OnClick();
        DOVirtual.DelayedCall(1f, () =>
        {
            Debug.LogError("ShowPopup");
            UIManager.Instance.ShowPopup<PopupMenu>();
        });
    }
}
