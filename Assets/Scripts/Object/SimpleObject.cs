using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleObject : SceneObject
{
    public override void GetHit(int GetHp, Man man)
    {
        if (Fly)
            return;
        Hp -= GetHp;
        if (Hp <= 0 && !Destroyed)
        {
            DestroyObj();
        }
        else
        {
            Hit();
        }
    }
}
