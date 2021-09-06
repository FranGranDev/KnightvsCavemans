using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int Damage;
    public Man.EffectType Effect;
    public Transform FireEffect;
    public GameObject Trail;
    private bool Fly;
    private HitInfo hitInfo;
    [Header("Components")]
    public Rigidbody2D Rig;

    public void Fire(HitInfo info)
    {
        Trail.SetActive(true);
        Rig.velocity = info.Velocity;
        hitInfo = info;
        Fly = true;
        transform.parent = null;
        Rig.bodyType = RigidbodyType2D.Dynamic;
        Rig.velocity = info.Velocity;
    }
    public void SetOnPlace()
    {
        Rig.bodyType = RigidbodyType2D.Kinematic;
        Rig.velocity = Vector2.zero;
        Rig.angularVelocity = 0;
    }

    private void OnManEnter(Man man)
    {
        if (man != hitInfo.Owner && SideOwn.isEnemy(man, hitInfo.Owner))
        {
            if(Vector2.Dot(man.BodyDirection(hitInfo.Owner), man.WeaponDir()) > 0.9f)
            {
                CreateSparks(man);

                man.OnBlock(hitInfo.Owner, 1);
                man.PlaySound("Block");
            }
            else
            {
                if (FireEffect != null)
                    FireEffect.gameObject.SetActive(false);
                man.GetHit(Damage, hitInfo.Owner, Man.HitType.Bullet, Effect);
                man.GetImpulse((Rig.velocity.normalized + Vector2.up).normalized * Mathf.Sqrt(Damage) * 2);
                man.GetPunched(hitInfo.Owner, true);

                Fly = false;
                Rig.bodyType = RigidbodyType2D.Kinematic;
                Rig.velocity = Vector2.zero;
                Rig.angularVelocity = 0;
                transform.parent = man.transform;
                Trail.SetActive(false);
            }

        }
    }
    private void OnGroundEnter()
    {
        Fly = false;
        Rig.bodyType = RigidbodyType2D.Kinematic;
        Rig.velocity = Vector2.zero;
        Rig.angularVelocity = 0;
        if (FireEffect != null)
            FireEffect.gameObject.SetActive(false);
        Trail.SetActive(false);
    }

    protected void CreateSparks(Man man)
    {
        if (man.weapon == null)
            return;
        GameObject Sparks = Instantiate(GameData.active.GetEffect("Sparks"), transform.position, transform.rotation, null);
        Sparks.transform.localScale = transform.localScale;
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Obstacle" && Fly)
        {
            OnGroundEnter();
        }
        if (collision.tag == "Player" && Fly)
        {
            OnManEnter(collision.GetComponent<Man>());
        }
    }

    private void FixedUpdate()
    {
        if(Fly)
        {
            transform.up = Rig.velocity.normalized;
        }
    }
    private void Start()
    {
        Trail.SetActive(false);
    }
}
public struct HitInfo
{
    public Man Owner;
    public int Damage;
    public Vector2 Velocity;

    public HitInfo(Man owner, int damage, Vector2 velocity)
    {
        Owner = owner;
        Damage = damage;
        Velocity = velocity;
    }
}