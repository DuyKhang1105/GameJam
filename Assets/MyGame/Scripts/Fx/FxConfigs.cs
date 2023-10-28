using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeFx
{
    HIT,
    HIT_AP,
    HIT_CRIT,
    AVOID,
    BLOOD,
    DEBUFF,
    BUFF_HERO,

    //PET
    UP_PET,
    SPAWN_PET,

    //ENEMY
    SUMMON,
    BUFF_POW,
    TREE_SKILL,
    BUFF_ENEMY,
    MAGIC_STUN,

    //ADD MORE
    PET_BUFF,
    BOMB,
    SUMMON_2,
}

[System.Serializable]
public class FxConfig
{
    public TypeFx typeFx;
    public GameObject graphic;
}

[CreateAssetMenu(fileName = "FxConfigs", menuName = "Configs/FxConfigs")]
public class FxConfigs : ScriptableObject
{
    public static FxConfigs Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load<FxConfigs>("Configs/FxConfigs");
            return instance;
        }
    }

    private static FxConfigs instance;

    public List<FxConfig> configs;

    public FxConfig GetFxConfig(TypeFx typeFx)
    {
        return configs.Find(x => x.typeFx == typeFx);
    }
}
