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
        UpdateVirtual();
    }

    private void UpdateVirtual()
    {
        slotTrans.ForEach(t => { 
            t.gameObject.SetActive(false); 
        });
        for (int i = 0; i < axies.Count; i++)
        {
            slotTrans[i].gameObject.SetActive(true);
            slotTrans[i].GetComponent<AxieInventorySlot>().ParseAxie(axies[i], 0f);
        }
    }

    public void AddAxie(AxieConfig axie)
    {
        axies.Add(axie);
        onChangeList?.Invoke();
        UpdateVirtual();
    }
}
