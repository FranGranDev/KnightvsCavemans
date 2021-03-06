using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColdWeapon : Weapon
{
    public override void Attack(Man man)
    {
        if (!CanAttack(man))
            return;
        if (Mathf.Abs(Rotation) >= 1f)
        {
            if (Vector2.Dot(man.BodyDirection(Owner), man.WeaponDir()) > 0.75f && Random.Range(0, 5) != 0)
            {
                //Block
                Owner.OnBlock(man, 1);
                Vector2 Dir = RightDir();
                man.GetImpulse(Owner.Rig.velocity.magnitude * Mathf.Abs(SqrtRotation()) * Owner.Size * Dir);
                Owner.GetImpulse(-Owner.Rig.velocity.magnitude * Impusle / Owner.Size * Dir * (Owner.OnGround ? 1 : 0.5f));
                man.GetPunched(Owner, true);
                CreateSparks(man);
                DelayAttack(man, 0.5f);

                Owner.PlaySound("Block");
            }
            else
            {
                //Attack
                Owner.OnAttack(man, Mathf.Abs(SqrtRotation()), Man.HitType.Hit);
                man.GetHit(Mathf.CeilToInt(Damage), Owner, Man.HitType.Hit, NowEffect);
                Vector2 Dir = ((Vector2)transform.up * 0.5f + Vector2.up * SqrtRotation() * (Owner.Velocity + 0.5f) * Right).normalized;
                man.GetImpulse(Dir * Mathf.Sqrt(Lenght() * Mathf.Sqrt(Impusle) *
                (man.OnTackle ? 0.25f : 1) * Weight) * Owner.Size * (Owner.OnGround ? 1 : 0.5f) * Owner.Power * 5);
                man.GetPunched(Owner, Mathf.Abs(Rotation) * Owner.Power > 1f);
                CreateHitEffect(man);
                DelayAttack(man, 0.5f);

                OnAttack();
            }

        }
    }
    public override void PiercingAttack(Man man)
    {
        if (!CanAttack(man))
            return;
        if (Speed * Owner.Size > 0.5f && Mathf.Abs(Rotation) < 0.4f)
        {
            if (Vector2.Dot(man.WeaponDir(), transform.up) < -0.75f && Random.Range(0, 5) != 0)
            {
                //Block
                Owner.OnBlock(man, 1);
                Vector2 Dir = ((Vector2)transform.up + Vector2.up * 0.5f).normalized;
                man.GetImpulse(Owner.Rig.velocity.magnitude * Speed * Dir * Owner.Size * Owner.Power
                * Mathf.Abs(Vector2.Dot(transform.up, Owner.Direction(man.transform)) * 0.5f));
                Owner.GetImpulse(-Owner.Rig.velocity.magnitude * Impusle
                    / Owner.Size * Dir * (Owner.OnGround ? 1 : 0.5f));
                man.GetPunched(Owner, true);

                CreateSparks(man);
                Owner.PlaySound("Block");
            }
            else
            {
                //Attack
                Owner.OnAttack(man, Speed, Man.HitType.Hit);
                man.GetHit(Mathf.CeilToInt(Damage), Owner, Man.HitType.Hit, NowEffect);
                Vector2 Dir = ((Vector2)transform.up + Vector2.up * 0.5f).normalized;
                man.GetImpulse(Owner.Rig.velocity.magnitude * Speed * Dir * Owner.Size * Owner.Power
                * Mathf.Abs(Vector2.Dot(transform.up, Owner.Direction(man.transform))
                * Mathf.Sqrt(Impusle) * (Owner.OnGround ? 1 : 0.5f)) * 1.25f);
                man.GetPunched(Owner, true);
                Owner.Rig.velocity *= 0.5f;

                CreateHitEffect(man);
                OnAttack();
            }
            DelayAttack(man, 1f);
        }
    }
    public override void Attack(SceneObject obj)
    {
        if (obj == null || obj.Fly)
            return;
        if (Mathf.Abs(Rotation) > 0.25f)
        {
            obj.GetHit(Mathf.CeilToInt((Damage + Owner.Size) * Power), Owner);
            Vector2 Dir = ((Vector2)transform.up * 0.5f + Vector2.up * SqrtRotation() * Right).normalized;
            obj.GetPunch(Dir * Mathf.Sqrt(Lenght() * Impusle * Weight * Owner.Size) * 3, Owner);
            //DelayAttack(man, 0.5f);
        }
        else if (Speed * Owner.Size > 0.5f)
        {
            obj.GetHit(Mathf.CeilToInt((Damage + Owner.Size) * Power), Owner);
            Vector2 Dir = transform.up.normalized;
            obj.GetPunch(Dir * Mathf.Sqrt(Lenght() * Impusle * Weight * Owner.Size) * 3, Owner);
            //DelayAttack(man, 1f);
        }
    }
    public override void ThrowAttack(Man man)
    {
        if (man == null || NoAttack || Owner == null)
            return;
        if (Vector2.Dot(Rig.velocity.normalized, man.WeaponDir()) < -0.75f)
        {
            NoAttack = true;
            Vector2 ImpulseDir = (transform.position - man.Body.transform.position).normalized;
            Rig.centerOfMass = new Vector2(0, 0);
            Rig.angularVelocity = Random.Range(-720, 720) / Mathf.Sqrt(Weight);
            Rig.velocity = new Vector2(Random.Range(-1, 1), 1) * Random.Range(0.5f, 1f) * 15f / Mathf.Sqrt(Weight);

            CreateSparks(man);
            Owner.PlaySound("Block");
        }
        else
        {
            DelayAttack(man, 0.5f);
            man.GetHit(Mathf.RoundToInt(Damage * Weight + Owner.Power) + 1, Owner, Man.HitType.Throw, NowEffect);
            Vector2 Dir = (Rig.velocity.normalized + Vector2.up).normalized;
            man.GetImpulse(Dir * Rig.velocity.magnitude * Weight * Owner.Size * 0.5f);

            NoAttack = true;
            Rig.centerOfMass = new Vector2(0, 0);
            Rig.angularVelocity = Random.Range(-720, 720) / Mathf.Sqrt(Weight);
            Rig.velocity = new Vector2(Random.Range(-1, 1), -1) * Random.Range(0.5f, 1f) * 10f / Mathf.Sqrt(Weight);

            Owner.OnAttack(man, 0.76f, Man.HitType.Throw);
        }
    }
    public override void Throw(Vector2 Dir)
    {
        InHand = false;
        Fly = true;
        Rig.bodyType = RigidbodyType2D.Dynamic;
        Rig.centerOfMass = new Vector2(0, Lenght() / 2);
        transform.parent = null;
        int DirX = transform.up.x > 0 ? -1 : 1;
        switch(WeaponType)
        {
            case Type.Axe:
                Rig.angularVelocity += DirX * 1300f / Mathf.Sqrt(Weight);
                Rig.velocity += (Vector2)transform.up * 30f / Mathf.Sqrt(Weight);
                break;
            case Type.Club:
                Rig.angularVelocity += DirX * 720f / Mathf.Sqrt(Weight);
                Rig.velocity += (Vector2)transform.up * 30f / Mathf.Sqrt(Weight);
                break;
            case Type.Hammer:
                Rig.angularVelocity += DirX * 720f / Mathf.Sqrt(Weight);
                Rig.velocity += (Vector2)transform.up * 30f / Mathf.Sqrt(Weight);
                break;
            case Type.Lance:
                Rig.angularVelocity += DirX * 1f / Mathf.Sqrt(Weight);
                Rig.velocity += (Vector2)transform.up * 30f / Mathf.Sqrt(Weight);
                break;
            case Type.Sword:
                Rig.angularVelocity += DirX * 1300f / Mathf.Sqrt(Weight);
                Rig.velocity += (Vector2)transform.up * 30f / Mathf.Sqrt(Weight);
                break;
            case Type.Tool:
                Rig.angularVelocity += DirX * 960f / Mathf.Sqrt(Weight);
                Rig.velocity += (Vector2)transform.up * 35f / Mathf.Sqrt(Weight);
                break;
        }
        SizeUpWeapon(Vector3.one);
        OnAttack();
        Handle.enabled = true;
    }

    public override void GetBuff()
    {
        FireBuff();
    }
}
