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
        level.PrintText("Босс: Уровень  " + GameData.NowLevel, 3f);
    }

    public override void Run()
    {
        SetBattleMan();
        CheckForEnd();
    }

    private void SetBattleMan()
    {
        level.BossEnemy.GetEnemy(level.MainPlayer);
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
        level.BossEnemy = level.CreateManBossEnemy(level.sceneMaker.GetEnemyPos(), Random.Range(0.9f, 1.1f));

        CameraMove.active.TurnBossFollow(level.BossEnemy, true);
    }

    private void LevelDone()
    {
        if (level.MainPlayer.Dead)
            return;
        GameData.active.DecreaseAttempt();
        GameData.BossCompleated++;
        BossDefeated = true;
        level.cameraMove.TurnAiFollow(level.LastOfMan, true);
        level.OnLevelDone(Level.LevelTypes.Boss);
        GameData.NowLevel++;
        isEnd = true;
        level.PrintText("Уложил жирного", 1.5f);
        GameData.Save();
    }

    private void LevelFailed()
    {
        GameData.active.IncreaseAttempt();
        level.PrintText("Толстый меня побил((", 1.5f);
        level.cameraMove.TurnFailedShow();
        level.PlayMenu(4);
    }

    public override void OnEnemyDie(Man man, Man Enemy, Man.HitType type)
    {
        if (man == level.BossEnemy)
        {
            BossDefeated = true;
            level.LastOfMan = level.BossEnemy;
        }
        
    }
    public override void OnPlayerDie(Man man, Man Enemy, Man.HitType type)
    {
        LevelFailed();
    }
}