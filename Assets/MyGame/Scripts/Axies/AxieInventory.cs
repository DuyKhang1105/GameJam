using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxieInventory : MonoBehaviour
{
    [SerializeField] private List<Transform> slotTrans;
    public List<AxieConfig> axies;

    public Action onChangeList;

    private void Start()
    {
        axies = new List<AxieConfig>();
    }

    private void UpdateVirtual()
    {
        slotTrans.ForEach(t => { 
            if (t.childCount > 0) Destroy(t.GetChild(0).gameObject);
            t.gameObject.SetActive(false); 
        });
        for (int i = 0; i < axies.Count; i++)
        {
            slotTrans[i].gameObject.SetActive(true);
            var axieUI = Instantiate(axies[i].graphicUI, slotTrans[i]);
            axieUI.transform.localScale = new Vector3 (-1f, 1f, 1f);
            axieUI.transform.localPosition = Vector3.up * -40f;
        }
    }

    public void AddAxie(AxieConfig axie)
    {
        axies.Add(axie);
        onChangeList?.Invoke();
        UpdateVirtual();
    }
}
