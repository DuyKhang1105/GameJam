using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    public int col;
    public int row;
    public bool isFree;
    public bool isLocked;

    private Image img;

    public void ParseSlot(int _c, int _r, bool _isFree, bool _isLocked)
    {
        col= _c; row = _r; isFree= _isFree; isLocked= _isLocked;
        gameObject.name = $"Slot[{col}, {row}]";
        gameObject.SetActive(!isLocked);
        img = GetComponent<Image>();
    }
}
