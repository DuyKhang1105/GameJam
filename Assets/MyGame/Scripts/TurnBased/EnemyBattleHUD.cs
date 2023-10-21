using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBattleHUD : BattleHUD
{
    public Image imgAction;
    public Image imgTarget;
    public Button btnTarget;
    public Sprite sprAttack;
    public Sprite sprShield;
    public TextMeshProUGUI tmpValueNextAction;

    public override void SetHUD(Unit unit)
    {
        base.SetHUD(unit);
        SetNextAction(true, unit.damage);
    }

    public override void SetHP(int hp)
    {
        base.SetHP(hp);
    }

    public override void SetShield(int shi)
    {
        base.SetShield(shi);
    }

    public void SetNextAction(bool isAttack, int value)
    {
        SetActiveNextAction(true);
        imgAction.sprite = isAttack ? sprAttack : sprShield;
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
