using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Man : MonoBehaviour
{
    public bool Static;
    public enum ManType { Player, Enemy, Boss, Duel, Menu, Bets }
    public ManType Type;
    public enum HitType { Hit, HitFire, Throw, Bullet, Punch, Magic, Fall, Tackle, Object, Lava };
    public enum EffectType { Null, Fire }
    [Header("Params")]
    [Range(1, 3f)]
    public float Size;
    public float Speed;
    public float Power;
    public float JumpForce;
    public int MaxHp;
    public int Hp;
    public int Experience;
    public int Money;
    [HideInInspector]
    public bool NoThrowOut;
    public float GroundAcceleration()
    {
        return GameData.Acceleration / GameData.Speed;
    }
    public float GroundSpeed()
    {
        return GameData.Speed;
    }
    protected float StartSpeed;
    protected float StartSize;
    protected float StartJump;
    protected float StartPower;
    public float HpProcent()
    {
        return (float)Hp / (float)MaxHp;
    }
    public bool Dead;
    [Header("Weapon")]
    public Weapon weapon;
    public float WeaponWeight()
    {
        return weapon == null ? 1 : (weapon.Weight + 0.1f);
    }

    private GameObject PrevObj;
    public Horse NowHorse { get; protected set; }
    public int Right { get; protected set; }
    public float Velocity { get; protected set; }
    public bool OnHouse { get; protected set; }
    public Vector2 VelocityNorm { get; protected set; }
    public bool OnTackle { get; protected set; }
    public bool Landing { get; protected set; }
    public bool NoAttack { get; protected set; }
    public bool NoTake { get; protected set; }
    public bool NoPunch { get; protected set; }
    public bool NoGetPunch { get; protected set; }
    public float RotationSpeed { get; protected set; }
    public Vector2 PrevRotation { get; protected set; }
    public float PrevDirX { get; protected set; }
    public Vector2 PrevDir { get; protected set; }
    public float BodyY { get; protected set; }
    public bool Punched { get; protected set; }
    public bool OnGround { get; protected set; }
    public Color SkinColor;
    public const float MaxImpulse = 15;

    public Coroutine PunchCoroutine { get; protected set; }
    public Coroutine DelayAttackCoroutine { get; protected set; }
    public Coroutine FireEffectCoroutine { get; protected set; }
    public Dictionary<Buff.Type, Coroutine> BuffCoroutine { get; protected set; }
    private List<Collider2D> PassedMan;

    [Header("Components")]
    public ParticleSystem Effect;
    public Transform fireEffect;
    public Transform Arm;
    public Transform Body;
    public Transform BodySolver;
    public Rigidbody2D Rig;
    public Collider2D Col;
    public Animator anim;
    public GameObject FakeSprite;
    public SpriteRenderer[] FakeSprites;
    public SpriteRenderer[] Sprites;
    [Header("Armor")]
    public SpriteRenderer HeadSprite;
    public SpriteRenderer BodySprite;
    public SpriteRenderer LLeg0Sprite;
    public SpriteRenderer LLeg1Sprite;
    public SpriteRenderer RLeg0Sprite;
    public SpriteRenderer RLeg1Sprite;

    public float DistX(Man Targer)
    {
        if (Targer == null)
            return float.MaxValue;
        return (Mathf.Abs(Targer.transform.position.x - transform.position.x));
    }
    public float DistX(Transform Targer)
    {
        if (Targer == null)
            return 0;
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
    public float DotArm(Vector2 Dir)
    {
        return Vector2.Dot(Arm.transform.up, Dir);
    }
    public Vector2 Direction(Transform Target)
    {
        return (Target.position - transform.position).normalized;
    }
    public Vector2 BodyDirection(Man Target)
    {
        return (Target.Body.position - Body.position).normalized;
    }
    public Vector2 WeaponDir()
    {
        return weapon != null ? (Vector2)weapon.transform.up : Vector2.zero;
    }

    private void Start()
    {
        SetParams();
    }
    public virtual void OnStart()
    {

    }
    public virtual void SetParams()
    {
        OnStart();

        Dead = false;
        anim.SetBool("Dead", false);
        Hp = MaxHp;
        Power = 1;
        transform.localScale = Vector3.one * Size;
        Rig.mass *= Size * Size;
        BuffCoroutine = new Dictionary<Buff.Type, Coroutine>();
        PassedMan = new List<Collider2D>();

        StartJump = JumpForce;
        StartPower = Power;
        StartSize = Size;
        StartSpeed = Speed;
    }
    public virtual void SetParams(PlayerInfo info, ArmorInfo armorInfo)
    {
        OnStart();

        Dead = false;
        anim.SetBool("Dead", false);
        Hp = MaxHp;
        Power = 1;
        transform.localScale = Vector3.one * Size;
        Rig.mass *= Size * Size;
        BuffCoroutine = new Dictionary<Buff.Type, Coroutine>();

        BodySprite.sprite = armorInfo.Body;
        BodySprite.sortingOrder = armorInfo.BodyLayer;
        HeadSprite.sprite = armorInfo.Head;
        HeadSprite.sortingOrder = armorInfo.HeadLayer;
        LLeg0Sprite.sprite = armorInfo.LeftLeg0;
        LLeg0Sprite.sortingOrder = armorInfo.LeftLeg0Layer;
        LLeg1Sprite.sprite = armorInfo.LeftLeg1;
        LLeg1Sprite.sortingOrder = armorInfo.LeftLeg1Layer;
        RLeg0Sprite.sprite = armorInfo.RightLeg0;
        RLeg0Sprite.sortingOrder = armorInfo.RightLeg0Layer;
        RLeg1Sprite.sprite = armorInfo.RightLeg1;
        RLeg1Sprite.sortingOrder = armorInfo.RightLeg1Layer;

        if(FakeSprite != null)
        {
            FakeSprites[0].sprite = armorInfo.Head;
            FakeSprites[0].sortingOrder = armorInfo.HeadLayer;

            FakeSprites[1].sprite = armorInfo.Body;
            FakeSprites[1].sortingOrder = armorInfo.BodyLayer;

            FakeSprites[2].sprite = armorInfo.LeftLeg0;
            FakeSprites[2].sortingOrder = armorInfo.LeftLeg0Layer;

            FakeSprites[3].sprite = armorInfo.LeftLeg1;
            FakeSprites[3].sortingOrder = armorInfo.LeftLeg1Layer;

            FakeSprites[4].sprite = armorInfo.RightLeg0;
            FakeSprites[4].sortingOrder = armorInfo.RightLeg0Layer;

            FakeSprites[5].sprite = armorInfo.RightLeg1;
            FakeSprites[5].sortingOrder = armorInfo.RightLeg1Layer;
        }

        switch(armorInfo.Effect)
        {
            case ArmorInfo.EffectType.Null:
                fireEffect.gameObject.SetActive(false);
                break;
            case ArmorInfo.EffectType.Fire:
                fireEffect.gameObject.SetActive(true);
                break;
                    
        }

        MaxHp = Mathf.RoundToInt(info.MaxHp * (armorInfo.Hp + 1));
        Hp = MaxHp;
        Power = info.Power * (armorInfo.Power + 1);
        Speed = info.Speed * (armorInfo.Speed + 1);
        JumpForce = info.JumpForce * (armorInfo.Jump + 1);
        Size = info.Size * (armorInfo.Size + 1);

        StartJump = JumpForce;
        StartPower = Power;
        StartSize = Size;
        StartSpeed = Speed;
    }
    public virtual void SetSize(float Size)
    {
        this.Size = Size;
        transform.localScale = Vector3.one * Size;
    }
    public virtual void SetController(AiController controller)
    {

    }
    public virtual void SetStatic(bool on)
    {
        
    }

    protected void TurnSkeleton(bool On)
    {
        for(int i = 0; i < Sprites.Length; i++)
        {
            Sprites[i].gameObject.SetActive(On);
        }
        FakeSprite.SetActive(!On);
    }

    public abstract void Movement(Vector2 Dir);

    public abstract void MoveArm(Vector2 Dir);

    public abstract void Jump();

    public abstract void Tackle();

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
    protected IEnumerator WaitGround(Man Enemy)
    {
        float PrevVelocity = Velocity;
        yield return new WaitForSeconds(0.25f);
        while (!OnGround)
        {
            PrevVelocity = Velocity;
            yield return new WaitForFixedUpdate();
        }
        if (PrevVelocity > 0.9f)
        {
            yield return new WaitForSeconds(0.05f);
            OnLandOof(Enemy, PrevVelocity);
        }
        yield return new WaitForFixedUpdate();
        Punched = false;
        yield break;
    }
    public virtual void OnLandOof(Man Enemy, float velocity)
    {
        GetHit(Mathf.RoundToInt(Mathf.Sqrt(Velocity)), Enemy, HitType.Punch, EffectType.Null);
        if(Punched && velocity > 1)
        {
            ThrowOutWeapon();
        }
    }

    public abstract void TakeWeapon(Weapon weapon);

    public virtual void ThrowOutWeapon()
    {
        if (weapon == null || NoThrowOut)
            return;
        weapon.ThrowOut();
        weapon = null;
        StartCoroutine(TakeDelay());
    }
    protected virtual IEnumerator TakeDelay()
    {
        NoTake = true;
        yield return new WaitForSeconds(2f);
        NoTake = false;
        yield break;
    }

    public abstract void Throw();

    public abstract void Punch(Man Enemy);
    public abstract void Punch(SceneObject Obj);
    public virtual void PunchDelay()
    {
        if(PunchCoroutine != null)
        {
            StopCoroutine(PunchCoroutine);
        }
        PunchCoroutine = StartCoroutine(PunchDelayCour());
    }
    public virtual IEnumerator PunchDelayCour()
    {
        NoPunch = true;
        yield return new WaitForSeconds(2f);
        NoPunch = false;
        yield break;
    }


    private void Pass(Man man, Collider2D col)
    {
        Physics2D.IgnoreCollision(Col, man.Col, true);
        StartCoroutine(PassCour(man, col));
    }
    private IEnumerator PassCour(Man man, Collider2D col)
    {
        PassedMan.Add(col);
        while (DistX(man) < 3)
        {
            yield return new WaitForFixedUpdate();
        }
        if (man != null)
        {
            Physics2D.IgnoreCollision(Col, man.Col, false);
        }
        PassedMan.Remove(col);
        yield break;
    }

    public virtual void ForceFlip(bool right)
    {
        if(right)
        {
            anim.SetTrigger("FlipRight");
        }
        else
        {
            anim.SetTrigger("FlipLeft");
        }
    }

    public abstract void OnAttack(Man Enemy, float Power, HitType Type);

    public virtual void GetEnemy(Man Enemy)
    {

    }
    public virtual void TryGetEnemy(Man Enemy)
    {

    }

    public void GetPunched(Man Enemy, bool Up)
    {
        if (Up)
        {
            Punched = true;
            StartCoroutine(WaitGround(Enemy));
            StartCoroutine(GetPunchedCour());
        }
    }
    public virtual IEnumerator GetPunchedCour()
    {
        NoGetPunch = true;
        yield return new WaitForSeconds(1f);
        NoGetPunch = false;
        yield break;
    }

    protected void DelayAttack(float Delay)
    {
        if(DelayAttackCoroutine == null)
        {
            DelayAttackCoroutine = StartCoroutine(NoAttackCour(Delay));
        }
    }
    private IEnumerator NoAttackCour(float Delay)
    {
        NoAttack = true;
        yield return new WaitForSeconds(Delay);
        NoAttack = false;
        DelayAttackCoroutine = null;
        yield break;
    }

    public abstract void GetHit(int Damage, Man Enemy, HitType type, EffectType effect);

    public abstract void Die(Man Enemy, HitType type);

    public virtual void GetBuff(Buff.Type type, float time)
    {
        if(BuffCoroutine.ContainsKey(type))
        {
            if(BuffCoroutine[type] != null)
                StopCoroutine(BuffCoroutine[type]);
            BuffCoroutine[type] = StartCoroutine(BuffCour(type, time));
        }
        else
        {
            BuffCoroutine.Add(type, StartCoroutine(BuffCour(type, time)));
        }

    }
    private IEnumerator BuffCour(Buff.Type type, float time)
    {
        switch(type)
        {
            case Buff.Type.Hp:
                Hp += Buff.HpBuff;

                break;
            case Buff.Type.Power:
                Power = StartPower * Buff.PowerBuff;
                if (weapon != null)
                {
                    weapon.GetBuff();
                }
                yield return new WaitForSeconds(time);
                Power = StartPower;
                break;
            case Buff.Type.Size:
                Size = StartSize * Buff.SizeBuff;
                yield return new WaitForSeconds(time);
                Size = StartSize;
                break;
            case Buff.Type.Speed:
                Speed = StartSpeed * Buff.SpeedBuff;
                JumpForce = StartJump * Buff.JumpBuff;
                yield return new WaitForSeconds(time);
                Speed = StartSpeed;
                JumpForce = StartJump;
                break;
        }
        BuffCoroutine.Remove(type);
        yield break;
    }
    public void StopAllBuff()
    {
        if(BuffCoroutine.ContainsKey(Buff.Type.Hp))
        {
            StopCoroutine(BuffCoroutine[Buff.Type.Hp]);
        }
        if (BuffCoroutine.ContainsKey(Buff.Type.Power))
        {
            StopCoroutine(BuffCoroutine[Buff.Type.Power]);
        }
        if (BuffCoroutine.ContainsKey(Buff.Type.Size))
        {
            StopCoroutine(BuffCoroutine[Buff.Type.Size]);
        }
        if (BuffCoroutine.ContainsKey(Buff.Type.Speed))
        {
            StopCoroutine(BuffCoroutine[Buff.Type.Speed]);
        }
        BuffCoroutine.Clear();
    }

    public void GetEffect(Man man, EffectType effect)
    {
        switch(effect)
        {
            case EffectType.Fire:
                if(FireEffectCoroutine == null)
                {
                    FireEffectCoroutine = StartCoroutine(FireEffectCour(man));
                }
                break;
        }
    }
    private IEnumerator FireEffectCour(Man man)
    {
        ParticleSystem fire = Instantiate(GameData.active.GetEffect("ManFired"), Body.position, Body.rotation, transform).GetComponent<ParticleSystem>();
        var Shape = fire.shape;
        Shape.scale = Vector3.one * Size;
        var Emmision = fire.emission;
        Emmision.rateOverTime = 100 * Size;

        float time = 5;
        for(int i = 0; i < time; i++)
        {
            GetHit(1, man, HitType.HitFire, EffectType.Null);
            yield return new WaitForSeconds(1);
        }

        Destroy(fire.gameObject);
        FireEffectCoroutine = null;
        yield break;
    }

    public virtual void GetImpulse(Vector2 Impulse)
    {
        Vector2 CurrantImpulse = (OnGround || Landing) ? Impulse : Impulse * 0.5f;

        if(Impulse.magnitude / Mathf.Sqrt(Size) < MaxImpulse)
        {
            Rig.velocity = CurrantImpulse / Mathf.Sqrt(Size);
        }
        else
        {
            Rig.velocity = CurrantImpulse.normalized / Mathf.Sqrt(Size) * MaxImpulse;
        }
        
    }

    private void MovementStuff()
    {
        if (Punched)
        {
            transform.up = Vector2.Lerp(transform.up, Rig.velocity.normalized, 0.05f);
        }
        else if (OnGround || Landing)
        {
            transform.up = Vector2.Lerp(transform.up, Vector2.up, 0.05f / Size);
        }

        Velocity = Mathf.Abs(Rig.velocity.magnitude / Speed);
        VelocityNorm = Rig.velocity / new Vector2(Speed, Speed);
        anim.SetFloat("Speed", Velocity);
        anim.SetFloat("Rotation", RotationSpeed);

        if (Mathf.Abs(Rig.velocity.x) > 0.1f)
        {
            PrevDirX = Rig.velocity.normalized.x;
        }
        BodySolver.localPosition = Vector2.Lerp(BodySolver.localPosition,
         new Vector2(-BodyY * 1.25f * Right, BodySolver.localPosition.y), 0.1f);
        float ScaleY = Size * (1 + (BodyY < 0 ? BodyY * 0.1f : 0));
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(transform.localScale.x, ScaleY, transform.localScale.z), 0.1f);
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * Size, 0.05f);
        if (weapon != null)
        {
            weapon.GetAttackInfo(!Punched, Velocity, RotationSpeed, Power / StartPower);
        }
        if (Static)
        {
            FakeSprite.transform.rotation = new Quaternion(0, Right == 1 ? 0 : 180, 0, 0);
        }

        RotationSpeed = Vector2.Dot(Arm.transform.right, PrevRotation) / Time.deltaTime * 0.25f;
        PrevRotation = Arm.transform.up;

        Effect.gameObject.SetActive(Velocity > 0.25f && OnGround);
        Effect.transform.localScale = Vector3.one * Size;
        var Main = Effect.main;
        Main.startColor = Speed <= StartSpeed ? GameData.active.GetGroundColor() : GameData.active.FastRun;
        var Emission = Effect.emission;
        Emission.rateOverTime = Rig.velocity.magnitude * (OnTackle ? 3 : 1) * 2;
    }
    private void Flip()
    {
        Right = PrevDirX > 0 ? 1 : -1;
        anim.SetBool("Right", PrevDirX > 0);
    }

    private void SetOnGround(bool on)
    {
        bool Prev = OnGround;
        OnGround = on;
        if(on && !Prev)
        {
            Land();
        }
        if(!on && Prev)
        {
            StartCoroutine(LeaveGround());
        }

        if(on)
        {
            Rig.velocity = new Vector2(Rig.velocity.x, Rig.velocity.y * 0.75f);
        }
    }
    protected IEnumerator LeaveGround()
    {
        yield return new WaitForSeconds(0.25f);
        if(!OnGround)
            anim.SetBool("OnGround", false);
        yield break;
    }

    protected void IgnoreObject(bool on, Collider2D obj)
    {
        if(on)
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
        if(obj == null)
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

    public virtual void OnHorseEnter(Horse horse)
    {

    }

    private void FixedUpdate()
    {
        MovementStuff();
        Flip();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Man man = collision.GetComponent<Man>();
            if(SideOwn.isEnemy(man, this))
            {
                Punch(man);
            }
        }
        else if(collision.tag == "Object")
        {
            SceneObject Obj = collision.GetComponent<SceneObject>();
            if(Velocity > 0.5f)
            {
                Punch(Obj);
            }

            IgnoreObject(true, collision);
        }
        else if(collision.tag == "Horse")
        {
            OnHorseEnter(collision.GetComponent<Horse>());
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !PassedMan.Exists(item => item == collision))
        {
            Man man = collision.GetComponent<Man>();
            if (!SideOwn.isEnemy(man, this))
            {
                Pass(man, collision);
            }
        }
        if (collision.tag == "Obstacle")
        {
            SetOnGround(true);
        }
        if (collision.tag == "Weapon" && !NoTake && weapon == null && collision.GetComponent<Weapon>() != null)
        {
            TakeWeapon(collision.GetComponent<Weapon>());
        }
        if(collision.tag == "Death")
        {
            GetHit(1, null, HitType.Lava, EffectType.Fire);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Obstacle")
        {
            SetOnGround(false);
        }
        if(collision.tag == "Object")
        {
            IgnoreObject(false, collision);
        }
    }
}

public struct ManInfo
{
    public string name;
    public Man man;
    public Man.ManType Type;
    public float ViewLength;
    public float Size;
    public int MaxHp;
    public float Speed;
    public int Exp;
    public Weapon weapon;

    public ManInfo(string name, Man man, Man.ManType type, float viewLength, float size, int maxHp, float speed, int Exp, Weapon weapon)
    {
        this.name = name;
        this.man = man;
        Type = type;
        ViewLength = viewLength;
        Size = size;
        MaxHp = maxHp;
        Speed = speed;
        this.Exp = Exp;
        this.weapon = weapon;
    }
}