using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ListInventorySlot
{
    public List<InventorySlot> slots = new List<InventorySlot>();
}

public class Inventory : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private List<InventorySlot> slotImages;
    public const int Width = 5;
    public const int Height = 3;
    public const int SlotCount = 15;
    public const float SlotSize = 150f;

    public InventorySlot[,] slots;

    public InventorySlot pointEnterSlot;

    private void Start()
    {
        slots = new InventorySlot[Width, Height];
        UpgradeLevel(1);
    }

    public void UpgradeLevel(int level = 1)
    {
        for (int i = 0; i < slotImages.Count; i++)
        {
            bool isFree = true;
            bool isLocked = false;

            if (i < 3) {
                isLocked = level < 2;
            }
            else if (i < 12) {
                isLocked = level < 1;
            }
            else {
                isLocked = level < 3;
            }

            int col = i / Height;
            int row = i % Height;
            slotImages[i].ParseSlot(col, row, isFree, isLocked);

            slots[col, row] = slotImages[i];
        }
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

    //Test
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
