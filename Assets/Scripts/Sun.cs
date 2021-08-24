using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    [Header("Params")]
    [Range(0, 1f)]
    public float Speed;
    public Vector2 Offset;
    public float VelocityOffset;
    [Header("Components")]
    public Animator anim;

    private Camera MainCam;
    private Man Target;
    public RectTransform ThisSun;

    private void Start()
    {
        Target = Player.active;
        MainCam = Camera.main;

        StartCoroutine(ChangeFace());
    }
    private void Follow()
    {
        Target = Level.active.cameraMove.Main;

        Vector2 Velocity = Target == null ? Vector2.zero : new Vector2(-Target.VelocityNorm.x * VelocityOffset, -Target.VelocityNorm.y * VelocityOffset / 2f);
        ThisSun.anchoredPosition = Vector2.Lerp(ThisSun.anchoredPosition, Offset + Velocity, Speed);
    }

    private IEnumerator ChangeFace()
    {
        while(true)
        {
            anim.Play("SunFace" + Random.Range(0, 4));
            yield return new WaitForSeconds(Random.Range(5, 10));
        }
    }
    private void FixedUpdate()
    {
        Follow();
    }
}
