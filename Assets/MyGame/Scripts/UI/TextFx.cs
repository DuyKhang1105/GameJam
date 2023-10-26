using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum TypeText { HIT, CRIT, HEAL, SHIELD }


public class TextFx : MonoBehaviour
{
    public static TextFx Create(Vector3 pos, int value, TypeText typeText)
    {
        Vector3 newPos = pos + new Vector3(0, 1, 0);
        GameObject textFxTransform = Instantiate(GameUI.Instance.textFx, newPos, Quaternion.identity);

        TextFx textFx = textFxTransform.GetComponent<TextFx>();
        textFx.SetUp(value, typeText);

        return textFx;
    }

    private TextMeshPro textMeshPro;
    private float disappearTimer;
    private Color textColor;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();
    }

    public void SetUp(int value, TypeText typeText)
    {
        switch (typeText)
        {
            case TypeText.HIT:
                textMeshPro.SetText($"- {value}");
                textMeshPro.fontSize = 5;
                textColor = new Color32(255, 238, 30, 255);
                break;

            case TypeText.CRIT:
                textMeshPro.SetText($"- {value}");
                textMeshPro.fontSize = 6.5f;
                textColor = new Color32(255, 0, 0, 255);
                break;

            case TypeText.HEAL:
                textMeshPro.SetText($"+ {value}");
                textMeshPro.fontSize = 5;
                textColor = new Color32(0, 255, 0, 255);
                break;

            case TypeText.SHIELD:
                textMeshPro.SetText($"+ {value}");
                textMeshPro.fontSize = 5;
                textColor = new Color32(0, 150, 255, 255);
                break;

            default:
                break;
        }

        textMeshPro.color = textColor;
        disappearTimer = 0.5f;
    }

    private void Update()
    {
        float moveYSpeed = 3f;
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMeshPro.color = textColor;

            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
