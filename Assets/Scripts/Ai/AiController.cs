using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
    [Header("Params")]
    public Man ThisPlayer;
    public float ViewLenght;
    public Man Enemy;
    public float SumVelocity()
    {
        return Mathf.Abs(ThisPlayer.VelocityNorm.x * 1.5f - (Enemy == null ? 0 : Enemy.VelocityNorm.x));
    }
    public bool NoThrow;
    public bool SuperAction;
    public bool HaveEnemy()
    {
        return Enemy != null;
    }
    public bool HaveWeapon()
    {
        return ThisPlayer.weapon != null;
    }
    public Coroutine SwingCoroutine;
    public Coroutine WalkCoroutine;
    public Coroutine RamCoroutine;
    public Coroutine LooseCoroutine;

    [Header("States")]
    public State Idle;
    public State Walk;
    public State Attack;
    public State BossAttack;
    public State DuelAttack;
    public State GunAttack;
    public State AnimalAttack;
    public State RunAway;
    [Header("Currant States")]
    public State CurrantState;
    [Header("Components")]
    public Transform Eye;
    public Animator anim;

    public void SetStatic(bool on)
    {
        if(on)
        {
            SetState(Idle);
        }
    }


    public void Movement(Vector2 Dir)
    {
        float RandMove = Random.Range(-0.1f, 0.1f);
        if (Dir != Vector2.zero)
        {
            ThisPlayer.Movement(new Vector2(Dir.x + RandMove, Dir.y));
            Eye.transform.right = Dir;
        }
        else
        {
            ThisPlayer.Movement(Dir);
        }
    }

    public void MoveArm(Vector2 Dir)
    {
        ThisPlayer.MoveArm(Dir);
    }

    public void Throw(Vector2 Dir)
    {
        StopSwing();
        StartCoroutine(ThrowCour(Dir));
        StartCoroutine(WaitThrow());
    }
    private IEnumerator ThrowCour(Vector2 Dir)
    {
        while (ThisPlayer.DotArm(Dir) < 0.97f)
        {
            MoveArm(Dir);
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(0.25f);
        ThisPlayer.Arm.up = Dir;
        ThisPlayer.Throw();
        yield break;
    }
    private IEnumerator WaitThrow()
    {
        NoThrow = true;
        yield return new WaitForSeconds(3);
        NoThrow = false;
        yield break;
    }

    public void Jump()
    {
        StartCoroutine(JumpCour());
    }
    private IEnumerator JumpCour()
    {
        yield return new WaitForSeconds(0.25f);
        ThisPlayer.Jump();
        yield break;
    }

    public Man CheckForEnemy(Vector2 Dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(Eye.position, Dir, ViewLenght, 1 << 8 | 1 << 9);
        if(hit.collider != null && hit.collider.GetComponent<Man>() != null)
        {
            Man man = hit.collider.GetComponent<Man>();
            if (SideOwn.isEnemy(man, ThisPlayer) && man != ThisPlayer && !Level.active.NextLava(man))
            {
                return man;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
    public Man CheckForFriend(Vector2 Dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(Eye.position, Dir, ViewLenght, 1 << 8 | 1 << 9);
        if (hit.collider != null && hit.collider.GetComponent<Man>() != null)
        {
            Man man = hit.collider.GetComponent<Man>();
            if (SideOwn.isFriend(man, ThisPlayer))
            {
                return man;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    public void GetEnemy(Man Enemy)
    {
        if(Enemy != ThisPlayer && SideOwn.isEnemy(ThisPlayer, Enemy))
            this.Enemy = Enemy;
    }
    public void TryGetEnemy(Man Enemy)
    {
        if (this.Enemy == null && Enemy != ThisPlayer)
        {
            this.Enemy = Enemy;
        }
    }

    public void LooseEnemy(Man man)
    {
        if(LooseCoroutine == null && this.Enemy != null)
        {
            LooseCoroutine = StartCoroutine(LooseEnemyCour(Enemy));
        }
    }
    public void LooseEnemyFixed(Man man)
    {
        if(Enemy == man)
        {
            Enemy = null;
        }
        
    }
    private IEnumerator LooseEnemyCour(Man man)
    {
        float time = 0;
        while(ThisPlayer.DistX(man) > ViewLenght && Enemy == man && time < 2)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        if(time >= 3)
        {
            LooseEnemyFixed(man);
        }
        LooseCoroutine = null;
        yield break;
    }

    public void StopSwing()
    {
        if (SwingCoroutine != null)
        {
            StopCoroutine(SwingCoroutine);
            SwingCoroutine = null;
            SuperAction = false;
        }
    }

    public void SwingReady(Vector2 Dir, bool UpHit)
    {
        if (SwingCoroutine == null)
        {
            SwingCoroutine = StartCoroutine(SwingReadyCour(Dir, UpHit));
        }
    }
    private IEnumerator SwingReadyCour(Vector2 Dir, bool UpHit)
    {
        Vector2 GapDir = UpHit ? Vector2.up : Vector2.down;
        Vector2 BackDir = (-Dir + GapDir).normalized;
        Vector2 NowDir = Vector2.zero;
        while (ThisPlayer.DotArm(BackDir) < 0.9f)
        {
            NowDir = Vector2.Lerp(NowDir, BackDir, 0.05f);
            MoveArm(NowDir);
            Movement(Dir * 0.5f);
            yield return new WaitForFixedUpdate();
        }
        SwingCoroutine = null;
        yield break;
    }

    public void Swing(Vector2 Dir, bool UpHit)
    {
        if(SwingCoroutine == null)
        {
            SwingCoroutine = StartCoroutine(SwingCour(Dir, UpHit));
        }
    }
    private IEnumerator SwingCour(Vector2 Dir, bool UpHit)
    {
        Vector2 GapDir = UpHit ? Vector2.up : Vector2.down;
        Vector2 BackDir = (-Dir + GapDir).normalized;
        Vector2 FrwDir = (Dir - GapDir).normalized;
        Vector2 NowDir = Vector2.zero;
        while (ThisPlayer.DotArm(BackDir) < 0.9f)
        {
            NowDir = Vector2.Lerp(NowDir, BackDir, 0.125f);
            MoveArm(NowDir);
            Movement(Dir * 0.5f);
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(Random.Range(0, 0.25f));
        while (ThisPlayer.DotArm(GapDir) < 0.8f)
        {
            NowDir = Vector2.Lerp(NowDir, GapDir, 0.25f);
            MoveArm(NowDir);
            Movement(Dir * 1.25f);
            yield return new WaitForFixedUpdate();
        }
        while (ThisPlayer.DotArm(FrwDir) < 0.95f)
        {
            NowDir = FrwDir;
            MoveArm(NowDir);
            Movement(Dir * 1.5f);
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(Random.Range(0, 0.25f));
        SwingCoroutine = null;
        yield break;
    }

    public void MegaSwing(float Delay)
    {
        if (SwingCoroutine != null)
        {
            StopCoroutine(SwingCoroutine);
        }
        SwingCoroutine = StartCoroutine(MegaSwingCour(Delay));
    }
    private IEnumerator MegaSwingCour(float Delay)
    {
        SuperAction = true;
        yield return new WaitForSeconds(Delay);
        for (int i = 0; i < 5; i++)
        {
            while (ThisPlayer.DotArm(Vector2.right) < 0.9f)
            {
                MoveArm(Vector2.right);
                yield return new WaitForFixedUpdate();
            }
            while (ThisPlayer.DotArm(Vector2.down) < 0.9f)
            {
                MoveArm(Vector2.down);
                yield return new WaitForFixedUpdate();
            }
            while (ThisPlayer.DotArm(Vector2.left) < 0.9f)
            {
                MoveArm(Vector2.left);
                yield return new WaitForFixedUpdate();
            }
            while (ThisPlayer.DotArm(Vector2.up) < 0.9f)
            {
                MoveArm(Vector2.up);
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(Delay);
        SwingCoroutine = null;
        SuperAction = false;
        yield break;
    }


    public void RamFast()
    {
        StopSwing();
        if (RamCoroutine == null)
        {
            RamCoroutine = StartCoroutine(RamFastCour());
        }
    }
    private IEnumerator RamFastCour()
    {
        float time = 0;
        while (Enemy != null && !HaveWeapon() && time < 0.75f)
        {
            MoveArm(ThisPlayer.Direction(Enemy.transform));
            Movement(ThisPlayer.Direction(Enemy.transform) * 2f);
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        RamCoroutine = null;
        yield break;
    }

    public void RamRun()
    {
        StopSwing();
        if (RamCoroutine == null)
        {
            RamCoroutine = StartCoroutine(RamCour());
        }
    }
    private IEnumerator RamCour()
    {
        float time = 0;
        while(Enemy != null && ThisPlayer.DistX(Enemy.transform) < 5 * Mathf.Sqrt(ThisPlayer.Size) && !HaveWeapon() && time < 3)
        {
            time += Time.fixedDeltaTime;
            Movement(-ThisPlayer.Direction(Enemy.transform));
            MoveArm(-ThisPlayer.Direction(Enemy.transform));
            if (Level.active.NextLava(transform, -ThisPlayer.Direction(Enemy.transform) * 5))
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        time = 0;
        while (Enemy != null && time < 3 && !HaveWeapon())
        {
            MoveArm(ThisPlayer.Direction(Enemy.transform));
            Movement(ThisPlayer.Direction(Enemy.transform) * 1.25f);
            yield return new WaitForFixedUpdate();
        }
        RamCoroutine = null;
        yield break;
    }

    public void RandomWalk()
    {
        if(WalkCoroutine == null)
        {
            WalkCoroutine = StartCoroutine(RandomWalkCour());
        }
    }
    private IEnumerator RandomWalkCour()
    {
        if (RamCoroutine != null)
        {
            StopCoroutine(RamCoroutine);
            RamCoroutine = null;
        }
        float x = Random.Range(transform.position.x - 10, transform.position.x + 10);
        while (Level.active.NextLava(x))
        {
            x = Random.Range(transform.position.x - 25, transform.position.x + 25);
            yield return new WaitForFixedUpdate();
        }
        while(Mathf.Abs(transform.position.x - x) > 1 && !CurrantState.isFinished)
        {
            MoveArm(Vector2.up);
            Movement(new Vector2(x > 0 ? 0.25f : -0.25f, 0));
            yield return new WaitForFixedUpdate();
        }
        ThisPlayer.Rig.velocity *= 0.1f;
        WalkCoroutine = null;
        yield break;
    }

    private void StopAllCour()
    {
        if(SwingCoroutine != null)
        {
            StopCoroutine(SwingCoroutine);
            SwingCoroutine = null;
        }
        if(RamCoroutine != null)
        {
            StopCoroutine(RamCoroutine);
            RamCoroutine = null;
        }
        if(WalkCoroutine != null)
        {
            StopCoroutine(WalkCoroutine);
            WalkCoroutine = null;
        }
    }

    private void SetState(State state)
    {
        StopAllCour();

        CurrantState = Instantiate(state);
        CurrantState.aiController = this;
        CurrantState.Init();
    }
    private void SelectState()
    {
        if (ThisPlayer == null)
            return;
        if(!CurrantState.isFinished)
        {
            if(GameData.GameStarted)
                CurrantState.Run();
        }
        else
        {
            if (ThisPlayer.Dead)
            {
                SetState(RunAway);
            }
            else if(ThisPlayer.Static)
            {
                SetState(Idle);
            }
            else if(HaveEnemy() && !Level.active.NextLava(Enemy))
            {
                if (ThisPlayer.weapon?.WeaponType == Weapon.Type.Gun)
                {
                    SetState(GunAttack);
                }
                else
                {
                    switch (ThisPlayer.Type)
                    {
                        case Man.ManType.Boss:
                            SetState(BossAttack);
                            break;
                        case Man.ManType.Duel:
                            SetState(DuelAttack);
                            break;
                        case Man.ManType.Enemy:
                            SetState(Attack);
                            break;
                        case Man.ManType.KnightEnemy:
                            SetState(DuelAttack);
                            break;
                        case Man.ManType.Menu:
                            SetState(DuelAttack);
                            break;
                        case Man.ManType.Player:
                            SetState(Attack);
                            break;
                    }
                }
            }
            else
            {
                SetState(Idle);
            }
        }
        
    }

    private void Awake()
    {
        ThisPlayer?.SetController(this);
    }
    private void Start()
    {
        SetState(Idle);
    }
    private void FixedUpdate()
    {
        SelectState();
    }

}