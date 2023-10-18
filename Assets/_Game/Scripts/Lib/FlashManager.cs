//using NatSuite.Devices;
using UnityEngine;
using DG.Tweening;
public class FlashManager : MonoSingleton<FlashManager>
{
    /*
    //private CameraDevice cameraDevice;

    private void Awake()
    {
        RequestFlash();
    }



    private void RequestFlash()
    {
#if !UNITY_EDITOR
        InitFlash();
#endif
    }
    public async void InitFlash()
    {
#if (UNITY_ANDROID || UNITY_IOS)
        var permissionStatus = await MediaDeviceQuery.RequestPermissions<CameraDevice>();
        if (permissionStatus != PermissionStatus.Authorized)
        {
            PlayerPrefs.SetInt("PermissionFlash", 0);
            Debug.LogError("User did not grant camera permissions");
            return;
        }
        PlayerPrefs.SetInt("PermissionFlash", 1);
        // Create a device query to find camera devices
        var filter = MediaDeviceCriteria.CameraDevice;
        var query = new MediaDeviceQuery(filter);
        cameraDevice = query.current as CameraDevice;
#endif
    }


    public void FlashMode()
    {
       bool  isAcceptPermission = PlayerPrefs.GetInt("PermissionFlash") == 0 ? false : true;
        if (!isAcceptPermission)
        {
            return;
        }
#if !UNITY_EDITOR
        // Check if flash is supported
        if (!cameraDevice.torchSupported)
            return;
        // Toggle
        cameraDevice.torchEnabled = true;
        DOVirtual.DelayedCall(0.1f, () => { cameraDevice.torchEnabled = false; });
#endif
    }

    public void FlashActive()
    {
        //cameraDevice.torchEnabled = !cameraDevice.torchEnabled;
    }
    */
}

