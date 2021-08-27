using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private bool Static;
    public void SetStatic(bool On)
    {
        Static = On;
    }
    public enum ShowTypes {PlayerSingle, Failed, Ai, Punch, Duel, Boss, Menu, Arena}
    public ShowTypes ShowType;
    private ShowTypes PrevShowType;
    public static CameraMove active;
    public Man Main;
    public Transform[] Target;
    public Vector3 Offset;
    public float Size;
    [Range(0, 0.1f)]
    public float Speed;
    private float CurrantSpeed;

    public Camera Cam;
    private Coroutine PunchCour;

    private void Awake()
    {
        active = this;
    }

    private void Follow()
    {
        Main = Level.active.MainPlayer;

        float TrackleRatio = Main.OnTackle ? 2 : 1;
        float GunWeapon = Main.weapon != null && Main.weapon.WeaponType == Weapon.Type.Gun ? 5 : 0;
        Vector3 GunDir = Main.weapon != null && Main.weapon.WeaponType == Weapon.Type.Gun ? Main.Arm.up * 5 : Vector3.zero;
        transform.position = Vector3.Lerp(transform.position, Main.transform.position + (Vector3)Main.Rig.velocity * Mathf.Sqrt(TrackleRatio) * 0.5f + GunDir + Offset, CurrantSpeed);
        Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, Size + Main.Rig.velocity.magnitude * TrackleRatio * 0.33f + GunWeapon, CurrantSpeed);
    }
    private void BossFollow()
    {
        Main = Level.active.MainPlayer;
        if(Main.DistX(Target[0]) > 30)
        {
            float GunWeapon = Main.weapon != null && Main.weapon.WeaponType == Weapon.Type.Gun ? 5 : 0;
            Vector3 GunDir = Main.weapon != null && Main.weapon.WeaponType == Weapon.Type.Gun ? Main.Arm.up * 5 : Vector3.zero;
            transform.position = Vector3.Lerp(transform.position, Main.transform.position + GunDir + Offset, CurrantSpeed);
            Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, Size + Main.Rig.velocity.magnitude * 0.5f + GunWeapon, CurrantSpeed);
        }
        else
        {
            Vector3 Position = (Main.transform.position + Target[0].position) / 2;
            transform.position = Vector3.Lerp(transform.position, Position + Offset, CurrantSpeed);
            Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, Size + Main.Dist(Target[0]) * 0.55f, CurrantSpeed);
        }
    }
    private void AiFollow()
    {
        transform.position = Vector3.Lerp(transform.position, Target[0].position + Offset, CurrantSpeed);
        Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, Size, CurrantSpeed);
    }
    private void FailedFollow()
    {
        transform.position = Vector3.Lerp(transform.position, Main.transform.position + Offset, CurrantSpeed + 0.01f);
        Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, Size * 0.25f, CurrantSpeed + 0.01f);
    }
    public void BetsFollow(List<Man> Enemy)
    {
        if (Static)
            return;
        ShowType = ShowTypes.Menu;
        Vector3 Pos = Vector3.zero;
        float Dist = 0;
        for (int i = 0; i < Enemy.Count; i++)
        {
            Pos += Enemy[i].transform.position;
        }
        if (Enemy.Count == 2)
        {
            Pos *= 0.5f;
            Dist = Mathf.Abs(Enemy[0].transform.position.x -
                             Enemy[1].transform.position.x);
            if (Dist > 20)
            {
                Pos = Enemy[0].transform.position;
                Dist = 0;
            }
        }

        transform.position = Vector3.Lerp(transform.position, Pos + Offset, CurrantSpeed);
        Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, Size + Dist * 0.6f, CurrantSpeed);
    }
    public void MenuFollow(List<Man> Enemy)
    {
        if (Static)
            return;
        ShowType = ShowTypes.Menu;
        Vector3 Pos = Vector3.zero;
        float Dist = 0;
        for(int i = 0; i < Enemy.Count; i++)
        {
            Pos += Enemy[i].transform.position;
        }
        if(Enemy.Count == 2)
        {
            Pos *= 0.5f;
            Dist = Mathf.Abs(Enemy[0].transform.position.x -
                             Enemy[1].transform.position.x);
            if(Dist > 20)
            {
                Pos = Enemy[0].transform.position;
                Dist = 0;
            }
        }

        transform.position = Vector3.Lerp(transform.position, Pos + Offset, CurrantSpeed);
        Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, Size + Dist * 0.6f, CurrantSpeed);
    }
    public void DuelFollow(Man enemy)
    {
        Vector3 Pos = Vector2.zero;
        float Dist = 0;
        if(Main.DistX(enemy) < 25)
        {
            Pos = (Main.transform.position + enemy.transform.position) / 2;
            Dist = Main.DistX(enemy) * 0.6f;
        }
        else
        {
            float TrackleRatio = Main.OnTackle ? 2 : 1;
            Pos = Main.transform.position + (Vector3)Main.Rig.velocity * Mathf.Sqrt(TrackleRatio) * 0.5f;
            Dist = Main.Rig.velocity.magnitude * TrackleRatio * 0.33f;
        }
        float GunWeapon = Main.weapon != null && Main.weapon.WeaponType == Weapon.Type.Gun ? 5 : 0;
        Vector3 GunDir = Main.weapon != null && Main.weapon.WeaponType == Weapon.Type.Gun ? Main.Arm.up * 5 : Vector3.zero;
        transform.position = Vector3.Lerp(transform.position, Pos + GunDir + Offset, CurrantSpeed);
        Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, Size + Dist + GunWeapon, CurrantSpeed);
    }

    public void TurnFailedShow()
    {
        Main = Player.active;
        PrevShowType = ShowTypes.Failed;
        ShowType = ShowTypes.Failed;
        Target = null;
        CurrantSpeed = 0;
    }
    public void TurnAiFollow(Man man, bool on)
    {
        if (man == null)
            return;
        if (on)
        {
            PrevShowType = ShowTypes.Ai;
            ShowType = ShowTypes.Ai;
            Target = new Transform[1] { man.transform };
        }
        else
        {
            Target = null;
            PrevShowType = ShowTypes.PlayerSingle;
            ShowType = ShowTypes.PlayerSingle;
        }
        CurrantSpeed = 0;
    }
    public void TurnPlayerFollow()
    {
        Main = Player.active;
        PrevShowType = ShowTypes.PlayerSingle;
        ShowType = ShowTypes.PlayerSingle;
        Target = null;
        CurrantSpeed = 0;
    }
    public void TurnDuelFollow()
    {
        Main = Player.active;
        PrevShowType = ShowTypes.Duel;
        ShowType = ShowTypes.Duel;
        Target = null;
        CurrantSpeed = 0;
    }
    public void TurnBossFollow(Man man, bool on)
    {
        if(on)
        {
            PrevShowType = ShowTypes.Boss;
            ShowType = ShowTypes.Boss;
            Target = new Transform[1] { man.transform };
        }
        else
        {
            Target = null;
            PrevShowType = ShowTypes.PlayerSingle;
            ShowType = ShowTypes.PlayerSingle;
        }
        CurrantSpeed = 0;
    }

    public void PunchShow(Man Enemy, float Power, Man.HitType Type)
    {
        if (ShowType == ShowTypes.Failed || ShowType == ShowTypes.Ai)
            return;
        if(PunchCour != null)
        {
            ShowType = PrevShowType;
            StopCoroutine(PunchCour);
        }
        PrevShowType = ShowType;
        PunchCour = StartCoroutine(PunchShowCour(Enemy, Power, Type));
    }
    private IEnumerator PunchShowCour(Man man, float Power, Man.HitType Type)
    {
        Man Enemy = man;
        ShowType = ShowTypes.Punch;
        int time = Mathf.RoundToInt(1.5f * Power / Time.deltaTime);
        for (int i = 0; i < time && ShowType == ShowTypes.Punch; i++)
        {
            if (Enemy == null)
            {
                CurrantSpeed = 0;
                ShowType = PrevShowType;
                PunchCour = null;
                yield break;
            }
            Vector3 Position = Vector3.zero;
            switch(Type)
            {
                case Man.HitType.Throw:
                    ShowType = PrevShowType;
                    PunchCour = null;
                    yield break;
                case Man.HitType.Hit:
                    Position = (Main.transform.position + Enemy.transform.position) / 2;
                    break;
                case Man.HitType.Punch:
                    Position = (Main.transform.position + Enemy.transform.position) / 2;
                    break;
                case Man.HitType.Tackle:
                    Position = (Main.transform.position + Enemy.transform.position) / 2;
                    break;
                case Man.HitType.Fall:
                    Position = (Main.transform.position + Enemy.transform.position) / 2;
                    break;
            }
            transform.position = Vector3.Lerp(transform.position, Position + Offset, Speed / 2);
            Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, Size + Main.Dist(Enemy.transform), Speed / 2);
            yield return new WaitForFixedUpdate();
        }
        CurrantSpeed = 0;
        ShowType = PrevShowType;
        PunchCour = null;
        yield break;
    }

    public void ShowArena()
    {
        StartCoroutine(ShowArenaCour());
    }
    private IEnumerator ShowArenaCour()
    {
        ShowType = ShowTypes.Arena;
        transform.position = new Vector3(-Level.active.sceneMaker.SizeX * 0.9f, 0, 0) + Offset;
        Vector3 Target0 = new Vector3(Level.active.sceneMaker.SizeX * 0.9f, 0, 0) + Offset;
        while ((transform.position - (Target0 + Offset)).magnitude > 5)
        {
            transform.position = Vector3.Lerp(transform.position, Target0 + Offset, Speed / 8);
            Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, 35, Speed);
            yield return new WaitForFixedUpdate();
        }
        Vector3 Target1 = Vector3.zero;
        while ((transform.position - (Target1 + Offset)).magnitude > 5)
        {
            transform.position = Vector3.Lerp(transform.position, Target1 + Offset, Speed / 4);
            Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, 35, Speed);
            yield return new WaitForFixedUpdate();
        }
        ShowType = PrevShowType;
        CurrantSpeed = 0;
        yield break;
    }

    public void UpToDown()
    {
        transform.position = Vector3.up * 10 + Offset;
        CurrantSpeed = 0;
    }

    private void FixedUpdate()
    {
        switch(ShowType)
        {
            case ShowTypes.PlayerSingle:
                Follow();
                break;
            case ShowTypes.Boss:
                BossFollow();
                break;
            case ShowTypes.Ai:
                AiFollow();
                break;
            case ShowTypes.Failed:
                FailedFollow();
                break;
            case ShowTypes.Duel:

                break;
        }

        CurrantSpeed = Mathf.Lerp(CurrantSpeed, Speed, 0.01f);
    }
}
