using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Man
{
    public static Man active;
    private bool Jumped;
    private Coroutine TackleCoroutine;


    public override void Movement(Vector2 Dir)
    {
        if (!OnHouse)
        {
            if (Velocity > 0.75f && OnGround && Vector2.Dot(Rig.velocity.normalized, Dir) < -0.9f)
            {
                FlipVelocity();
            }
            Vector2 SetVelocity = new Vector2(Speed * Dir.x, Rig.velocity.y) * GroundSpeed();
            if (OnTackle)
            {
                Rig.velocity = Vector2.Lerp(Rig.velocity, Physics2D.gravity, 0.01f * (OnGround ? 1 : 0.2f) * GroundAcceleration());
            }
            else if (Landing)
            {
                Rig.velocity = Vector2.Lerp(Rig.velocity, Physics2D.gravity, 0.07f * GroundAcceleration());
            }
            else if (Mathf.Abs(SetVelocity.x) > 0.5f)
            {
                Rig.velocity = Vector2.Lerp(Rig.velocity, SetVelocity, 0.07f * (OnGround ? 1 : 0.2f) * GroundAcceleration());
            }
            else
            {
                Rig.velocity = Vector2.Lerp(Rig.velocity, Physics2D.gravity, 0.03f * (OnGround ? 1 : 0.2f) * GroundAcceleration());
            }
            if (!OnGround && !Landing)
            {
                transform.up = Vector2.Lerp(transform.up, PrevDir, 0.025f);
            }
            BodyY = Dir.y;
        }
        else
        {
            NowHorse.Movement(Dir);
        }
        Level.active.OnPlayerMove(Dir);
    }
    private void FlipVelocity()
    {
        Rig.velocity = new Vector2(Rig.velocity.x * -0.25f, Rig.velocity.y);
    }

    public override void MoveArm(Vector2 Dir)
    {
        if (Dir != Vector2.zero)
        {
            PrevDir = Dir;
        }
        Arm.transform.up = Vector2.Lerp(Arm.transform.up, PrevDir, 0.35f / Mathf.Sqrt(Size * WeaponWeight()));
    }

    public override void Jump()
    {
        if (!OnHouse)
        {
            if (OnGround && !Jumped && RotationSpeed < Velocity)
            {
                Rig.velocity = new Vector2(Rig.velocity.x, 0);
                Rig.velocity += Vector2.up * JumpForce;
                StartCoroutine(JumpDelay());
                Level.active.OnPlayerJump();
            }
        }
        else
        {
            NowHorse.Jump();
        }
    }
    private IEnumerator JumpDelay()
    {
        Jumped = true;
        yield return new WaitForSeconds(0.25f);
        Jumped = false;
        yield break;
    }

    public override void Tackle()
    {
        if (!OnHouse)
        {
            if (TackleCoroutine == null && OnGround && Velocity > 0.25f && RotationSpeed < 2f)
            {
                TackleCoroutine = StartCoroutine(TackleCour());
                Level.active.OnPlayerTackle();
            }
        }
        else
        {

        }
    }
    private IEnumerator TackleCour()
    {
        OnTackle = true;
        anim.Play("Tackle");
        anim.SetBool("Tackle", true);
        Rig.velocity *= 1.5f;
        while (PrevDir.y < -0.8f && Velocity > 0.3f && OnGround)
        {
            transform.up = Vector2.Lerp(transform.up, (Vector2.left * Right + Vector2.up * 0.1f).normalized, 0.1f);

            yield return new WaitForFixedUpdate();
        }
        anim.SetBool("Tackle", false);
        Rig.velocity *= 0.5f;
        TackleCoroutine = null;
        OnTackle = false;
        yield break;
    }

    public override void Throw()
    {
        if (weapon == null)
            return;
        weapon.Throw(PrevDir);
        weapon = null;
        StartCoroutine(TakeDelay());
        Level.active.OnPlayerThrow();
    }

    public override void OnAttack(Man Enemy, float Power, HitType Type)
    {
        if (Power >= 0.5f)
        {
            CameraMove.active.PunchShow(Enemy, Power, Type);
        }

    }

    public override void Punch(Man Enemy)
    {
        if (OnTackle && Vector2.Dot(Rig.velocity, Direction(Enemy.transform)) > 0.5f)
        {
            Vector2 Up = Velocity > 0.25f ? Vector2.up : Vector2.zero;
            Enemy.GetImpulse((Rig.velocity.normalized * 0.5f + Up).normalized * Rig.velocity.magnitude * Size);
            Enemy.GetPunched(this, Velocity > 0.25f);
            OnAttack(Enemy, Velocity, HitType.Tackle);
            Rig.velocity *= 1.025f;
            if (Velocity > 0.75f && Random.Range(0, 3) == 0)
            {
                Enemy.ThrowOutWeapon();
            }
        }
        else if (Rig.velocity.magnitude * Size > Enemy.Rig.velocity.magnitude * Enemy.Size * 0.75f && !NoPunch && !Punched)
        {
            Vector2 Dir = (Enemy.transform.position - transform.position).normalized;
            if (Mathf.Abs(Dir.y) > Mathf.Abs(Dir.x))
            {
                Enemy.GetHit(Mathf.RoundToInt(Velocity * 5), Enemy, HitType.Fall);
                Enemy.GetImpulse(new Vector2(Mathf.CeilToInt(Dir.x), Dir.y * 0.25f).normalized * Rig.velocity.magnitude);
                Enemy.GetPunched(this, true);
                Rig.velocity *= 1f;
                OnAttack(Enemy, Velocity, HitType.Fall);
            }
            else
            {
                Vector2 Up = Velocity > 0.5f ? Vector2.up : Vector2.zero;
                Enemy.GetImpulse((Rig.velocity.normalized + Up).normalized * Rig.velocity.magnitude * Size * 1.25f);
                Enemy.GetPunched(this, Velocity * Size > 0.25f);
                Rig.velocity *= 0.5f * Size;
                OnAttack(Enemy, Velocity, HitType.Punch);
            }

            PunchDelay();
        }

    }
    public override void Punch(SceneObject Obj)
    {
        if (OnTackle)
        {
            Vector2 Dir = (Rig.velocity.normalized * 0.75f + Vector2.up).normalized;
            Obj.GetPunch(Dir * Mathf.Sqrt(Rig.velocity.magnitude) * Size * 5f, this);
            Rig.velocity *= 0.75f / Obj.Weight;
        }
        else
        {
            Vector2 Dir = (Rig.velocity.normalized + Vector2.up * 0.25f).normalized;
            Obj.GetPunch(Dir * Mathf.Sqrt(Rig.velocity.magnitude) * Size * 7f, this);
            Rig.velocity *= 0.75f / Obj.Weight;
        }

    }
    public override void PunchDelay()
    {
        if (PunchCoroutine != null)
        {
            StopCoroutine(PunchCoroutine);
        }
        PunchCoroutine = StartCoroutine(PunchDelayCour());
    }
    public override IEnumerator PunchDelayCour()
    {
        NoPunch = true;
        yield return new WaitForSeconds(1f);
        NoPunch = false;
        yield break;
    }

    public override void OnLandOof(Man Enemy, float velocity)
    {
        if (Vector2.Dot(transform.up, Vector2.up) > 0.5f)
            return;
        GetHit(Mathf.RoundToInt(Mathf.Sqrt(Velocity)), Enemy, HitType.Fall);
        if (Punched && velocity > 1f)
        {
            ThrowOutWeapon();
        }
    }
    public override void GetHit(int Damage, Man Enemy, HitType type)
    {
        if (Dead)
            return;
        Hp -= Damage;
        anim.SetTrigger("Hit");
        if (Hp <= 0)
        {
            Die(Enemy, type);
        }
        Level.active.PrintDamageText();
    }

    public override void Die(Man Enemy, HitType type)
    {
        anim.SetBool("Dead", true);
        Dead = true;
        ThrowOutWeapon();

        Level.active.OnPlayerDie(this, Enemy, type);
    }

    private IEnumerator HitEffect()
    {
        yield break;
    }

    public override void TakeWeapon(Weapon weapon)
    {
        if (!weapon.InHand && !weapon.Fly && !weapon.InObject && !Dead)
        {
            StartCoroutine(TakeAnim(weapon));
            this.weapon = weapon;
            weapon.Take(this, Arm);
            StartCoroutine(TakeDelay());

            Level.active.OnPlayerTakeWeapon(weapon);
        }

    }
    private IEnumerator TakeAnim(Weapon weapon)
    {
        PrevDir = weapon.transform.up;
        yield break;
    }
    protected override IEnumerator TakeDelay()
    {
        NoTake = true;
        yield return new WaitForSeconds(0.25f);
        NoTake = false;
        yield break;
    }

    public override void OnHorseEnter(Horse horse)
    {
        SetOnHorse(horse);
    }
    private void SetOnHorse(Horse horse)
    {
        OnHouse = true;
        NowHorse = horse;
        OnGround = true;
        anim.SetBool("OnGround", true);
        IgnoreObject(true, horse.Col);

        transform.parent = horse.Body;
        Rig.bodyType = RigidbodyType2D.Kinematic;
        Rig.velocity = Vector2.zero;
        Rig.angularVelocity = 0;
        transform.position = horse.Body.position;
    }
    private void ExitHorse(Horse horse)
    {
        OnHouse = false;
        NowHorse = null;
        Rig.bodyType = RigidbodyType2D.Dynamic;
        transform.parent = null;
    }


    public void Awake()
    {
        active = this;
    }
}
