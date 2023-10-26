using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WikiJewelryContent : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;

    private void Awake()
    {
        foreach (var jewelry in ItemConfigs.Instance.jewelries)
        {
            var chain = ItemConfigs.Instance.GetItemConfig(jewelry.chainId);
            foreach (var ring in jewelry.rings)
            {
                var ringIC = ItemConfigs.Instance.GetItemConfig(ring.ringId);
                var targetIC = ItemConfigs.Instance.GetItemConfig(ring.targetId);
                var itemUI = Instantiate(itemPrefab, transform);
                itemUI.SetActive(true);

                itemUI.transform.GetChild(0).GetComponent<Image>().sprite = chain.sprite;
                itemUI.transform.GetChild(1).GetComponent<Image>().sprite = ringIC.sprite;
                itemUI.transform.GetChild(2).GetComponent<Image>().sprite = targetIC.sprite;

                var equip = ItemConfigs.Instance.equips.Find(x => x.id == ring.targetId);

                var info = $"{equip.statistic.ToString()} {equip.value} + {ring.value}";
                itemUI.GetComponentInChildren<TextMeshProUGUI>().text = info;
            }
        }
    }
}
