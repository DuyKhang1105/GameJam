using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AxieChestPopup : MonoBehaviour
{
    [SerializeField] private Button confirmBtn;
    [SerializeField] private Button skipBtn;
    [SerializeField] private List<Transform> slotTrans;
    public int count;
    private int indexSelected;
    private bool isSelected;
    private List<AxieConfig> axies;

    private void Awake()
    {
        confirmBtn.onClick.AddListener(() =>
        {
            ConfirmAxie();
        });

        skipBtn.onClick.AddListener(() =>
        {
            SkipSelect();
        });

        for (int i = 0; i < slotTrans.Count; i++)
        {
            int index = i;
            slotTrans[index].GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectAxie(index);
            });
        } 
    }


    public void OpenChest(int c=3)
    {
        count = c;
        _OpenChest();
        SelectAxie(1);
    }

    private void SelectAxie(int index)
    {
        slotTrans[indexSelected].GetComponent<Image>().color = Color.black;
        indexSelected = index;
        slotTrans[indexSelected].GetComponent<Image>().color = Color.yellow;
    }

    private void ConfirmAxie()
    {
        if (!isSelected)
        {
            isSelected = true;
            FindObjectOfType<AxieInventory>().AddAxie(axies[indexSelected]);
            GameUI.Instance.bg.SetActive(false);
            gameObject.SetActive(false);
            GameUI.Instance.axieChest.SetActive(false);
            GameUI.Instance.nextBtn.SetActive(true);
        }
    }

    private void SkipSelect()
    {
        ItemConfig itemConfig = ItemConfigs.Instance.configs.Find(x => x.id.Contains("Item_Upgrade_Pet"));
        gameObject.SetActive(false);
        var inventory = FindObjectOfType<Inventory>();
        var lst = new List<ItemConfig>();
        lst.Add(itemConfig);
        inventory.SpawnItems(lst, skipBtn.transform);
    }

    private void _OpenChest()
    {
        isSelected = false;
        GameUI.Instance.bg.SetActive(true); 
        slotTrans.ForEach(t => { t.gameObject.SetActive(false); });
        {
            axies = new List<AxieConfig>();

            var axiesIgnore = new List<AxieConfig>();
            var axieInventory = FindObjectOfType<AxieInventory>();
            axieInventory.axies.ForEach(x =>
            {
                var upgrade = AxieConfigs.Instance.upgradeConfigs.Find(y => y.axieIds.Contains(x.axieId));
                if (upgrade != null)
                    upgrade.axieIds.ForEach(z=> axiesIgnore.Add(AxieConfigs.Instance.GetAxieConfig(z)));
            });

            axiesIgnore.Add(AxieConfigs.Instance.GetAxieConfig("5_dps-beast")); //Temp fix

            for (int i = 0; i < count; i++)
            {
                var axie = AxieConfigs.Instance.GetRandom(axiesIgnore);
                slotTrans[i].gameObject.SetActive(true);

                slotTrans[i].GetComponentInChildren<TextMeshProUGUI>().text = AxieConfigs.Instance.GetInfoAxie(axie.axieId);

                var axieSlot = slotTrans[i].GetComponentInChildren<AxieInventorySlot>();
                axieSlot.ParseAxie(axie, 0f);
                axies.Add(axie);
            }           
        }
    }
}
