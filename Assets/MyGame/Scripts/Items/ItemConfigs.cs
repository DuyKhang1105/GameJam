using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemType
{
    None,
    Equipment,
    Support,
    Jewelry
}

[System.Serializable]
public class ItemConfig
{
    public string id;
    public Sprite sprite;

    [Header("Place")]
    public int width;
    public int height;

    [Header("Statistic")]
    public ItemType type;
    public bool useable;
    public int stamina;

    [Header("Rare")]
    [Range(0f, 1f)]
    public float rare; //0 -> 1
}


public enum EquipType
{
    None,
    Weapon, //=> attack
    Shield, //=> shield
    Shoe, 
    Helmet
}

public enum StatisticType
{
    None = -1,
    Attack, //=> weapon
    Shield, //=> shield
    Avoid, //=> shoe
    Critical, //=> helmet
    HP,
    Stamina,

    Coin = 100,
    UpgradePet = 101
}

[System.Serializable]
public class EquipConfig
{
    public string id;
    public EquipType equipType;
    public StatisticType statistic;
    public float value;
}

[System.Serializable]
public class JewelryConfig
{
    public string chainId;
    public List<RingJewelryConfig> rings;

    public List<JewelrySetData> AllSets()
    {
        var sets = new List<JewelrySetData>();
        foreach (var ring in rings)
        {
            var effect = new JewelrySetData();
            effect.chainId = chainId;
            effect.ringId = ring.ringId;
            effect.targetId = ring.targetId;
            effect.value = ring.value;
            sets.Add(effect);
        }
        return sets;
    }
}

public class JewelrySetData
{
    public string chainId;
    public string ringId;
    public string targetId;
    public float value;
}

[System.Serializable]
public class RingJewelryConfig
{
    public string ringId;
    public string targetId;
    public float value;
}

[System.Serializable]
public class SupportConfig
{
    public string id;
    public StatisticType statistic;
    public float value;
    public StatisticType statistic2;
    public float value2;
}

[CreateAssetMenu(fileName ="ItemConfigs",menuName = "Configs/ItemConfigs")]
public class ItemConfigs : ScriptableObject
{
    public static ItemConfigs Instance
    {
        get {
            if (instance == null)
                instance = Resources.Load<ItemConfigs>("Configs/ItemConfigs");
            return instance;
        }
    }

    private static ItemConfigs instance;

    //CONFIG 

    public List<ItemConfig> configs;

    public List<EquipConfig> equips;

    public List<JewelryConfig> jewelries;

    public List<SupportConfig> supports;
   
    public List<ItemConfig> GetItemsInChests(float rareMax, int count)
    {
        var sortedConfigs = configs.FindAll(x => x.rare <= rareMax).ToList();

        List<ItemConfig> result = new List<ItemConfig>();
        for (int i =0; i < count; i++)
        {
            if (sortedConfigs.Count > 0)
            {
                int index = Random.Range(0, sortedConfigs.Count);
                result.Add(sortedConfigs[index]);
                sortedConfigs.RemoveAt(index);
            }
        }
        return result;
    }

    public ItemConfig GetItemConfig(string id)
    {
        return configs.Find(x=>x.id == id);
    }

    public List<JewelrySetData> GetAllJewelrySets(List<ItemConfig> currentJewelries)
    {
        var result = new List<JewelrySetData>();
        foreach (var jewelry in currentJewelries)
        {
            foreach (var config in jewelries)
            {
                foreach (var set in config.AllSets())
                {     
                    if (result.Any(x=> x.chainId == set.chainId && x.ringId == set.ringId)) continue;
                    //Debug.Log("Check set: " + set.chainId);
                    if (set.chainId == jewelry.id && currentJewelries.Any(x => x.id == set.ringId)
                        || (set.ringId == jewelry.id && currentJewelries.Any(x => x.id == set.chainId)))
                    {
                        result.Add(set);
                    }
                }
            }
        }
        return result;
    }
}
