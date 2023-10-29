using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class ListInventorySlot
{
    public List<InventorySlot> slots = new List<InventorySlot>();
}

public class Inventory : MonoBehaviour
{
    public Canvas canvas;
    [SerializeField] private List<InventorySlot> slotImages;

    [Header("Dragable")]
    [SerializeField] private Transform dragParent;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject confrimBtn;

    [Header("Drop")]
    [SerializeField] private List<Transform> dropPos;

    public const int Width = 7;
    public const int Height = 3;
    public const int SlotCount = Width * Height;
    public const float SlotSize = 120f;

    [NonSerialized]
    public InventorySlot[,] slots;
    [NonSerialized]
    public InventorySlot pointEnterSlot;

    private List<InventoryItem> items;
    private List<GameObject> dragItems = new List<GameObject>();

    public int level;

    private void Awake()
    {
        confrimBtn.GetComponent<Button>().onClick.AddListener(Confirm);
    }

    private void Start()
    {
        slots = new InventorySlot[Width, Height];
        level = 1;
        UpgradeLevel(1);

        var axieInventory = FindObjectOfType<AxieInventory>();
        axieInventory.onChangeList += OnChangeAxieList;
    }

    [ContextMenu("Change axie list")]
    private void OnChangeAxieList()
    {
        Debug.Log("Change list axies");
        CheckAxieUpgradeSlots();
    }

    private void CheckAxieUpgradeSlots()
    {
        var axieInventory = FindObjectOfType<AxieInventory>();
        var axieUpgradeChest = axieInventory.axies.Find(x => x.skillType == AxieSkillType.ExtensionSlot);
        if (axieUpgradeChest != null)
        {
            for (int i = 0; i < axieUpgradeChest.skillValue; i++)
            {
                int col = 0;
                int row = i;
                slotImages[i].ParseSlot(col, row, true, false);
            }
        }
        else
        {
            for (int i = 0; i < 3; i++) //just 3 slots
            {
                bool isFree = true;
                bool isLocked = true;
                int col = i / Height;
                int row = i % Height;
                slotImages[i].ParseSlot(col, row, isFree, isLocked);
                slots[col, row] = slotImages[i];
            }
        }

    }

    public void UpgradeLevel(int level = 1)
    {
        for (int i = 0; i < slotImages.Count; i++)
        {
            bool isFree = true;
            bool isLocked = false;

            if (i < 3) {
                continue;
            }
            else if (i < 12) {
                isLocked = level < 1;
            }
            else if (i < 15) {
                isLocked = level < 2;
            }
            else if (i < 18) {
                isLocked = level < 3;
            }
            else
            {
                isLocked = level < 4;
            }

            int col = i / Height;
            int row = i % Height;
            slotImages[i].ParseSlot(col, row, isFree, isLocked);

            slots[col, row] = slotImages[i];
        }

        CheckAxieUpgradeSlots();
    }

    public InventorySlot DragToSlot(Vector3 pos)
    {
        //Debug.Log("Drag to: " + pos);
        Debug.DrawLine(transform.position, pos, Color.red, 1f);

        var deltaPos = pos - transform.position;
        //Debug.Log("Delta pos: " + deltaPos);
        int col = (int)(Width / 2f + (deltaPos.x / SlotSize));
        int row = (int)(Height/ 2f - (deltaPos.y / SlotSize)); 
        //Debug.Log($"{col}, {row}");
        return GetSlot(col, row);
    }

    private InventorySlot GetSlot(int col, int row)
    {
        if (col >= 0 && col < Width && row >= 0 && row < Height)
        {
            return slots[col, row];
        }
        return null;
    }

