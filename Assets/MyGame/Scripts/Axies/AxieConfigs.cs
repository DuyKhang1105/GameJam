using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AxieConfig
{
    public string axieId;
    public GameObject graphic;
    public GameObject graphicUI;

    [Range(0f, 1f)]
    public float dodgeRate;
    [Range(0f, 1f)]
    public float criticalRate;
}

public enum AxieEffectType
{
    None,
    ExpansionSlot, //+1 -> +3 slot
    ExpansionChest, //+1 cheast item -> +1 axie chest
    DealDamage, //rate deal 1 damage -> 100%
    UpgradeItem, //all item +1 value -> all equipment +1 value
    HealHP, //50% heal after be attacked -> 50% collect all damage 1 turn
    HealStamina, //30% heal stamina after turn -> +1 stamina when on turn

}

[System.Serializable]
public class AxieUpgradeConfig
{
    public List<string> axieIds;
}

[CreateAssetMenu(fileName = "AxieConfigs", menuName = "Configs/AxieConfigs")]
public class AxieConfigs : ScriptableObject
{
    public static AxieConfigs Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load<AxieConfigs>("Configs/AxieConfigs");
            return instance;
        }
    }

    private static AxieConfigs instance;

    [Header("Configs")]
    public List<AxieConfig> configs;

    [Header("Upgrades")]
    public List<AxieUpgradeConfig> upgradeConfigs;

    public AxieConfig GetRandom(List<AxieConfig> ignore, int level=0)
    {
        var listUpgrade = new List<AxieUpgradeConfig>(upgradeConfigs);
        if (listUpgrade != null && listUpgrade.Count > 0)
        {
            var lst = listUpgrade[Random.Range(0, upgradeConfigs.Count)];
            if (lst.axieIds.Count > level)
            {
                return GetAxieConfig(lst.axieIds[level]);
            }
        }
        return null;
    }

    public AxieConfig GetAxieConfig(string axieId) 
    { 
        return configs.Find(x=>x.axieId == axieId);
    }

    public AxieConfig GetUpgrade(string axieId)
    {
        var upgrade = upgradeConfigs.Find(x => x.axieIds.Contains(axieId));
        if (upgrade!=null)
        {
            int index = upgrade.axieIds.IndexOf(axieId);
            int indexNext = index + 1;
            if (indexNext < upgrade.axieIds.Count)
                return GetAxieConfig(upgrade.axieIds[indexNext]);
        }
        return null; 
    }
}
