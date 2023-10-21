using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public List<Parallax> parallaxs = new List<Parallax>();
    public bool isLeft;

    public void RunBG(bool isEnable)
    {
        foreach (var parallax in parallaxs)
        {
            parallax.SetScrollLeft(isLeft);
            parallax.enabled = isEnable;
        }
    }
}
