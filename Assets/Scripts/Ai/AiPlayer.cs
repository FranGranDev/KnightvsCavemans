using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPlayer : Man
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
        if(!Static)
        {
            Control.SetStatic(on);
        }
        Static = on;
        TurnSkeleton(!on);
    }

    public override void SetParams()
    {
        base.SetParams();

        SkinColor = GameData.active.GetRandColorHVS();
        if (Type != ManType.Player && Type != ManType.KnightEnemy)
        {
            for (int i = 0; i < Sprites.Length; i++)
            {
                Sprites[i].material.SetColor("_Color", SkinColor);
            }
            for (int i = 0; i < FakeSprites.Length; i++)
            {
                FakeSprites[i].material.SetColor("_Color", SkinColor);
            }
        }
    }

    public override void Movement(Vector2 Dir)
    {
        Vector2 Velocity = new Vector2(Speed * Dir.x, Rig.velocity.y) * GroundSpeed();
        if (Landing)
        {
            Rig.velocity = Vector2.Lerp(Rig.velocity, Physics2D.gravity, 0.07f * Hard() * GroundAcceleration());
        }
        else if (Mathf.Abs(Velocity.x) > 0.1f)
        {
            Rig.velocity = Vector2.Lerp(Rig.velocity, Velocity, 0.04f * Hard() * (OnGround ? 1 : 0.1f) / Mathf.Sqrt(Size) * GroundAcceleration());
        }
        else
        {
            Rig.velocity = Vector2.Lerp(Rig.velocity, Velocity, 0.1f * Hard() * (OnGround ? 1 : 0.1f) / Mathf.Sqrt(Size) * GroundAcceleration());
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
        Arm.transform.up = Vector2.Lerp(Arm.transform.up, PrevDir, 0.45f * Hard() / Mathf.Sqrt(Size * WeaponWeight()));
    }

    public override void Jump()
    {
        if (OnGround && !Landing)
        {
            Rig.velocity = new Vector2(Rig.velocity.x, 0);
            Rig.velocity += Vector2.up * JumpForce;
            PlaySound("Jump");
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
        PlaySound("Throw");
    }

    public override void OnAttack(Man Enemy, float Power, HitType Type)
    {
        if (Enemy != null && Enemy.GetComponent<Player>() != null && Power >= 0.5f)
        {
            CameraMove.active.PunchShow(this, Power, Type);
        }
        DelayAttack(0.5f / Size);
    }

    public override void Punch(Man Enemy)
    {
        if (Rig.velocity.magnitude * Size > Enemy.Rig.velocity.magnitude * Enemy.Size * Random.Range(0.9f, 1.1f) && DistY(Enemy) < 0.25f && !Punched && !NoPunch && !Enemy.NoPunch)
        {
            Vector2 Dir = (Enemy.transform.position - transform.position).normalized;
            if (Mathf.Abs(Dir.y) > Mathf.Abs(Dir.x))
            {
                Enemy.GetHit(Mathf.FloorToInt(Size * 3), this, HitType.Punch, EffectType.Null);
                Enemy.GetImpulse(new Vector2(Mathf.CeilToInt(Dir.x), Dir.y * 0.25f).normalized * Hard() * Rig.velocity.magnitude);
                Enemy.GetHit(Mathf.RoundToInt(Velocity), Enemy, HitType.Fall, EffectType.Null);
                Enemy.GetPunched(this, Velocity * Size > 0.25f);
                Rig.velocity *= 1f;
                OnAttack(Enemy, Velocity, HitType.Fall);
            }
            else
            {
                Enemy.GetHit(Mathf.FloorToInt(Velocity * 2), this, HitType.Punch, EffectType.Null);
                Vector2 Up = Velocity > 0.25f ? Vector2.up : Vector2.zero;
                Enemy.GetImpulse((Rig.velocity.normalized + Up).normalized * Rig.velocity.magnitude * Hard() * Size * 2.25f);
                Enemy.GetPunched(this, Velocity * Size > 0.2f);
                Rig.velocity *= 0.5f * Size;
                OnAttack(Enemy, Velocity, HitType.Punch);
            }
            PunchDelay();
            PlaySound("Punch");
        }
    }
    public override void Punch(SceneObject Obj)
    {
        if (VelocityNorm.x * Size < 0.25f)
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
        yield return new WaitForSeconds(2f);
        NoPunch = false;
        yield break;
    }

    public override void OnLandOof(Man Enemy, float velocity)
    {
        GetHit(Mathf.RoundToInt(Mathf.Sqrt(Velocity) * 3), Enemy, HitType.Fall, EffectType.Null);
        if (Punched && velocity > 1)
        {
            ThrowOutWeapon();
        }
    }
    public override void GetHit(int Damage, Man Enemy, HitType type, EffectType effect)
    {
        if (Dead || Damage == 0)
            return;
        Control.GetEnemy(Enemy);

        Hp -= Damage;
        anim.SetTrigger("Hit");
        if (Hp <= 0)
        {
            Die(Enemy, type);
        }
        StartCoroutine(TakeDelay());
        DelayAttack(1 / Size);
        GetEffect(Enemy, effect);

        PlaySound("Hit");
    }

    public override void GetEnemy(Man Enemy)
    {
        Control.GetEnemy(Enemy);
    }
    public override void TryGetEnemy(Man Enemy)
    {
        Control.TryGetEnemy(Enemy);
    }

    public override void Die(Man Enemy, HitType type)
    {
        anim.SetBool("Dead", true);
        Dead = true;
        ThrowOutWeapon();
        gameObject.layer = 7;
        if (Type != ManType.Player)
        {
            Level.active.OnEnemyDie(this, Enemy, type);
        }
        else
        {
            Level.active.OnFriendDie(this, Enemy, type);
        }

        PlaySound("Die");
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

}
