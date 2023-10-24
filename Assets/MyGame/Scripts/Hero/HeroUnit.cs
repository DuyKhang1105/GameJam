using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroUnit : Unit
{
    [Header("Hero")]
    public int initStamina;
    public int currentStamina;

    public bool TakeStamina(int sta)
    {
        int a = currentStamina;
        a -= sta;

        if (a >= 0) //enough for action
        {
            currentStamina = a;
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
}
