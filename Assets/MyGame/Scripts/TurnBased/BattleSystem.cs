using DG.Tweening;
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

public enum BattleState { START, HEROTURN, ENEMYTURN, ACTING, WON, LOST , LOOTITEM, LOOTAXIE, MOVETONEXT}

public enum ActionType { ATTACK, HEAL, SHIELD, POW}


public class BattleSystem : MonoBehaviour
{
    [Header("Stage")]
    public int stageIndex;

    [Header("Pet")]
    public List<Transform> axieBattleStations;
    public Dictionary<string, GameObject> dicAxies;

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

    public int indexEnemy;
    public int indexTarget;

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
            axieGO.GetComponent<AxieUnit>().Parse(config);

            dicAxies.Add(config.axieId, axieGO);

            FxManager.Instance.Create(axieBattleStations[i].position, TypeFx.SPAWN_PET);
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
        IeSetupStage();
    }

    private void IeSetupStage()
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

        var axieInventory = FindObjectOfType<AxieInventory>();

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
                //yield return new WaitForSeconds(0.5f);

                ResetTarget();

                indexEnemy = 0;
                indexTarget = 0;
                NextActionEnemy();

                StartCoroutine(HeroTurn());
                break;
            //case StageType.MiniBoss:
            //    //TODO 
            //    break;
            //case StageType.Boss:
            //    //TODO
            //    break;
            case StageType.Chest:
                GameUI.Instance.chest.GetComponent<Chest>().isOpend = false;
                GameUI.Instance.chest.SetActive(true);
                var chest = stage.chest;
                var hasUpgradeChest = axieInventory.axies.Any(x => x.skillType == AxieSkillType.ExtensionChest);
                var count = hasUpgradeChest? chest.count + 1 : chest.count;
                GameUI.Instance.chest.GetComponent<Chest>().InitChest(chest.rareMax, count, chest.itemIds);
                state = BattleState.LOOTITEM;
                break;
            case StageType.AxieChest:
                GameUI.Instance.axieChest.GetComponent<AxieChest>().isOpend = false;
                GameUI.Instance.axieChest.SetActive(true);
                var hasUpgradeAxieChest = axieInventory.axies.Any(x => x.skillType == AxieSkillType.ExtensionAxieChest);
                GameUI.Instance.axieChest.GetComponent<AxieChest>().count = hasUpgradeAxieChest ? 4 : 3; 
                state = BattleState.LOOTAXIE;
                break;
        }
    }

	IEnumerator HeroAttack()
	{
        int damage = heroUnit.GetDamage();

        bool isMiss = enemyUnits[indexTarget].TakeDamage(damage, heroUnit.CheckAP());
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
            }
            else
            {
                if (heroUnit.isCrit)
                {
                    FxManager.Instance.Create(enemyControls[indexTarget].transform.position, TypeFx.HIT_CRIT);
                    TextFx.Create(enemyControls[indexTarget].transform.position, damage, TypeText.CRIT);
                }
                else
                {
                    FxManager.Instance.Create(enemyControls[indexTarget].transform.position, TypeFx.HIT);
                    TextFx.Create(enemyControls[indexTarget].transform.position, damage, TypeText.HIT);
                }

                if (!isDead)
                {
                    enemyControls[indexTarget].OneHit();
                }
                else
                {
                    enemyControls[indexTarget].Die();
                }

                Debug.LogError("heroUnit.isBloodSucking: " + heroUnit.isBloodSucking);
                if (heroUnit.isBloodSucking)
                {
                    int bloodEnemyLost = enemyUnits[indexTarget].bloodLost;
                    heroUnit.isBloodSucking = false;
                    heroUnit.Heal(bloodEnemyLost);
                    heroHUD.SetHP(heroUnit.currentHP);
                    TextFx.Create(heroUnit.transform.position, bloodEnemyLost, TypeText.HEAL);
                    FxManager.Instance.Create(enemyControls[indexTarget].transform.position, TypeFx.BLOOD);
                }

                enemyHUDs[indexTarget].SetPow(enemyUnits[indexTarget].currentPow);
                enemyHUDs[indexTarget].SetHP(enemyUnits[indexTarget].currentHP);
                enemyHUDs[indexTarget].SetShield(enemyUnits[indexTarget].shield);
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

    IEnumerator HeroShiel(int shi)
    {
        state = BattleState.ACTING;

        heroUnit.Shield(shi);
        heroControl.Shield();

        heroHUD.SetShield(heroUnit.shield);

        TextFx.Create(heroUnit.transform.position, shi, TypeText.SHIELD);
        FxManager.Instance.Create(heroUnit.transform.position, TypeFx.BUFF_HERO);
        yield return new WaitForSeconds(2f);
        state = BattleState.HEROTURN;
    }

    IEnumerator HeroHeal(int heal)
	{
        state = BattleState.ACTING;

        heroUnit.Heal(heal);
		heroControl.Heal();


        heroHUD.SetHP(heroUnit.currentHP);
        TextFx.Create(heroUnit.transform.position, heal, TypeText.HEAL);
        FxManager.Instance.Create(heroUnit.transform.position, TypeFx.BUFF_HERO);

        yield return new WaitForSeconds(2f);
        state = BattleState.HEROTURN;
    }

    IEnumerator HeroStamina(int stamina)
    {
        state = BattleState.ACTING;

        heroUnit.GetStamina(stamina);
        heroControl.Heal();

        heroHUD.SetStamina(heroUnit.currentStamina);
        TextFx.Create(heroUnit.transform.position, stamina, TypeText.STAMINA);
        FxManager.Instance.Create(heroUnit.transform.position, TypeFx.BUFF_HERO);

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

		bool isDead = ActionEnemy(indexEnemy);

        yield return new WaitForSeconds(2.5f);

        if (isDead)
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
        
        NextActionEnemy();

        heroUnit.ClearFightsback();
        StartCoroutine(HeroTurn());    
    }

    IEnumerator HeroTurn()
	{
        heroUnit.ResetStamina();
        heroHUD.SetStamina(heroUnit.currentStamina);

        //Check action axie
        foreach (var axie in dicAxies.Values)
        {
            if (axie.GetComponent<AxieUnit>().CheckBuff())
            {
                //TODO time wait for each axie
                state = BattleState.ACTING;
                yield return new WaitForSeconds(2f);
            }
        }

        state = BattleState.HEROTURN;

        if (heroUnit.CheckStun())
        {
            DOVirtual.DelayedCall(1f, OnEndTurn);
            yield break;
        }
        else
        {
            heroControl.Idle();
        }
        
		GameUI.Instance.endTurnBtn.SetActive(true);
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
        int damage = enemyUnits[indexEnemy].damage;

        if (heroUnit.isFightsback)
        {
            damage = 0;
        }
        
        bool isMiss = heroUnit.TakeDamage(damage, enemyUnits[indexEnemy].CheckAP());
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
            if (heroUnit.isFightsback)
            {
                heroUnit.axieBuff.GetComponent<AxieUnit>().Fightsback(enemyUnits[indexEnemy].damage);
                FxManager.Instance.Create(heroUnit.transform.position, TypeFx.HIT);
                TextFx.Create(heroControl.transform.position, 0, TypeText.HIT);
            }
            else
            {
                if (isMiss)
                {
                    heroControl.Dodge();
                }
                else
                {
                    //Currently enemy dont has crit
                    TextFx.Create(heroControl.transform.position, enemyUnits[indexEnemy].damage, TypeText.HIT);
                    FxManager.Instance.Create(heroUnit.transform.position, TypeFx.HIT);

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
        FxManager.Instance.Create(enemyUnits[indexEnemy].transform.position, TypeFx.BUFF_ENEMY);
    }

    void EnemyHeal(int indexEnemy)
    {
        enemyUnits[indexEnemy].Heal(5);
        enemyControls[indexEnemy].Buff();
        enemyHUDs[indexEnemy].SetHP(enemyUnits[indexEnemy].currentHP);
        TextFx.Create(enemyUnits[indexEnemy].transform.position, 5, TypeText.HEAL);
        FxManager.Instance.Create(enemyUnits[indexEnemy].transform.position, TypeFx.BUFF_ENEMY);
    }

    void EnemySkill(int indexEnemy)
    {
        enemyControls[indexEnemy].Buff();
        enemyUnits[indexEnemy].Skill()?.Invoke();
        enemyHUDs[indexEnemy].SetPow(enemyUnits[indexEnemy].currentPow);
    }
        
	public void OnEndTurn()
	{
        if (state != BattleState.HEROTURN)
            return;

        GameUI.Instance.endTurnBtn.SetActive(false);

        bool isAllEnemiesDead = enemyUnits.All(e => e.isDead);
        if (isAllEnemiesDead)
        {
            DOVirtual.DelayedCall(1f, OnNext);
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }


    public void OnNext()
    {
        heroUnit.ClearFightsback();
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

        Debug.LogError("AutoEndTurn");
        OnEndTurn();  
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

    public void EndBattle()
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

    public void OnHealButton(int heal)
	{
        if (state != BattleState.HEROTURN)
            return;
        OnHeroHeal(heal);
    }

    public void OnHeroHeal(int heal)
    {
        heroHUD.SetStamina(heroUnit.currentStamina);
        StartCoroutine(HeroHeal(heal));
    }

    public void OnShielButton(int shi)
    {
        if (state != BattleState.HEROTURN)
            return;
        OnHeroShiel(shi);
    }

    public void OnHeroShiel(int shi)
    {
        heroHUD.SetStamina(heroUnit.currentStamina);
        StartCoroutine(HeroShiel(shi));
    }

    public void OnHeroStamina(int stamina)
    {
        heroHUD.SetStamina(heroUnit.currentStamina);
        StartCoroutine(HeroStamina(stamina));
    }
}
