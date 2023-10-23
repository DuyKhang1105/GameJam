﻿using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

public enum BattleState { START, HEROTURN, ENEMYTURN, ACTING, WON, LOST }

public enum ActionType { ATTACK, HEAL, SHIELD, POW}


public class BattleSystem : MonoBehaviour
{
    [Header("Stage")]
    public int stageIndex;

    [Header("Pet")]
    public List<Transform> axieBattleStations;
    private Dictionary<string, GameObject> dicAxies;

    public GameObject heroPrefab;
	public List<GameObject> enemyPrefabs;

	public Transform heroBattleStation;
	public List<Transform> enemyBattleStations;

	Unit heroUnit;
    public List<Unit> enemyUnits;

	HeroControl heroControl;
    public List<EnemyControl> enemyControls;

    public TextMeshProUGUI dialogueText;

	public HeroBattleHUD heroHUD;
	public List<EnemyBattleHUD> enemyHUDs;

	public BattleState state;

	[Header("GUI")]
    public Background BG;


	int indexEnemy;
    int indexTarget;

	List<int> indexActions = new List<int>();

    public static BattleSystem Instance;

    private void Awake()
    {
        Instance = this;
        dicAxies = new Dictionary<string, GameObject>();
    }

    void Start()
    {
        var axieInventory = GameUI.Instance.axieInventory.GetComponent<AxieInventory>();
        axieInventory.onChangeList += OnChangePets;

        state = BattleState.START;

        InitHero();
        SetupStage();
    }

    private void OnChangePets()
    {
        var axieIds = dicAxies.Keys.ToList();
        foreach (var axieId in axieIds) {
            Destroy(dicAxies[axieId]);
            dicAxies.Remove(axieId);
        }

        var axieInventory = GameUI.Instance.axieInventory.GetComponent<AxieInventory>();
        for (int i = 0; i < axieInventory.axies.Count; i++)
        {
            AxieConfig config = axieInventory.axies[i];
            GameObject axieGO = Instantiate(config.graphic, axieBattleStations[i]);
            dicAxies.Add(config.axieId, axieGO);
        }
    }

    private void InitHero()
    {
        if (heroUnit == null)
        {
            GameObject heroGO = Instantiate(heroPrefab, heroBattleStation);
            heroUnit = heroGO.GetComponent<Unit>();
            heroControl = heroGO.GetComponent<HeroControl>();
        }
        heroHUD.SetHUD(heroUnit);
    }

	private void SetupStage()
	{
        StartCoroutine(IeSetupStage());
	}

    private IEnumerator IeSetupStage()
    {
        enemyHUDs.ForEach(x=>x.gameObject.SetActive(false));
        GameUI.Instance.nextBtn.gameObject.SetActive(false);
        GameUI.Instance.endTurnBtn.gameObject.SetActive(false);

        StageConfig stage = StageConfigs.Instance.GetStage(stageIndex);
        switch (stage.stageType) {
            case StageType.Enemy:
                for (int i = 0; i < enemyPrefabs.Count; i++)
                {
                    GameObject enemyGO = Instantiate(enemyPrefabs[i], enemyBattleStations[i]);
                    enemyUnits.Add(enemyGO.GetComponent<Unit>());
                    enemyControls.Add(enemyGO.GetComponent<EnemyControl>());
                    enemyHUDs[i].gameObject.SetActive(true);
                    enemyHUDs[i].SetHUD(enemyUnits[i]);
                }
                dialogueText.text = "A wild approaches...";
                yield return new WaitForSeconds(2f);

                ResetTarget();

                indexEnemy = 0;
                indexTarget = 0;
                NextActionEnemy();

                state = BattleState.HEROTURN;
                HeroTurn();
                break;
            case StageType.MiniBoss:
                //TODO 
                break;
            case StageType.Boss:
                //TODO
                break;
            case StageType.Chest:
                if (stageIndex == 0)
                {
                    GameUI.Instance.startChest.GetComponent<StartChest>().isOpened = false;
                    GameUI.Instance.startChest.SetActive(true);
                }       
                else
                {
                    GameUI.Instance.startChest.GetComponent<Chest>().isOpend = false;
                    GameUI.Instance.startChest.SetActive(true);
                }
                break;
            case StageType.AxieChest:
                break;
        }
    }

