using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public Image imgBG;
    public Button btnNext;
    public Button btnPrev;
    public Button btnPlay;
    public List<Sprite> sprTutorial;

    int index;

    private void Start()
    {
        if (PlayerPrefs.GetInt("Tutorial") == 1)
        {
            transform.gameObject.SetActive(false);
            return;
        }

        PlayerPrefs.SetInt("Tutorial", 1);

        btnPrev.gameObject.SetActive(false);
        btnPlay.gameObject.SetActive(false);
        index = 0;
        imgBG.sprite = sprTutorial[0];
    }

    public void OnNext()
    {
        index++;
        btnPrev.gameObject.SetActive(true);

        if (index >= sprTutorial.Count - 1)
        {
            btnNext.gameObject.SetActive(false);
            btnPrev.gameObject.SetActive(false);
            btnPlay.gameObject.SetActive(true);

            index = sprTutorial.Count - 1;
        }

        imgBG.sprite = sprTutorial[index];
    }

    public void OnPrev()
    {
        index--;

        if (index <= 0)
        {
            btnPrev.gameObject.SetActive(false);
            index = 0;
        }

        imgBG.sprite = sprTutorial[index];
    }

    public void OnPlay()
    {
        transform.gameObject.SetActive(false);
    }
}
