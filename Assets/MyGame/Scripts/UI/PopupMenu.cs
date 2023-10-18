using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupMenu : PopupBase
{
    UIManager uiManager;

    public override void OnInit()
    {
        base.OnInit();
        uiManager = UIManager.Instance;
    }

    public override void OnShow()
    {
        base.OnShow();
        Debug.LogError("ShowPopupMenu");
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public void StartGame()
    {
        OnHide();
        uiManager.ShowPopup<PopupSelect>();
    }

    public void Continue()
    {
        SceneManager.LoadScene(2);
    }

    public void Options()
    {
        OnHide();
        uiManager.ShowPopup<PopupOptions>();
    }
}
