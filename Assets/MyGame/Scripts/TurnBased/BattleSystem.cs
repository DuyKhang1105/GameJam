using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

public enum BattleState { START, HEROTURN, ENEMYTURN, ACTING, WON, LOST , LOOTITEM, LOOTAXIE, MOVETONEXT}

public enum ActionType { ATTACK, HEAL, SHIELD, POW}


public class BattleSystem : MonoBehaviour
{
    [Header("Stage")]
    public int stageIndex;

    [Header("Pet")]
    public List<Transform> axieBattleStations;
    private Dictionary<string, GameObject> dicAxies;

    [Header("Hero")]
    public GameObject heroPrefab;

    public HeroUnit heroUnit;
	public HeroBattleHUD heroHUD;
    public HeroControl heroControl;
	public Transform heroBattleStation;

    [Header("Hero")]
    public List<EnemyUnit> enemyUnits;
	public List<EnemyBattleHUD> enemyHUDs;
    public List<EnemyControl> enemyControls;
    public List<Transform> enemyBattleStations;

	[Header("GUI")]
	public BattleState state;
    public TextMeshProUGUI dialogueText;

    public Background BG;
    public ProgressLevel progressLevel;

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
            axieGO.transform.localPosition = Vector3.zero;
            axieGO.transform.localScale = new Vector3(-0.8f, 0.8f, 0.8f);
            axieGO.GetComponent<AxieControl>().Idle();
            dicAxies.Add(config.axieId, axieGO);
        }

        Debug.Log("Axies: " + string.Join(", ", axieInventory.axies));
    }

    private void InitHero()
    {
        if (heroUnit == null)
        {
            GameObject heroGO = Instantiate(heroPrefab, heroBattleStation);
            heroUnit = heroGO.GetComponent<HeroUnit>();
            heroControl = heroGO.GetComponent<HeroControl>();
        }
        heroHUD.SetHeroHUD(heroUnit);
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
        List<EnemyConfig> enemyConfigs = new List<EnemyConfig>();

        foreach (var name in stage.enemyNames)
        {
            EnemyConfig enemy = EnemyConfigs.Instance.GetEnemyConfig(name);
            enemyConfigs.Add(enemy);
        }

        Debug.LogError("stage.stageType: " + stage.stageType);
        Debug.LogError("count enemyConfigs: " + enemyConfigs.Count);
        switch (stage.stageType) {
            case StageType.Enemy:
            case StageType.MiniBoss:
            case StageType.Boss:
                for (int i = 0; i < enemyConfigs.Count; i++)
                {
                    GameObject enemyGO = Instantiate(enemyConfigs[i].graphic, enemyBattleStations[i]);
                    EnemyUnit enemyUnit = enemyGO.GetComponent<EnemyUnit>();
                    enemyUnit.Parse(enemyConfigs[i]);
                    enemyUnits.Add(enemyUnit);

                    enemyControls.Add(enemyGO.GetComponent<EnemyControl>());

                    enemyHUDs[i].gameObject.SetActive(true);
                    enemyHUDs[i].SetEnemyHUD(enemyUnits[i]);
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
            //case StageType.MiniBoss:
            //    //TODO 
            //    break;
            //case StageType.Boss:
            //    //TODO
            //    break;
            case StageType.Chest:
                //if (stageIndex == 0)
                //{
                //    GameUI.Instance.startChest.GetComponent<StartChest>().isOpened = false;
                //    GameUI.Instance.startChest.SetActive(true);
                //}       
                //else
                {
                    GameUI.Instance.chest.GetComponent<Chest>().isOpend = false;
                    GameUI.Instance.chest.SetActive(true);
                    //TODO check axie add item
                    var chest = stage.chest;
                    GameUI.Instance.chest.GetComponent<Chest>().InitChest(chest.rareMax, chest.count, chest.itemIds);
                }
                state = BattleState.LOOTITEM;
                break;
            case StageType.AxieChest:
                GameUI.Instance.axieChest.GetComponent<AxieChest>().isOpend = false;
                GameUI.Instance.axieChest.SetActive(true);
                GameUI.Instance.axieChest.GetComponent<AxieChest>().count = 3; //TODO check axie add count
                state = BattleState.LOOTAXIE;
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
            {
                heroControl.Critical();
            }
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
                if (heroUnit.isCrit)
                    TextFx.Create(enemyControls[indexTarget].transform.position, heroUnit.GetDamage(), TypeText.CRIT);
                else
                    TextFx.Create(enemyControls[indexTarget].transform.position, heroUnit.GetDamage(), TypeText.HIT);

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
                enemyHUDs[indexTarget].SetShield(enemyUnits[indexTarget].shield);
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

	IEnumerator HeroHeal(int heal)
	{
        state = BattleState.ACTING;

        heroUnit.Heal(heal);
		heroControl.Heal();


        heroHUD.SetHP(heroUnit.currentHP);
		dialogueText.text = "You feel renewed strength!";

		yield return new WaitForSeconds(2f);
        state = BattleState.HEROTURN;
    }

    IEnumerator HeroStamina(int stamina)
    {
        state = BattleState.ACTING;

        heroUnit.GetStamina(stamina);
        heroControl.Heal();


        heroHUD.SetStamina(heroUnit.currentStamina);
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

        yield return new WaitForSeconds(2f);

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
        if (heroUnit.CheckStun())
        {
            DOVirtual.DelayedCall(1f, OnEndTurn);
            return;
        }
        else
        {
            heroControl.Idle();
        }

		dialogueText.text = heroUnit.unitName + " Turn. Choose An Action";
        heroUnit.ResetStamina();
        heroHUD.SetStamina(heroUnit.currentStamina);
		GameUI.Instance.endTurnBtn.SetActive(true);
        GameUI.Instance.endTurnBtn.GetComponentInChildren<TurnClockUI>().StartTime(30f, OnEndTurn);
        //GameUI.Instance.currentTurn.SetActive(true);
        //GameUI.Instance.currentTurn.GetComponentInChildren<TextMeshProUGUI>().text = "Hero Turn";
        //Notification.Instance.ShowNoti("Hero turn begin");
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
                //Currently enemy dont has crit
                TextFx.Create(heroControl.transform.position, enemyUnits[indexEnemy].damage, TypeText.HIT);

                if (isDead)
                {
                    heroControl.Dead();
                }
                else
                {
                    heroControl.OneHit();
                }
                heroHUD.SetHP(heroUnit.currentHP);
                heroHUD.SetShield(heroUnit.shield);
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
        enemyHUDs[indexEnemy].SetShield(enemyUnits[indexEnemy].shield);
        TextFx.Create(enemyUnits[indexEnemy].transform.position, 5, TypeText.SHIELD);
    }

    void EnemyHeal(int indexEnemy)
    {
        enemyUnits[indexEnemy].Heal(5);
        enemyControls[indexEnemy].Buff();
        enemyHUDs[indexEnemy].SetHP(enemyUnits[indexEnemy].currentHP);
        TextFx.Create(enemyUnits[indexEnemy].transform.position, 5, TypeText.HEAL);
    }

    void EnemySkill(int indexEnemy)
    {
        enemyControls[indexEnemy].Buff();
        enemyUnits[indexEnemy].Skill()?.Invoke();
        enemyHUDs[indexEnemy].SetPow(enemyUnits[indexEnemy].currentPow);
    }
        
	public void OnEndTurn()
	{
        GameUI.Instance.endTurnBtn.SetActive(false);

        if (state != BattleState.HEROTURN)
            return;

        bool isAllEnemiesDead = enemyUnits.All(e => e.isDead);
        if (isAllEnemiesDead)
        {
            OnNext();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
            GameUI.Instance.endTurnBtn.SetActive(false);
        }
        
    }

    public void OnNext()
    {
        GameUI.Instance.nextBtn.SetActive(false);

        //check upgrade level
        var stageConfig = StageConfigs.Instance.GetStage(stageIndex);
        if (stageConfig.isLevelUp)
        {
            var inventory = GameUI.Instance.inventory.GetComponent<Inventory>();
            inventory.level += 1;
            inventory.UpgradeLevel(inventory.level);
            Notification.Instance.ShowNoti($"Congrat. Inventory upgrade to level {inventory.level}!!");
        }
        stageIndex += 1;

        progressLevel.UpdateProgressLevel(stageIndex);

        state = BattleState.MOVETONEXT;

        //check end battle
        if (stageIndex >= StageConfigs.Instance.stageConfigs.Count)
        {
            state = BattleState.WON;
            EndBattle();
            return;
        }

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
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            if (enemyUnits[i].currentHP > 0)
            {
                SetTarget(i);
                return;
            }
        }
    }

    public bool CheckWin()
    {
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            if (enemyUnits[i].currentHP > 0)
            {
                return false;
            }
        }
        return true;
    }

    void EndBattle()
	{
		if(state == BattleState.WON)
		{
			dialogueText.text = "You won the battle!";
            GameUI.Instance.endGamePopup.SetActive(true);
            GameUI.Instance.endGamePopup.GetComponent<EndGamePopup>().Show(true);
            return;
		} 
        else if (state == BattleState.LOST)
		{
			dialogueText.text = "You were defeated.";
            GameUI.Instance.endGamePopup.SetActive(true);
            GameUI.Instance.endGamePopup.GetComponent<EndGamePopup>().Show(false);
            return;
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
        OnHeroHeal(5);
    }

    public void OnHeroHeal(int heal)
    {
        heroHUD.SetStamina(heroUnit.currentStamina);
        StartCoroutine(HeroHeal(heal));
    }

    public void OnHeroStamina(int stamina)
    {
        heroHUD.SetStamina(heroUnit.currentStamina);
        StartCoroutine(HeroStamina(stamina));
    }
}
