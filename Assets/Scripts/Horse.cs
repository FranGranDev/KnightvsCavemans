using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horse : MonoBehaviour
{
    public enum AnimalTypes {Horse}
    [Header("Params")]
    public AnimalTypes AnimalType;
    public float Size;
    public float Speed;
    public float JumpForce;
    public int MaxHp;
    public int Hp;
    public bool Dead;

    private GameObject PrevObj;
    public bool Punched { get; protected set; }
    public bool OnGround { get; protected set; }
    public bool Landing { get; protected set; }
    public int Right { get; protected set; }
    public float Velocity { get; protected set; }
    public float PrevDirX { get; protected set; }
    public Vector2 PrevDir { get; protected set; }

    [Header("Components")]
    public ParticleSystem Effect;
    public Transform Body;
    public Rigidbody2D Rig;
    public Collider2D Col;
    public Animator anim;

    public float DistX(Man Targer)
    {
        if (Targer == null)
            return float.MaxValue;
        return (Mathf.Abs(Targer.transform.position.x - transform.position.x));
    }
    public float DistX(Transform Targer)
    {
        return (Mathf.Abs(Targer.position.x - transform.position.x));
    }
    public float Dist(Transform Targer)
    {
        return (Targer.position - transform.position).magnitude;
    }
    public float DistY(Transform Targer)
    {
        return (Targer.position.y - transform.position.y);
    }
    public float DistY(Man Targer)
    {
        return (Targer.transform.position.y - transform.position.y);
    }
    public Vector2 Direction(Transform Target)
    {
        return (Target.position - transform.position).normalized;
    }
    public Vector2 BodyDirection(Man Target)
    {
        return (Target.Body.position - Body.position).normalized;
    }

    public void Movement(Vector2 Dir)
    {
        Vector2 Velocity = new Vector2(Speed * Dir.x, Rig.velocity.y);
        if (Mathf.Abs(Velocity.x) > 0.1f)
        {
            Rig.velocity = Vector2.Lerp(Rig.velocity, Velocity, 0.04f * (OnGround ? 1 : 0.1f) / Mathf.Sqrt(Size) * GameData.Speed);
        }
        else
        {
            Rig.velocity = Vector2.Lerp(Rig.velocity, Velocity, 0.03f * (OnGround ? 1 : 0.1f) / Mathf.Sqrt(Size) * GameData.Speed);
        }
    }
    public void Jump()
    {
        if (OnGround)
        {
            Rig.velocity = new Vector2(Rig.velocity.x, 0);
            Rig.velocity += Vector2.up * JumpForce;
        }
    }

    private void GetHit(int Damage, Man Enemy, Man.HitType type)
    {
        if (Dead)
            return;
        Hp -= Damage;
        anim.SetTrigger("Hit");
        if (Hp <= 0)
        {
            Die(Enemy, type);
        }
    }

    private void Die(Man Enemy, Man.HitType type)
    {
        anim.SetBool("Dead", true);
        Dead = true;
    }


    private void Land()
    {
        Landing = true;
        StartCoroutine(LandingCour());
        anim.SetBool("OnGround", true);
        if (Vector2.Dot(transform.up, Vector2.up) > 0.5f)
        {
            anim.Play("Land");
        }
        float Size = Mathf.Abs(Vector2.Dot(transform.up, Vector2.right));
        GameData.active.CreateLandEffect(Body.transform, Size);
    }
    protected IEnumerator LandingCour()
    {
        yield return new WaitForSeconds(0.1f);
        while (Vector2.Dot(Vector2.up, transform.up) < 0.75f)
        {
            yield return new WaitForFixedUpdate();
        }
        Landing = false;
        yield break;
    }
    private void SetOnGround(bool on)
    {
        bool Prev = OnGround;
        OnGround = on;
        if (on && !Prev)
        {
            Land();
        }
        if (!on && Prev)
        {
            StartCoroutine(LeaveGround());
        }

        if (on)
        {
            Rig.velocity = new Vector2(Rig.velocity.x, Rig.velocity.y * 0.75f);
        }
    }
    protected IEnumerator LeaveGround()
    {
        yield return new WaitForSeconds(0.25f);
        if (!OnGround)
            anim.SetBool("OnGround", false);
        yield break;
    }

    public void MovementStuff()
    {
        Velocity = Mathf.Abs(Rig.velocity.magnitude / Speed);
        anim.SetFloat("Speed", Velocity);
        if (Mathf.Abs(Rig.velocity.x) > 0.1f)
        {
            PrevDirX = Rig.velocity.normalized.x;
        }
    }
    private void Flip()
    {
        Right = PrevDirX > 0 ? 1 : -1;
        anim.SetBool("Right", PrevDirX > 0);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Obstacle")
        {
            SetOnGround(true);
        }
        if (collision.tag == "Death")
        {
            GetHit(1, null, Man.HitType.Lava);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Obstacle")
        {
            SetOnGround(false);
        }
        if (collision.tag == "Object")
        {
            IgnoreObject(false, collision);
        }
    }
    private void IgnoreObject(bool on, Collider2D obj)
    {
        if (on)
        {
            StartCoroutine(IgonoreObjCour(obj));
        }
        else
        {
            Physics2D.IgnoreCollision(Col, obj, false);
        }
    }
    private IEnumerator IgonoreObjCour(Collider2D obj)
    {
        yield return new WaitForSeconds(OnGround ? 0.25f : 0);
        if (obj == null)
        {
            PrevObj = null;
            yield break;
        }
        Physics2D.IgnoreCollision(Col, obj, true);
        PrevObj = obj.gameObject;
        while (DistX(obj.transform) < 2)
        {
            yield return new WaitForFixedUpdate();
            if (obj == null || obj.transform == null)
            {
                PrevObj = null;
                yield break;
            }
        }
        Physics2D.IgnoreCollision(Col, obj, false);
        PrevObj = null;
        yield break;
    }


    private void Awake()
    {

    }
    private void FixedUpdate()
    {
        MovementStuff();
        Flip();
    }
}
