using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public Player player;
    public LevelController levelController;
    public void OnInit()
    {
        //player.OnInit();
    }


    private void LoadLevel(int level)
    {
        levelController.OnLevelLoad(level);
    }

    public void StartLevel()
    {
        player.StartLevel();
        levelController.OnLevelStart();
    }

    public void LoadCurrentLevel()
    {
        LoadLevel(DataManager.Instance.GetData<DataLevel>().CurrentLevelId);
        player.OnLoadLevel();
    }


    public void OnWin()
    {
        DataManager.Instance.GetData<DataLevel>().PassLevel();
        UIManager.Instance.ShowPopup<PopupWin>();
    }

    public void OnLoss()
    {
        UIManager.Instance.ShowPopup<PopupLose>();
    }


}
