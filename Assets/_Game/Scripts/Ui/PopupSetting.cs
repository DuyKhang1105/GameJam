using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PopupSetting : PopupBase
{
    public Button btnSound;
    public Button btnVibrate;
    public Button btnFlash;
    public Button btnClose;

    public GameObject iconSound_On;
    public GameObject iconSound_Off;

    public GameObject iconVibrate_On;
    public GameObject iconVibrate_Off;

    public GameObject iconFlash_On;
    public GameObject iconFlash_Off;

    bool isSoundOn = true;
    bool isVibrateOn = true;
    bool isFlash = true;

    //public RectTransform panel;
    //public float moveDuration;

    public Action hideSettingCallback;
    public override void OnInit()
    {
        isSoundOn = false;// DataManager.Instance.GetData<DataUser>().HasSound();
        isVibrateOn = false; // DataManager.Instance.GetData<DataUser>().HasVibration();
        isFlash = false; // DataManager.Instance.GetData<DataUser>().HasFlash();

        btnSound.onClick.AddListener(() =>
        {
            OnSoundButtonClick();
            AudioManager.Instance.PlaySound(AudioType.CLICK);
            //   SoundManager.Instance.PlayOneShot(AudioSrcType.button);
        }
        );


        btnVibrate.onClick.AddListener(() =>
        {
            OnVibrateButtonClick();
            AudioManager.Instance.PlaySound(AudioType.CLICK);
            //  SoundManager.Instance.PlayOneShot(AudioSrcType.button);
        });


        btnClose.onClick.AddListener(() =>
        {
            OnHide();
       
            AudioManager.Instance.PlaySound(AudioType.CLICK);
            //   SoundManager.Instance.PlayOneShot(AudioSrcType.button);
        });


        btnFlash.onClick.AddListener(() => {
            OnFlashButtonClick();
            AudioManager.Instance.PlaySound(AudioType.CLICK);
        });


        base.OnInit();
    }


    public void SubscribeCallback( Action callback )
    {
        hideSettingCallback += callback;
    }


    public override void OnShow()
    {
        base.OnShow();
        Refesh();

       // InputManager.Instance.SetActive(false);
       // panel.anchoredPosition = new Vector2(panel.anchoredPosition.x + 1000f, panel.anchoredPosition.y);
        //panel.DOAnchorPosX(0f, moveDuration);
    }


    private void Refesh()
    {
        iconSound_On.SetActive(isSoundOn);
        iconSound_Off.SetActive(!isSoundOn);

        iconVibrate_On.SetActive(isVibrateOn);
        iconVibrate_Off.SetActive(!isVibrateOn);

        iconFlash_On.SetActive(isFlash);
        iconFlash_Off.SetActive(!isFlash);
    }


    private void OnVibrateButtonClick()
    {
        isVibrateOn = !isVibrateOn;
        DataManager.Instance.GetData<DataUser>().SetVibration(isVibrateOn);
        Refesh();
    }


    private void OnSoundButtonClick()
    {
        isSoundOn = !isSoundOn;
        DataManager.Instance.GetData<DataUser>().SetSound(isSoundOn);
        Refesh();
    }

    private void OnFlashButtonClick()
    {
        isFlash = !isFlash;
        //DataManager.Instance.GetData<DataUser>().SetFlash(isFlash);
        Refesh();
    }

    public override void OnHide()
    {
        base.OnHide();
        //InputManager.Instance.SetActive(true);

        hideSettingCallback?.Invoke();
        hideSettingCallback = null;
        //panel.DOAnchorPos(new Vector2(panel.anchoredPosition.x - 1000f, panel.anchoredPosition.y), moveDuration).OnComplete(() =>
        //{

        //});
    }

}
