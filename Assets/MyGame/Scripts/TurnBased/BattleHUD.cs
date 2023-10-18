using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{

	public Text nameText;
	public Text levelText;
	public TextMeshProUGUI tmpHP;
	public Image imgHP;

	int maxHP;

	public void SetHUD(Unit unit)
	{
		//nameText.text = unit.unitName;
		//levelText.text = "Lvl " + unit.unitLevel;
		maxHP = unit.maxHP;

        tmpHP.SetText($"{unit.currentHP}/{unit.maxHP}");
		imgHP.fillAmount = (float)unit.currentHP/unit.maxHP;

    }

	public void SetHP(int hp)
	{
        tmpHP.SetText($"{hp}/{maxHP}");
        imgHP.fillAmount = (float)hp / maxHP;
    }

}
