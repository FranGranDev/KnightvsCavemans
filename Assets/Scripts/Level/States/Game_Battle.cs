using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Battle", menuName = "GameType/Battle")]
public class Game_Battle : GameState
{
    public int MaxBattleEnemy;


    public override void OnStart()
    {
        level.Learning(false);
        level.SetGame(true);
        level.ClearGame();
        level.SetPlayerWeapon();
        level.cameraMove.TurnPlayerFollow();
        level.sceneMaker.MakeRandomScene(150);
        level.SetPlayerBattle();
        level.BoxSpawn();
        level.BuffSpawn();
        level.MeteorSpawn();
        CreateBattle();

        level.cameraMove.UpToDown();
        level.DelayGame(1f);

        level.OnLevelStart();
    }

    public override void Run()
    {
        SetBattleMans();
        CheckForEnd();
    }

    private void CreateBattle()
    {
        int FriendCount = Mathf.RoundToInt(Mathf.Pow(GameData.NowLevel, 0.5f)) + Random.Range(0, 2);
        int EnemyCount = Mathf.RoundToInt(Mathf.Pow(GameData.NowLevel, 0.5f) * 2);

        level.AllFriends = level.CreateEnemy(Level.EnemyCreateType.Friend, FriendCount, level.sceneMaker.GetEnemyPos(), 50);
        level.AliveFriends = level.AllFriends;
        level.DefeatedFriends = new List<Man>();
        level.BattleFriends = new List<Man>();

        level.AllEnemy = level.CreateEnemy(Level.EnemyCreateType.BattleEnemy, EnemyCount, level.sceneMaker.GetEnemyPos(), 50);
        level.AliveEnemy = level.AllEnemy;
        level.DefeatedEnemy = new List<Man>();
        level.BattleEnemy = new List<Man>();

        for (int i = 0; i < level.AliveFriends.Count; i++)
        {
            level.AliveFriends[i].GetEnemy(level.AliveEnemy[Random.Range(0, level.AliveEnemy.Count)]);
        }
        for (int i = 0; i < level.AliveEnemy.Count; i++)
        {
            level.AliveEnemy[i].GetEnemy(level.MainPlayer);
        }
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
        for (int i = 0; i < level.DefeatedEnemy.Count; i++)
        {
            level.DefeatedEnemy[i].SetStatic(level.DefeatedEnemy[i].DistX(level.MainPlayer) > 15f);
        }
        for (int i = 0; i < level.BattleEnemy.Count; i++)
        {
            level.BattleEnemy[i].TryGetEnemy(level.MainPlayer);
        }

        for (int i = 0; i < level.AliveFriends.Count - 1; i++)
        {
            for (int a = i + 1; a < level.AliveFriends.Count; a++)
            {
                if (level.AliveFriends[a].DistX(level.MainPlayer) <
                   level.AliveFriends[i].DistX(level.MainPlayer))
                {
                    Man temp = level.AliveFriends[i];
                    level.AliveFriends[i] = level.AliveFriends[a];
                    level.AliveFriends[a] = temp;
                }
            }
        }
        for (int i = 0; i < level.AliveFriends.Count; i++)
        {
            if (i < MaxBattleEnemy * 2)
            {
                if (!level.BattleFriends.Exists(item => item == level.AliveFriends[i]))
                {
                    level.BattleFriends.Add(level.AliveFriends[i]);
                }
                level.AliveFriends[i].SetStatic(false);
            }
            else
            {
                if (level.BattleFriends.Exists(item => item == level.AliveFriends[i]))
                {
                    level.BattleFriends.Remove(level.AliveFriends[i]);
                }
                level.AliveFriends[i].SetStatic(true);
            }
        }
        for (int i = 0; i < level.DefeatedFriends.Count; i++)
        {
            level.DefeatedFriends[i].SetStatic(level.DefeatedFriends[i].DistX(level.MainPlayer) > 15f);
        }

        if (level.LastOfMan != null)
        {
            level.LastOfMan.SetStatic(false);
        }
    }
    private void CheckForEnd()
    {
        if (level.AliveEnemy.Count > 0)
        {

        }
        else if (!isEnd)
        {
            LevelDone();
        }

    }

    private void LevelDone()
    {
        if (level.MainPlayer.Dead)
            return;
        level.OnLevelDone(Level.LevelTypes.Levels);
        isEnd = true;

        for (int i = 0; i < level.AliveFriends.Count; i++)
        {
            level.AliveFriends[i].MakeFun();
        }
    }
    private void LevelFailed()
    {
        level.OnLevelFailed(Level.LevelTypes.Levels);

        for (int i = 0; i < level.AliveEnemy.Count; i++)
        {
            level.AliveEnemy[i].MakeFun();
        }
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
    public override void OnFriendDie(Man man, Man Enemy, Man.HitType type)
    {
        if (level.AliveFriends.Exists(item => item == man))
        {
            level.AliveFriends.Remove(man);
            level.DefeatedFriends.Add(man);
        }
    }
    public override void OnPlayerDie(Man man, Man Enemy, Man.HitType type)
    {
        LevelFailed();
    }
}

