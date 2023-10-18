using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{

	public GameObject playerPrefab;
	public GameObject enemyPrefab;

	public Transform playerBattleStation;
	public Transform enemyBattleStation;

	Unit playerUnit;
	Unit enemyUnit;

	KnightControl knightControl;
    SkeletonAnimation skeletonAnimationEnemy;

    public TextMeshProUGUI dialogueText;

	public BattleHUD playerHUD;
	public BattleHUD enemyHUD;

	public BattleState state;

    // Start is called before the first frame update
    void Start()
    {
		state = BattleState.START;
		StartCoroutine(SetupBattle());
    }

	IEnumerator SetupBattle()
	{
		GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
		playerUnit = playerGO.GetComponent<Unit>();
        knightControl = playerGO.GetComponent<KnightControl>();

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
		enemyUnit = enemyGO.GetComponent<Unit>();
        skeletonAnimationEnemy = enemyGO.GetComponent<SkeletonAnimation>();

        dialogueText.text = "A wild " + enemyUnit.unitName + " approaches...";

        playerHUD.SetHUD(playerUnit);
		enemyHUD.SetHUD(enemyUnit);

		yield return new WaitForSeconds(2f);

		state = BattleState.PLAYERTURN;
		PlayerTurn();
	}

	IEnumerator PlayerAttack()
	{
		bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

		// Code tam
		Sequence s = DOTween.Sequence();
		s.Append(playerUnit.transform.DOMoveX(5f, 1f));
		s.AppendCallback(() =>
		{ 
			knightControl.OneAttack();
        });
		s.AppendInterval(0.5f);
        s.AppendCallback(() =>
        {
			skeletonAnimationEnemy.AnimationState.SetAnimation(0, "defense/hit-by-normal", false);
            skeletonAnimationEnemy.AnimationState.AddAnimation(0, "action/idle/normal", true, 0);

            enemyHUD.SetHP(enemyUnit.currentHP);
            dialogueText.text = "The attack is successful!";
        });
		s.AppendInterval(0.4f);
        s.Append(playerUnit.transform.DOMoveX(-4.53f, 1f));

		// Code tam 

		yield return new WaitForSeconds(3f);

		if(isDead)
		{
			state = BattleState.WON;
			EndBattle();
		} else
		{
			state = BattleState.ENEMYTURN;
			StartCoroutine(EnemyTurn());
		}
	}

	IEnumerator EnemyTurn()
	{
		dialogueText.text = enemyUnit.unitName + " attacks!";

        yield return new WaitForSeconds(1f);

		bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

        // Code tam
        Sequence s = DOTween.Sequence();
        s.Append(enemyUnit.transform.DOMoveX(-3f, 1f));
        s.AppendCallback(() =>
        {
            skeletonAnimationEnemy.AnimationState.SetAnimation(0, "attack/melee/normal-attack", false);
            skeletonAnimationEnemy.AnimationState.AddAnimation(0, "action/idle/normal", true, 0);
        });
        s.AppendInterval(0.6f);
        s.AppendCallback(() =>
        {
            knightControl.OneHit();
            playerHUD.SetHP(playerUnit.currentHP);
        });
        s.AppendInterval(0.4f);
        s.Append(enemyUnit.transform.DOMoveX(6.78f, 1f));

        // Code tam 

        yield return new WaitForSeconds(3f);

		if(isDead)
		{
			state = BattleState.LOST;
			EndBattle();
		} else
		{
			state = BattleState.PLAYERTURN;
			PlayerTurn();
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
	}

	void PlayerTurn()
	{
		dialogueText.text = playerUnit.unitName + " Turn. Choose An Action";

	}

	IEnumerator PlayerHeal()
	{
		playerUnit.Heal(5);
		knightControl.Heal();


        playerHUD.SetHP(playerUnit.currentHP);
		dialogueText.text = "You feel renewed strength!";

		yield return new WaitForSeconds(2f);

		state = BattleState.ENEMYTURN;
		StartCoroutine(EnemyTurn());
	}

	public void OnAttackButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		StartCoroutine(PlayerAttack());
	}

	public void OnHealButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		StartCoroutine(PlayerHeal());
	}

}
