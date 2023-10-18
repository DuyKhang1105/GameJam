using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenHome : ScreenBase
{
    public void OnBtnPlayClick()
    {
        OnHide();
        GameManager.Instance.StartLevel();
        UIManager.Instance.ShowScreen<ScreenInGame>();
    }
}
