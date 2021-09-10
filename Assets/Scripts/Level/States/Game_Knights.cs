using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Knights", menuName = "GameType/Knights")]
public class Game_Knights : GameState
{
    public int MaxBattleEnemy;
    private bool Started;

    public override void OnStart()
    {
        level.Learning(false);
        level.ClearGame();
        level.SetPlayerWeapon();
        level.cameraMove.TurnPlayerFollow();
        level.sceneMaker.MakeScene(100, SceneMaker.SceneType.Arena);
        level.SetPlayerKnightBattle();
        GameData.active.SetRandomEnemyArmor();

        level.cameraMove.UpToDown();
        Started = false;
        CreateStartTeam();
    }

    public override void Run()
    {
        SetEnemy();
    }

    private void CreateEnemys()
    {
        int FriendCount = 4;
        int EnemyCount = Mathf.RoundToInt(Mathf.Pow(GameData.NowStage + 1, 0.5f) * 2f) + Random.Range(0, 3);

        level.AllFriends = level.CreateEnemy(Level.EnemyCreateType.SimilarFriend, FriendCount, level.sceneMaker.GetEnemyPos(), 50);
        level.AliveFriends = level.AllFriends;
        level.DefeatedFriends = new List<Man>();
        level.BattleFriends = new List<Man>();

        level.AllEnemy = level.CreateEnemy(Level.EnemyCreateType.KnightEnemy, EnemyCount, level.sceneMaker.GetEnemyPos(), 50);
        level.AliveEnemy = level.AllEnemy;
        level.DefeatedEnemy = new List<Man>();
        level.BattleEnemy = new List<Man>();
    }

    private void SetEnemy()
    {
        if (!Started)
            return;
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

        if (level.LastOfMan != null)
        {
            level.LastOfMan.SetStatic(false);
        }
    }

    public override void Action()
    {
        if(Started)
        {
            OnWaveStart();
        }
        else
        {
            StartGame();
        }
        
    }

    private void StartGame()
    {
        level.OnLevelStart();
        Started = true;
        level.BoxSpawn();
        level.BuffSpawn();
        
        for (int i = 0; i < level.AliveFriends.Count; i++)
        {
            level.AliveFriends[i].GetEnemy(level.AliveEnemy[Random.Range(0, level.AliveEnemy.Count)]);
        }
        for (int i = 0; i < level.AliveEnemy.Count; i++)
        {
            level.AliveEnemy[i].GetEnemy(level.MainPlayer);
        }

        level.PrintText("Рыцарская арена славы: " + (GameData.NowStage + 1).ToString() + " | " + GameData.active.GetEnemyArmor().Name, 3f);
    }
    private void CreateStartTeam()
    {
        level.ClearEnemy();
        level.ClearWeapon();
        level.SetPlayerLikeNew();

        CreateEnemys();
        level.DelayGame(1f);
    }

    private void OnWaveStart()
    {
        level.cameraMove.transform.position = Vector3.back * 10;
        level.PlayFastFade();
        level.SetPlayerKnightBattle();
        level.ClearEnemy();
        if (GameData.NowStage % 2 == 0)
        {
            level.ClearWeapon();
        }
        level.SetPlayerLikeNew();
        GameData.active.SetRandomEnemyArmor();
        CreateEnemys();
        for (int i = 0; i < level.AliveFriends.Count; i++)
        {
            level.AliveFriends[i].GetEnemy(level.AliveEnemy[Random.Range(0, level.AliveEnemy.Count)]);
        }
        for (int i = 0; i < level.AliveEnemy.Count; i++)
        {
            level.AliveEnemy[i].GetEnemy(level.MainPlayer);
        }
        level.DelayGame(1f);

        level.PrintText("Волна " + (GameData.NowStage + 1).ToString() + " | " + GameData.active.GetEnemyArmor().Name, 3f);
    }
    private void OnWaveDone()
    {
        if (level.MainPlayer.Dead)
            return;
        GameData.active.DecreaseAttempt();
        GameData.NowStage++;
        GameData.IncreaseMaxStage();

        GameData.Save();
        level.OnKnightsWavesDone(3f);
        for (int i = 0; i < level.AliveFriends.Count; i++)
        {
            level.AliveFriends[i].MakeFun();
        }

        level.PrintText("Волна " + (GameData.NowStage + 1).ToString() + " Пройдена", 2f);
    }

    private void LevelFailed()
    {
        level.OnLevelFailed(Level.LevelTypes.KnightBattle);

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