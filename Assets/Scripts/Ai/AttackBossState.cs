using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackBoss", menuName = "States/AttackBoss")]
public class AttackBossState : State
{
    private Vector2 ArmDir;
    private bool HelpDone;
    private bool MeteorDone;

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
        if (!Me.OnGround)
        {
            ArmDir = Vector2.Lerp(ArmDir, Me.BodyDirection(Enemy), 0.25f);
            aiController.Movement(Me.Direction(Enemy.transform));
            aiController.MoveArm(ArmDir);
        }
        else if(Me.HpProcent() < 0.66f && !HelpDone)
        {
            HelpDone = true;
            Level.active.CreateBossHelpEnemy(3);
        }
        else if(Me.HpProcent() < 0.33f && !MeteorDone)
        {
            MeteorDone = true;
            Level.active.Meteor(3, 0f);
        }
        else if(Me.NoAttack)
        {
            aiController.RamRun();
        }
        else if (aiController.SuperAction)
        {
            aiController.Movement(Me.Direction(Enemy.transform) * 1f);
        }
        else if (!aiController.HaveWeapon())
        {
            aiController.RamFast();
        }
        else if (!aiController.NoThrow && Enemy.HpProcent() < 0.25f && Me.DistX(Enemy) > 5)
        {
            Vector2 Dir = (Me.BodyDirection(Enemy) + Vector2.up * Me.DistX(Enemy) / 50).normalized;
            aiController.Throw(Dir);
        }
        else if (Me.DistX(Enemy) > Me.weapon.Lenght() * (aiController.SumVelocity() + 0.9f))
        {
            ArmDir = Vector2.Lerp(ArmDir, (-Me.BodyDirection(Enemy) + Vector2.up).normalized, 0.1f);
            aiController.Movement(Me.Direction(Enemy.transform));
            if (aiController.SwingCoroutine == null)
            {
                aiController.MoveArm(ArmDir);
            }
        }
        else if(Me.DistX(Enemy) < Me.weapon.Lenght() * 0.5f)
        {
            if(Random.Range(0, 2) == 0)
            {
                aiController.RamFast();
            }
            else
            {
                aiController.MegaSwing(Random.Range(0.5f, 1f));
            }
        }
        else
        {
            aiController.Swing(Me.BodyDirection(Enemy), Random.Range(0, 2) == 0);
            aiController.Movement(Me.Direction(Enemy.transform) * 0.1f);
        }


        if (Me.DistX(Enemy) > aiController.ViewLenght)
        {
            aiController.LooseEnemy(Enemy);
        }
    }
}
