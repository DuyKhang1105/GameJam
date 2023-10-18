using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupNointernet : MonoBehaviour
{
    [SerializeField] private Button btnOk;
    public bool isShow = false;
    void Awake()
    {
        btnOk.onClick.AddListener(OnBtnOkClicked);
    }

    void Update()
    {
        if (isShow)
        {
            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
                Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                OnHide();
            }
        }

        if (Application.internetReachability == NetworkReachability.NotReachable && !isShow)
        {
            OnShow();
        }
    }

    private void OnShow()
    {
        if (isShow)
            return;
        isShow = true;
        this.gameObject.SetActive(isShow);
    }

    private void OnHide()
    {
        if (!isShow)
            return;
        isShow = false;
 
        this.gameObject.SetActive(isShow);
    }


    private void OnBtnOkClicked()
    {
        try
        {
#if UNITY_ANDROID
            using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.WIFI_SETTINGS"))
                {
                    currentActivityObject.Call("startActivity", intentObject);
                }
            }
#elif UNITY_IOS
        OnHide();
#endif
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}
