using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WikiItemContent : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;

    private void Awake()
    {
        foreach (var support in ItemConfigs.Instance.supports)
        {
            var itemConfig = ItemConfigs.Instance.GetItemConfig(support.id);
            var itemUI = Instantiate(itemPrefab, transform);
            itemUI.SetActive(true);
            itemUI.transform.GetChild(0).GetComponent<Image>().sprite = itemConfig.sprite;
            string info = "";
            switch (support.statistic)
            {
                case StatisticType.Coin:
                    info = "Buy a item in shop";
                    break;
                case StatisticType.UpgradePet:
                    info = "Feed to pet";
                    break;
                default:
                    info = $"{support.statistic.ToString()} {support.value}";
                    if (support.statistic2 != StatisticType.None)
                    {
                        info += $"\n{support.statistic2.ToString()} {support.value2}";
                    }
                    break;
            }
            itemUI.GetComponentInChildren<TextMeshProUGUI>().text = info;
        }
    }
}
