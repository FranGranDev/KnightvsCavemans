using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunWeapon : Weapon
{
    [Header("Params")]
    [Range(0, 1f)]
    public float Miss;
    public int MaxAmmo;
    public int NowAmmo;
    public float AttackRange;
    public float ReloadTime;
    private bool Reloading;
    private bool Reloaded;
    private bool TryingGetEnemy;
    private Man TryGetEnemy()
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, transform.up, AttackRange, 1 << 8 | 1 << 9 | 1 << 11);
        for(int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider != null && hit[i].collider.GetComponent<Man>() != null)
            {
                Man man = hit[i].collider.GetComponent<Man>();
                if (SideOwn.isEnemy(man, Owner) && man != Owner && !Level.active.NextLava(man))
                {
                    return man;
                }
            }
        }
        return null;
    }
    private Bullet NowBullet;
    [Header("Components")]
    public Animator anim;
    public Transform BulletPos;
    public Bullet bullet;
    public ParticleSystem StartFireEffect;
    private Coroutine ThrowOutCoroutine;

    public override void Attack(Man man)
    {
        if (!CanAttack(man))
            return;
        if (Mathf.Abs(Rotation) >= 1f)
        {
            if (Vector2.Dot(man.BodyDirection(Owner), man.WeaponDir()) > 0.98f && Power == 1)
            {
                //Block
                Owner.OnAttack(man, Speed, Man.HitType.Hit);
                Vector2 Dir = RightDir();
                man.GetImpulse(Owner.Rig.velocity.magnitude * Mathf.Abs(SqrtRotation()) * Owner.Size * Dir);
                Owner.GetImpulse(-Owner.Rig.velocity.magnitude * Impusle / Owner.Size * Dir * (Owner.OnGround ? 1 : 0.5f));
                man.GetPunched(Owner, true);
                CreateSparks(man);
                DelayAttack(man, 0.5f);
            }
            else
            {
                //Attack
                Owner.OnAttack(man, Mathf.Abs(SqrtRotation()), Man.HitType.Hit);
                man.GetHit(Mathf.CeilToInt(Damage * Power), Owner, Man.HitType.Hit, NowEffect);
                Vector2 Dir = ((Vector2)transform.up * 0.5f + Vector2.up * SqrtRotation() * (Owner.Velocity + 0.5f) * Right).normalized;
                man.GetImpulse(Dir * Mathf.Sqrt(Lenght() * Mathf.Sqrt(Impusle) *
                (man.OnTackle ? 0.25f : 1) * Weight) * Owner.Size * (Owner.OnGround ? 1 : 0.5f) * Owner.Power * 6);
                man.GetPunched(Owner, Mathf.Abs(Rotation) * Owner.Power > 1f);
                CreateHitEffect(man);
                DelayAttack(man, 0.5f);
            }

        }
    }
    public override void PiercingAttack(Man man)
    {
        if (!CanAttack(man))
            return;
        if (Speed * Owner.Size > 0.5f && Mathf.Abs(Rotation) < 0.4f)
        {
            if (Vector2.Dot(man.WeaponDir(), transform.up) < -0.95f && Power == 1)
            {
                //Block
                Owner.OnAttack(man, Speed, Man.HitType.Hit);
                Vector2 Dir = ((Vector2)transform.up + Vector2.up * 0.5f).normalized;
                man.GetImpulse(Owner.Rig.velocity.magnitude * Speed * Dir * Owner.Size * Owner.Power
                * Mathf.Abs(Vector2.Dot(transform.up, Owner.Direction(man.transform)) * 0.5f));
                Owner.GetImpulse(-Owner.Rig.velocity.magnitude * Impusle
                    / Owner.Size * Dir * (Owner.OnGround ? 1 : 0.5f));
                man.GetPunched(Owner, true);

                CreateSparks(man);
            }
            else
            {
                //Attack
                Owner.OnAttack(man, Speed, Man.HitType.Hit);
                man.GetHit(Mathf.CeilToInt(Damage * Power), Owner, Man.HitType.Hit, NowEffect);
                Vector2 Dir = ((Vector2)transform.up + Vector2.up * 0.5f).normalized;
                man.GetImpulse(Owner.Rig.velocity.magnitude * Speed * Dir * Owner.Size * Owner.Power
                * Mathf.Abs(Vector2.Dot(transform.up, Owner.Direction(man.transform))
                * Mathf.Sqrt(Impusle) * (Owner.OnGround ? 1 : 0.5f)) * 1.5f);
                man.GetPunched(Owner, true);
                Owner.Rig.velocity *= 0.5f;

                CreateHitEffect(man);
            }
            DelayAttack(man, 1f);
        }
    }
    public override void Attack(SceneObject obj)
    {
        if (obj == null || obj.Fly || Owner.NoAttack || Owner.Punched || NoAttack)
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
        if (man == null || NoAttack)
            return;
        //DelayAttack(0.25f);
        man.GetHit(Mathf.RoundToInt(Damage * Weight + Owner.Power) + 1, Owner, Man.HitType.Throw, NowEffect);
        Owner.OnAttack(man, 0.76f, Man.HitType.Throw);
        Vector2 Dir = (Rig.velocity.normalized + Vector2.up * 0.5f).normalized;
        man.Rig.velocity *= 0.25f;
        man.GetImpulse(Rig.velocity.magnitude * Owner.Power * Dir * Weight * Owner.Size * 0.5f);

        NoAttack = true;
        Vector2 ImpulseDir = (transform.position - man.Body.transform.position).normalized;
        Rig.centerOfMass = new Vector2(0, 0);
        Rig.angularVelocity = Random.Range(-720, 720) / Mathf.Sqrt(Weight);
        Rig.velocity = Dir * Random.Range(0.5f, 1f) * 10f / Mathf.Sqrt(Weight);

        CreateHitEffect(man);
    }
    public override void Throw(Vector2 Dir)
    {
        InHand = false;
        Fly = true;
        Rig.bodyType = RigidbodyType2D.Dynamic;
        Rig.centerOfMass = new Vector2(0, Lenght() / 2);
        transform.parent = null;
        int DirX = transform.up.x > 0 ? -1 : 1;
        Rig.angularVelocity += DirX * 720f / Mathf.Sqrt(Weight);
        Rig.velocity += (Vector2)transform.up * 15f / Mathf.Sqrt(Weight);
        SizeUpWeapon(Vector3.one);

        Handle.enabled = true;
    }


    public override void GetBuff()
    {
        
    }



    public override void OnFixedUpdate()
    {
        if (!InHand)
            return;
        if(NowAmmo == 0)
        {
            if(ThrowOutCoroutine == null)
            {
                ThrowOutCoroutine = StartCoroutine(ThrowOutCour());
            }
            return;
        }
        if(!Reloaded && !Reloading)
        {
            StartCoroutine(ReloadCour());
        }

        if(Reloaded && !TryingGetEnemy)
        {
            StartCoroutine(TryGetEnemyCour());
            Man man = TryGetEnemy();
            if(man != null)
            {
                StartFire();
            }
        }
    }
    private void StartFire()
    {
        if(Reloaded)
        {
            NowAmmo--;
            anim.SetTrigger("Fire");
        }
        else if(!Reloading)
        {
            StartCoroutine(ReloadCour());
        }
    }
    public void Fire()
    {
        if(Owner != null)
        {
            Owner.OnAttack(null, 0, Man.HitType.Bullet);
        }
        Reloaded = false;
        if (StartFireEffect != null)
        {
            StartCoroutine(StartFireEffectCour());
        }
        Vector2 Velocity = (transform.up + transform.right * Random.Range(-Miss, Miss) * 0.5f) * 30 * Impusle;
        NowBullet.Fire(new HitInfo(Owner, Damage, Velocity));
    }
    private IEnumerator StartFireEffectCour()
    {
        StartFireEffect.gameObject.SetActive(true);
        StartFireEffect.Play();
        yield return new WaitForSeconds(StartFireEffect.main.startLifetime.constant);
        StartFireEffect.gameObject.SetActive(false);
        yield break;
    }
    private IEnumerator TryGetEnemyCour()
    {
        TryingGetEnemy = true;
        yield return new WaitForSeconds(0.25f);
        TryingGetEnemy = false;
        yield break;
    }
    private IEnumerator ReloadCour()
    {
        Reloading = true;
        anim.SetTrigger("Reload");
        yield return new WaitForSeconds(ReloadTime);
        NowBullet = Instantiate(bullet, BulletPos.position, BulletPos.rotation, BulletPos);
        NowBullet.SetOnPlace();
        Reloading = false;
        Reloaded = true;
        yield break;
    }
    private IEnumerator ThrowOutCour()
    {
        yield return new WaitForSeconds(1f);
        if(Owner != null)
        {
            Owner.Throw();
        }
        ThrowOutCoroutine = null;
        yield break;
    }

    public override void OnStart()
    {
        NowAmmo = MaxAmmo;
        anim.SetFloat("ReloadSpeed", 1 / ReloadTime);
    }
}
