﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Duel", menuName = "GameType/Duel")]
public class Game_Duel : GameState
{
    public int MaxBattleEnemy;
    private int Waves;
    private int NowWave;

    public override void OnStart()
    {
        level.Learning(false);
        level.SetGame(true);
        level.ClearGame();
        level.SetPlayerGame();
        level.SetPlayerWeapon();
        level.cameraMove.TurnDuelFollow();
        level.sceneMaker.MakeRandomScene(100);
        level.BoxSpawn();
        level.BuffSpawn();

        Waves = 3 + Mathf.RoundToInt(GameData.NowLevel / 5);
        level.cameraMove.UpToDown();
        level.DelayGame(1f);
        level.PrintText("Дуэльки: Уровень " + GameData.NowLevel, 3f);
    }

    public override void Run()
    {
        SetEnemy();
        CheckForEnd();
    }

    private void CreateEnemys()
    {
        level.AliveEnemy = new List<Man>();
        level.DefeatedEnemy = new List<Man>();
        level.BattleEnemy = new List<Man>();
    }

    private void SetEnemy()
    {
        if (isEnd)
            return;
        if(level.AliveEnemy.Count == 0)
        {
            Vector2 Pos = level.sceneMaker.GetLongEnemyPos()[Random.Range(0, 2)];
            float Power = Random.Range(0.75f, 1.25f);
            level.AliveEnemy.Add(level.CreateDuelEnemy(Pos, Power));
        }
        else
        {
            level.cameraMove.DuelFollow(level.AliveEnemy[0]);
        }

        for (int i = 0; i < level.DefeatedEnemy.Count; i++)
        {
            level.DefeatedEnemy[i].SetStatic(level.DefeatedEnemy[i].DistX(level.MainPlayer) > 15f);
        }
        for(int i = 0; i < level.AliveEnemy.Count; i++)
        {
            level.AliveEnemy[i].GetEnemy(level.MainPlayer);
        }
    }
    private void CheckForEnd()
    {
        if (NowWave >= Waves && !isEnd)
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
        level.OnLevelDone(Level.LevelTypes.Duel);
        GameData.NowLevel++;
        isEnd = true;
        level.PrintText("Красава, победил турнир епта", 1.5f);
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
            level.AliveEnemy.Remove(man);
            level.DefeatedEnemy.Add(man);
            level.LastOfMan = man;
            NowWave++;
            level.PrintText("Осталось " + (Waves - NowWave) + " аболтусов", 2);
        }
    }
    public override void OnPlayerDie(Man man, Man Enemy, Man.HitType type)
    {
        LevelFailed();
    }
}