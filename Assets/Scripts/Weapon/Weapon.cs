using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public enum Type { Sword, Axe, Lance, Hammer, Club, Tool, Gun };
    public Type WeaponType;
    public int Index;
    public Man.EffectType NowEffect;
    private Man.EffectType PrevEffect;
    [Range(0, 5)]
    public int Damage;
    [Range(1, 3f)]
    public float Weight;
    [Range(0.5f, 3f)]
    public float Impusle;
    public float Lenght()
    {
        return Renderer.sprite.bounds.size.y * Mathf.Abs(transform.localScale.x);
    }
    public bool CanAttack(Man man)
    {
        return man != null && Owner != null && SideOwn.isEnemy(man, Owner) && PrevAttack != null && !PrevAttack.Exists(item => item == man) && !Owner.NoAttack && !man.OnTackle;
    }
    protected List<Man> PrevAttack;

    public Vector2 RightDir()
    {
        return transform.right * Right * (Rotation > 0 ? -1 : 1);
    }
    public Vector2 EffectPos()
    {
        return transform.position + transform.up * Lenght();
    }
    public bool NoAttack { get; protected set; }
    public bool ManAttack { get; protected set; }
    public int Right { get; protected set; }
    public float Speed { get; protected set; }
    public float Rotation { get; protected set; }
    public float Power { get; protected set; }
    public float SqrtRotation()
    {
        return Rotation > 0 ? Mathf.Sqrt(Rotation) : -Mathf.Sqrt(-Rotation);
    }
    public bool SizingUp { get; protected set; }
    public bool InHand { get; protected set; }
    public bool Fly { get; protected set; }
    public bool InObject { get; protected set; }
    public Coroutine TurnColCour;
    public Coroutine SizeUpCour;
    public Coroutine FireEffectCoroutine;
    private GameObject FireEffect;
    public Man Owner;

    [Header("Components")]
    public TrailRenderer Trail;
    public Collider2D Handle;
    public Rigidbody2D Rig;
    public SpriteRenderer Renderer;

    public void Awake()
    {
        Handle = GetComponent<Collider2D>();
        Rig = GetComponent<Rigidbody2D>();
        Renderer = GetComponent<SpriteRenderer>();
        Trail = transform.GetChild(1).GetComponent<TrailRenderer>();
    }
    public void Start()
    {
        PrevAttack = new List<Man>();
        Trail.minVertexDistance = 0.01f;
        OnStart();
    }
    public virtual void OnStart()
    {

    }

    public abstract void Attack(Man man);
    public abstract void PiercingAttack(Man man);
    public abstract void Attack(SceneObject obj);
    public abstract void ThrowAttack(Man man);
    public abstract void Throw(Vector2 Dir);
    public virtual void Take(Man Owner, Transform Arm)
    {
        NoAttack = false;
        InHand = true;
        this.Owner = Owner;
        transform.parent = Arm;
        transform.position = Vector3.zero;
        transform.localPosition = Vector3.zero;
        transform.localRotation = new Quaternion(0, 0, 0, 0);
        Rig.velocity = Vector2.zero;
        Rig.angularVelocity = 0;
        Rig.bodyType = RigidbodyType2D.Kinematic;
        SizeUpWeapon(Vector3.one * Mathf.Sqrt(Owner.Size));

        Handle.enabled = false;
    }
    public abstract void GetBuff();

    public void FireBuff()
    {
        if (FireEffectCoroutine != null)
        {
            Destroy(FireEffect);
            StopCoroutine(FireEffectCoroutine);
        }
        FireEffectCoroutine = StartCoroutine(FireEffectCour());
    }
    protected IEnumerator FireEffectCour()
    {
        PrevEffect = NowEffect;
        NowEffect = Man.EffectType.Fire;
        FireEffect = Instantiate(GameData.active.GetEffect("Fire"));
        FireEffect.transform.parent = Trail.transform;
        FireEffect.transform.position = Trail.transform.position;
        yield return new WaitForSeconds(1f);
        while (Power > 1)
        {
            yield return new WaitForFixedUpdate();
        }
        Destroy(FireEffect);
        NowEffect = PrevEffect;
        FireEffectCoroutine = null;
        yield break;
    }


    protected void CreateSparks(Man man)
    {
        if (man.weapon == null)
            return;
        GameObject Sparks = Instantiate(GameData.active.GetEffect("Sparks"), man.weapon.EffectPos(), transform.rotation, null);
        Sparks.transform.localScale = transform.localScale;
    }
    protected void CreateHitEffect(Man man)
    {
        GameObject Eff = Instantiate(GameData.active.GetEffect("Collide"), EffectPos(), transform.rotation, transform);
        Eff.transform.localScale = transform.localScale;
        var Main = Eff.GetComponent<ParticleSystem>().main;
        Main.startColor = new Color(man.SkinColor.r, man.SkinColor.g, man.SkinColor.b, 0.25f);
    }

    protected void DelayAttack(Man man, float time)
    {
        StartCoroutine(DelayAttackCour(man, time));
    }
    private IEnumerator DelayAttackCour(Man man, float time)
    {
        if (PrevAttack.Exists(item => item == man))
            yield break;
        PrevAttack.Add(man);
        yield return new WaitForSeconds(time);
        PrevAttack.Remove(man);
        yield break;
    }

    public void ThrowOut()
    {
        Fly = true;
        NoAttack = true;
        InHand = false;
        Rig.bodyType = RigidbodyType2D.Dynamic;
        Rig.centerOfMass = new Vector2(0, Lenght() / 2);
        transform.parent = null;
        SizeUpWeapon(Vector3.one);

        Rig.centerOfMass = new Vector2(0, Lenght() / 2);
        Rig.angularVelocity += Random.Range(360, 720) / Mathf.Sqrt(Weight);
        Rig.velocity += new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * 5f / Mathf.Sqrt(Weight);
        Handle.enabled = true;
    }
    public void FreeFall()
    {
        Fly = true;
        NoAttack = true;
        InHand = false;
        Rig.bodyType = RigidbodyType2D.Dynamic;
        Rig.centerOfMass = new Vector2(0, Lenght() / 2);
        transform.parent = null;
        SizeUpWeapon(Vector3.one);
        Handle.enabled = true;
    }
    protected void StopFly()
    {
        Fly = false;
        Rig.velocity *= 0f;
        Rig.angularVelocity = 0f;
        Rig.bodyType = RigidbodyType2D.Kinematic;
        Handle.enabled = true;
    }
    protected void SizeUpWeapon(Vector3 Size)
    {
        if(SizeUpCour != null)
        {
            StopCoroutine(SizeUpCour);
        }
        SizeUpCour = StartCoroutine(SizeUpWeaponCour(Size));
    }
    private IEnumerator SizeUpWeaponCour(Vector3 Size)
    {
        Vector3 ThisSize = Size.magnitude > 1 ? Size : Vector3.one;
        SizingUp = true;
        while(Mathf.Abs(transform.localScale.magnitude - ThisSize.magnitude) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, ThisSize, 0.1f);
            yield return new WaitForFixedUpdate();
        }
        transform.localScale = ThisSize;
        SizeUpCour = null;
        SizingUp = false;
        yield break;
    }

    public void GetAttackInfo(bool ManAttack, float Speed, float Rotation, float Power)
    {
        this.ManAttack = ManAttack;
        this.Speed = Speed;
        this.Rotation = Rotation;
        this.Power = Power;
        Right = Vector2.Dot(transform.up, Vector2.right) > 0 ? 1 : -1;
    }
    private void MovementStuff()
    {
        if(!SizingUp)
        {
            int xRot = Vector2.Dot(transform.up, Vector2.right) < 0 ? -1 : 1;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * xRot, transform.localScale.y, transform.localScale.z);
        }
        if(Owner == null)
        {
            Power = 1;
        }

        if (Trail != null)
        {
            Trail.gameObject.SetActive(InHand || Fly);
            Trail.startColor = Power > 1 ? GameData.active.WeaponRedTrail : GameData.active.WeaponTrail;
            Trail.endColor = GameData.ClearColor(Power > 1 ? GameData.active.WeaponRedTrail : GameData.active.WeaponTrail);
            Trail.time = Mathf.Lerp(Trail.time, Mathf.Abs(Rotation / 10) + Rig.velocity.magnitude / 100 + Speed / 5 + 0.1f, 0.1f);
        }
    }

    public virtual void OnFixedUpdate()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Owner != null && collision.tag == "Player" && collision.gameObject != Owner.gameObject)
        {
            if (InHand)
            {
                PiercingAttack(collision.GetComponent<Man>());
            }
            else if (Fly)
            {
                ThrowAttack(collision.GetComponent<Man>());
            }
        }
        if (Owner != null && collision.tag == "Object" && collision.gameObject != Owner.gameObject)
        {
            if (InHand)
            {
                Attack(collision.GetComponent<SceneObject>());
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Owner != null && collision.tag == "Player" && collision.gameObject != Owner.gameObject)
        {
            if (InHand)
            {
                Attack(collision.GetComponent<Man>());
            }
        }
        if (collision.tag == "Obstacle" && Fly)
        {
            StopFly();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {

    }

    private void FixedUpdate()
    {
        if(Fly)
        {
            switch (WeaponType)
            {
                case Type.Lance:
                    transform.up = Vector2.Lerp(transform.up, Rig.velocity.normalized, 0.1f);
                    break;
            }
        }
        MovementStuff();
        OnFixedUpdate();
    }
}
