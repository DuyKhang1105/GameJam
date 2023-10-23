using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : Drag
{
    [SerializeField] private int width = 1;
    [SerializeField] private int height = 1;

    public bool isInInventory;

    private List<InventorySlot> placeds;

    public Action onDropedToInventory;
    public Action<GameObject> onDropedOutInventory;

    private ItemConfig itemConfig;

    private Vector3 startDragPos;

    public void ParseItem(ItemConfig config)
    {
        itemConfig = config;
        width = config.width; height = config.height;
        GetComponent<RectTransform>().sizeDelta = new Vector2(width * Inventory.SlotSize, height * Inventory.SlotSize);
        GetComponent<Image>().sprite = config.sprite;
        if (UnityEngine.Random.value < 0.5f) Rotate();
    }

    public override void PointDownHandler(BaseEventData data)
    {
        base.PointDownHandler(data);
        startDragPos = GetPoint(data);
    }

    public override void PointUpHandler(BaseEventData data)
    {
        base.PointUpHandler(data);

        var endDragPos = GetPoint(data);
        var deltaDrag = Vector3.Distance(startDragPos, endDragPos);
        Debug.Log("deltaDrag " + deltaDrag);
        if (deltaDrag > 10f) {
            if (placeds != null)
            {
                placeds.ForEach(x => {
                    x.isFree = true;
                    x.GetComponent<Image>().color = Color.white;
                });
            }

            List<InventorySlot> temp = null;
            var inventory = FindObjectOfType<Inventory>();
            if (inventory.IsCanDrop(transform.position, width, height, out temp))
            {
                placeds = temp;

                //set position
                Vector3 sumPos = Vector3.zero;
                placeds.ForEach(x => sumPos += x.transform.position);
                Vector3 center = sumPos / placeds.Count;
                transform.position = center;

                if (placeds != null)
                {
                    placeds.ForEach((x) => {
                        x.isFree = false;
                        x.GetComponent<Image>().color = itemConfig.useable? Color.green : Color.yellow;
                    });
                    isInInventory = true;
                    onDropedToInventory?.Invoke();
                }

                transform.SetAsFirstSibling();
            }
            else if (placeds != null)
            {
                transform.SetAsLastSibling();
                onDropedOutInventory?.Invoke(gameObject);
            }
        }
        else
        {
            if (placeds != null)
            {
                //set position
                Vector3 sumPos = Vector3.zero;
                placeds.ForEach(x => sumPos += x.transform.position);
                Vector3 center = sumPos / placeds.Count;
                transform.position = center;
            }
        } 
    }

    private void Update()
    { 
        if (Input.GetMouseButtonDown(1) && onSelect)
        { 
            ShowInfo();
        }

        if (Input.GetKeyDown("r") && onDrag)
        {
            Rotate();
        }
    }

    private void Rotate()
    {
        Debug.Log("Rotate");
        transform.localRotation = Quaternion.Euler(transform.eulerAngles - Vector3.forward * 90);
        int temp = width;
        width = height;
        height = temp;   
    }

    private void ShowInfo()
    {
        Debug.Log("Show info");
    }

    protected override void OnClick()
    {
        base.OnClick();

        if (isInInventory && itemConfig!=null)
        {
            transform.DOKill();
            transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);

            if (!itemConfig.useable)
            {
                Notification.Instance.ShowNoti("It's a unuseable item!!");
                return;
            }

            var battleSystem = FindObjectOfType<BattleSystem>();
            if (battleSystem.state == BattleState.HEROTURN)
            {
                var hero = FindObjectOfType<HeroControl>();
                var unit = hero.GetComponent<Unit>();

                if (unit.TakeStamina(itemConfig.stamina))
                {
                    switch (itemConfig.type)
                    {
                        case ItemType.Equipment:
                            var equip = ItemConfigs.Instance.equips.Find(x => x.id == itemConfig.id);
                            if (equip != null)
                            {
                                switch (equip.statistic)
                                {
                                    case StatisticType.Attack:
                                        int damage = (int)equip.value;
                                        unit.damage = damage; //TODO cal damage
                                        battleSystem.OnAttackButton();
                                        Debug.Log("Damage " + damage);
                                        Notification.Instance.ShowNoti($"Yee, Deal {damage} damages!!");
                                        break;
                                    case StatisticType.Shield:
                                        int shield = (int)equip.value;
                                        unit.Shield(shield);
                                        battleSystem.OnHealButton();
                                        Debug.Log("Shield " + shield);
                                        Notification.Instance.ShowNoti($"Oho, Shield {shield}");
                                        break;
                                }
                            }

                            break;
                        case ItemType.Support:
                            break;
                    }
                }
                else
                {
                    //TODO show noti
                    //Debug.Log("Not enought stamina");
                    Notification.Instance.ShowNoti("Not enought stamina!!");
                }
            }
            
        }
    }
}
