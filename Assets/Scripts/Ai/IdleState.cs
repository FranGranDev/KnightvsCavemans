using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Idle", menuName = "States/Idle")]
public class IdleState : State
{
    public override void Run()
    {
        aiController.Movement(Vector2.zero);
        aiController.MoveArm(aiController.ThisPlayer.Rig.velocity.normalized);
        Man man = aiController.CheckForEnemy(aiController.Eye.right);
        if(man != null)
        {
            aiController.GetEnemy(man);
        }

        if(aiController.Enemy != null)
        {
            isFinished = true; 
        }
    }
}
