using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxManager : MonoSingleton<FxManager>
{
    public void Create(Vector3 pos, TypeFx typeFx)
    {
        Vector3 newPos = pos;

        if (typeFx == TypeFx.SUMMON_2)
        {
            newPos += new Vector3(0, 1, 0);
        }

        GameObject graphicFx = FxConfigs.Instance.GetFxConfig(typeFx).graphic;

        GameObject goFx = Instantiate(graphicFx);
        goFx.transform.position = newPos;

    }
}
