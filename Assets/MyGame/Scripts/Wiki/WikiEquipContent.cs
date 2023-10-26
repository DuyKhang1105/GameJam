using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WikiEquipContent : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;

    private void Awake()
    {
        foreach (var equip in ItemConfigs.Instance.equips)
        {
            var itemConfig = ItemConfigs.Instance.GetItemConfig(equip.id);
            var itemUI = Instantiate(itemPrefab, transform);
            itemUI.SetActive(true);
            itemUI.transform.GetChild(0).GetComponent<Image>().sprite = itemConfig.sprite;
            var info = $"{equip.statistic.ToString()} {equip.value}";
            if (itemConfig.useable) info += $"\n <size=25>-Use ({itemConfig.stamina} stamina)-</size>";
            else info += $"\n <size=25>-UnUseable-</size>";
            itemUI.GetComponentInChildren<TextMeshProUGUI>().text = info;
        }
    }
}
