using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    public enum Type {Power, Speed, Hp, Size}
    public const int HpBuff = 10;
    public const float BuffTime = 10f;
    public const float PowerBuff = 3f;
    public const float SpeedBuff = 1.5f;
    public const float JumpBuff = 1.25f;
    public const float SizeBuff = 1.5f;
    public Type BuffType;
    private bool Used;

    public Rigidbody2D Rig;

    private void Awake()
    {
        Rig = GetComponent<Rigidbody2D>();
    }

    private IEnumerator DestroyThis()
    {
        Instantiate(GameData.active.GetEffect("TakeBuff"), transform.position, transform.rotation, null);
        while (transform.localScale.magnitude < 2f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 3, 0.1f);
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(0.25f);
        while (transform.localScale.magnitude > 0.25f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.1f);
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
        yield break;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !Used)
        {
            Used = true;
            collision.GetComponent<Man>().GetBuff(BuffType, BuffTime);
            StartCoroutine(DestroyThis());
        }
    }
}
