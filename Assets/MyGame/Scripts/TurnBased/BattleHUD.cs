﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{

	public Image imgHP;
	public Image imgShield;
	public TextMeshProUGUI tmpHP;
	public TextMeshProUGUI tmpShield;

	int maxHP;

	public virtual void SetHUD(Unit unit)
	{
		maxHP = unit.maxHP;

        tmpShield.SetText($"{unit.shield}");
		SetHP(unit.currentHP);
		SetShield(unit.shield);
    }

	public virtual void SetHP(int hp)
	{
        tmpHP.SetText($"{hp}/{maxHP}");
        imgHP.fillAmount = (float)hp / maxHP;
    }

	public virtual void SetShield(int shi)
	{
		if (shi <= 0)
		{
			imgShield.gameObject.SetActive(false);
        }else
		{
            imgShield.gameObject.SetActive(true);
            tmpShield.SetText($"{shi}");
        }
    }
}
