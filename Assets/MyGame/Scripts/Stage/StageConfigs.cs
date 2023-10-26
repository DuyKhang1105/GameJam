using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StageType
{
    None,
    Enemy,
    MiniBoss,
    Boss,
    Chest,
    AxieChest
}

[System.Serializable]
public class StageConfig
{
    public StageType stageType;
    public bool isLevelUp;

    [Header("Enemies")]
    public List<EnemyName> enemyNames;
    [Header("Chest")]
    public ChestData chest;
}

[CreateAssetMenu(fileName = "StageConfigs", menuName = "Configs/StageConfigs")]
public class StageConfigs : ScriptableObject
{
    public static StageConfigs Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load<StageConfigs>("Configs/StageConfigs");
            return instance;
        }
    }

    private static StageConfigs instance;

    public List<StageConfig> stageConfigs;

    public StageConfig GetStage(int index)
    {
        if (index >=0 && index < stageConfigs.Count)
        {
            return stageConfigs[index];
        }
        return null;
    }
}
