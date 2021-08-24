using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bets", menuName = "GameType/Bets")]
public class Game_Bets : GameState
{
    private bool FirstDeath;

    public override void OnStart()
    {
        level.Learning(false);
        level.SetPlayerMenu();
        level.ClearGame();
        level.AllEnemy = new List<Man>();
        level.BattleEnemy = new List<Man>();
        level.DefeatedEnemy = new List<Man>();
        level.sceneMaker.MakeScene(50, SceneMaker.SceneType.Cave);

        CreateDuel();
    }

    public override void Run()
    {
        CameraMove.active.MenuFollow(level.AliveEnemy);
    }

    public override void Action()
    {
        StartDuel();
    }

    private void CreateDuel()
    {
        level.AllEnemy = level.CreateEnemy(Level.EnemyCreateType.Bets, 2, level.sceneMaker.GetEnemyPos(), 0);
        level.AllEnemy[0].GetEnemy(level.AllEnemy[1]);
        level.AllEnemy[1].GetEnemy(level.AllEnemy[0]);
        level.AliveEnemy = level.AllEnemy;
    }
    private void StartDuel()
    {
        for(int i = 0; i < level.AllEnemy.Count; i++)
        {
            level.AllEnemy[i].Type = Man.ManType.Menu;
        }
    }
    private void CheckForEnd()
    {
        if (!isEnd)
        {

        }
    }


    private void Play()
    {
        isEnd = true;
    }

    public override void OnEnemyDie(Man man, Man Enemy, Man.HitType type)
    {
        if (level.AliveEnemy.Exists(item => item == man))
        {
            level.AliveEnemy.Remove(man);
            level.DefeatedEnemy.Add(man);
            if (!FirstDeath)
            {
                level.OnBetEnd(man);
                FirstDeath = true;
            }
        }
    }
    public override void OnPlayerDie(Man man, Man Enemy, Man.HitType type)
    {

    }
}