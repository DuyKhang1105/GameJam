using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyName
{
    None,
    BearDad,
    BearMom,
    DryadFighter,
    DryadMage,
    DryadRanger,
    Slime,
    SlimeAttack,
    SlimeDefense,
    SlimeForestA,
    SlimeForestB,
    SlimeFusion,
    SlimeSupport,
    Treant,
    TreantFighter,
    TreantFlowering,
    WereWolf,
    WolfAlpha,
    WolfAquatic,
    WolfAquaticAlpha,
    WolfGray
}

public enum EnemySkillType
{
    None,
    HealAll,
    StunHero,
    BuffPow,
    SelfDestruct,
    Summon,
    Evolution,
    Assassinate,
}

[System.Serializable]
public class EnemyStatistics
{
    public int maxHP;
    public int maxPow;
    public int damage;
    public int shield;
    public int hpBuff;
    public int shieldBuff;
}

[System.Serializable]
public class EnemyConfig
{
    public EnemyName name;
    public EnemySkillType skillType;
    public int skillValue;
    public GameObject graphic;
    public EnemyStatistics statistics;
}

[CreateAssetMenu(fileName = "EnemyConfigs", menuName = "Configs/EnemyConfigs")]
public class EnemyConfigs : ScriptableObject
{
    public static EnemyConfigs Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load<EnemyConfigs>("Configs/EnemyConfigs");
            return instance;
        }
    }

    private static EnemyConfigs instance;

    public List<EnemyConfig> configs;

    public EnemyConfig GetEnemyConfig(EnemyName name)
    {
        return configs.Find(x => x.name == name);
    }
}
