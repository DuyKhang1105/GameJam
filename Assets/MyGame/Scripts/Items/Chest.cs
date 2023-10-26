using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class ChestData
{
    public float rareMax;
    public int count;
    public List<string> itemIds;
}

public class Chest : MonoBehaviour
{
    [SerializeField] private Sprite commonSpr;
    [SerializeField] private Sprite commonOpenedSpr;
    [SerializeField] private Sprite rareSpr;
    [SerializeField] private Sprite rareOpenedSpr;
    [SerializeField] private Sprite legendSpr;
    [SerializeField] private Sprite legendOpenedSpr;

    [SerializeField] private AudioClip openSnd;

    public ChestData chestData;

    public bool isOpend;

    private void OnEnable()
    {
        UpdateVirtual();
    }

    public void InitChest(float _rareMax, int _count, List<string> itemIds)
    {
        chestData = new ChestData();
        chestData.rareMax = _rareMax;
        chestData.count = _count;
        chestData.itemIds = new List<string>(itemIds);
        UpdateVirtual();
    }

    private void UpdateVirtual()
    {
        var chestImg = GetComponent<Image>();
        if (chestData.rareMax <= 0.3f) chestImg.sprite = isOpend? commonOpenedSpr: commonSpr;
        else if (chestData.rareMax <= 0.6f) chestImg.sprite = isOpend ? rareOpenedSpr: rareSpr;
        else chestImg.sprite = isOpend ? legendOpenedSpr : legendSpr;
    }

    private void OpenChest()
    {
        SoundManager.Instance.PlayOneShot(openSnd);

        int count = chestData.count - chestData.itemIds.Count;
        var items = new List<ItemConfig>();
        chestData.itemIds.ForEach(x=> items.Add(ItemConfigs.Instance.GetItemConfig(x)));
        if (count > 0)
        {
            items.AddRange(ItemConfigs.Instance.GetItemsInChests(chestData.rareMax, count));
        }       
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
