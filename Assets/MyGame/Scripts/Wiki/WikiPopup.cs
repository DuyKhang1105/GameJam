using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WikiPopup : MonoBehaviour
{
    [SerializeField] private Sprite tabNormalSpr;
    [SerializeField] private Sprite tabHighlightSpr;
    [SerializeField] private List<GameObject> tabBtns;
    [SerializeField] private List<GameObject> tabContents;
    [SerializeField] private ScrollRect scrollRect;

    private int tabSelected;

    private void Awake()
    {
        for (int i = 0; i < tabBtns.Count; i++)
        {
            int index = i;
            tabBtns[i].GetComponent<Button>().onClick.AddListener(() =>
            {
                OnSelectTab(index);
            });
        }
    }

    private void OnSelectTab(int index)
    {
        tabSelected = index;
        Debug.Log("Select tab: " + tabSelected);
        tabBtns.ForEach(t => { t.GetComponent<Image>().sprite = tabNormalSpr; });
        tabBtns[index].GetComponent<Image>().sprite = tabHighlightSpr;
        tabContents.ForEach(t => t.SetActive(false));
        tabContents[index].SetActive(true);
        scrollRect.content = tabContents[index].GetComponent<RectTransform>();
    }

    private void Start()
    {
        OpenWiki();
    }

    private void OpenWiki()
    {
        GameUI.Instance.bg.SetActive(true);
        //TODO wiki
        OnSelectTab(0);
    }
}
