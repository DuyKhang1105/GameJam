using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeroBattleHUD : BattleHUD
{
    [Header("HeroBattleHUD")]
    public TextMeshProUGUI tmpStamina;

    public void SetHeroHUD(HeroUnit unit)
    {
        maxHP = unit.maxHP;

        tmpShield.SetText($"{unit.shield}");
        SetHP(unit.currentHP);
        SetShield(unit.shield);

        SetStamina(unit.currentStamina);
    }

    public void SetStamina(int sta)
    {
        tmpStamina.SetText($"{sta}");
    }

    public void SetActiveStamina(bool isActive)
    {
        tmpStamina.transform.parent.gameObject.SetActive(isActive);
    }    

}
