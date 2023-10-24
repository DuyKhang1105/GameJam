using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour
{

    protected bool onSelect;
    protected bool onDrag;
    
    protected Vector3 GetPoint(BaseEventData data)
    {
        var canvas = FindObjectOfType<Inventory>().canvas;

        PointerEventData pointerData = data as PointerEventData;
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            pointerData.position,
            canvas.worldCamera,
            out position);

        return canvas.transform.TransformPoint(position);
    }

    public virtual void DragHandler(BaseEventData data)
    {
        onDrag = true;
        transform.position = GetPoint(data);
    }

    //Click
    private Vector3 pointDown;
    private Vector3 pointUp;

    public virtual void PointDownHandler(BaseEventData data)
    {
        onDrag = true;
        pointDown = GetPoint(data);
    }

    public virtual void PointUpHandler(BaseEventData data)
    {
        onDrag = false;
        pointUp = GetPoint(data);
      
        var distance = Vector3.Distance(pointDown, pointUp);
        if (distance < 10f)
        {
            PointerEventData pointerEventData = data as PointerEventData;
            Debug.Log("Button " + pointerEventData.button);
            if (pointerEventData.button == PointerEventData.InputButton.Left)
            {
                OnClick();
            }
            else if (pointerEventData.button == PointerEventData.InputButton.Right)
            {
                OnRightClick();
            }
        }
    }

    public virtual void PointEnterHandler(BaseEventData data)
    {
        onSelect = true;
    }

    public virtual void PointExitHandler(BaseEventData data)
    {
        onSelect = false;
    }

    protected virtual void OnClick()
    {
        Debug.Log("Click");
    }

    protected virtual void OnRightClick()
    {
        Debug.Log("Right click");
    }
}
