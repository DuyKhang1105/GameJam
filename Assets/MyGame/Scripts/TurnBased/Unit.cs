using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Unit : MonoBehaviour
{
    [Header("Base")]
	public string unitName;
	public int damage;
	public int shield;
	public int maxHP;
	public int currentHP;
    public int turnStun;

    [Range(0f, 1f)]
	public float dodgeRate;
    [Range(0f, 1f)]
    public float criticalRate;
    [Range(0f, 1f)]
    public float armorPenetrationRate;

    public int bloodLost = 0;
	public bool isDead = false;
    public bool isCrit = false;

    [Header("AxieBuff")]

    public bool isBuffAP = false;
    public bool isFightsback = false;
    public bool isBloodSucking = false;

    public virtual int TakeShield(int dmg)
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

    public virtual bool TakeDamage(int dmg, bool isAP)
	{
        bloodLost = 0;

        if (CheckDodge())
            return true;

        if (isAP)
        {
            bloodLost = dmg;
            currentHP -= dmg;
        }
        else
        {
            int newDmg = TakeShield(dmg);
            bloodLost = newDmg;
            currentHP -= newDmg;
        }

        if (currentHP <= 0)
		{
            bloodLost += currentHP;
            currentHP = 0;
			isDead = true;
        }
		else
		{
            isDead = false;
        }
        return false;
    }

    public virtual bool CheckAP()
    {
        if (isBuffAP)
        {
            isBuffAP = false;
            return true;
        }

        float r = Random.Range(0, 1f);
        if (r <= armorPenetrationRate)
            return true;
        return false;
    }

    public virtual bool CheckDodge()
    {
        float r = Random.Range(0, 1f);
        if (r <= dodgeRate)
            return true;
        return false;
    }

    public virtual int GetDamage()
    {
        //criticalRate
        float r = Random.Range(0, 1f);
        if (r <= criticalRate)
        {
            isCrit = true;
            return damage * 2;
        }

        isCrit = false;
        return damage;
    }

    public virtual void Heal(int amount)
	{
		currentHP += amount;
		if (currentHP > maxHP)
			currentHP = maxHP;
	}

    public virtual void Shield(int amount)
	{
		shield += amount;
	}

    public virtual void Stun(int amount)
    {
        turnStun += amount;
    }

    public virtual bool CheckStun()
    {
        if (turnStun <= 0)
        {
            turnStun = 0;
            return false;
        }

        turnStun--;
        return true;
    }
}
