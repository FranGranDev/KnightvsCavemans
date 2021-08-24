using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private bool Fly;
    [Header("Components")]
    public Rigidbody2D Rig;

    public void Fire(HitInfo info)
    {
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

    }
    private void OnGroundEnter()
    {
        Fly = false;
        Rig.bodyType = RigidbodyType2D.Kinematic;
        Rig.velocity = Vector2.zero;
        Rig.angularVelocity = 0;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Obstacle" && Fly)
        {
            OnGroundEnter();
        }
    }

    private void FixedUpdate()
    {
        if(Fly)
        {
            transform.up = Rig.velocity.normalized;
        }
    }
}
public struct HitInfo
{
    public Man Owner;
    public int Damage;
    public float Impulse;
    public Vector2 Velocity;

    public HitInfo(Man owner, int damage, float impulse, Vector2 velocity)
    {
        Owner = owner;
        Damage = damage;
        Impulse = impulse;
        Velocity = velocity;
    }
}