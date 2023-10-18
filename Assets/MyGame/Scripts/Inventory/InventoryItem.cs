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

    [SerializeField] private Inventory inventory;

    public bool inInventory;

    private List<InventorySlot> placeds;

    public override void DragHandler(BaseEventData data)
    {
        base.DragHandler(data);
    }

    public override void PointEnterHandler(BaseEventData data)
    {
        base.PointEnterHandler(data);
    }

    public override void PointUpHandler(BaseEventData data)
    {
        base.PointUpHandler(data);


        if (placeds != null)
        {
            placeds.ForEach(x => {
                x.isFree = true;
                x.GetComponent<Image>().color = Color.white;
            });
        }     

        List<InventorySlot> temp = null;
        if (inventory.IsCanDrop(transform.position,  width, height, out temp))
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
                    x.GetComponent<Image>().color = Color.green;
                });
            }

            transform.SetAsFirstSibling();
        }
        else if (placeds != null)
        {
            transform.SetAsLastSibling();
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
        if (inInventory)
        {
            //TODO interact
        }
    }
}
