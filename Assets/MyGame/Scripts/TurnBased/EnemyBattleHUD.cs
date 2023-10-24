using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class EnemyBattleHUD : BattleHUD
{
    [Header("EnemyBattleHUD")]
    public Image imgPow;
    public Image imgAction;
    public Image imgTarget;
    public Button btnTarget;
    public Sprite sprPow;
    public Sprite sprHeal;
    public Sprite sprAttack;
    public Sprite sprShield;
    public TextMeshProUGUI tmpPow;
    public TextMeshProUGUI tmpValueNextAction;

    int maxPow;
    public void SetEnemyHUD(EnemyUnit unit)
    {
        maxHP = unit.maxHP;

        tmpShield.SetText($"{unit.shield}");
        SetHP(unit.currentHP);
        SetShield(unit.shield);

        maxPow = unit.maxPow;
        SetPow(unit.currentPow);
        SetActiveNextAction(false);
    }

    public void SetPow(int pow)
    {
        tmpPow.SetText($"{pow}/{maxPow}");
        imgPow.DOFillAmount((float)pow / maxPow, 1f);
    }

    public void SetNextAction(ActionType actionType, int value)
    {
        SetActiveNextAction(true);

        switch (actionType)
        {
            case ActionType.ATTACK:
                tmpValueNextAction.enabled = true;
                imgAction.sprite = sprAttack;
                break;

            case ActionType.HEAL:
                tmpValueNextAction.enabled = true;
                imgAction.sprite = sprHeal;
                break;

            case ActionType.SHIELD:
                tmpValueNextAction.enabled = true;
                imgAction.sprite = sprShield;
                break;

            case ActionType.POW:
                tmpValueNextAction.enabled = false;
                imgAction.sprite = sprPow;
                break;

            default:
                break;
        }

        tmpValueNextAction.SetText($"{value}");
    }

    public void SetActiveNextAction(bool isActive)
    {
        imgAction.gameObject.SetActive(isActive);
    }

    public void SetTarget(bool isEnable)
    {
        if (isEnable)
        {
            imgTarget.color = new Color32(255, 255, 255, 255);
        }
        else
        {
            imgTarget.color = new Color32(255, 255, 255, 0);
        }
    }    
}
