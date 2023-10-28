using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class AxieConfig
{
    public string axieId;
    public GameObject graphic;
    public GameObject graphicUI;

    public AxieSkillType skillType = AxieSkillType.BuffHP;
    public float skillValue;
    [Range(0f, 1f)]
    public float actionRate = 1f;
}

public enum AxieSkillType
{
    None,
    ExtensionSlot, // + inventory slot
    ExtensionChest, //+ 1 chest item
    ExtensionAxieChest, //+1 cheast axie              
    UpgradeItem, //all item +1 value
    UpgradeAllEquipAndItem,             //-> all equipment +1 value

    AxieHit,
    BuffHP,
    BuffStamina,
    Fightsback,
    BloodSucking,
    ArmorPenetration,
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
        var ignoreIds = ignore.Select(x => x.axieId).ToList();
        var listUpgrade = new List<AxieUpgradeConfig>(upgradeConfigs);
        listUpgrade.RemoveAll(x => ignoreIds.Contains(x.axieIds[0]));
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

    public string GetInfoAxie(string axieId)
    {
        var axie = GetAxieConfig(axieId);
        var info = "Hi hi hello";
        switch (axie.skillType)
        {
            case AxieSkillType.ExtensionSlot:
                info = $"Extension {axie.skillValue} slot in inventory";
                break;
            case AxieSkillType.ExtensionChest:
                info = $"Bonus {axie.skillValue} item \nwhen open chest";
                break;
            case AxieSkillType.ExtensionAxieChest:
                info = $"Bonus {axie.skillValue} pet \nwhen open chest";
                break;
            case AxieSkillType.UpgradeItem:
                info = $"Bonus {axie.skillValue} value of item\n in inventory";
                break;
            case AxieSkillType.UpgradeAllEquipAndItem:
                info = $"Bonus {axie.skillValue} value of all\n items and equipments in inventory";
                break;
            case AxieSkillType.AxieHit:
                info = $"At start turn, \npet has a {axie.actionRate * 100}% chance \nto attack for {axie.skillValue} damage";
                break;
            case AxieSkillType.BuffHP:
                info = $"At start turn, \npet has a {axie.actionRate * 100}% chance \nto heal for {axie.skillValue} HP";
                break;
            case AxieSkillType.BuffStamina:
                info = $"At start turn, \npet has a {axie.actionRate * 100}% chance \nto buff for {axie.skillValue} starmina";
                break;
            case AxieSkillType.Fightsback:
                info = $"At start turn, \npet has a {axie.actionRate * 100}% chance \nto move to front and fight back\n enemy's attack";
                break;
            case AxieSkillType.BloodSucking:
                info = $"At every hero's attack, \npet has a {axie.actionRate * 100}% chance \nto return 1/2 of enemy's lost HP";
                break;
            case AxieSkillType.ArmorPenetration:
                info = $"At every hero's attack, \npet has a {axie.actionRate * 100}% chance \nto skip enemy's armor";
                break;

                //ExtensionSlot, // + inventory slot
                //ExtensionChest, //+ 1 chest item
                //ExtensionAxieChest, //+1 cheast axie              
                //UpgradeItem, //all item +1 value
                //UpgradeAllEquipAndItem,             //-> all equipment +1 value

                //AxieHit,
                //BuffHP,
                //BuffStamina,
                //Fightsback,
                //BloodSucking,
                //ArmorPenetration,
        }
        return info;
    }
}
