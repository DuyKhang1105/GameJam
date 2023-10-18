using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LevelController : MonoBehaviour
{

    public Level level;
    public LevelAsset currentMapAsset;

    public void OnLevelLoad(int levelId)
    {
        DestroyCurrentLevel();

        level = PoolManager.Instance.SpawnObject(currentMapAsset.GetLevel(levelId).transform, Vector3.zero, Quaternion.identity, transform).GetComponent<Level>();
        if (level != null)
        {
            level.OnInit();
        }

    }

    public void DestroyCurrentLevel()
    {

        if (level != null)
        {
            level.Clear();
            Destroy(level.gameObject);
            level = null;
        }
    }

    public void OnLevelStart()
    {
    }


    #region DEBUG
    #endregion
}
