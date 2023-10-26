using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public static class ListExtension
{
    public static List<T> Clone<T>(this List<T> listData)
    {
        List<T> clone = new List<T>();
        foreach (var item in listData)
        {
            clone.Add(item);
        }

        return clone; 
    }


    public static List<T> RemoveList<T>(this List<T> listData,List<T> listRemove)
    {
        foreach (var item in listRemove)
        {
            if (listData.Contains(item))
            {
                listData.Remove(item);
            }
        }
        return listData;
    }
}

