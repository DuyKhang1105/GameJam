using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoSingleton<GameUI>
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
    public GameObject endGamePopup;

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
    public GameObject textFx;

    [Header("Sound")]
    public AudioClip musicSnd;
    public AudioClip hitHeroSnd;
    public AudioClip hitEnemySnd;
    public AudioClip buffSnd;
    public AudioClip fireSnd;
    public AudioClip spawnSnd;
    public AudioClip stunSnd;
    public AudioClip evolutionSnd;
    public AudioClip drinkSnd;
    public AudioClip bombSnd;
    public AudioClip winSnd;
    public AudioClip clickCoinSnd;
    public AudioClip summonSnd;
    public AudioClip buffEnemySnd;

    private void Start()
    {
        SoundManager.Instance.PlayLoop(musicSnd);
    }
}
