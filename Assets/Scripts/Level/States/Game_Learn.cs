using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Learn", menuName = "GameType/Learn")]
public class Game_Learn : GameState
{
    public enum LearnStateTypes {Text, Go, Jump, ArmMove, Throw, Tacke, TakeWeapon, Kill, End}
    public LearnState[] learnStates;
    private LearnState nowState;
    private int StateNum;

    public override void OnStart()
    {
        level.Learning(true);
        level.SetGame(true);
        level.ClearGame();
        level.cameraMove.TurnPlayerFollow();
        level.sceneMaker.MakeEmptyScene(300);

        level.cameraMove.UpToDown();
        level.DelayGame(1f);

        StartState();
        level.OnLevelStart();
        level.SetPlayerLearn();
    }

    public override void Run()
    {
        if(GameData.LearningEnded)
        {
            isEnd = true;
        }
    }

    private void StartState()
    {
        nowState = learnStates[StateNum];
        level.PrintText(Language.Lang.learningText[StateNum].StartText);
        switch(nowState.Type)
        {
            case LearnStateTypes.Text:
                level.NextLearnState(nowState.Delay);
                break;
            case LearnStateTypes.End:
                GameData.LearningEnded = true;
                GameData.Save();
                break;
            case LearnStateTypes.Kill:
                Vector2 EnemyPos1 = level.MainPlayer.transform.position + new Vector3(10, 5, 0);
                Man enemy1 = level.CreateLevelEnemy(EnemyPos1, 0.5f);
                if(enemy1.weapon != null)
                {
                    Destroy(enemy1.weapon.gameObject);
                    enemy1.weapon = null;
                }
                enemy1.Static = true;

                Vector2 EnemyPos2 = level.MainPlayer.transform.position + new Vector3(-10, 5, 0);
                Man enemy2 = level.CreateLevelEnemy(EnemyPos2, 0.5f);
                if (enemy2.weapon != null)
                {
                    Destroy(enemy2.weapon.gameObject);
                    enemy2.weapon = null;
                }
                enemy2.Static = true;
                break;
            case LearnStateTypes.TakeWeapon:
                Vector2 Pos1 = level.MainPlayer.transform.position + new Vector3(7, 5, 0);
                Weapon weapon1 = Instantiate(GameData.active.GetRandomWeapon(), Pos1, Quaternion.identity, null);
                weapon1.FreeFall();

                Vector2 Pos2 = level.MainPlayer.transform.position + new Vector3(-7, 5, 0);
                Weapon weapon2 = Instantiate(GameData.active.GetRandomWeapon(), Pos2, Quaternion.identity, null);
                weapon2.FreeFall();
                break;
        }
        if(nowState.AnimPlay != "")
        {
            level.LearningStartShow(nowState.AnimPlay);
        }
    }
    private void StopAnimation()
    {
        level.LearningStopShow();
    }
    public override void Action()
    {
        StateNum++;
        StartState();
    }

    public override void OnPlayerMove(Vector2 Dir)
    {
        if(nowState.Type == LearnStateTypes.Go)
        {
            if (Mathf.Abs(Dir.x) < 0.75f)
                return;
            StopAnimation();
            level.PrintText(nowState.DoneText);
            level.NextLearnState(nowState.Delay);
        }
    }
    public override void OnPlayerJump()
    {
        if (nowState.Type == LearnStateTypes.Jump)
        {
            StopAnimation();
            level.PrintText(nowState.DoneText);
            level.NextLearnState(nowState.Delay);
        }
    }
    public override void OnPlayerThrow()
    {
        if (nowState.Type == LearnStateTypes.Throw)
        {
            StopAnimation();
            level.PrintText(nowState.DoneText);
            level.NextLearnState(nowState.Delay);
        }
    }
    public override void OnPlayerTackle()
    {
        if (nowState.Type == LearnStateTypes.Tacke)
        {
            StopAnimation();
            level.PrintText(nowState.DoneText);
            level.NextLearnState(nowState.Delay);
        }
    }
    public override void OnPlayerTakeWeapon(Weapon weapon)
    {
        if (nowState.Type == LearnStateTypes.TakeWeapon)
        {
            level.PrintText(Language.Lang.learningText[StateNum].DoneText);
            level.NextLearnState(nowState.Delay);
        }
    }

    public override void OnEnemyDie(Man man, Man Enemy, Man.HitType type)
    {
        if (nowState.Type == LearnStateTypes.Kill)
        {
            level.PrintText(Language.Lang.learningText[StateNum].StartText);
            level.NextLearnState(nowState.Delay);
        }
    }
    public override void OnPlayerDie(Man man, Man Enemy, Man.HitType type)
    {
        level.Restart();
    }
}
[System.Serializable]
public struct LearnState
{
    public string Name;
    public Game_Learn.LearnStateTypes Type;
    public string AnimPlay;
    public float Delay;
    public string StartText;
    public string DoneText;
}