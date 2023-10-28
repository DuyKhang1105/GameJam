using DG.Tweening;
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

            case EnemySkillType.BuffPow:
                return BuffPow;

            case EnemySkillType.SelfDestruct:
                return SelfDestruct;
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
                FxManager.Instance.Create(battleSystem.enemyUnits[i].transform.position, TypeFx.BUFF_ENEMY);
            }
        }
    }

    void StunHero()
    {
        battleSystem.heroControl.Stun();
        battleSystem.heroUnit.Stun(skillValue);
        FxManager.Instance.Create(battleSystem.heroUnit.transform.position, TypeFx.MAGIC_STUN);
    }

    void BuffPow()
    {
        for (int i = 0; i < battleSystem.enemyUnits.Count; i++)
        {
            if (battleSystem.enemyUnits[i].currentHP > 0)
            {
                battleSystem.enemyUnits[i].TakePow(skillValue);
                battleSystem.enemyHUDs[i].SetPow(battleSystem.enemyUnits[i].currentPow);
                TextFx.Create(battleSystem.enemyUnits[i].transform.position, skillValue, TypeText.POW);
                FxManager.Instance.Create(battleSystem.enemyUnits[i].transform.position, TypeFx.BUFF_POW);
            }
        }
    }

    void SelfDestruct()
    {
        int damage = skillValue;
        

        bool isDead = battleSystem.heroUnit.isDead;

        Sequence s = DOTween.Sequence();
        s.Append(battleSystem.enemyUnits[battleSystem.indexEnemy].transform.DOMoveX(battleSystem.heroUnit.transform.position.x, 1f));//
        s.AppendCallback(() =>
        {
            battleSystem.enemyControls[battleSystem.indexEnemy].Buff();

        });
        s.AppendInterval(0.6f);
        s.AppendCallback(() =>
        {
            //Currently enemy dont has crit
            TextFx.Create(battleSystem.heroControl.transform.position, battleSystem.enemyUnits[battleSystem.indexEnemy].damage, TypeText.CRIT);
            FxManager.Instance.Create(battleSystem.heroUnit.transform.position, TypeFx.BOMB);

            if (isDead)
            {
                battleSystem.heroControl.Dead();
            }
            else
            {
                battleSystem.heroControl.OneHit();
            }
            battleSystem.heroHUD.SetHP(battleSystem.heroUnit.currentHP);
            battleSystem.heroHUD.SetShield(battleSystem.heroUnit.shield);

        });
        s.AppendCallback(() =>
        {
            Debug.LogError("Boom");
            var indexDie = battleSystem.indexEnemy;
            battleSystem.ResetTarget();
            battleSystem.enemyHUDs[indexDie].gameObject.SetActive(false);
            battleSystem.enemyControls[indexDie].gameObject.SetActive(false);
        });
    }

    void Evolution()
    {

    }    


}
    