	IEnumerator HeroAttack()
	{
        bool isMiss = enemyUnits[indexTarget].TakeDamage(heroUnit.GetDamage());
        bool isDead = enemyUnits[indexTarget].isDead;
        if (!isMiss && !isDead)
        {
            enemyUnits[indexTarget].TakePow(heroUnit.damage);
        }
        state = BattleState.ACTING;

        // Code tam
        Sequence s = DOTween.Sequence();
		s.Append(heroUnit.transform.DOMoveX(0f, 1f));
		s.AppendCallback(() =>
		{
            if (heroUnit.isCrit)
                heroControl.Critical();
            else
                heroControl.OneAttack();

            if (isMiss)
                enemyControls[indexTarget].Dodge();
        });
		s.AppendInterval(0.5f);
        s.AppendCallback(() =>
        {
            if (isMiss)
            {
                //enemyControls[indexTarget].Dodge();
                dialogueText.text = "The attack is failure!";
            }
            else
            {
                if (!isDead)
                {
                    enemyControls[indexTarget].OneHit();
                }
                else
                {
                    enemyControls[indexTarget].Die();
                }

                enemyHUDs[indexTarget].SetPow(enemyUnits[indexTarget].currentPow);
                enemyHUDs[indexTarget].SetHP(enemyUnits[indexTarget].currentHP);
                enemyHUDs[indexTarget].SetShield(enemyUnits[indexTarget].currentShield);
                dialogueText.text = "The attack is successful!";
            }
            
        });
		s.AppendInterval(0.4f);
        s.Append(heroUnit.transform.DOMoveX(heroBattleStation.position.x, 1f));

		// Code tam 

		yield return new WaitForSeconds(3f);

        state = BattleState.HEROTURN;
        if (isDead)
		{
            var indexDie = indexTarget;
            ResetTarget();
            enemyHUDs[indexDie].gameObject.SetActive(false);

            yield return new WaitForSeconds(1f);
            enemyControls[indexDie].gameObject.SetActive(false);
        }
	}

	IEnumerator HeroHeal()
	{
        state = BattleState.ACTING;

        heroUnit.Heal(5);
		heroControl.Heal();


        heroHUD.SetHP(heroUnit.currentHP);
		dialogueText.text = "You feel renewed strength!";

		yield return new WaitForSeconds(2f);
        state = BattleState.HEROTURN;
    }



    IEnumerator EnemyTurn()
	{
        int totalEnemy = enemyUnits.Count;

        while (enemyUnits[indexEnemy].isDead)
        {
            indexEnemy++;

            if (indexEnemy >= totalEnemy)
            {
                EndEnemyTurn();
                yield break;
            }
        }

        dialogueText.text = enemyUnits[indexEnemy].unitName + " " + indexEnemy + " attacks!";

        yield return new WaitForSeconds(1f);

		bool isDead = ActionEnemy(indexEnemy);

        yield return new WaitForSeconds(3f);

		if(isDead)
		{
			state = BattleState.LOST;
			EndBattle();
		}
        else
		{
            // Next Enemy
            if (indexEnemy < (totalEnemy - 1))
            {
                indexEnemy++;
                StartCoroutine(EnemyTurn());
            }
            else
            {
                EndEnemyTurn();
            }
		}
	}

    void EndEnemyTurn()
    {
        ResetTarget();
        indexEnemy = 0;
        state = BattleState.HEROTURN;
        NextActionEnemy();
        HeroTurn();
    }

	void HeroTurn()
	{
		dialogueText.text = heroUnit.unitName + " Turn. Choose An Action";
        heroUnit.ResetStamina();
        heroHUD.SetStamina(heroUnit.currentStamina);
		GameUI.Instance.endTurnBtn.SetActive(true);
    }

	bool ActionEnemy(int indexEnemy)
	{
        bool isDead = false;
		switch (indexActions[indexEnemy])
		{
			case 0:
                isDead = EnemyAttack(indexEnemy);
                break;

            case 1:
				EnemyShield(indexEnemy);
                break;

            case 2:
                EnemyHeal(indexEnemy);
                break;
            case 3:
                EnemySkill(indexEnemy);
                break;

            default:
				break;
        }
		enemyHUDs[indexEnemy].SetActiveNextAction(false);
        return isDead;
    }

	void NextActionEnemy()
	{
        indexActions.Clear();

        for (int i = 0; i < enemyUnits.Count; i++)
        {
            int indexNext = Random.Range(0, 3);

            if (enemyUnits[i].isPow)
                indexNext = 3;

            switch (indexNext)
            {
                case 0:
                    enemyHUDs[i].SetNextAction(ActionType.ATTACK, enemyUnits[i].damage);
                    break;

                case 1:
                    enemyHUDs[i].SetNextAction(ActionType.SHIELD, 5);
                    break;

                case 2:
                    enemyHUDs[i].SetNextAction(ActionType.HEAL, 5);
                    break;

                case 3: 
                    enemyHUDs[i].SetNextAction(ActionType.POW, 5);
                    break ;

                default:
                    break;
            }

            indexActions.Add(indexNext);
        }
    }

