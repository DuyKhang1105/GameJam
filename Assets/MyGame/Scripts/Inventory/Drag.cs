using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    protected bool onSelect;
    protected bool onDrag;

    private Vector3 GetPoint(BaseEventData data)
    {
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
        //Debug.Log("Distance: " + distance);

        if (distance < 1f)
        {
            OnClick();
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
}
