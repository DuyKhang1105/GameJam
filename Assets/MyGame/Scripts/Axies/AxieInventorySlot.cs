using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AxieInventorySlot : MonoBehaviour
{
    [SerializeField] private Image circle;
    [SerializeField] private Image star;
    [SerializeField] private Sprite starOffSpr;
    [SerializeField] private Sprite starOnSpr;

    [SerializeField] private Transform axieTran;

    public void ParseAxie(AxieConfig config, float progress) //progress 0 =>1
    {
        if (axieTran.childCount > 0) { Destroy(axieTran.GetChild(0).gameObject); };
        var axieUI = Instantiate(config.graphicUI, axieTran);
        axieUI.transform.localScale = Vector3.one;
        axieUI.transform.localPosition = Vector3.zero;

        circle.fillAmount = progress;
        circle.color = progress >0.99f ? Color.yellow : Color.green;
        star.sprite = progress > 0.99f? starOnSpr: starOffSpr;
    }
}
