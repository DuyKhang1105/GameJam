using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeedAxiePopup : MonoBehaviour
{
    [SerializeField] private Button confirmBtn;
    [SerializeField] private Button closeBtn;
    [SerializeField] private List<Transform> slotTrans;

    private int indexSelected;
    private bool isSelected;
    public Action onFeeded;
    public Action onCanceled;

    private void Awake()
    {
        confirmBtn.onClick.AddListener(Confirm);
        closeBtn.onClick.AddListener(Cancel);
        for (int i = 0; i < slotTrans.Count; i++)
        {
            int index = i;
            slotTrans[index].GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectAxie(index);
            });
        }
    }

    private void OnEnable()
    {
        isSelected = false;
        GameUI.Instance.bg.SetActive(true);
        slotTrans.ForEach(t => { t.gameObject.SetActive(false); });
        {
            var axies = FindObjectOfType<AxieInventory>().axies;
            var progresses = FindObjectOfType<AxieInventory>().progresses;
            for (int i = 0; i < axies.Count; i++)
            {
                slotTrans[i].gameObject.SetActive(true);

                slotTrans[i].GetComponentInChildren<TextMeshProUGUI>().text = AxieConfigs.Instance.GetInfoAxie(axies[i].axieId);

                var axieSlot = slotTrans[i].GetComponentInChildren<AxieInventorySlot>();
                axieSlot.ParseAxie(axies[i], progresses[i]);
            }
        }
    }


    private void SelectAxie(int index)
    {
        slotTrans[indexSelected].GetComponent<Image>().color = Color.black;
        indexSelected = index;
        slotTrans[indexSelected].GetComponent<Image>().color = Color.yellow;
    }

    private void Confirm()
    {
        if (!isSelected)
        {
            isSelected = true;
            var axieIneventory = FindObjectOfType<AxieInventory>();
            axieIneventory.FeedAxie(indexSelected);

            onFeeded?.Invoke();
            GameUI.Instance.bg.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    private void Cancel()
    {
        onCanceled?.Invoke();
        GameUI.Instance.bg.SetActive(false);
        gameObject.SetActive(false);
    }
}
