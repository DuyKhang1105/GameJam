using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AxieChest : MonoBehaviour
{
    [SerializeField] private Sprite normalSpr;
    [SerializeField] private Sprite openedSpr;
    public bool isOpend;
    public int count = 3; 

    [SerializeField] private AudioClip openSnd;

    private void OnEnable()
    {
        UpdateVirtual();
    }

    private void UpdateVirtual()
    {
        GetComponent<Image>().sprite = isOpend? openedSpr : normalSpr;
    }


    private void OpenChest()
    {
        SoundManager.Instance.PlayOneShot(openSnd);
        GameUI.Instance.axieChestPopup.gameObject.SetActive(true);
        GameUI.Instance.axieChestPopup.GetComponent<AxieChestPopup>().OpenChest(count);
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
