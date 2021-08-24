using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState : ScriptableObject
{
    public Level.LevelTypes Type;
    public bool isEnd { get; protected set; }
    [HideInInspector]
    public Level level;

    public abstract void Run();

    public abstract void OnPlayerDie(Man man, Man Enemy, Man.HitType type);

    public abstract void OnEnemyDie(Man man, Man Enemy, Man.HitType type);

    public virtual void Action()
    {

    }

    public virtual void OnPlayerMove(Vector2 Dir)
    {

    }
    public virtual void OnPlayerJump()
    {

    }
    public virtual void OnPlayerTackle()
    {

    }
    public virtual void OnPlayerThrow()
    {

    }
    public virtual void OnPlayerTakeWeapon(Weapon weapon)
    {

    }

    public abstract void OnStart();
}
