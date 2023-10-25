using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnClockUI : MonoBehaviour
{
    [SerializeField] private Image circleImg;
    [SerializeField] private TextMeshProUGUI timeTmp;

    private bool isStart;
    private float time;
    private float totalTime;

    private Action onEnded;

    public void StartTime(float t, Action callback)
    {
        time = t;
        totalTime = time;
        isStart = true;
        onEnded = callback; 
    }

    private void Update()
    {
        if (isStart)
        {
            time -= Time.deltaTime;
            if (time < 0)
            {
                isStart = false;
                onEnded?.Invoke();
                return;
            }
            timeTmp.text = ((int)time).ToString();
            circleImg.fillAmount = time / totalTime;
        }
    }

    private void OnDisable()
    {
        isStart = false;
    }
}
