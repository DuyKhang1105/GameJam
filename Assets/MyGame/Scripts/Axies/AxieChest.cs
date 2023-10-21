using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AxieChest : MonoBehaviour
{
    [SerializeField] private Sprite normalSpr;
    [SerializeField] private Sprite openedSpr;
    private bool isOpend;

    private void Start()
    {
        UpdateVirtual();
    }

    private void UpdateVirtual()
    {
        GetComponent<Image>().sprite = isOpend? openedSpr : normalSpr;
    }

    private void OpenChest()
    {
        var gameUI = FindObjectOfType<GameUI>();
        gameUI.axieChestPopup.gameObject.SetActive(true);
        UpdateVirtual();
    }

    public virtual void PointClickHandler(BaseEventData data)
    {
        Debug.Log("Click");
        if (!isOpend)
        {
            isOpend = true;
            OpenChest();
        }
    }
}
