using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "States/Gun")]
public class GunState : State
{
    private Vector2 ArmDir;

    public override void Run()
    {
        Man Enemy = aiController.Enemy;
        Man Me = aiController.ThisPlayer;

        if (Enemy == null || Enemy.Dead || Me.Dead)
        {
            Enemy = null;
            isFinished = true;
            return;
        }
        if (Me.DistY(Enemy.transform) > 4f * Me.Size && Me.DistX(Enemy) < 5)
        {
            aiController.Jump();
        }
        if (!Me.OnGround)
        {
            ArmDir = Vector2.Lerp(ArmDir, Me.BodyDirection(Enemy), 0.05f);
            aiController.Movement(ArmDir);
            aiController.MoveArm(ArmDir);
        }
        else if (!aiController.HaveWeapon())
        {
            aiController.RamRun();
        }
        else if (Me.NoAttack)
        {
            aiController.Movement(Vector2.zero);
        }
        else if(Me.DistX(Enemy) >= 10)
        {
            ArmDir = Vector2.Lerp(ArmDir, Me.BodyDirection(Enemy), 0.025f);
            aiController.Movement(ArmDir);
            aiController.MoveArm(Vector2.up);
        }
        else if (Me.DistX(Enemy) < 10 && Me.DistX(Enemy) > 3)
        {
            ArmDir = Vector2.Lerp(ArmDir, Me.Direction(Enemy.transform), 0.01f);
            aiController.Movement(Vector2.zero);
            aiController.MoveArm(ArmDir);
        }
        else
        {
            aiController.Movement(Me.BodyDirection(Enemy) * 0.5f);
            aiController.Swing(Me.BodyDirection(Enemy), Random.Range(0, 2) == 0);
            //Me.Direction(Enemy.transform) * 0.25f
        }


        if (Me.DistX(Enemy) > aiController.ViewLenght)
        {
            aiController.LooseEnemy(Enemy);
        }
    }
}

