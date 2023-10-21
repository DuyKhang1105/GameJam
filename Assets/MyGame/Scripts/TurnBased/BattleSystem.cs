using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, HEROTURN, ENEMYTURN, ACTING, WON, LOST }

public class BattleSystem : MonoBehaviour
{

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

	[Header("other test")]
    public Background BG;
	public GameObject goNext;
	public GameObject goEndTurn;

	int indexEnemy;
    int indexTarget;
	List<int> indexActions = new List<int>();

    void Start()
    {
        goNext.SetActive(false);
        state = BattleState.START;

        StartCoroutine(SetupBattle());
        ResetTarget();
    }

	IEnumerator SetupBattle()
	{
        if (heroUnit == null)
        {
            GameObject heroGO = Instantiate(heroPrefab, heroBattleStation);
            heroUnit = heroGO.GetComponent<Unit>();
            heroControl = heroGO.GetComponent<HeroControl>();
        }

        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            Debug.LogError("Instantiate");
            GameObject enemyGO = Instantiate(enemyPrefabs[i], enemyBattleStations[i]);
            enemyUnits.Add(enemyGO.GetComponent<Unit>());
            enemyControls.Add(enemyGO.GetComponent<EnemyControl>());
            enemyHUDs[i].gameObject.SetActive(true);
            enemyHUDs[i].SetHUD(enemyUnits[i]);
        }

        dialogueText.text = "A wild approaches...";

        heroHUD.SetHUD(heroUnit);

		yield return new WaitForSeconds(2f);

        indexEnemy = 0;
        indexTarget = 0;
        NextActionEnemy();

        state = BattleState.HEROTURN;
		HeroTurn();
	}

	IEnumerator HeroAttack()
	{
		bool isDead = enemyUnits[indexTarget].TakeDamage(heroUnit.damage);
        state = BattleState.ACTING;

        // Code tam
        Sequence s = DOTween.Sequence();
		s.Append(heroUnit.transform.DOMoveX(0f, 1f));
		s.AppendCallback(() =>
		{ 
			heroControl.OneAttack();
        });
		s.AppendInterval(0.5f);
        s.AppendCallback(() =>
        {
            if (!isDead)
            {
                enemyControls[indexTarget].OneHit();
            }else
            {
                enemyControls[indexTarget].Die();
            }

            enemyHUDs[indexTarget].SetHP(enemyUnits[indexTarget].currentHP);
            enemyHUDs[indexTarget].SetShield(enemyUnits[indexTarget].shield);
            dialogueText.text = "The attack is successful!";
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
            indexEnemy++;
            CheckIgnoresDeadEnemies();

            if (indexEnemy < totalEnemy)
            {
                StartCoroutine(EnemyTurn());
            }
            else
            {
                ResetTarget();
                state = BattleState.HEROTURN;
                NextActionEnemy();
                HeroTurn();
            }
		}

	}

    void CheckIgnoresDeadEnemies()
    {
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            if (indexEnemy == i && enemyUnits[i].currentHP <= 0)
            {
                indexEnemy++;
            }
        }
    }

	void HeroTurn()
	{
		dialogueText.text = heroUnit.unitName + " Turn. Choose An Action";
        heroUnit.ResetStamina();
        heroHUD.SetStamina(heroUnit.currentStamina);
		goEndTurn.SetActive(true);
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
            int indexNext = Random.Range(0, 2);

            switch (indexNext)
            {
                case 0:
                    enemyHUDs[i].SetNextAction(true, enemyUnits[i].damage);
                    break;

                case 1:
                    enemyHUDs[i].SetNextAction(false, enemyUnits[i].shield);
                    break;

                default:
                    break;
            }

            indexActions.Add(indexNext);
        }
    }


    bool EnemyAttack(int indexEnemy)
	{
        bool isDead = heroUnit.TakeDamage(enemyUnits[indexEnemy].damage);

        Sequence s = DOTween.Sequence();
        s.Append(enemyUnits[indexEnemy].transform.DOMoveX(0f, 1f));//
        s.AppendCallback(() =>
        {
            enemyControls[indexEnemy].OneAttack();
        });
        s.AppendInterval(0.6f);
        s.AppendCallback(() =>
        {
            heroControl.OneHit();
            heroHUD.SetHP(heroUnit.currentHP);
            heroHUD.SetShield(heroUnit.shield);
        });
        s.AppendInterval(0.4f);
        s.Append(enemyUnits[indexEnemy].transform.DOMoveX(enemyBattleStations[indexEnemy].position.x, 1f));

		return isDead;
    }
		
	void EnemyShield(int indexEnemy)
	{
        enemyUnits[indexEnemy].Shield(5);
        enemyControls[indexEnemy].Heal();

        enemyHUDs[indexEnemy].SetShield(enemyUnits[indexEnemy].shield);
    }

	public void OnEndTurn()
	{
        if (state != BattleState.HEROTURN)
            return;

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
        goEndTurn.SetActive(false);
    }

    public void OnNext()
    {
        goNext.SetActive(false);
        BG.RunBG(true);
        heroControl.Run();
        
        DOVirtual.DelayedCall(2f, () =>
        {
            enemyUnits.Clear();
            enemyControls.Clear();

            heroControl.Idle();
            BG.RunBG(false);
            state = BattleState.START;

            StartCoroutine(SetupBattle());
            ResetTarget();
        });
    }

    void SetTarget(int index)
    {
        Debug.LogError("index: " + index);
        indexTarget = index;
        indexEnemy = index;
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
        goEndTurn.SetActive(false);
        goNext.SetActive(true);
    }

	public void OnAttackButton()
	{
        if (state != BattleState.HEROTURN)
            return;

        bool isEnough = heroUnit.TakeStamina(1);
        heroHUD.SetStamina(heroUnit.currentStamina);

        if (!isEnough)
            return;

        StartCoroutine(HeroAttack());
	}

	public void OnHealButton()
	{
        if (state != BattleState.HEROTURN)
			return;

        bool isEnough = heroUnit.TakeStamina(1);
        heroHUD.SetStamina(heroUnit.currentStamina);

        if (!isEnough)
            return;

        StartCoroutine(HeroHeal());
	}
}