    bool EnemyAttack(int indexEnemy)
	{
        bool isMiss = heroUnit.TakeDamage(enemyUnits[indexEnemy].damage);
        bool isDead = heroUnit.isDead;

        Sequence s = DOTween.Sequence();
        s.Append(enemyUnits[indexEnemy].transform.DOMoveX(0f, 1f));//
        s.AppendCallback(() =>
        {
            enemyControls[indexEnemy].OneAttack();

            if (isMiss)
            {
                DOVirtual.DelayedCall(0.2f, () =>
                {
                    heroControl.Dodge();
                });
            }
        });
        s.AppendInterval(0.6f);
        s.AppendCallback(() =>
        {
            if (isMiss)
            {
                //heroControl.Dodge();
            }
            else
            {
                if (isDead)
                {
                    heroControl.Dead();
                }
                else
                {
                    heroControl.OneHit();
                }
                heroHUD.SetHP(heroUnit.currentHP);
                heroHUD.SetShield(heroUnit.currentShield);
            }
        });
        s.AppendInterval(0.4f);
        s.Append(enemyUnits[indexEnemy].transform.DOMoveX(enemyBattleStations[indexEnemy].position.x, 1f));

		return isDead;
    }
		
	void EnemyShield(int indexEnemy)
	{
        enemyUnits[indexEnemy].Shield(5);
        enemyControls[indexEnemy].Buff();

        enemyHUDs[indexEnemy].SetShield(enemyUnits[indexEnemy].currentShield);
    }

    void EnemyHeal(int indexEnemy)
    {
        enemyUnits[indexEnemy].Heal(5);
        enemyControls[indexEnemy].Buff();

        enemyHUDs[indexEnemy].SetHP(enemyUnits[indexEnemy].currentHP);
    }

    void EnemySkill(int indexEnemy)
    {
        //Test skill heal all
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            enemyUnits[i].Heal(5);
            enemyControls[i].Buff();
            enemyHUDs[i].SetHP(enemyUnits[i].currentHP);
        }

        enemyUnits[indexEnemy].ResetPow();
        enemyHUDs[indexEnemy].SetPow(enemyUnits[indexEnemy].currentPow);
    }
        
	public void OnEndTurn()
	{
        if (state != BattleState.HEROTURN)
            return;

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
        GameUI.Instance.endTurnBtn.SetActive(false);
    }

    public void OnNext()
    {
        stageIndex += 1;

        //virtual
        GameUI.Instance.nextBtn.SetActive(false);
        BG.RunBG(true);
        heroControl.Run();

        foreach (var axie in dicAxies.Values)
        {
            axie.GetComponent<AxieControl>().Run();
        }
        
        DOVirtual.DelayedCall(2f, () =>
        {
            enemyUnits.Clear();
            enemyControls.Clear();

            heroControl.Idle();
            foreach (var axie in dicAxies.Values)
            {
                axie.GetComponent<AxieControl>().Idle();
            }

            BG.RunBG(false);
            state = BattleState.START;

            SetupStage();
            ResetTarget();
        });
    }

    void SetTarget(int index)
    {
        indexTarget = index;

        for (int i = 0; i < enemyHUDs.Count; i++)
        {
            if (i == index)
            {
                enemyHUDs[i].SetTarget(true);
            }else
            {
                enemyHUDs[i].SetTarget(false);
            }
        }
    }

    public void OnSetTarget(int index)
    {
        if (state != BattleState.HEROTURN)
            return;

        SetTarget(index);
    }

    public void ResetTarget()
    {
        bool isWon = true;
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            if (enemyUnits[i].currentHP > 0)
            {
                isWon = false;
                SetTarget(i);
                return;
            }
        }

        if (isWon)
        {
            state = BattleState.WON;
            EndBattle();
        }
    }

    void EndBattle()
	{
		if(state == BattleState.WON)
		{
			dialogueText.text = "You won the battle!";
		} else if (state == BattleState.LOST)
		{
			dialogueText.text = "You were defeated.";
		}
        GameUI.Instance.endTurnBtn.SetActive(false);
        GameUI.Instance.nextBtn.SetActive(true);
    }

	public void OnAttackButton()
	{
        if (state != BattleState.HEROTURN)
            return;

        heroHUD.SetStamina(heroUnit.currentStamina);

        StartCoroutine(HeroAttack());
	}

	public void OnHealButton()
	{
        if (state != BattleState.HEROTURN)
			return;

        heroHUD.SetStamina(heroUnit.currentStamina);
        StartCoroutine(HeroHeal());
	}
}
