using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Unit : MonoBehaviour
{

	public string unitName;

	public int damage;

	public int buffShield;
	public int currentShield;

	public int initStamina;
	public int currentStamina;

	public int maxHP;
	public int buffHP;
	public int currentHP;

    public int maxPow;
    public int currentPow;

    [Range(0f, 1f)]
	public float dodgeRate;
    [Range(0f, 1f)]
    public float criticalRate;
    [Range(0f, 1f)]
    public float armorPenetrationRate;

	public bool isPow = false;
	public bool isDead = false;
    public bool isCrit = false;

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
        currentStamina = initStamina;
    }

    public int TakeShield(int dmg)
    {
        currentShield -= dmg;

        if (currentShield >= 0)
            return 0;
        else
		{
			int newDmg = -currentShield;
			currentShield = 0;
            return newDmg;
        }
    }

    public bool TakeDamage(int dmg)
	{
		float r = Random.Range(0, 1f);
		Debug.LogError("r dodgeRate: " + r);
		if (r <= dodgeRate)
            return true;

        currentHP -= TakeShield(dmg);

		if (currentHP <= 0)
		{
            currentHP = 0;
			isDead = true;
        }
		else
		{
            isDead = false;
        }
        return false;
    }

    public int GetDamage()
    {
        //criticalRate
        float r = Random.Range(0, 1f);
        Debug.LogError("r criticalRate: " + r);
        if (r <= criticalRate)
        {
            isCrit = true;
            return damage * 2;
        }

        isCrit = false;
        return damage;
    }

    public void TakePow(int dmg)
	{
        currentPow += dmg;
        if (currentPow >= maxPow)
        {
            currentPow = maxPow;
            isPow = true;
        }else
        {
            isPow = false;
        }
    }

    public void ResetPow()
    {
        currentPow = 0;
        isPow = false;
    }

	public void Heal(int amount)
	{
		currentHP += amount;
		if (currentHP > maxHP)
			currentHP = maxHP;
	}

	public void Shield(int amount)
	{
		currentShield += amount;
	}

}
