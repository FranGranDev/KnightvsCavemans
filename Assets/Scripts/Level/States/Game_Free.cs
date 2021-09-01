using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Free", menuName = "GameType/Free")]
public class Game_Free : GameState
{
    public int MaxBattleEnemy;


    public override void OnStart()
    {
        level.Learning(false);
        level.SetGame(true);
        
        level.SetPlayerGame();
        level.SetPlayerWeapon();
        level.cameraMove.TurnPlayerFollow();
        level.sceneMaker.MakeEmptyScene(100);
        level.BoxSpawn();
        //CreateEnemys();

        level.cameraMove.UpToDown();
        level.DelayGame(1f);
        level.PrintText("Test", 3f);
        level.OnLevelStart();
    }

    public override void Run()
    {
        SetBattleMans();
        CheckForEnd();
    }

    private void CreateEnemys()
    {
        level.AllEnemy = level.CreateEnemy(Level.EnemyCreateType.Nude, 10, level.sceneMaker.GetEnemyPos(), 50);
        level.AliveEnemy = level.AllEnemy;
        level.DefeatedEnemy = new List<Man>();
        level.BattleEnemy = new List<Man>();
    }

    private void SetBattleMans()
    {
        for (int i = 0; i < level.AliveEnemy.Count - 1; i++)
        {
            for (int a = i + 1; a < level.AliveEnemy.Count; a++)
            {
                if (level.AliveEnemy[a].DistX(level.MainPlayer) <
                   level.AliveEnemy[i].DistX(level.MainPlayer))
                {
                    Man temp = level.AliveEnemy[i];
                    level.AliveEnemy[i] = level.AliveEnemy[a];
                    level.AliveEnemy[a] = temp;
                }
            }
        }

        for (int i = 0; i < level.AliveEnemy.Count; i++)
        {
            if (i < MaxBattleEnemy)
            {
                if (!level.BattleEnemy.Exists(item => item == level.AliveEnemy[i]))
                {
                    level.BattleEnemy.Add(level.AliveEnemy[i]);
                }
            }
            else
            {
                if (level.BattleEnemy.Exists(item => item == level.AliveEnemy[i]))
                {
                    level.BattleEnemy.Remove(level.AliveEnemy[i]);
                }
            }
        }
    }
    private void CheckForEnd()
    {

    }

    private void LevelDone()
    {
        level.cameraMove.TurnAiFollow(level.LastOfMan, true);
        GameData.IncreaseNowLevel();
        isEnd = true;
        level.PrintText("Красава, всех раскидал", 1.5f);
        GameData.Save();
    }
    private void LevelFailed()
    {
        level.PrintText("Ребята не бейте, бабло под тахтой((", 1.5f);
        level.cameraMove.TurnFailedShow();
        level.PlayMenu(4);
    }


    public override void OnEnemyDie(Man man, Man Enemy, Man.HitType type)
    {
        if (level.AliveEnemy.Exists(item => item == man))
        {
            level.LastOfMan = man;
            level.AliveEnemy.Remove(man);
            level.DefeatedEnemy.Add(man);
        }
    }
    public override void OnPlayerDie(Man man, Man Enemy, Man.HitType type)
    {
        LevelFailed();
    }
}
