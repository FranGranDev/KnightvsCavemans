using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunWeapon : Weapon
{
    [Header("Params")]
    public float AttackRange;
    public float ReloadTime;
    private bool Reloading;
    private bool Reloaded;
    private bool TryingGetEnemy;
    private Man TryGetEnemy()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, AttackRange, 1 << 8 | 1 << 9 | 1 << 11);
        if (hit.collider != null && hit.collider.GetComponent<Man>() != null)
        {
            Man man = hit.collider.GetComponent<Man>();
            if (SideOwn.isEnemy(man, Owner) && man != Owner && !Level.active.NextLava(man))
            {
                return man;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
    private Bullet NowBullet;
    [Header("Components")]
    public Animator anim;
    public Transform BulletPos;
    public Bullet bullet;

    public override void Attack(Man man)
    {
        
    }
    public override void Attack(SceneObject obj)
    {
        
    }

    public override void GetBuff()
    {
        
    }

    public override void PiercingAttack(Man man)
    {
        
    }

    public override void ThrowAttack(Man man)
    {
        if (man == null || NoAttack)
            return;
        man.GetHit(Mathf.RoundToInt(Damage * Weight + Owner.Power) + 1, Owner, Man.HitType.Throw);
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
        Rig.velocity += (Vector2)transform.up * 30f / Mathf.Sqrt(Weight);
        SizeUpWeapon(Vector3.one);

        Handle.enabled = true;
    }


    public override void OnFixedUpdate()
    {
        if (Owner == null)
            return;
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
        if(Input.GetKeyDown(KeyCode.W) && Reloaded)
        {
            StartFire();
        }
    }
    private void StartFire()
    {
        if(Reloaded)
        {
            anim.SetTrigger("Fire");
        }
        else if(!Reloading)
        {
            StartCoroutine(ReloadCour());
        }
    }
    public void Fire()
    {
        Reloaded = false;

        Vector2 Velocity = transform.up * 30 * Impusle;
        NowBullet.Fire(new HitInfo(Owner, Damage, Impusle, Velocity));
    }

    private IEnumerator TryGetEnemyCour()
    {
        TryingGetEnemy = true;
        yield return new WaitForSeconds(1f);
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
}
