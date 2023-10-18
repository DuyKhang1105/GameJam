using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupSelect : PopupBase
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
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public void GoAdvenTuring()
    {
        SceneManager.LoadScene(2);
    }

    public void Cancel()
    {
        OnHide();
        uiManager.ShowPopup<PopupMenu>();
    }
}
