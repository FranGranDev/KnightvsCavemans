using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakeObject : SceneObject
{
    public override void GetHit(int GetHp, Man man)
    {
        if (Fly)
            return;
        Hp -= GetHp;
        if(Hp <= 0 && !Destroyed)
        {
            ThrowItem();
            DestroyObj();
        }
        else
        {
            Hit();
        }
    }

    private void ThrowItem()
    {
        int type = (Random.Range(0, 2));
        switch (type)
        {
            case 0:
                Buff buff = Instantiate(GameData.active.GetRandomBuff());
                buff.transform.position = transform.position;
                buff.transform.rotation = transform.rotation;
                buff.Rig.velocity = Rig.velocity * 0.5f;
                buff.Rig.angularVelocity = Rig.angularVelocity * 0.5f;
                break;
            case 1:
                Weapon weapon = Instantiate(GameData.active.GetRandomWeapon());
                weapon.transform.position = transform.position;
                weapon.transform.rotation = transform.rotation;
                weapon.ThrowOut();
                break;
        }
        

    }

}
