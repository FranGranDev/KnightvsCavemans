using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneObject : MonoBehaviour
{
    [Header("Params")]
    public int Hp;
    public bool Destroyed;
    [Range(1, 3)]
    public float Weight;
    private Man PrevMan;
    public bool Fly;

    [Header("Componenst")]
    public Rigidbody2D Rig;
    public Collider2D Col;
    public GameObject Effect;

    private void Awake()
    {
        Rig = GetComponent<Rigidbody2D>();
    }

    public abstract void GetHit(int Hp, Man man);
    public virtual void GetPunch(Vector2 Impulse, Man man)
    {
        if (Impulse.magnitude > Man.MaxImpulse)
        {
            Rig.velocity = Impulse.normalized * Man.MaxImpulse;
        }
        else
        {
            Rig.velocity = Impulse;
        }
        PrevMan = man;
        Fly = true;
    }
    public virtual void Land()
    {
        GetHit(Mathf.RoundToInt(Rig.velocity.magnitude / 5), PrevMan);
        Fly = false;
    }
    private void Punch(Man man)
    {
        if (man == PrevMan || !Fly)
            return;
        man.GetPunched(PrevMan, true);
        man.GetImpulse(Rig.velocity.normalized * Mathf.Sqrt(Weight * Rig.velocity.magnitude) * 2);
        man.GetHit(3, man, Man.HitType.Object);
        if(Random.Range(0, 2) == 0)
        {
            man.ThrowOutWeapon();
        }
        GetHit(3, man);
        Rig.velocity *= 0.5f;
        PrevMan = null;
        Fly = false;
    }

    protected void Hit()
    {
        GameObject ThisEffect = Instantiate(Effect, transform.position, transform.rotation, null);
        ThisEffect.transform.localScale = Vector3.one * 0.75f;
    }

    protected void DestroyObj()
    {
        StartCoroutine(DestroyCour());
    }
    private IEnumerator DestroyCour()
    {
        Destroyed = true;
        Col.enabled = false;
        Instantiate(Effect, transform.position, transform.rotation, null);
        Level.active.OnObjectDestroyed(this);
        while (transform.localScale.magnitude > 0.1f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.075f);
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
        yield break;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && Fly)
        {
            Punch(collision.GetComponent<Man>());
        }
        if(collision.tag == "Obstacle")
        {
            Land();
        }
    }
}
