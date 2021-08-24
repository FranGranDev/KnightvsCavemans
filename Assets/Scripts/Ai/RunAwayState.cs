using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RunAway", menuName = "States/RunAway")]
public class RunAwayState : State
{
    public override void Run()
    {
        Man Enemy = aiController.Enemy;
        Man Me = aiController.ThisPlayer;
        if (Me.DistX(Enemy) < 25)
        {
            aiController.Movement(-Me.Direction(Enemy.transform));
        }
        else
        {
            aiController.Movement(Vector2.zero);
        }
    }
}
