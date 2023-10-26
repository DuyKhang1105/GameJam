using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WikiPetContent : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;

    private void Awake()
    {
        foreach (var axieLevel in AxieConfigs.Instance.upgradeConfigs)
        {
            foreach (var axieId in axieLevel.axieIds)
            {
                var item = Instantiate(itemPrefab, transform);
                item.SetActive(true);
                var axie = AxieConfigs.Instance.GetAxieConfig(axieId);
                var axieGo = Instantiate(axie.graphicUI, item.transform.GetChild(0));
                item.transform.GetChild(1).gameObject.SetActive(axieLevel.axieIds.IndexOf(axieId) != axieLevel.axieIds.Count - 1); //show upgrade icon
                axieGo.transform.localPosition = new Vector3(0, -60, 0);
                axieGo.transform.localScale = new Vector3(-1, 1, 1);

                
                item.GetComponentInChildren<TextMeshProUGUI>().text = AxieConfigs.Instance.GetInfoAxie(axieId);
            }
        }
    }
}
