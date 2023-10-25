using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class ProgressLevel : MonoBehaviour
{
    public int maxLevel;
    public Image imgFill;

    public Sprite sprEmptyTag;
    public Sprite sprActiveTag;
    public Sprite sprBottleEmty;
    public Sprite sprBottleActive;
    public Sprite sprChest;
    public Sprite sprChestAxie;
    public Sprite sprChestReceived;

    public List<Image> imgCheckPoints = new List<Image>();

    private void Start()
    {
        Reset();
    }

    public void UpdateProgressLevel(int stageIndex)
    {
        imgFill.DOFillAmount((float)stageIndex / maxLevel, 1f);

        int indexOldStage = stageIndex - 1;
        StageConfig stage = StageConfigs.Instance.GetStage(indexOldStage);


        Debug.LogError("stageIndex: " + stageIndex);
        Debug.LogError("index: " + indexOldStage);
        switch (stage.stageType)
        {
            case StageType.Enemy:
            case StageType.MiniBoss:
                imgCheckPoints[indexOldStage].sprite = sprEmptyTag;
                break;

            case StageType.Boss:
                imgCheckPoints[indexOldStage].sprite = sprBottleEmty;
                break;

            case StageType.Chest:
            case StageType.AxieChest:
                imgCheckPoints[indexOldStage].sprite = sprChestReceived;
                break;

            default:
                break;
        }
    }

    public void Reset()
    {
        StageConfig stage;

        for (int i = 0; i < imgCheckPoints.Count; i++)
        {
            stage = StageConfigs.Instance.GetStage(i);

            switch (stage.stageType)
            {
                case StageType.Enemy:
                case StageType.MiniBoss:
                    imgCheckPoints[i].sprite = sprActiveTag;
                    break;

                case StageType.Boss:
                    imgCheckPoints[i].sprite = sprBottleActive;
                    break;

                case StageType.Chest:
                    imgCheckPoints[i].sprite = sprChest;
                    break;

                case StageType.AxieChest:
                    imgCheckPoints[i].sprite = sprChestAxie;
                    break;

                default:
                    break;
            }
        }
    }
}
