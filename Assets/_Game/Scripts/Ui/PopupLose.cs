using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupLose : PopupBase
{
    public void OnBtnReplayClick()
    {
        OnHide();
        GameManager.Instance.LoadCurrentLevel();
        GameManager.Instance.StartLevel();
    }

    public void OnBtnHomeClick()
    {
        OnHide();
        UIManager.Instance.HideScreen<ScreenInGame>();
        UIManager.Instance.ShowScreen<ScreenHome>();
        GameManager.Instance.LoadCurrentLevel();
    }
}
