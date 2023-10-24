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
	public int buffShield;

	public int maxHP;
	public int buffHP;
	public int currentHP;

    public int turnStun;

    [Range(0f, 1f)]
	public float dodgeRate;
    [Range(0f, 1f)]
    public float criticalRate;
    [Range(0f, 1f)]
    public float armorPenetrationRate;

	public bool isDead = false;
    public bool isCrit = false;

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

    public virtual bool TakeDamage(int dmg)
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

    public virtual int GetDamage()
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
