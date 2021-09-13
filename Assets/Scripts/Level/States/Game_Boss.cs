using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boss", menuName = "GameType/Boss")]
public class Game_Boss : GameState
{
    public bool BossDefeated;


    public override void OnStart()
    {
        level.Learning(false);
        level.SetGame(true);
        level.ClearGame();
        level.SetPlayerGame();
        level.SetPlayerWeapon();
        level.cameraMove.TurnPlayerFollow();
        level.sceneMaker.MakeRandomScene(75);
        level.BoxSpawn();
        level.BuffSpawn();
        CreateBoss();

        level.cameraMove.UpToDown();
        level.DelayGame(1f);
        level.OnLevelStart();
    }

    public override void Run()
    {
        SetBattleMan();
        CheckForEnd();
    }

    private void SetBattleMan()
    {
        level.BossEnemy.GetEnemy(level.MainPlayer);

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
            if (i < 3)
            {
                if (!level.BattleEnemy.Exists(item => item == level.AliveEnemy[i]))
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
            if (i < 3)
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

        if (level.LastOfMan != null)
        {
            level.LastOfMan.SetStatic(false);
        }
    }
    private void CheckForEnd()
    {
        if(BossDefeated && !isEnd)
        {

            LevelDone();
        }
    }
    private void CreateBoss()
    {
        level.AllEnemy = new List<Man>();
        level.AliveEnemy = new List<Man>();
        level.DefeatedEnemy = new List<Man>();
        level.BattleEnemy = new List<Man>();
        level.BossEnemy = level.CreateManBossEnemy(level.sceneMaker.GetEnemyPos(), Random.Range(0.9f, 1.25f));

        CameraMove.active.TurnBossFollow(level.BossEnemy, true);
    }

    private void LevelDone()
    {
        if (level.MainPlayer.Dead)
            return;
        level.OnLevelDone(Level.LevelTypes.Boss);
        BossDefeated = true;
        isEnd = true;
    }

    private void LevelFailed()
    {
        level.OnLevelFailed(Level.LevelTypes.Boss);

        if(!level.BossEnemy.Dead)
        {
            level.BossEnemy.MakeFun();
        }
    }

    public override void OnEnemyDie(Man man, Man Enemy, Man.HitType type)
    {
        if (man == level.BossEnemy)
        {
            BossDefeated = true;
            level.LastOfMan = level.BossEnemy;
        }
        if (level.AliveEnemy.Exists(item => item == man))
        {
            level.AliveEnemy.Remove(man);
            level.DefeatedEnemy.Add(man);
        }
    }
    public override void OnPlayerDie(Man man, Man Enemy, Man.HitType type)
    {
        LevelFailed();
    }
}