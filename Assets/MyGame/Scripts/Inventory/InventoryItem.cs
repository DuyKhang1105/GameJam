using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : Drag
{
    [SerializeField] private int width = 1;
    [SerializeField] private int height = 1;

    public bool isInInventory;

    private List<InventorySlot> placeds;

    //public Action onDropedToInventory;
    //public Action<GameObject> onDropedOutInventory;

    private ItemConfig itemConfig;

    private Vector3 startDragPos;

    [Header("Sound")]
    [SerializeField] private AudioClip lootSnd;

    public void ParseItem(ItemConfig config)
    {
        itemConfig = config;
        width = config.width; height = config.height;
        GetComponent<RectTransform>().sizeDelta = new Vector2(width * Inventory.SlotSize, height * Inventory.SlotSize);
        GetComponent<Image>().sprite = config.sprite;
        if (UnityEngine.Random.value < 0.5f) Rotate();
    }

    public override void DragHandler(BaseEventData data)
    {
        if (BattleSystem.Instance.state == BattleState.LOOTITEM)
        {
            base.DragHandler(data);
        }
    }

    public override void PointDownHandler(BaseEventData data)
    {
        base.PointDownHandler(data);

        SoundManager.Instance.PlayButtonSound();
        startDragPos = GetPoint(data);
    }

    public override void PointUpHandler(BaseEventData data)
    {
        base.PointUpHandler(data);

        var endDragPos = GetPoint(data);
        var deltaDrag = Vector3.Distance(startDragPos, endDragPos);
        //Debug.Log("deltaDrag " + deltaDrag);
        if (deltaDrag > 10f)
        {
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
                isInInventory = true;
                placeds = temp;

                SoundManager.Instance.PlayOneShot(lootSnd);

                //set position
                Vector3 sumPos = Vector3.zero;
                placeds.ForEach(x => sumPos += x.transform.position);
                Vector3 center = sumPos / placeds.Count;
                transform.position = center;

                placeds?.ForEach((x) =>
                {
                    x.isFree = false;
                });

                switch (itemConfig.type)
                {
                    case ItemType.Equipment:
                        placeds?.ForEach((x) =>
                        {
                            x.GetComponent<Image>().color = itemConfig.useable ? Color.green : Color.yellow;
                        });
                        //update all equip
                        float critRare = 0f;
                        float avoidRare = 0f;
                        var allEquips = FindObjectsOfType<InventoryItem>().Where(x => x.isInInventory).ToList();
                        foreach (var equipItem in allEquips)
                        {
                            var equip = ItemConfigs.Instance.equips.Find(x=>x.id == equipItem.itemConfig.id);
                            if (equip != null)
                            {
                                if (equip.equipType == EquipType.Shoe)
                                {
                                    avoidRare += equip.value + GetBonusValue(equip.id) + GetAxieBonusEquip(equip.id);
                                }
                                else if (equip.equipType == EquipType.Helmet)
                                {
                                    critRare += equip.value + GetBonusValue(equip.id) + GetAxieBonusEquip(equip.id);
                                }
                            }                          
                        }
                        BattleSystem.Instance.heroUnit.criticalRate = critRare;
                        BattleSystem.Instance.heroUnit.dodgeRate = avoidRare;
                        break;
                    case ItemType.Support:
                        placeds?.ForEach((x) =>
                        {
                            x.GetComponent<Image>().color = itemConfig.useable ? Color.green : Color.yellow;
                        });
                        break;
                    case ItemType.Jewelry:
                        Debug.Log("Drop jewelry");
                        UpdateJewelryVirtual();
                        break;
                }
                transform.SetAsFirstSibling();
                return;
            }
            else
            {
                placeds = null;
                isInInventory = false;
                UpdateJewelryVirtual();
                transform.SetAsLastSibling();
            }
        }
    }

    private void UpdateJewelryVirtual()
    {
        Debug.Log("Upgrade jewelry Virtual");

        var currentJewelries = FindObjectsOfType<InventoryItem>().ToList().FindAll(x => x.isInInventory && x.itemConfig.type == ItemType.Jewelry);
        //Debug.Log("Jewelries: " + string.Join(", ", currentJewelries.Select(x => x.itemConfig.id)));
        var allSets = ItemConfigs.Instance.GetAllJewelrySets(currentJewelries.Select(x => x.itemConfig).ToList());

        Debug.Log("Sets: " + allSets.Count);

        currentJewelries.ForEach(item => item.placeds?.ForEach(x => x.GetComponent<Image>().color = Color.gray));
        foreach (var set in allSets)
        {
            var items = currentJewelries.FindAll(x => x.itemConfig.id == set.chainId || x.itemConfig.id == set.ringId);
            items.ForEach(item => item.placeds?.ForEach((x) => x.GetComponent<Image>().color = Color.yellow));
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
        SoundManager.Instance.PlayButtonSound();
    }

    private void ShowInfo()
    {
        Debug.Log("Show info");
    }

    protected override void OnClick()
    {
        base.OnClick();

        if (isInInventory && itemConfig != null)
        {
            transform.DOKill();
            transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);

            if (!itemConfig.useable)
            {
                Notification.Instance.ShowNoti("It's a unuseable item!!");
                return;
            }

            var battleSystem = FindObjectOfType<BattleSystem>();
            var axieInventory = FindObjectOfType<AxieInventory>();
            if (battleSystem.state == BattleState.HEROTURN)
            {
                var hero = FindObjectOfType<HeroControl>();
                var unit = hero.GetComponent<HeroUnit>();

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
                                        int damage = (int)equip.value + (int)GetBonusValue(equip.id) + (int)GetAxieBonusEquip(equip.id);
                                        unit.damage = damage;
                                        battleSystem.OnAttackButton();
                                        Debug.Log("Damage " + damage);
                                        Notification.Instance.ShowNoti($"Yee, Deal {damage} damages!!");
                                        break;
                                    case StatisticType.Shield:
                                        int shield = (int)equip.value + (int)GetBonusValue(equip.id) + (int)GetAxieBonusEquip(equip.id);
                                        battleSystem.OnShielButton(shield);
                                        Debug.Log("Shield " + shield);
                                        Notification.Instance.ShowNoti($"Oho, Shield {shield}");
                                        break;
                                }
                            }

                            break;
                        case ItemType.Support:
                            Debug.Log("Support " + itemConfig.id);
                            var support = ItemConfigs.Instance.supports.Find(x => x.id == itemConfig.id);
                            if (support != null)
                            {
                                float bonus = GetAxieBonusItem(support.id);
                                UseSupport(support.statistic, support.value + bonus);
                                UseSupport(support.statistic2, support.value2 + bonus); 
                            }
                            break;
                    }
                    //show stamina
                    BattleSystem.Instance.heroHUD.SetStamina(BattleSystem.Instance.heroUnit.currentStamina);
                }
                else
                {
                    //TODO show noti
                    //Debug.Log("Not enought stamina");
                    Notification.Instance.ShowNoti("Not enought stamina!!");
                }
            }
            else
            {
                Notification.Instance.ShowNoti("Just use at hero turn in combat!!");
            }
        }
    }

    private void DestroyItem()
    {
        placeds?.ForEach(x =>
        {
            x.isFree = true;
            x.GetComponent<Image>().color = Color.white;
        });
        Destroy(gameObject);
    }
    private void UseSupport(StatisticType statistic, float value)
    {
        switch (statistic)
        {
            case StatisticType.HP:
                SoundManager.Instance.PlayOneShot(GameUI.Instance.drinkSnd);
                BattleSystem.Instance.OnHeroHeal((int)value);
                DestroyItem();
                break;
            case StatisticType.Stamina:
                SoundManager.Instance.PlayOneShot(GameUI.Instance.drinkSnd);
                Debug.Log("Add stamina");
                BattleSystem.Instance.OnHeroStamina((int)value);
                DestroyItem();
                break;
            case StatisticType.Coin:
                SoundManager.Instance.PlayOneShot(GameUI.Instance.clickCoinSnd);
                GameUI.Instance.shopPopup.SetActive(true);
                GameUI.Instance.shopPopup.GetComponent<ShopPopup>().onBuyed = (arg) =>
                {
                    ParseItem(arg);
                };
                GameUI.Instance.shopPopup.GetComponent<ShopPopup>().onCanceled = () =>
                {  
                    BattleSystem.Instance.heroUnit.GetStamina(itemConfig.stamina); //return stamina
                };
                break;
            case StatisticType.UpgradePet:
                GameUI.Instance.feedAxiePopup.SetActive(true);
                GameUI.Instance.feedAxiePopup.GetComponent<FeedAxiePopup>().onFeeded = () =>
                {
                    DestroyItem();
                };
                GameUI.Instance.feedAxiePopup.GetComponent<FeedAxiePopup>().onCanceled = () =>
                {
                    BattleSystem.Instance.heroUnit.GetStamina(itemConfig.stamina); //return stamina
                };
                break;
        }
        //show stamina
        BattleSystem.Instance.heroHUD.SetStamina(BattleSystem.Instance.heroUnit.currentStamina);
    }

    public static float GetBonusValue(string id)
    {
        float bonusValue = 0f;
        var currentJewelries = FindObjectsOfType<InventoryItem>().Where(x => x.isInInventory && x.itemConfig.type == ItemType.Jewelry);
        //Debug.Log("Jewelries: " + string.Join(", ", currentJewelries.Select(x => x.itemConfig.id)));
        var allSets = ItemConfigs.Instance.GetAllJewelrySets(currentJewelries.Select(x => x.itemConfig).ToList());
        var set = allSets.Find(x => x.targetId == id);
        if (set != null) bonusValue = set.value;
        return bonusValue;
    }

    public static float GetAxieBonusEquip(string equipId)
    {
        var axieInventory = FindObjectOfType<AxieInventory>();
        var eqyipBonus = 0f;
        var equip = ItemConfigs.Instance.equips.Find(x => x.id == equipId);

        var axieBuffEquip = axieInventory.axies.Find(x => x.skillType == AxieSkillType.UpgradeAllEquipAndItem);
        if (axieBuffEquip != null && equip!=null)
        {
            if (equip.equipType == EquipType.Shoe || equip.equipType == EquipType.Helmet)
                eqyipBonus = 0.01f * axieBuffEquip.skillValue;
            else eqyipBonus = axieBuffEquip.skillValue;
        }
        return eqyipBonus;
    }

    public static float GetAxieBonusItem(string itemId)
    {
        var axieInventory = FindObjectOfType<AxieInventory>();
        var itemBonus = 0f;
        var item = ItemConfigs.Instance.supports.Find(x => x.id == itemId);

        var axieBuffEquip = axieInventory.axies.Find(x => x.skillType == AxieSkillType.UpgradeItem || x.skillType == AxieSkillType.UpgradeAllEquipAndItem);
        if (axieBuffEquip != null && item != null)
        {
            itemBonus = axieBuffEquip.skillValue;
        }
        return itemBonus;
    }

    protected override void OnRightClick()
    {
        base.OnRightClick();

        var axieInventory = FindObjectOfType<AxieInventory>();
        string bubbleText = "";
        switch (itemConfig.type)
        {
            case ItemType.Equipment:
                var equip = ItemConfigs.Instance.equips.Find(x=>x.id == itemConfig.id);
                float bonusValue = GetBonusValue(equip.id);            
                bubbleText = $"{equip.statistic.ToString()} {equip.value}";
                if (bonusValue>0) bubbleText += $" + {bonusValue}";

                
                float axieBonusEquip = GetAxieBonusEquip(equip.id);
                if ( axieBonusEquip>0f)
                {
                    bubbleText += $" + {axieBonusEquip}";
                }
                break;
            case ItemType.Support:
                var support = ItemConfigs.Instance.supports.Find(x=>x.id == itemConfig.id);
                switch (support.statistic)
                {
                    case StatisticType.Coin:
                        bubbleText = "Buy a item in shop";
                        break;
                    case StatisticType.UpgradePet:
                        bubbleText = "Feed to pet";
                        break;
                    default:
                        float axieBonusItem = 0f;

                        var axieBuffItem = axieInventory.axies.Find(x => x.skillType == AxieSkillType.UpgradeItem || x.skillType == AxieSkillType.UpgradeAllEquipAndItem);
                        if (axieBuffItem != null) axieBonusItem = axieBuffItem.skillValue;

                        bubbleText = $"{support.statistic.ToString()} {support.value}";
                        if (axieBonusItem > 0) bubbleText += $" + {axieBonusItem}";
                        
                        if (support.statistic2 != StatisticType.None)
                        {
                            bubbleText += $"\n{support.statistic2.ToString()} {support.value2}";
                            if (axieBonusItem > 0) bubbleText += $" + {axieBonusItem}";
                        }
                        break;
                }
                break;
            case ItemType.Jewelry:
                bubbleText = "It's is a Jewelry";
                break;
        }

        if (itemConfig.useable) bubbleText += $"\n <size=25>-Use ({itemConfig.stamina} stamina)-</size>";
        else bubbleText += $"\n <size=25>-UnUseable-</size>";
        BubbleFx.Show(transform.position, bubbleText);
    }
}
