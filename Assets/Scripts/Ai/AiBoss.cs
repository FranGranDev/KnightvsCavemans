using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiBoss : Man
{
    private float Hard()
    {
        return 0.5f + GameData.active.GameHard * GameData.GameGlobalHard * 0.5f;
    }
    private AiController Control;

    public override void SetController(AiController controller)
    {
        Control = controller;
    }
    public override void SetStatic(bool on)
    {
        if (!Static)
        {
            Control.SetStatic(on);
        }
        Static = on;
        TurnSkeleton(!on);
    }

    public override void SetParams()
    {
        Dead = false;
        anim.SetBool("Dead", false);
        MaxHp = Mathf.RoundToInt(MaxHp * Hard());
        Hp = MaxHp;
        transform.localScale = Vector3.one * Size;
        Rig.mass *= Size * Size;
        BuffCoroutine = new Dictionary<Buff.Type, Coroutine>();
        PassedMan = new List<Collider2D>();

        StartJump = JumpForce;
        StartPower = Power;
        StartSize = Size;
        StartSpeed = Speed;

        SkinColor = GameData.active.GetRandColorHVS();
        for (int i = 0; i < Sprites.Length; i++)
        {
            Sprites[i].material.SetColor("_Color", SkinColor);
        }
    }

    public override void Movement(Vector2 Dir)
    {
        Vector2 Velocity = new Vector2(Speed * Dir.x, Rig.velocity.y) * GroundSpeed();
        if (Mathf.Abs(Velocity.x) > 0.1f)
        {
            Rig.velocity = Vector2.Lerp(Rig.velocity, Velocity, 0.04f * Hard() * (OnGround ? 1 : 0.1f) * GroundAcceleration());
        }
        else
        {
            Rig.velocity = Vector2.Lerp(Rig.velocity, Velocity, 0.03f * Hard() * (OnGround ? 1 : 0.1f) * GroundAcceleration());
        }
        if (!OnGround && !Landing && !Punched)
        {
            transform.up = Vector2.Lerp(transform.up, PrevDir, 0.025f);
        }
        BodyY = Dir.y;
    }

    public override void MoveArm(Vector2 Dir)
    {
        if (Dir != Vector2.zero)
        {
            PrevDir = Dir;
        }
        Arm.transform.up = Vector2.Lerp(Arm.transform.up, PrevDir, 0.25f * Hard() / Mathf.Sqrt(Size * WeaponWeight()));
    }

    public override void Jump()
    {
        if (OnGround && !Landing)
        {
            Rig.velocity = new Vector2(Rig.velocity.x, 0);
            Rig.velocity += Vector2.up * JumpForce;
        }
    }

    public override void Tackle()
    {

    }

    public override void Throw()
    {
        if (weapon == null)
            return;
        weapon.Throw(PrevDir);
        weapon = null;
    }

    public override void OnAttack(Man Enemy, float Power, HitType Type)
    {
        if (Enemy.GetComponent<Player>() != null && Power >= 0.5f)
        {
            CameraMove.active.PunchShow(this, Power, Type);
        }
        DelayAttack(0.25f / Size);
    }

    public override void Punch(Man Enemy)
    {
        if (Rig.velocity.magnitude * Size > Enemy.Rig.velocity.magnitude * Enemy.Size * Random.Range(0.9f, 1.1f) && DistY(Enemy) < 0.25f && !Punched && !NoPunch && !Enemy.NoPunch)
        {
            Vector2 Dir = (Enemy.transform.position - transform.position).normalized;
            if (Mathf.Abs(Dir.y) > Mathf.Abs(Dir.x))
            {
                Enemy.GetImpulse(new Vector2(Mathf.CeilToInt(Dir.x), Dir.y * 0.25f).normalized * Rig.velocity.magnitude);
                Enemy.GetHit(Mathf.RoundToInt(Velocity), Enemy, HitType.Fall, EffectType.Null);
                Enemy.GetPunched(this, Velocity * Size > 0.25f);
                Rig.velocity *= 1f;
                OnAttack(Enemy, Velocity, HitType.Fall);
            }
            else
            {
                Vector2 Up = Velocity > 0.25f ? Vector2.up : Vector2.zero;
                Enemy.GetImpulse((Rig.velocity.normalized + Up).normalized * Rig.velocity.magnitude * Size * 2f);
                Enemy.GetPunched(this, Velocity * Size > 0.25f);
                Rig.velocity *= 0.5f * Size;
                OnAttack(Enemy, Velocity, HitType.Punch);
            }
            PunchDelay();
        }
    }
    public override void Punch(SceneObject Obj)
    {
        if (VelocityNorm.x * Size < 0.1f)
            return;
        if (OnTackle)
        {
            Vector2 Dir = (Rig.velocity.normalized * 0.75f + Vector2.up).normalized;
            Obj.GetPunch(Dir * Mathf.Sqrt(Rig.velocity.magnitude) * Size * 3f, this);
            Rig.velocity *= 0.5f / Obj.Weight;
        }
        else
        {
            Vector2 Dir = (Rig.velocity.normalized + Vector2.up * 0.25f).normalized;
            Obj.GetPunch(Dir * Mathf.Sqrt(Rig.velocity.magnitude) * Size * 7f, this);
            Rig.velocity *= 0.5f / Obj.Weight;
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
        GetHit(Mathf.RoundToInt(Mathf.Sqrt(Velocity) * 5), Enemy, HitType.Punch, EffectType.Null);
        if (Punched && Random.Range(0, 5) == 0)
        {
            ThrowOutWeapon();
        }
    }
    public override void GetHit(int Damage, Man Enemy, HitType type, EffectType effect)
    {
        if (Dead)
            return;
        Hp -= Damage;
        anim.SetTrigger("Hit");
        if (Hp <= 0)
        {
            Die(Enemy, type);
        }
        StartCoroutine(TakeDelay());
        if (type != HitType.Throw)
            Control.GetEnemy(Enemy);
    }

    public override void GetEnemy(Man Enemy)
    {
        Control.GetEnemy(Enemy);
    }

    public override void Die(Man Enemy, HitType type)
    {
        anim.SetBool("Dead", true);
        Dead = true;
        ThrowOutWeapon();

        Level.active.OnEnemyDie(this, Enemy, type);
    }

    private IEnumerator HitEffect()
    {
        yield break;
    }

    public override void TakeWeapon(Weapon weapon)
    {
        if (!weapon.InHand && !weapon.Fly && !weapon.InObject && !Dead)
        {
            this.weapon = weapon;
            weapon.Take(this, Arm);
            StartCoroutine(TakeDelay());
        }

    }
    protected override IEnumerator TakeDelay()
    {
        NoTake = true;
        yield return new WaitForSeconds(0.5f);
        NoTake = false;
        yield break;
    }
}
