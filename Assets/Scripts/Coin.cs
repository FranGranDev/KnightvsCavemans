using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public static void CreateCoin(Vector2 Pos, int Cost)
    {
        Coin now = Instantiate(GameData.active.coin, Pos, Quaternion.identity, null);
        now.Cost = Cost;
    }

    public float Speed;
    public int Cost;
    private bool Taken;
    private bool FlyTo()
    {
        return (transform.position - Player.active.Body.position).magnitude < 25;
    }
    public Rigidbody2D Rig;

    void FixedUpdate()
    {
        if (FlyTo())
        {
            Rig.velocity = Vector2.Lerp(Rig.velocity, (Player.active.Body.position - transform.position).normalized * Speed, 0.05f);
        }
        if (!Taken && (transform.position - Player.active.Body.position).magnitude < 0.5f)
        {
            OnPlayerEnter();
        }
    }

    private void OnPlayerEnter()
    {
        Taken = true;
        Level.active.UpdateMoney(Cost);
        StartCoroutine(DestroyCour());
    }
    private IEnumerator DestroyCour()
    {
        Instantiate(GameData.active.GetEffect("CoinTaken"), transform.position, Quaternion.identity, null);
        while (transform.localScale.magnitude > 0.1f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.075f);
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }
}