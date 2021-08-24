using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Walk", menuName = "States/Walk")]
public class WalkState : State
{
    public override void Run()
    {
        aiController.RandomWalk();
        Man man = aiController.CheckForEnemy(aiController.Eye.right);
        if (man != null)
        {
            aiController.GetEnemy(man);
        }

        if (aiController.Enemy != null)
        {
            isFinished = true;
        }
    }
}
