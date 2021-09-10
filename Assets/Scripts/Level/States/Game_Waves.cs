using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Waves", menuName = "GameType/Waves")]
public class Game_Waves : GameState
{
    public int MaxBattleEnemy;

    public override void OnStart()
    {
        level.Learning(false);
        level.SetGameWaves();
        level.ClearGame();
        level.SetPlayerGame();
        level.SetPlayerWeapon();
        level.cameraMove.TurnPlayerFollow();
        level.sceneMaker.MakeScene(100, SceneMaker.SceneType.Arena);
        level.BoxSpawn();
        level.BuffSpawn();

        level.cameraMove.UpToDown();
        OnWaveStart();

        level.OnLevelStart();
    }

    public override void Run()
    {
        SetEnemy();
    }

    private void CreateEnemys()
    {
        int count = Mathf.CeilToInt(Mathf.Pow(GameData.NowWave + 1, 0.75f) * 2);
        level.AliveEnemy = level.CreateEnemy(Level.EnemyCreateType.Waves, count, level.sceneMaker.GetLongEnemyPos(), 5);
        level.DefeatedEnemy = new List<Man>();
        level.BattleEnemy = new List<Man>();
    }

    private void SetEnemy()
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
        for (int i = 0; i < level.DefeatedEnemy.Count; i++)
        {
            level.DefeatedEnemy[i].SetStatic(level.DefeatedEnemy[i].DistX(level.MainPlayer) > 15f);
        }

        for (int i = 0; i < level.AliveEnemy.Count; i++)
        {
            if (i < MaxBattleEnemy)
            {
                if (!level.BattleEnemy.Exists(item => item == level.AliveEnemy[i]))
                {
                    level.BattleEnemy.Add(level.AliveEnemy[i]);
                }
                if (i < level.AliveEnemy.Count)
                    level.AliveEnemy[i].SetStatic(false);
            }
            else
            {
                
                if (level.BattleEnemy.Exists(item => item == level.AliveEnemy[i]))
                {
                    level.BattleEnemy.Remove(level.AliveEnemy[i]);
                }
                if (i < level.AliveEnemy.Count)
                    level.AliveEnemy[i].SetStatic(true);
            }
        }

        for (int i = 0; i < level.BattleEnemy.Count; i++)
        {
            level.BattleEnemy[i].GetEnemy(level.MainPlayer);
        }

        if(level.LastOfMan != null)
        {
            level.LastOfMan.SetStatic(false);
        }
    }

    public override void Action()
    {
        OnWaveStart();
    }
    private void OnWaveStart()
    {
        level.ClearEnemy();
        if (GameData.NowWave % 2 == 0)
        {
            level.ClearWeapon();
        }
        level.SetPlayerLikeNew();
        if (GameData.NowWave == 0)
        {
            level.PrintText("Бесконечная арена жызни: Волна " + (GameData.NowWave + 1).ToString(), 3f);
        }
        else
        {
            level.PrintText("Волна " + (GameData.NowWave + 1).ToString(), 3f);
        }
        CreateEnemys();
        level.DelayGame(1f);
    }
    private void OnWaveDone()
    {
        if (level.MainPlayer.Dead)
            return;
        GameData.active.DecreaseAttempt();
        level.PrintText("Волна " + (GameData.NowWave + 1).ToString() + " Пройдена", 2f);
        GameData.NowWave++;
        GameData.IncreaseMaxWave();

        GameData.Save();
        level.OnWavesDone(3f);
    }

    private void LevelFailed()
    {
        level.OnLevelFailed(Level.LevelTypes.Waves);

        for (int i = 0; i < level.AliveEnemy.Count; i++)
        {
            level.AliveEnemy[i].MakeFun();
        }
    }


    public override void OnEnemyDie(Man man, Man Enemy, Man.HitType type)
    {
        if (level.AliveEnemy.Exists(item => item == man))
        {
            level.AliveEnemy.Remove(man);
            level.DefeatedEnemy.Add(man);
            level.LastOfMan = man;
        }
        if (level.AliveEnemy.Count <= 0)
        {
            OnWaveDone();
        }
    }
    public override void OnPlayerDie(Man man, Man Enemy, Man.HitType type)
    {
        LevelFailed();
    }
}