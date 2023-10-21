using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    [SerializeField] private Sprite commonSpr;
    [SerializeField] private Sprite commonOpenedSpr;
    [SerializeField] private Sprite rareSpr;
    [SerializeField] private Sprite rareOpenedSpr;
    [SerializeField] private Sprite legendSpr;
    [SerializeField] private Sprite legendOpenedSpr;

    public float rareMax;
    public int count;

    private bool isOpend;

    private void Start()
    {
        UpdateVirtual();
    }

    public void InitChest(float _rareMax, int _count)
    {
        rareMax = _rareMax;
        count = _count;
        UpdateVirtual();
    }

    private void UpdateVirtual()
    {
        var chestImg = GetComponent<Image>();
        if (rareMax <= 0.1f) chestImg.sprite = isOpend? commonOpenedSpr: commonSpr;
        else if (rareMax <= 0.8f) chestImg.sprite = isOpend ? rareOpenedSpr: rareSpr;
        else chestImg.sprite = isOpend ? legendOpenedSpr : legendSpr;
    }

    private void OpenChest()
    {
        var items = ItemConfigs.Instance.GetItemsInChests(rareMax, count);
        FindObjectOfType<Inventory>().SpawnItems(items, transform);
        UpdateVirtual();
    }

    public virtual void PointClickHandler(BaseEventData data)
    {
        Debug.Log("Click");
        if (!isOpend)
        {
            isOpend = true;
            OpenChest();
        }
    }
}
