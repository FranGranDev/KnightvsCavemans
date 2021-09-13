using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneObject : MonoBehaviour
{
    [Header("Params")]
    public int Hp;
    public bool Destroyed;
    public Man.EffectType EffectHit;
    [Range(1, 3)]
    public float Weight;
    private Man PrevMan;
    public bool Fly;
    public bool HitOnGround;

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
        if(HitOnGround)
        {
            DestroyObj();
        }
    }
    private void Punch(Man man)
    {
        if (man == PrevMan || !Fly || SideOwn.isFriend(PrevMan, man))
            return;
        man.GetPunched(PrevMan, true);
        man.GetImpulse(Rig.velocity.normalized * Mathf.Sqrt(Weight * Rig.velocity.magnitude) * 2);
        man.GetHit(2 * Hp, man, Man.HitType.Object, EffectHit);
        if(Random.Range(0, 2) == 0)
        {
            man.ThrowOutWeapon();
        }
        GetHit(3, man);
        Rig.velocity *= 0.5f;
        PrevMan = null;
        Fly = false;
        if (HitOnGround)
        {
            DestroyObj();
        }
    }

    protected void Hit()
    {
        GameObject ThisEffect = Instantiate(Effect, transform.position, transform.rotation, null);
        ThisEffect.transform.localScale = Vector3.one * 0.75f;
        PlaySound("Hit");
    }

    protected void DestroyObj()
    {
        StartCoroutine(DestroyCour());
        PlaySound("Hit");
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

    public void PlaySound(string name)
    {
        ClipInfo info = GameData.active.GetSoundRand(name);
        StartCoroutine(PlaySoundCour(info));
    }
    private IEnumerator PlaySoundCour(ClipInfo info)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();

        source.clip = info.Clip;
        source.spatialBlend = 1f;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.maxDistance = 25f;
        source.volume = info.Volume * GameData.EffectVol;
        source.pitch = Random.Range(0.9f, 1.1f);
        source.Play();
        while (source.isPlaying)
        {
            yield return new WaitForFixedUpdate();
        }

        Destroy(source);

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
