using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

	public string unitName;

	public int shield;
	public int damage;

	public int maxStamina;
	public int currentStamina;

	public int maxHP;
	public int currentHP;

	public int criticalRate;
	public int criticalDamage;

	public int normalEvasionRate;
	public int criticalEvasionRate;

	public int armorPenetrationRate;

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

	public void ResetStamina()
	{
        currentStamina = maxStamina;
    }

    public int TakeShield(int dmg)
    {
        shield -= dmg;

        if (shield >= 0)
            return 0;
        else
		{
			int newDmg = -shield;
			shield = 0;
            return newDmg;
        }
    }

    public bool TakeDamage(int dmg)
	{
		currentHP -= TakeShield(dmg);

		if (currentHP <= 0)
		{
            currentHP = 0;
            return true;
        }
		else
			return false;
	}

	public void Heal(int amount)
	{
		currentHP += amount;
		if (currentHP > maxHP)
			currentHP = maxHP;
	}

	public void Shield(int shi)
	{
		shield += shi;
	}
}
