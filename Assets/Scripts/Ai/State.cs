using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : ScriptableObject
{
    public bool isFinished { get; protected set; }
    [HideInInspector]
    public AiController aiController;

    public abstract void Run();
    public virtual void Init() {}

}