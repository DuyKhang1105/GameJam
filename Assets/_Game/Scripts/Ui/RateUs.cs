using System.Collections;
using System.Collections.Generic;
// using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RateUs : PopupBase
{
    [SerializeField] Button[] btnArray = new Button[0];
    [SerializeField] Image[] imgArray = new Image[0];
    [SerializeField] Sprite goldStar;
    [SerializeField] Sprite silverStar;
    public Button btnRate;
    [SerializeField] string androidId, iosId;
    private int rateCount;
    void Start()
    {
        // UIManager.onRate += OnShow;
        for (int i = 0; i < btnArray.Length; i++)
        {
            int starIndex = i;
            var btn = btnArray[starIndex];

            btn.onClick.AddListener(() =>
            {
                OnChooseStar(starIndex);
            });
        }
        btnRate.onClick.AddListener(() =>
       {
           AudioManager.Instance.PlaySound(AudioType.CLICK);
           RateForUs(rateCount);
       });

#if UNITY_ANDROID
        androidId = Application.identifier;
#endif
    }

    public override void OnShow()
    {
        base.OnShow();
        rateCount = 0;

        for (int i = 0; i < imgArray.Length; i++)
        {
            imgArray[i].sprite = silverStar;
        }
        btnRate.interactable = false;
    }


    private void OnChooseStar(int star)
    {
        this.rateCount = star;
        StartCoroutine(I_Choose());
    }

    private IEnumerator I_Choose()
    {
        for (int i = 0; i < imgArray.Length; i++)
        {
            if (i <= rateCount)
                imgArray[i].sprite = goldStar;
            else
                imgArray[i].sprite = silverStar;
            yield return new WaitForSeconds(0.1f);
        }

        // for (int i = 0; i < rateCount + 1; i++)
        // {
        //     imgArray[i].sprite = goldStar;
        // }
        btnRate.interactable = true;
    }

    public void RateForUs(int rateCount)
    {
        this.rateCount = rateCount;
        StartCoroutine(I_Rate(rateCount));
    }

    private IEnumerator I_Rate(int rateCount)
    {
        float delay = rateCount * 0.1f + 0.5f;

        // PlayerPrefs.SetInt("rate", 1);

        if (rateCount >= 4)
        {
#if UNITY_ANDROID
            Application.OpenURL("market://details?id=" + androidId);
#elif UNITY_IPHONE
            Application.OpenURL("itms-apps://itunes.apple.com/app/id"+iosId);
#endif

        }
        yield return new WaitForSeconds(delay);
        yield return new WaitForSeconds(0.15f);
        OnHide();
    }
}
