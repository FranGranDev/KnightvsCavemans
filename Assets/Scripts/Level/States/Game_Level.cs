using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "GameType/Level")]
public class Game_Level : GameState
{
    public int MaxBattleEnemy;


    public override void OnStart()
    {
        level.Learning(false);
        level.SetGame(true);
        level.ClearGame();
        level.SetPlayerGame();
        level.SetPlayerWeapon();
        level.cameraMove.TurnPlayerFollow();
        level.sceneMaker.MakeRandomScene(100);
        level.BoxSpawn();
        level.BuffSpawn();
        CreateEnemys();

        level.cameraMove.UpToDown();
        level.DelayGame(1f);
        level.PrintText("Рубилово: Уровень  " + GameData.NowLevel, 3f);
    }

    public override void Run()
    {
        SetBattleMans();
        CheckForEnd();
    }

    private void CreateEnemys()
    {
        int EnemyCount = Mathf.RoundToInt(Mathf.Pow(GameData.NowLevel, 0.5f) + Random.Range(1, 3));
        level.AllEnemy = level.CreateEnemy(Level.EnemyCreateType.Similar, EnemyCount, level.sceneMaker.GetEnemyPos(), 50);
        level.AliveEnemy = level.AllEnemy;
        level.DefeatedEnemy = new List<Man>();
        level.BattleEnemy = new List<Man>();
    }

    private void SetBattleMans()
    {   
        for (int i = 0; i < level.AliveEnemy.Count - 1; i++)
        {
            for(int a = i + 1; a < level.AliveEnemy.Count; a++)
            {
                if(level.AliveEnemy[a].DistX(level.MainPlayer) <
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
            if(i < MaxBattleEnemy)
            {
                if(!level.BattleEnemy.Exists(item => item == level.AliveEnemy[i]))
                {
                    level.BattleEnemy.Add(level.AliveEnemy[i]);
                }
                level.AliveEnemy[i].SetStatic(false);
            }
            else
            {
                if (level.BattleEnemy.Exists(item => item == level.AliveEnemy[i]))
                {
                    level.BattleEnemy.Remove(level.AliveEnemy[i]);
                }
                level.AliveEnemy[i].SetStatic(true);
            }
        }
        for (int i = 0; i < level.AllEnemy.Count; i++)
        {
            if (i < MaxBattleEnemy)
            {
                level.AllEnemy[i].SetStatic(false);
            }
            else
            {
                level.AllEnemy[i].SetStatic(true);
            }
        }
        for (int i = 0; i < level.DefeatedEnemy.Count; i++)
        {
            level.DefeatedEnemy[i].SetStatic(level.DefeatedEnemy[i].DistX(level.MainPlayer) > 15f);
        }
        for (int i = 0; i < level.BattleEnemy.Count; i++)
        {
            level.BattleEnemy[i].GetEnemy(level.MainPlayer);
        }
    }
    private void CheckForEnd()
    {
        if(level.AliveEnemy.Count > 0)
        {

        }
        else if(!isEnd)
        {
            LevelDone();
        }
        
    }

    private void LevelDone()
    {
        if (level.MainPlayer.Dead)
            return;
        GameData.active.DecreaseAttempt();
        level.cameraMove.TurnAiFollow(level.LastOfMan, true);
        level.OnLevelDone(Level.LevelTypes.Levels);
        GameData.NowLevel++;
        isEnd = true;
        level.PrintText("Красава, всех раскидал", 1.5f);
        GameData.Save();
    }
    private void LevelFailed()
    {
        GameData.active.IncreaseAttempt();
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
