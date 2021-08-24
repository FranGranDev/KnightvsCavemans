using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour
{
    public RectTransform Main;
    public RectTransform Fill;
    public RectTransform SecondFill;
    private float PrevProcent;
    public float Value()
    {
        return PrevProcent;
    }

    public void FillArea(float Procent)
    {
        PrevProcent = Procent;
        Fill.offsetMax = new Vector2(-1 * Main.rect.width * (1 - Procent), 0);
        if (SecondFill != null)
        {
            SecondFill.offsetMax = new Vector2(-1 * Main.rect.width, 0);
            Fill.transform.SetAsFirstSibling();
        }
    }

    public void FillArea(float Procent, float Second)
    {
        PrevProcent = Procent;
        Fill.offsetMax = new Vector2(-1 * Main.rect.width * (1 - Procent), 0);
        SecondFill.offsetMax = new Vector2(-1 * Main.rect.width * (1 - Second), 0);
        if(Second >= 0)
        {
            SecondFill.transform.SetAsFirstSibling();
        }
        else
        {
            Fill.transform.SetAsFirstSibling();
        }
    }
}
