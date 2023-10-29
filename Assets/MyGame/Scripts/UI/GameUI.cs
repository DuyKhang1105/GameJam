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
    [SerializeField] private AudioClip musicSnd;
    [SerializeField] private AudioClip hitHeroSnd;
    [SerializeField] private AudioClip hitEnemySnd;
    [SerializeField] private AudioClip buffSnd;
    [SerializeField] private AudioClip fireSnd;
    [SerializeField] private AudioClip spawnSnd;
    [SerializeField] private AudioClip stunSnd;
    [SerializeField] private AudioClip evolutionSnd;
    [SerializeField] private AudioClip drinkSnd;
    [SerializeField] private AudioClip bombSnd;
    [SerializeField] private AudioClip winSnd;
    [SerializeField] private AudioClip clickCoinSnd;
    [SerializeField] private AudioClip summonSnd;
    [SerializeField] private AudioClip buffEnemySnd;

    private void Start()
    {
        SoundManager.Instance.PlayLoop(musicSnd);
    }
}
