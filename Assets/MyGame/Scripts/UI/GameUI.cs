using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [Header("Inventory")]
    public GameObject inventory;
    public GameObject axieInventory;

    [Header("Popup")]
    public GameObject bg;
    public GameObject inventoryPopup;
    public GameObject axieChestPopup;
    public GameObject feedAxiePopup;
    public GameObject shopPopup;

    [Header("Chest")]
    public GameObject startChest;
    public GameObject chest;
    public GameObject axieChest;

    [Header("Wiki")]
    public GameObject wikiBtn;
    public GameObject wikiPopup;

    [Header("Noti")]
    public GameObject noti;

    [Header("Game")]
    public GameObject nextBtn;
    public GameObject endTurnBtn;
    public GameObject currentTurn;

    [Header("Fx prerab")]
    public GameObject bubbleFx;

    public static GameUI Instance;

    private void Awake()
    {
        Instance = this;
    }
}
