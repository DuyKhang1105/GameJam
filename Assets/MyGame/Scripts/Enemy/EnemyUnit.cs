using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

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

            case EnemySkillType.Summon:
                return Summon;

            case EnemySkillType.Evolution:
                return OnEvolution;

            case EnemySkillType.Assassinate:
                return OnAssassinate;
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

        battleSystem.heroUnit.TakeDamage(damage, false);
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

            if (isDead)
            {
                battleSystem.state = BattleState.LOST;
                battleSystem.EndBattle();
            }
        });
    }

    void OnEvolution()
    {
        StartCoroutine(Evolution());
    }

    IEnumerator Evolution()
    {
        FxManager.Instance.Create(battleSystem.enemyBattleStations[0].position, TypeFx.SUMMON_2);
        FxManager.Instance.Create(battleSystem.enemyBattleStations[1].position, TypeFx.SUMMON_2);
        yield return new WaitForSeconds(1.5f);

        int currentHP = battleSystem.enemyUnits[0].currentHP; // Save hp

        for (int i = 0; i < 2; i++)
        {
            battleSystem.enemyHUDs[i].gameObject.SetActive(false);
            battleSystem.enemyUnits[i].gameObject.SetActive(false);
        }

        battleSystem.enemyUnits.Clear();
        battleSystem.enemyControls.Clear();

        /*heroControl.Idle();
        foreach (var axie in dicAxies.Values)
        {
            axie.GetComponent<AxieControl>().Idle();
        }*/

        List<EnemyConfig> enemys = new List<EnemyConfig>();
        EnemyConfig enemy1 = EnemyConfigs.Instance.GetEnemyConfig(EnemyName.WolfAlpha);
        EnemyConfig enemy2 = EnemyConfigs.Instance.GetEnemyConfig(EnemyName.WolfGray);
        enemys.Add(enemy1);
        enemys.Add(enemy2);

        for (int i = 0; i < 2; i++)
        {
            GameObject enemyGO = Instantiate(enemys[i].graphic, battleSystem.enemyBattleStations[i]);
            EnemyUnit enemyUnit = enemyGO.GetComponent<EnemyUnit>();
            enemyUnit.Parse(enemys[i]);
            enemyUnit.currentHP = currentHP;

            battleSystem.enemyUnits.Add(enemyUnit);

            battleSystem.enemyControls.Add(enemyGO.GetComponent<EnemyControl>());

            battleSystem.enemyHUDs[i].gameObject.SetActive(true);
            battleSystem.enemyHUDs[i].SetEnemyHUD(battleSystem.enemyUnits[i]);
        }
    }    

    void Summon()
    {
        EnemyConfig enemy = EnemyConfigs.Instance.GetEnemyConfig(EnemyName.Slime);

        for (int i = 0; i < skillValue; i++)
        {
            GameObject enemyGO = Instantiate(enemy.graphic, battleSystem.enemyBattleStations[i+1]);
            EnemyUnit enemyUnit = enemyGO.GetComponent<EnemyUnit>();
            enemyUnit.Parse(enemy);
            battleSystem.enemyUnits.Add(enemyUnit);

            battleSystem.enemyControls.Add(enemyGO.GetComponent<EnemyControl>());

            battleSystem.enemyHUDs[i+1].gameObject.SetActive(true);
            battleSystem.enemyHUDs[i+1].SetEnemyHUD(battleSystem.enemyUnits[i+1]);
            FxManager.Instance.Create(battleSystem.enemyBattleStations[i + 1].position, TypeFx.SUMMON);

        }
    }

    void OnAssassinate()
    {
        StartCoroutine(Assassinate());
    }

    IEnumerator Assassinate()
    {
        battleSystem.enemyControls[battleSystem.indexEnemy].Combo1();
        for (int i = 0; i < 3; i++)
        {
            int damage = skillValue;

            if (i == 2)
            {
                skillValue += 3;
            }

            HitCombo(skillValue);
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1.5f);
        battleSystem.enemyUnits[battleSystem.indexEnemy].transform.DOMoveX(battleSystem.enemyBattleStations[battleSystem.indexEnemy].position.x, 1f);
    }

    void HitCombo(int damage)
    {
        bool isMiss = battleSystem.heroUnit.TakeDamage(damage, true);
        bool isDead = battleSystem.heroUnit.isDead;

        Sequence s = DOTween.Sequence();
        s.Append(battleSystem.enemyUnits[battleSystem.indexEnemy].transform.DOMoveX(-3f, 1f));//
        s.AppendCallback(() =>
        {
            for (int i = 0; i < 3; i++)
            {
                battleSystem.enemyControls[battleSystem.indexEnemy].Combo2();

                if (isMiss)
                {
                    DOVirtual.DelayedCall(0.2f, () =>
                    {
                        battleSystem.heroControl.Dodge();
                    });
                }
            }
        });
        s.AppendInterval(0.6f);
        s.AppendCallback(() =>
        {
            if (isMiss)
            {
                battleSystem.heroControl.Dodge();
            }
            else
            {
                //Currently enemy dont has crit
                TextFx.Create(battleSystem.heroControl.transform.position, damage, TypeText.CRIT);
                FxManager.Instance.Create(battleSystem.heroUnit.transform.position, TypeFx.HIT_CRIT);

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
            }
        });
        s.AppendInterval(0.4f);
        s.AppendCallback(() => 
        {
            if (isDead)
            {
                battleSystem.state = BattleState.LOST;
                battleSystem.EndBattle();
            }
        });
    }    

}
    