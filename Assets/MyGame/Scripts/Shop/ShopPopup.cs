using DG.Tweening.Plugins.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPopup : MonoBehaviour
{
    [SerializeField] private Button confirmBtn;
    [SerializeField] private Button cancelBtn;
    private int indexSelected;

    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Sprite itemNormalSpr;
    [SerializeField] private Sprite itemHighlightSpr;
    [SerializeField] private Transform itemParent;
    private List<GameObject> items;
    private List<ItemConfig> itemConfigs;

    public Action<ItemConfig> onBuyed;
    public Action onCanceled;

    private void Awake()
    {
        confirmBtn.onClick.AddListener(Confirm);
        cancelBtn.onClick.AddListener(Cancel);
    }

    private void Start()
    {
        OpenShop();    
    }

    private void OpenShop()
    {
        GameUI.Instance.bg.SetActive(true);
        Debug.Log("Start shop");
        items = new List<GameObject>();
        itemConfigs = ItemConfigs.Instance.configs.FindAll(x => x.type == ItemType.Support && !x.id.Contains("Coin"));
        if (itemConfigs != null)
        {
            foreach (ItemConfig itemConfig in itemConfigs)
            {
                int index = items.Count;
                var item = Instantiate(itemPrefab, itemParent);
                item.SetActive(true);
                item.transform.GetChild(0).GetComponent<Image>().sprite = itemConfig.sprite;
                item.GetComponent<Button>().onClick.AddListener(() =>
                {
                    indexSelected = index;
                    items.ForEach(x => x.GetComponent<Image>().sprite = itemNormalSpr);
                    items[index].GetComponent<Image>().sprite = itemHighlightSpr;
                });
                items.Add(item);
            }
        }
    }
    private void Confirm()
    {
        var itemConfig = (itemConfigs[indexSelected]);
        onBuyed?.Invoke(itemConfig);
        GameUI.Instance.bg.SetActive(false);
        gameObject.SetActive(false); 
    }

    private void Cancel()
    {
        onCanceled?.Invoke();
        gameObject.SetActive(false);
    }
}
