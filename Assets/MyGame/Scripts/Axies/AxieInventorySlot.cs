using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AxieInventorySlot : MonoBehaviour
{
    [SerializeField] private Image circle;
    [SerializeField] private Image star;
    [SerializeField] private Sprite starOffSpr;
    [SerializeField] private Sprite starOnSpr;

    [SerializeField] private Transform axieTran;

    private AxieConfig axie;

    public void ParseAxie(AxieConfig config, float progress) //progress 0 =>1
    {
        axie = config;

        if (axieTran.childCount > 0) { Destroy(axieTran.GetChild(0).gameObject); };
        var axieUI = Instantiate(config.graphicUI, axieTran);
        axieUI.transform.localScale = Vector3.one;
        axieUI.transform.localPosition = Vector3.zero;

        circle.fillAmount = progress;
        circle.color = progress >0.99f ? Color.yellow : Color.green;
        star.sprite = progress > 0.99f? starOnSpr: starOffSpr;
    }

    public virtual void PointUpHandler(BaseEventData data)
    {
        PointerEventData pointerEventData = data as PointerEventData;
        Debug.Log("Button " + pointerEventData.button);
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            //OnClick();
        }
        else if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    private void OnRightClick()
    {
        if (axie!=null)
        {
            var bubbleText = AxieConfigs.Instance.GetInfoAxie(axie.axieId);
            BubbleFx.Show(transform.position, bubbleText);
        }
    }
}
