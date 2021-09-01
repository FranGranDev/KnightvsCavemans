using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Idle", menuName = "GameType/Idle")]
public class Game_Idle : GameState
{
    public override void OnStart()
    {
        level.SetPlayerMenu();
        level.ClearGame();
        level.AllEnemy = new List<Man>();
        level.sceneMaker.MakeScene(50, SceneMaker.SceneType.Arena);
        level.DelayGame(0f);

        level.OnLevelStart();
    }

    public override void Run()
    {
        int Count = level.AllEnemy.Count;
        for (int i = 0; i < 2 - Count; i++)
        {
            float PosX = i == 0 ? Random.Range(-10f, 0) : level.AllEnemy[0].transform.position.x + Random.Range(0, 10f);
            ManInfo info = new ManInfo("Man", GameData.active.Enemy[0].Enemy, Man.ManType.Menu,
            50, Random.Range(0.75f, 1.25f), 15, 10f, 0, GameData.active.GetRandomWeapon());
            Man man = level.CreateSpecialEnemy(new Vector2(PosX, -0.25f), info);
            level.AllEnemy.Add(man);
        }
        if (Count == 2)
        {
            level.AllEnemy[0].GetEnemy(level.AllEnemy[1]);
            level.AllEnemy[1].GetEnemy(level.AllEnemy[0]);
        }
        CameraMove.active.MenuFollow(level.AllEnemy);
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
        if (level.AllEnemy.Exists(item => item == man))
        {
            level.RemoveEnemy_All(man, 3f);
        }
    }
    public override void OnPlayerDie(Man man, Man Enemy, Man.HitType type)
    {

    }
}
