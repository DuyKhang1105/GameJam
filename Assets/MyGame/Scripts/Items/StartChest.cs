using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartChest : MonoBehaviour
{
    [SerializeField] private Sprite normalSpr;
    [SerializeField] private Sprite openedSpr;

    [SerializeField] private AudioClip openSnd;

    public bool isOpened;

    private void OnEnable()
    {
        UpdateVirtual();
    }

    private void UpdateVirtual()
    {
        var chestImg = GetComponent<Image>();
        chestImg.sprite = isOpened? openedSpr : normalSpr;
    }

    private void OpenChest()
    {
        SoundManager.Instance.PlayOneShot(openSnd);

        //TEST open all
        var items = ItemConfigs.Instance.configs;

        FindObjectOfType<Inventory>().SpawnItems(items, transform);
        UpdateVirtual();
    }

    public virtual void PointClickHandler(BaseEventData data)
    {
        Debug.Log("Click");
        if (!isOpened)
        {
            isOpened = true;
            OpenChest();
        }
    }
}
