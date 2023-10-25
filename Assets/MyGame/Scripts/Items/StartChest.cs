using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartChest : MonoBehaviour
{
    [SerializeField] private Sprite normalSpr;
    [SerializeField] private Sprite openedSpr;

    public bool isOpened;

    private void OnEnable()
    {
        UpdateVirtual();
    }

    private void UpdateVirtual()
    {
        var chestImg = GetComponent<Image>();
        chestImg.sprite = isOpened? openedSpr : normalSpr;
    }

    private void OpenChest()
    {
        var items = new List<ItemConfig>();
        items.Add(ItemConfigs.Instance.GetItemConfig("0_Equip_Weapon_Axe"));
        items.Add(ItemConfigs.Instance.GetItemConfig("5_Equip_Shield"));
        items.Add(ItemConfigs.Instance.GetItemConfig("11_Equip_Helmet"));


        //TEST
        items = ItemConfigs.Instance.configs;



        FindObjectOfType<Inventory>().SpawnItems(items, transform);
        UpdateVirtual();
    }

    public virtual void PointClickHandler(BaseEventData data)
    {
        Debug.Log("Click");
        if (!isOpened)
        {
            isOpened = true;
            OpenChest();
        }
    }
}
