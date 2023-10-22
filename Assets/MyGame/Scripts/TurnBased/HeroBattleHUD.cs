using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeroBattleHUD : BattleHUD
{
    [Header("HeroBattleHUD")]
    public TextMeshProUGUI tmpStamina;

    public override void SetHUD(Unit unit)
    {
        base.SetHUD(unit);
        SetStamina(unit.currentStamina);
    }

    public override void SetHP(int hp)
    {
        base.SetHP(hp);
    }

    public override void SetShield(int shi)
    {
        base.SetShield(shi);
    }

    public void SetStamina(int sta)
    {
        tmpStamina.SetText($"{sta}");
    }
}
