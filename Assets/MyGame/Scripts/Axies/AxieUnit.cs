using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

public class AxieUnit : Unit
{
    [Header("Axie")]
    public AxieSkillType skillType;
    public Vector3 axieBattleStation;

    [Range(0f, 1f)]
    public float actionRate;

    string axieID;
    BattleSystem battleSystem;

    public void Parse(AxieConfig axieConfig)
    {
        this.axieID = axieConfig.axieId;
        this.actionRate = axieConfig.actionRate;
        this.skillType = axieConfig.skillType;

        axieBattleStation = transform.position;
        battleSystem = BattleSystem.Instance;
    }

    public bool CheckBuff()
    {
        float r = Random.Range(0, 1f);
        Debug.LogError("r: " + r);
        if (r <= actionRate)
        {
            Skill();
            return true;
        }

        return false;
    }

    public Action Skill()
    {
        switch (skillType)
        {
            case AxieSkillType.ExpansionSlot:
                break;

            case AxieSkillType.ExpansionChest:

                break;

            case AxieSkillType.UpgradeItem:

                break;

            case AxieSkillType.AxieHit:
                StartCoroutine(AxieHit(2));
                break;

            case AxieSkillType.BuffHP:
                BuffHP(5);
                break;

            case AxieSkillType.BuffStamina:
                BuffStamina(3);
                break;

            case AxieSkillType.Fightsback:
                SetFightsback();
                break;

            case AxieSkillType.ArmorPenetration:
                BuffAP();
                break;

            case AxieSkillType.BloodSucking:
                BuffBS();
                break;

            default:
                break;
        }

        return null;
    }

    void SetFightsback()
    {
        transform.DOMoveX(0f, 1f);
        battleSystem.heroUnit.Fightsback(this);
        DOVirtual.DelayedCall(0.5f, () =>
        {
            battleSystem.dicAxies[axieID].GetComponent<AxieControl>().Buff();
        });
    }

    void BuffAP()
    {
        battleSystem.dicAxies[axieID].GetComponent<AxieControl>().Buff();
        battleSystem.heroUnit.isBuffAP = true;
    }

    void BuffBS()
    {
        battleSystem.dicAxies[axieID].GetComponent<AxieControl>().Buff();
        battleSystem.heroUnit.isBloodSucking = true;
    }

    void BuffHP(int buffValue)
    {
        battleSystem.dicAxies[axieID].GetComponent<AxieControl>().Buff();
        battleSystem.heroUnit.Heal(buffValue);
        battleSystem.heroHUD.SetHP(battleSystem.heroUnit.currentHP);
        TextFx.Create(battleSystem.heroUnit.transform.position, buffValue, TypeText.HEAL);
    }

    void BuffStamina(int buffValue)
    {
        battleSystem.dicAxies[axieID].GetComponent<AxieControl>().Buff();
        battleSystem.heroUnit.GetStamina(buffValue);
        battleSystem.heroHUD.SetStamina(battleSystem.heroUnit.currentStamina);
        TextFx.Create(battleSystem.heroUnit.transform.position, buffValue, TypeText.STAMINA);
    }

    IEnumerator AxieHit(int activeValue)
    {
        var enemyHUDTarget = battleSystem.enemyHUDs[battleSystem.indexTarget];
        var enemyUnitTarget = battleSystem.enemyUnits[battleSystem.indexTarget];
        var enemyControlTarget = battleSystem.enemyControls[battleSystem.indexTarget];
        
        bool isMiss = enemyUnitTarget.TakeDamage(activeValue, false);
        bool isDead = enemyUnitTarget.isDead;
        if (!isMiss && !isDead)
        {
            enemyUnitTarget.TakePow(activeValue);
        }

        // Code tam
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOMoveX(0f, 1f));
        s.AppendCallback(() =>
        {
            battleSystem.dicAxies[axieID].GetComponent<AxieControl>().Attack();

            if (isMiss)
                battleSystem.enemyControls[battleSystem.indexTarget].Dodge();
        });
        s.AppendInterval(0.5f);
        s.AppendCallback(() =>
        {
            if (isMiss)
            {
                //enemyControls[indexTarget].Dodge();
            }
            else
            {
                TextFx.Create(enemyUnitTarget.transform.position, activeValue, TypeText.HIT);

                if (!isDead)
                {
                    enemyControlTarget.OneHit();
                }
                else
                {
                    enemyControlTarget.Die();
                }

                enemyHUDTarget.SetPow(enemyUnitTarget.currentPow);
                enemyHUDTarget.SetHP(enemyUnitTarget.currentHP);
                enemyHUDTarget.SetShield(enemyUnitTarget.shield);
            }

        });
        s.AppendInterval(0.4f);
        s.AppendCallback(() => MoveBack());


        // Code tam 

        yield return new WaitForSeconds(3f);

        if (isDead)
        {
            var indexDie = battleSystem.indexTarget;
            battleSystem.ResetTarget();
            battleSystem.enemyHUDs[indexDie].gameObject.SetActive(false);

            yield return new WaitForSeconds(1f);
            battleSystem.enemyControls[indexDie].gameObject.SetActive(false);
        }
    }

    public void Fightsback(int dmg)
    {
        StartCoroutine(ActionFightsback(dmg));
    }

    IEnumerator ActionFightsback(int activeValue)
    {
        var enemyHUDTarget = battleSystem.enemyHUDs[battleSystem.indexTarget];
        var enemyUnitTarget = battleSystem.enemyUnits[battleSystem.indexTarget];
        var enemyControlTarget = battleSystem.enemyControls[battleSystem.indexTarget];

        bool isMiss = enemyUnitTarget.TakeDamage(activeValue, false);
        bool isDead = enemyUnitTarget.isDead;
        if (!isMiss && !isDead)
        {
            enemyUnitTarget.TakePow(activeValue);
        }

        // Code tam
        Sequence s = DOTween.Sequence();
        s.AppendCallback(() =>
        {
            battleSystem.dicAxies[axieID].GetComponent<AxieControl>().Attack();

            if (isMiss)
                battleSystem.enemyControls[battleSystem.indexTarget].Dodge();
        });
        s.AppendInterval(0.5f);
        s.AppendCallback(() =>
        {
            if (isMiss)
            {
                //enemyControls[indexTarget].Dodge();
            }
            else
            {
                TextFx.Create(enemyUnitTarget.transform.position, activeValue, TypeText.HIT);

                if (!isDead)
                {
                    enemyControlTarget.OneHit();
                }
                else
                {
                    enemyControlTarget.Die();
                }

                enemyHUDTarget.SetPow(enemyUnitTarget.currentPow);
                enemyHUDTarget.SetHP(enemyUnitTarget.currentHP);
                enemyHUDTarget.SetShield(enemyUnitTarget.shield);
            }
        });
        s.AppendInterval(0.4f);
        s.AppendCallback(() => MoveBack());
        s.AppendCallback(() =>
        {
            battleSystem.heroUnit.ClearFightsback();
        });

        // Code tam 
        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            var indexDie = battleSystem.indexTarget;
            battleSystem.ResetTarget();
            battleSystem.enemyHUDs[indexDie].gameObject.SetActive(false);

            yield return new WaitForSeconds(1f);
            battleSystem.enemyControls[indexDie].gameObject.SetActive(false);
        }
    }

    public void MoveBack()
    {
        transform.DOMoveX(axieBattleStation.x, 1f);
    }    
}
