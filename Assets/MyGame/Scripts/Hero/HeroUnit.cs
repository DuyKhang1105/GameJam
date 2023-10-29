using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroUnit : Unit
{
    [Header("Hero")]
    public int initStamina;
    public int currentStamina;

    public AxieUnit axieBuff;

    public bool TakeStamina(int sta)
    {
        int a = currentStamina;
        a -= sta;

        if (a >= 0) //enough for action
        {
            currentStamina = a;

            if (currentStamina == 0)
            {
                BattleSystem.Instance.heroHUD.SetActiveStamina(false);
                DOVirtual.DelayedCall(2.5f, BattleSystem.Instance.AutoEndTurn);
            }

            return true;
        }
        else
            return false;
    }

    public void GetStamina(int sta)
    {
        currentStamina += sta;
    }

    public void ResetStamina()
    {
        currentStamina = initStamina;
    }

    public void Fightsback(AxieUnit axieBuff)
    {
        isFightsback = true;
        this.axieBuff = axieBuff;
    }

    public void ClearFightsback()
    {
        isFightsback = false;

        if (axieBuff != null)
        {
            axieBuff.MoveBack();
            this.axieBuff = null;
        }
    }
}
