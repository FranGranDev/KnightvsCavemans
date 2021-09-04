using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public static Controller active;
    public Man Target;
    public VariableJoystick joystick;
    public float DeadZoneX;
    private float x;
    private float y;
    private Vector2 Dir()
    {
        return new Vector2(x, y);
    }
    private Vector2 PrevDir;
    private float RotationSpeed;

    private Coroutine ClickCoroutine;

    private void Awake()
    {
        active = this;

        SwipeManager.OnFastSwipeEvent += OnSwipe;
        SwipeManager.OnSwipeEvent += OnLongSwipe;
    }
    private void Start()
    {
        
    }


    public void OnSwipe(Vector2 SwipeDir, Vector2 Pos)
    {
        if (!Level.active.InGame())
            return;
        if (RotationSpeed > 1)
            return;
        if (Dir() != Vector2.zero)
        {
            if (SwipeDir.y > 0.25f)
            {
                JumpForce(Dir());
            }
            else if(SwipeDir.y < 0f)
            {
                TackleForce(Dir());
            }
        }
    }
    public void OnLongSwipe(Vector2 SwipeDir, Vector2 Start)
    {
        if (!Level.active.InGame())
            return;
        if (Dir() == Vector2.zero)
        {
            ThrowForce(SwipeDir);
        }
    }

    public void CheckMovement()
    {
        if (!joystick.enabled)
            return;
        x = Mathf.Abs(joystick.Direction.x) > DeadZoneX ? joystick.Direction.x : 0;
        y = joystick.Direction.y;
    }
    private void CalculateSpeed()
    {
        if(Dir() == Vector2.zero)
        {
            RotationSpeed = 0;
        }
        else
        {
            RotationSpeed = (1 - Vector2.Dot(PrevDir, Dir())) / Time.fixedDeltaTime;
            PrevDir = Dir();
        }
    }

    public void Movement()
    {
        Target.Movement(new Vector2(x, y));
    }
    public void MoveArm()
    {
        Target.MoveArm(new Vector2(x, y));
    }
    public void Throw()
    {
        Target.Throw();
    }
    public void ThrowForce(Vector2 Dir)
    {
        Target.ThrowForce(Dir);
    }
    public void Jump()
    {
        Target.Jump();
    }
    public void JumpForce(Vector2 Dir)
    {
        Target.JumpDir(Dir);
    }
    public void Tackle()
    {
        Target.Tackle();
    }
    public void TackleForce(Vector2 Dir)
    {
        Target.TackleDir(Dir);
    }

    public void Update()
    {
        CheckMovement();
    }
    public void FixedUpdate()
    {
        Movement();
        MoveArm();

        CalculateSpeed();
    }
}
