using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Duel", menuName = "States/Duel")]
public class DuelState : State
{
    private bool SwingReady;
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
        if (Me.DistY(Enemy.transform) > 4f * Me.Size && Me.DistX(Enemy) < 5)
        {
            aiController.Jump();
        }
        if (!Me.OnGround && aiController.HaveWeapon())
        {
            if (Me.DistX(Enemy) < Me.weapon.Lenght())
            {
                aiController.Swing(Me.BodyDirection(Enemy), Random.Range(0, 2) == 0);
            }
            else if (aiController.SwingCoroutine == null)
            {
                ArmDir = Vector2.Lerp(ArmDir, Me.BodyDirection(Enemy), 0.25f);
                aiController.MoveArm(ArmDir);
            }
            aiController.Movement(ArmDir);
        }
        else if(!Me.OnGround)
        {
            ArmDir = Vector2.Lerp(ArmDir, Me.BodyDirection(Enemy), 0.25f);
            aiController.MoveArm(ArmDir);
            aiController.Movement(ArmDir);
        }
        else if (!aiController.HaveWeapon())
        {
            aiController.RamRun();
        }
        else if (!aiController.NoThrow && Enemy.HpProcent() < 0.25f && Me.DistX(Enemy) > 5)
        {
            Vector2 Dir = (Me.BodyDirection(Enemy) + Vector2.up * Me.DistX(Enemy) / 50).normalized;
            aiController.Throw(Dir);
        }
        else if (Me.DistX(Enemy) > Me.weapon.Lenght() * (aiController.SumVelocity() + 0.9f))
        {
            if (!SwingReady)
            {
                SwingReady = true;
                aiController.SwingReady(Me.BodyDirection(Enemy), Random.Range(0, 2) == 0);
            }
            aiController.Movement(Me.Direction(Enemy.transform));
        }
        else
        {
            SwingReady = false;
            aiController.Swing(Me.BodyDirection(Enemy), Random.Range(0, 2) == 0);
        }


        if (Me.DistX(Enemy) > aiController.ViewLenght)
        {
            aiController.LooseEnemy(Enemy);
        }
    }
}

