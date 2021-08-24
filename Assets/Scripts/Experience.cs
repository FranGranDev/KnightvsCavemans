using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experience : MonoBehaviour
{
    public static void CreateExp(Vector2 Pos, int Exp)
    {
        Experience now = Instantiate(GameData.active.Exp, Pos, Quaternion.identity, null);
        now.Exp = Exp;
    }

    public float Speed;
    public int Exp;
    private bool Taken;
    private bool FlyTo()
    {
        return (transform.position - Player.active.Body.position).magnitude < 10;
    }
    public Rigidbody2D Rig;

    void FixedUpdate()
    {
        if(FlyTo())
        {
            Rig.velocity = Vector2.Lerp(Rig.velocity, (Player.active.Body.position - transform.position).normalized * Speed, 0.05f);
        }
        if(!Taken && (transform.position - Player.active.Body.position).magnitude < 0.5f)
        {
            OnPlayerEnter();
        }
    }

    private void OnPlayerEnter()
    {
        Taken = true;
        Level.active.UpdatePlayerExperience(Exp);
        StartCoroutine(DestroyCour());
    }
    private IEnumerator DestroyCour()
    {
        Instantiate(GameData.active.GetEffect("ExpTaken"), transform.position, Quaternion.identity, null);
        while(transform.localScale.magnitude > 0.1f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.075f);
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }
}
