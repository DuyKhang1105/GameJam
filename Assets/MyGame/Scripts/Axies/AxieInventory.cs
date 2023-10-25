using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxieInventory : MonoBehaviour
{
    [SerializeField] private List<Transform> slotTrans;
    public List<AxieConfig> axies;
    public List<float> progresses;

    public Action onChangeList;

    private void Start()
    {
        axies = new List<AxieConfig>();
        progresses = new List<float>();
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
            slotTrans[i].GetComponent<AxieInventorySlot>().ParseAxie(axies[i], progresses[i]);
        }
    }

    public void AddAxie(AxieConfig axie)
    {
        axies.Add(axie);
        progresses.Add(0f);
        onChangeList?.Invoke();
        UpdateVirtual();
    }

    public void FeedAxie(int index)
    {
        progresses[index] += 1f / 3f;
        //TODO check upgrade axie
        UpdateVirtual();
    }
}