    private List<ListInventorySlot> GetListPlace(InventorySlot slot, int width, int height)
    {
        List<ListInventorySlot> result = new List<ListInventorySlot>();


        ListInventorySlot lst0 = new ListInventorySlot();
        for (int iCol = 0; iCol < width; iCol++)
        {
            for (int iRow = 0; iRow < height; iRow++)
            {
                lst0.slots.Add(GetSlot(slot.col + iCol, slot.row + iRow));
            }
        }
        result.Add(lst0);

        ListInventorySlot lst1 = new ListInventorySlot();
        for (int iCol = -width + 1; iCol <= 0; iCol++)
        {
            for (int iRow = -height + 1; iRow <= 0; iRow++)
            {
                lst1.slots.Add(GetSlot(slot.col + iCol, slot.row + iRow));
            }
        }
        result.Add(lst1);

        //ListInventorySlot lst3 = new ListInventorySlot();
        //for (int iCol = 0; iCol < width; iCol++)
        //{
        //    for (int iRow = -height + 1; iRow <= 0; iRow++)
        //    {
        //        lst3.slots.Add(GetSlot(slot.col + iCol, slot.row + iRow));
        //    }
        //}
        //result.Add(lst3);

        //ListInventorySlot lst4 = new ListInventorySlot();
        //for (int iCol = -width + 1; iCol <= 0; iCol++)
        //{
        //    for (int iRow = 0; iRow < height; iRow++)
        //    {
        //        lst4.slots.Add(GetSlot(slot.col + iCol, slot.row + iRow));
        //    }
        //}
        //result.Add(lst4);

        return result;
    }

    public bool IsCanDrop(Vector3 position, int width, int height, out List<InventorySlot> listPlace)
    {
        listPlace = new List<InventorySlot>();
        pointEnterSlot = DragToSlot(position);

        if (pointEnterSlot == null) return false;

        var allListPlace = GetListPlace(pointEnterSlot, width, height);
        
        foreach (ListInventorySlot lst in allListPlace)
        {
            listPlace = lst.slots;
            if (listPlace.Any(x => x == null) || listPlace.Any(x => x != null && (!x.isFree || x.isLocked)))
            {
                //None
            }
            else
            {
                return true;
            }
        }
        
        return false;
    }


    public void SpawnItems(List<ItemConfig> lstItems, Transform from)
    {
        BattleSystem.Instance.state = BattleState.LOOTITEM;
        if (dragItems == null) dragItems = new List<GameObject>();
        foreach (var item in lstItems)
        {
            int index = dragItems.Count;
            var goItem = Instantiate(itemPrefab, dragParent);
            dragItems.Add(goItem);
            goItem.GetComponent<InventoryItem>().ParseItem(item);
            //goItem.GetComponent<InventoryItem>().onDropedToInventory = () =>
            //{
            //    dragItems[index] = null;
            //};
            //goItem.GetComponent<InventoryItem>().onDropedOutInventory = (arg) =>
            //{
            //    dragItems.Add(arg);
            //};
            Vector3 startPos = from.position;
            Vector3 endPos = dropPos[UnityEngine.Random.Range(0, dropPos.Count)].position;

            goItem.transform.localScale = Vector3.zero;
            goItem.transform.position = startPos;
            goItem.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            goItem.transform.DOJump(endPos, 1, 1, 0.3f);
        }
        confrimBtn.SetActive(true);
        GameUI.Instance.bg.SetActive(true);
    }

    private void Confirm()
    {
        dragItems.ForEach(x=> {
            if (x != null)
            {
                if (!x.GetComponent<InventoryItem>().isInInventory)
                {
                    Destroy(x);
                }
            }     
        });
        confrimBtn.SetActive(false);
        GameUI.Instance.bg.SetActive(false);
        GameUI.Instance.nextBtn.SetActive(true);
        GameUI.Instance.chest.SetActive(false);
        GameUI.Instance.startChest.SetActive(false);
        GameUI.Instance.axieChest.SetActive(false);

        SoundManager.Instance.PlayButtonSound();
    }

    //Test
    [ContextMenu("Test spawn items")]
    private void TestSpawnItems()
    {
        var lstItems = ItemConfigs.Instance.GetItemsInChests(UnityEngine.Random.value, 5);
        SpawnItems(lstItems, transform);
    }

    [ContextMenu("Upgrade level 2")]
    private void UpdateLevel2()
    {
        UpgradeLevel(2);
    }

    [ContextMenu("Upgrade level 3")]
    private void UpdateLevel3()
    {
        UpgradeLevel(3);
    }
}
