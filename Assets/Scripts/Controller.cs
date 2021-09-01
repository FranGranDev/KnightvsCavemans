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
    private bool JoystickActive;
    private bool isJoystickActive()
    {
        return joystick.transform.GetChild(0).gameObject.activeSelf;
    }
    public delegate void OnJoystickActive();
    public event OnJoystickActive JoystickClick;

    private void Awake()
    {
        active = this;

        SwipeManager.OnSwipeEvent += OnSwipe;
    }
    private void Start()
    {
        
    }
    public void Subscribe(OnJoystickActive SomeVoid)
    {
        JoystickClick += SomeVoid;
    }

    public void OnSwipe(Vector2 Dir)
    {
        if (!Level.active.InGame())
            return;
        if (!isJoystickActive())
        {
            if(Mathf.Abs(Dir.y) > Mathf.Abs(Dir.x) * 0.25f)
            {
                if(Dir.y > 0)
                {
                    JumpForce(Dir);
                }
                else
                {
                    TackleForce(Dir);
                }
            }
            
        }
    }

    public void CheckMovement()
    {
        if (!joystick.enabled)
            return;
        x = Mathf.Abs(joystick.Direction.x) > DeadZoneX ? joystick.Direction.x : 0;
        y = joystick.Direction.y;
        if(joystick.transform.GetChild(0).gameObject.activeSelf && !JoystickActive)
        {
            StartCoroutine(OnJoystickClick());
            JoystickActive = true;
            JoystickClick?.Invoke();
        }
        if(!joystick.transform.GetChild(0).gameObject.activeSelf)
        {
            JoystickActive = false;
        }
        else if (y > 0.8f)
        {
            Jump();
        }
        else if(y < -0.8f)
        {
            Tackle();
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
    private IEnumerator OnJoystickClick()
    {
        float time = Time.time;
        while(joystick.transform.GetChild(0).gameObject.activeSelf)
        {
            if(joystick.Direction.magnitude > 0.5f)
            {
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
        if(Time.time - time < 0.5f)
        {
            Throw();
        }
        yield break;
    }

    public void Update()
    {
        CheckMovement();
    }
    public void FixedUpdate()
    {
        Movement();
        MoveArm();
    }
}

public interface IController
{
    void Movement();
    void Throw();
    void Jump();
}