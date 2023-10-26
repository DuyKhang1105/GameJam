using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : Unit
{
    [Header("Enemy")]
    EnemySkillType skillType;
    public int skillValue;
    public int maxPow;
    public int currentPow;

    public bool isPow = false;
    BattleSystem battleSystem;

    public void Parse(EnemyConfig enemyConfig)
    {
        this.skillType = enemyConfig.skillType;
        this.skillValue = enemyConfig.skillValue;
        this.maxHP = enemyConfig.statistics.maxHP;
        this.maxPow = enemyConfig.statistics.maxPow;
        this.damage = enemyConfig.statistics.damage;
        this.shield = enemyConfig.statistics.shield;

        currentHP = maxHP;
        battleSystem = BattleSystem.Instance;
    }

    public void TakePow(int dmg)
    {
        currentPow += dmg;
        if (currentPow >= maxPow)
        {
            currentPow = maxPow;
            isPow = true;
        }
        else
        {
            isPow = false;
        }
    }

    public void ResetPow()
    {
        currentPow = 0;
        isPow = false;
    }

    public Action Skill()
    {
        ResetPow();
        switch (skillType)
        {
            case EnemySkillType.None:
                return StunHero;

            case EnemySkillType.HealAll:
                return HealAll;

            case EnemySkillType.StunHero:
                return StunHero;

            case EnemySkillType.StunPet:
                return StunHero;
        }

        return null;
    }

    void HealAll()
    {
        for (int i = 0; i < battleSystem.enemyUnits.Count; i++)
        {
            if (battleSystem.enemyUnits[i].currentHP > 0)
            {
                battleSystem.enemyUnits[i].Heal(skillValue);
                battleSystem.enemyHUDs[i].SetHP(battleSystem.enemyUnits[i].currentHP);
                TextFx.Create(battleSystem.enemyUnits[i].transform.position, skillValue, TypeText.HEAL);
            }
        }
    }

    void StunHero()
    {
        battleSystem.heroControl.Stun();
        battleSystem.heroUnit.Stun(skillValue);
    }   
    
    void StunPet()
    {

    }

    void HitCrit()
    {
        float crit = criticalRate;
    }
}
    