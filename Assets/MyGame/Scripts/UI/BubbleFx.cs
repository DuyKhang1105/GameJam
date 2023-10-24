using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BubbleFx : MonoBehaviour
{
    public static BubbleFx Show(Vector3 pos, string text)
    { 
        var go = GameUI.Instance.bubbleFx;
        Vector3 newPos = pos + new Vector3(-50f, 50f);
        go.transform.position = newPos;
        BubbleFx bubbleFx = go.GetComponent<BubbleFx>();
        bubbleFx.SetUp(text);
        return bubbleFx;
    }

    public void SetUp(string text)
    {
        GetComponentInChildren<TextMeshProUGUI>().text = text;
        StartCoroutine(IeReloadUI());
    }

    private IEnumerator IeReloadUI()
    {
        transform.localScale = Vector3.zero;
        GetComponent<LayoutGroup>().enabled = false;
        yield return new WaitForSeconds(0.1f);
        GetComponent<LayoutGroup>().enabled = true;
        transform.localScale = Vector3.one;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            gameObject.transform.localScale = Vector3.zero;
        }
    }
}
