using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "States/Attack")]
public class AttackState : State
{
    private Vector2 ArmDir;

    public override void Run()
    {
        Man Enemy = aiController.Enemy;
        Man Me = aiController.ThisPlayer;

        if (Enemy == null || Enemy.Dead || Me.Dead || Level.active.NextLava(Enemy))
        {
            Enemy = null;
            isFinished = true;
            return;
        }
        if(Me.DistY(Enemy.transform) > 4f * Me.Size && Me.DistX(Enemy) < 5)
        {
            aiController.Jump();
        }
        if(!Me.OnGround)
        {
            ArmDir = Vector2.Lerp(ArmDir, Me.BodyDirection(Enemy), 0.05f);
            aiController.Movement(ArmDir);
            aiController.MoveArm(ArmDir);
        }
        else if(!aiController.HaveWeapon())
        {
            aiController.RamRun();
        }
        else if(Me.NoAttack)
        {
            aiController.Movement(Vector2.zero);
        }
        else if(!aiController.NoThrow && Enemy.HpProcent() < 0.25f && Me.DistX(Enemy) > 10)
        {
            Vector2 Dir = (Me.BodyDirection(Enemy) + Vector2.up * Me.DistX(Enemy) / 50).normalized;
            aiController.Throw(Dir);
        }
        else if (Me.DistX(Enemy) > Me.weapon.Lenght() * (aiController.SumVelocity() + 0.9f))
        {
            ArmDir = Vector2.Lerp(ArmDir, (Me.BodyDirection(Enemy) + Vector2.up * 0.25f).normalized, 0.1f);
            aiController.Movement(Me.Direction(Enemy.transform));
            if (aiController.SwingCoroutine == null)
            {
                aiController.MoveArm(ArmDir);
            }
        }
        else
        {
            aiController.Swing(Me.BodyDirection(Enemy), Random.Range(0, 2) == 0);
            //Me.Direction(Enemy.transform) * 0.25f
        }


        if (Me.DistX(Enemy) > aiController.ViewLenght)
        {
            aiController.LooseEnemy(Enemy);
        }
    }
}
