using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public static class GameobjectExtension
{
    public static void DestroyAllChild(this GameObject gameObject)
    {
        foreach (Transform child in gameObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public static T SetParent<T>(this T gameobject, Transform parent) where T : MonoBehaviour
    {
        Vector3 localPosition = gameobject.transform.localPosition;
        Vector3 localScale = gameobject.transform.localScale;
        Quaternion localRotate = gameobject.transform.localRotation;
        gameobject.transform.SetParent(parent);
        gameobject.transform.localPosition = localPosition;
        gameobject.transform.localScale = localScale;
        gameobject.transform.localRotation = localRotate;
        return gameobject;
    }

    public static T SetLocalPostion<T>(this T gameobject, Vector3 localPosition) where T : MonoBehaviour
    {
        gameobject.transform.localPosition = localPosition;
        return gameobject;
    }


    public static void DespawnAllChild(this GameObject gameObject)
    {
        foreach (Transform child in gameObject.transform)
        {
            if(child.GetComponent<PoolElement>() != null)
            {
                PoolManager.Instance.DespawnObject(child);
            }
            else
            {
                GameObject.Destroy(child.gameObject);
            }
            
        }
    }

    
}
