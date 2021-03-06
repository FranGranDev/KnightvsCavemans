using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    public static Level active;
    public Man MainPlayer;
    private Player PlayerScript;
    private int NowArmor;
    private int TempWeapon;
    public enum GameType { Levels, Bets, Waves, Knights}
    private bool Loaded;
    private bool SaveLifeUsed;
    private Man.HitType PlayerDeath;

    public static bool OnPause;

    [Header("Level Settings")]
    public int NowGameType;
    public LevelTypes LevelType;
    public enum LevelTypes { Menu, Idle, Learn, Duel, Levels, Boss, Nude, Battle, KnightBattle, Waves};
    public enum EnemyCreateType {Nude, Bets, Random, Similar, Duel, NoBrain, Waves, Friend, SimilarFriend, BattleEnemy, KnightEnemy, Boss}
    public bool InGame()
    {
        return Ui_Game.activeSelf && !Ui_Pause.activeSelf;
    }
    public bool CanSwipe(Vector2 Pos)
    {
        return Pos.y > 300 && !Ui_Game.activeSelf && !Ui_Amunition.activeSelf && !Ui_Settings.activeSelf && !Ui_Store.activeSelf && !Ui_LevelUp.activeSelf;
    }
    public bool NoInternet()
    {
        return Application.internetReachability == NetworkReachability.NotReachable;
    }

    [Header("Enemys")]
    public List<Man> AllEnemy;
    public List<Man> AliveEnemy;
    public List<Man> DefeatedEnemy;
    public List<Man> BattleEnemy;
    public Man BossEnemy;
    public Man LastOfMan;
    private Man PrevHit;
    private int HitInRow;
    [Header("Friends")]
    public List<Man> AllFriends;
    public List<Man> AliveFriends;
    public List<Man> DefeatedFriends;
    public List<Man> BattleFriends;

    [Header("Bet's")]
    private int BetForNum;
    private int BetMoney;

    [Header("States")]
    public GameState MenuState;
    public GameState BetsState;
    public GameState EasyState;
    public GameState LevelState;
    public GameState DuelState;
    public GameState BattleState;
    public GameState BossState;
    public GameState WavesState;
    public GameState KnightsState;
    public GameState DragonState;
    public GameState FreeState;
    public GameState LearnState;
    public GameState IdleBattle;
    [Header("Currant State")]
    public GameState CurrantState;
    [Header("Components")]
    public Animator anim;
    public TextMeshProUGUI GameText;
    public Transform PlayerMenuPos;
    public Transform CameraMenuPos;
    public SceneMaker sceneMaker;
    public CameraMove cameraMove;
    public AudioSource musicSource;
    public AudioSource effectSource;
    //--------Coroutines----------
    private Coroutine LevelFailedCoroutine;
    private Coroutine SelectStateCoroutine;
    private Coroutine DelayGameCoroutine;
    private Coroutine BoxSpawnCoroutine;
    private Coroutine BuffSpawnCoroutine;
    private Coroutine MeteorSpawnCoroutine;
    private Coroutine PrintTextCoroutine;
    private Coroutine NextLearnStateCoroutine;
    private Coroutine GoAmunitionCoroutine;
    private Coroutine ExperienceUpdateCoroutine;
    private Coroutine MoneyUpdateCoroutine;
    private Coroutine SetGameTypeCoroutine;
    private Coroutine NoLevelCoroutine;
    private Coroutine NoMoneyCoroutine;
    private Coroutine HitInRowCoroutine;
    private Coroutine TimeStopCoroutine;
    private Coroutine MaskCoroutine;
    [Header("UI Game")]
    public GameObject Ui_Game;
    public Image Ui_Game_BackGround;
    public GameObject Ui_Game_Ammo;
    public TextMeshProUGUI Ui_Game_AmmoText;
    public Bar GameHp;
    public RectTransform Ui_Game_HitRow;
    public TextMeshProUGUI Ui_Game_HitRowText;
    public TextMeshProUGUI Ui_Game_Skip;
    public TextMeshProUGUI Ui_Game_Combo;
    public TextMeshProUGUI Ui_Game_SaveLifeNo;
    public TextMeshProUGUI Ui_Game_BossHp;

    [Header("UI Stats")]
    public Bar Ui_Game_LevelBar;
    public TextMeshProUGUI Ui_Game_LevelBarText;
    public TextMeshProUGUI Ui_Game_LevelNum;
    public TextMeshProUGUI Ui_Game_Money;

    public GameObject Ui_LevelUp;
    public GameObject Ui_LevelUp_Close;
    public TextMeshProUGUI Ui_LevelUp_Text;
    public TextMeshProUGUI Ui_LevelUp_CloseText;
    public TextMeshProUGUI Ui_LevelUp_From;
    public TextMeshProUGUI Ui_LevelUp_To;
    public TextMeshProUGUI Ui_LevelUp_Money;
    public GameObject Ui_LevelUp_WeaponUnlock;
    public GameObject Ui_LevelUp_WeaponUnlockContent;
    private WeaponIcon[] Ui_LevelUp_WeaponIcons;
    public TextMeshProUGUI Ui_LevelUp_WeaponUnlockText;

    [Header("UI Menu")]
    public GameObject Ui_Menu;
    public TextMeshProUGUI Ui_Menu_Name;
    public TextMeshProUGUI Ui_Menu_PlayText;
    public TextMeshProUGUI Ui_Menu_Amunition;
    public TextMeshProUGUI Ui_Menu_Settings;
    public GameObject Ui_Menu_Present;
    public TextMeshProUGUI Ui_Menu_Timer;
    public GameObject Ui_Menu_PresentTake;
    public TextMeshProUGUI Ui_Menu_PresentText;
    public TextMeshProUGUI Ui_Menu_PresentGotText;
    public TextMeshProUGUI Ui_Menu_PresentMoneyText;
    public GameObject Ui_Menu_PresentWeapon;
    public GameObject Ui_Menu_PresentWeaponContent;
    public TextMeshProUGUI Ui_Menu_PresentWeaponText;

    [Header("UI Bets")]
    public GameObject Ui_Bets;
    public Slider Ui_Bets_Slider;
    public TextMeshProUGUI Ui_Bets_Name;
    public TextMeshProUGUI Ui_Bets_MoneySlider;
    public TextMeshProUGUI Ui_Bets_MoneyGetText;
    public TextMeshProUGUI Ui_Bets_Left;
    public TextMeshProUGUI Ui_Bets_Right;
    public GameObject Ui_BetsWin;
    public TextMeshProUGUI Ui_BetsWin_Text;
    public TextMeshProUGUI Ui_BetsWin_Money;
    public TextMeshProUGUI Ui_BetsWin_Close;
    public GameObject Ui_BetsLose;
    public TextMeshProUGUI Ui_BetsLose_Text;
    public TextMeshProUGUI Ui_BetsLose_Money;
    public TextMeshProUGUI Ui_BetsLose_Close;
    

    [Header("UI Pause")]
    public GameObject Ui_Pause;
    public Slider Ui_Pause_Volume;
    public Slider Ui_Pause_Music;
    public TextMeshProUGUI Ui_Pause_ResumeText;
    public TextMeshProUGUI Ui_Pause_RestartText;
    public TextMeshProUGUI Ui_Pause_VolumeText;
    public TextMeshProUGUI Ui_Pause_MusicText;
    public TextMeshProUGUI Ui_Pause_MainMenuText;

    [Header("UI Settings")]
    public GameObject Ui_Settings;
    public Slider Ui_Settings_Volume;
    public Slider Ui_Settings_Music;
    public Toggle Ui_Setting_Vibration;
    public TextMeshProUGUI Ui_Settings_ResumeText;
    public TextMeshProUGUI Ui_Settings_VolumeText;
    public TextMeshProUGUI Ui_Settings_MusicText;
    public TextMeshProUGUI Ui_Settings_Vibration;


    [Header("UI Premium Store")]
    public GameObject Ui_Store;
    public Transform Ui_Store_Weapon;
    public GameObject Ui_Store_Thanks;
    public TextMeshProUGUI Ui_Store_Name;
    public TextMeshProUGUI Ui_Store_WhatGet;
    public TextMeshProUGUI Ui_Store_NoAds;
    public TextMeshProUGUI Ui_Store_CoolWeapon;
    public TextMeshProUGUI Ui_Store_Skin;
    public TextMeshProUGUI Ui_Store_GameMode;
    public TextMeshProUGUI Ui_Store_x2Exp;
    public TextMeshProUGUI Ui_Store_Coins;
    public TextMeshProUGUI Ui_Store_Buy;
    public TextMeshProUGUI Ui_Store_NoThanks;
    public TextMeshProUGUI Ui_Store_Restore;
    public TextMeshProUGUI Ui_Store_ThanksFor;

    public GameObject Ui_StoreOnButton;
    public GameObject Ui_StoreOffButton;
    public GameObject Ui_StoreAlready;
    public TextMeshProUGUI Ui_StoreAlready_Name;
    public TextMeshProUGUI Ui_StoreAlready_WhatGot;
    public TextMeshProUGUI Ui_StoreAlready_NoAds;
    public TextMeshProUGUI Ui_StoreAlready_Weapon;
    public TextMeshProUGUI Ui_StoreAlready_Skin;
    public TextMeshProUGUI Ui_StoreAlready_GameMode;
    public TextMeshProUGUI Ui_StoreAlready_x2Exp;
    public TextMeshProUGUI Ui_StoreAlready_Coins;

    [Header("UI Waves")]
    public GameObject Ui_Waves;
    public TextMeshProUGUI Ui_Waves_Name;
    public TextMeshProUGUI Ui_Waves_Play;
    public TextMeshProUGUI Ui_Waves_Max;
    public TextMeshProUGUI Ui_Waves_Now;

    [Header("UI Knights")]
    public GameObject Ui_Knights;
    public TextMeshProUGUI Ui_Knights_Max;
    public TextMeshProUGUI Ui_Knights_Play;
    public TextMeshProUGUI Ui_Knights_Name;
    public TextMeshProUGUI Ui_Knights_Now;

    [Header("UI Amunition")]
    public GameObject Ui_Amunition;
    public RectTransform Ui_Anum_ScrollWeapon;

    private WeaponIcon[] Ui_Anum_WeaponIcons;
    public Bar Ui_Anum_HpBar;
    public TextMeshProUGUI Ui_Anum_HpLevel;
    public TextMeshProUGUI Ui_Anum_HpCost;
    public Bar Ui_Anum_SpeedBar;
    public TextMeshProUGUI Ui_Anum_SpeedLevel;
    public TextMeshProUGUI Ui_Anum_SpeedCost;
    public Bar Ui_Anum_SizeBar;
    public TextMeshProUGUI Ui_Anum_SizeLevel;
    public TextMeshProUGUI Ui_Anum_SizeCost;
    public Bar Ui_Anum_PowerBar;
    public TextMeshProUGUI Ui_Anum_PowerLevel;
    public TextMeshProUGUI Ui_Anum_PowerCost;

    public TextMeshProUGUI Ui_Anum_ArmorName;
    public Button Ui_Anum_ArmorButton;
    public Image Ui_Anum_ArmorButtonColor;
    public GameObject Ui_Anum_ArmorCosts;
    public TextMeshProUGUI Ui_Anum_ArmorExpText;
    public TextMeshProUGUI Ui_Anum_ArmorCostText;

    public TextMeshProUGUI Ui_Anum_BackText;
    public TextMeshProUGUI Ui_Anum_StartWeaponText;

    [Header("UI Learn")]
    public GameObject Ui_SkipLearn;

    public float GetRandX(Transform Pos, float Rand)
    {
        if(Pos.position.x + Rand > sceneMaker.SizeX)
        {
            return Pos.position.x - Random.Range(0, Rand);
        }
        else if(Pos.position.x - Rand < sceneMaker.SizeX)
        {
            return Pos.position.x + Random.Range(0, Rand);
        }
        else
        {
            return Pos.position.x + Random.Range(-Rand, Rand);
        }
    }
    public bool NextLava(Transform Pos, Vector2 Dir)
    {
        return Mathf.Abs(Pos.position.x + Dir.x) > sceneMaker.SizeX;
    }
    public bool NextLava(Man man)
    {
        if (man == null)
            return false;
        return Mathf.Abs(man.transform.position.x) > sceneMaker.SizeX;
    }
    public bool NextLava(float x)
    {
        return Mathf.Abs(x) > sceneMaker.SizeX;
    }

    #region Player
    //-------------------------------Player---------------------------------------

    public void UpdatePlayerExperience(int Up)
    {
        GameData.UpdateExperience(Up);

        if (ExperienceUpdateCoroutine != null)
        {
            StopCoroutine(ExperienceUpdateCoroutine);
        }
        ExperienceUpdateCoroutine = StartCoroutine(ExperienceUpdateCour());
    }
    private IEnumerator ExperienceUpdateCour()
    {
        float Target = (float)GameData.PlayerExperience / (float)GameData.NextLevelExperience();
        float Now = Ui_Game_LevelBar.Value();
        while (Mathf.Abs(Now - Target) < 0.1f)
        {
            Now = Mathf.Lerp(Now, Target, 0.1f);
            Ui_Game_LevelBar.FillArea(Now);
            yield return new WaitForFixedUpdate();
        }

        Ui_Game_LevelBar.FillArea(Target);
        Ui_Game_LevelNum.text = GameData.PlayerLevel.ToString();
        Ui_Game_LevelBarText.text = GameData.PlayerExperience.ToString() + "/" + GameData.NextLevelExperience().ToString();
        yield break;
    }
    public void OnLevelUp()
    {

    }

    public void UpdateMoney(int Up)
    {
        GameData.UpdateMoney(Up);

        if (MoneyUpdateCoroutine != null)
        {
            StopCoroutine(MoneyUpdateCoroutine);
        }
        MoneyUpdateCoroutine = StartCoroutine(UpdateMoneyCour(Up));
        GameData.Save();
    }
    private IEnumerator UpdateMoneyCour(int Up)
    {
        int StartMoney = GameData.Money - Up;
        for (int i = StartMoney; i < StartMoney + Up; i += 7)
        {
            Ui_Game_Money.text = i.ToString();
            yield return new WaitForFixedUpdate();
        }
        Ui_Game_Money.text = GameData.Money.ToString();
        yield break;
    }

    private void UpdatePlayerStatsUI()
    {
        Ui_Game_LevelNum.text = GameData.PlayerLevel.ToString();
        float Target = (float)GameData.PlayerExperience / (float)GameData.NextLevelExperience();
        Ui_Game_LevelBar.FillArea(Target);
        Ui_Game_LevelBarText.text = GameData.PlayerExperience.ToString() + "/" + GameData.NextLevelExperience().ToString();
        Ui_Game_Money.text = GameData.Money.ToString();
    }
    #endregion
    #region UI in Game
    //--------------------------------UI------------------------------------------
    public void PrintText(string text)
    {
        if(PrintTextCoroutine != null)
        {
            StopCoroutine(PrintTextCoroutine);
        }
        anim.Play("Main_PrintIdle");
        GameText.text = text;
    }
    public void ClearText()
    {
        if (PrintTextCoroutine != null)
        {
            StopCoroutine(PrintTextCoroutine);
        }
        GameText.text = "";
    }
    public void PrintText(string text, float Delay)
    {
        if(PrintTextCoroutine != null)
        {
            StopCoroutine(PrintTextCoroutine);
        }
        PrintTextCoroutine = StartCoroutine(PrintTextCour(text, Delay));
    }
    public void PrintDamageText()
    {
        if (PrintTextCoroutine == null)
        {
            PrintText(GameData.active.DamagePhase[Random.Range(0, GameData.active.DamagePhase.Length)], 1);
        }
    }
    public void PrintAttackPhase()
    {
        if (PrintTextCoroutine == null)
        {
            PrintText(GameData.active.AttackPhase[Random.Range(0, GameData.active.AttackPhase.Length)], 1);
        }
    }
    private IEnumerator PrintTextCour(string text, float Delay)
    {
        anim.Play("Main_PrintStart");
        GameText.text = text;
        yield return new WaitForSeconds(Delay);
        anim.SetTrigger("PrintEnd");
        yield return new WaitForSeconds(0.25f);
        GameText.text = "";
        PrintTextCoroutine = null;
        yield break;
    }

    private void SetHitinRow(int num)
    {
        HitInRow = num;
        int NumForMega = 7;
        switch(LevelType)
        {
            case LevelTypes.Battle:
                NumForMega = 14;
                break;
            case LevelTypes.Boss:
                NumForMega = 5;
                break;
            case LevelTypes.KnightBattle:
                NumForMega = 15;
                break;
            case LevelTypes.Nude:
                NumForMega = 10;
                break;
            case LevelTypes.Waves:
                NumForMega = 8;
                break;
        }
        if(num == 0)
        {
            anim.Play("ComboIdle", 3);
            Ui_Game_HitRow.gameObject.SetActive(false);
        }
        else if(num >= NumForMega)
        {
            anim.Play("ComboMega", 3);
            Ui_Game_HitRow.gameObject.SetActive(true);
            int Right = Random.Range(0, 2) == 0 ? 1 : -1;
            Ui_Game_HitRow.anchoredPosition = new Vector3(Right * Random.Range(400, 350), Random.Range(300, 400), 0);
            Ui_Game_HitRowText.text = Language.Lang.gameText.MegaCombo + "x " + num.ToString();
        }
        else
        {
            anim.Play("ComboNew", 3);
            Ui_Game_HitRow.gameObject.SetActive(true);
            int Right = Random.Range(0, 2) == 0 ? 1 : -1;
            Ui_Game_HitRow.anchoredPosition = new Vector3(Right * Random.Range(400, 350), Random.Range(300, 400), 0);
            Ui_Game_HitRowText.text = Language.Lang.gameText.Combo + "x " + num.ToString();
        }
    }

    public void Learning(bool on)
    {
        if (on)
        {
            Ui_SkipLearn.SetActive(true);
        }
        else
        {
            Ui_SkipLearn.SetActive(false);
            LearningStopShow();
        }
    }
    public void LearningStartShow(string name)
    {
        StartCoroutine(LearningShow(name));
    }
    public void LearningStopShow()
    {
        anim.Play("Learn_Idle");
    }
    private IEnumerator LearningShow(string name)
    {
        anim.enabled = true;
        anim.Play(name);
        while(anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return new WaitForFixedUpdate();
        }
        anim.Play("Learn_Idle");
        yield return new WaitForFixedUpdate();
        yield break;
    }
    #endregion
    #region Game
    public void GameStuff()
    {
        GameHp.FillArea((float)MainPlayer.Hp / (float)MainPlayer.MaxHp);
    }
    public void TimeStopEffect(float Power, float time)
    {
        if (TimeStopCoroutine == null)
        {
            TimeStopCoroutine = StartCoroutine(TimeStopEffectCour(Power, time));
        }
    }
    private IEnumerator TimeStopEffectCour(float Power, float time)
    {
        while(Time.timeScale > Power + 0.01f)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, Power, 0.05f);
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        Time.timeScale = Power;
        yield return new WaitForSecondsRealtime(time);
        while (Time.timeScale < 0.95f)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1, 0.05f);
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        Time.timeScale = 1;
        TimeStopCoroutine = null;
        yield break;
    }

    public List<Man> CreateEnemy(EnemyCreateType type, int count, Vector2[] Pos, float Amplitude)
    {
        List<Man> Enemys = new List<Man>();
        for(int i = 0; i < count; i++)
        {
            Vector2 Position = Pos[Random.Range(0, 2)] + new Vector2(Random.Range(-Amplitude / 2, Amplitude / 2), -0.5f);
            switch (type)
            {
                case EnemyCreateType.Nude:
                    Enemys.Add(CreateSimpleEnemy(Position));
                    break;
                case EnemyCreateType.NoBrain:
                    Enemys.Add(CreateNoBrainEnemy(Position));
                    break;
                case EnemyCreateType.Random:
                    Enemys.Add(CreateLevelEnemy(Position, Random.Range(0.65f, 1.25f)));
                    break;
                case EnemyCreateType.Similar:
                    Enemys.Add(CreateLevelEnemy(Position, 0.75f));
                    break;
                case EnemyCreateType.Duel:
                    Position = sceneMaker.GetRandomEnemyPos();
                    Enemys.Add(CreateDuelEnemy(Position, Random.Range(0.9f, 1.35f)));
                    break;
                case EnemyCreateType.Waves:
                    Position = sceneMaker.GetRandomEnemyPos();
                    Enemys.Add(CreateWavesEnemy(Position, Random.Range(0.75f, 1.25f)));
                    break;
                case EnemyCreateType.Bets:
                    Vector2 BetsPos = i == 0 ? new Vector2(Random.Range(-5, -2), 0) : new Vector2(Random.Range(2, 5), 0);
                    Enemys.Add(CreateBetsEnemy(BetsPos, Random.Range(0.9f, 1.1f), i));
                    break;
                case EnemyCreateType.Friend:
                    Position = Pos[0] + new Vector2(Random.Range(-5f, 5f), -0.5f);
                    Enemys.Add(CreateFriend(Position, Random.Range(0.9f, 1.1f)));
                    break;
                case EnemyCreateType.SimilarFriend:
                    Position = Pos[0] + new Vector2(Random.Range(-5f, 5f), -0.5f);
                    Enemys.Add(CreateSimilarFriend(Position, Random.Range(0.9f, 1.1f)));
                    break;
                case EnemyCreateType.BattleEnemy:
                    Position = Pos[1] + new Vector2(Random.Range(-5f, 5f), -0.5f);
                    Enemys.Add(CreateBattleEnemy(Position, Random.Range(1f, 2f)));
                    break;
                case EnemyCreateType.KnightEnemy:
                    Position = Pos[1] + new Vector2(Random.Range(-5f, 5f), -0.5f);
                    Enemys.Add(CreateKnightEnemy(Position, Random.Range(0.75f, 1.25f)));
                    break;
            }

        }
        return Enemys;
    }
    public void CreateBossHelpEnemy(int count)
    {
        if (BossEnemy == null)
            return;
        for (int i = 0; i < count; i++)
        {
            Vector2 Position = BossEnemy.transform.position + new Vector3(Random.Range(-5f, 5f), 25, 0);
            Man man = CreateLevelEnemy(Position, Random.Range(0.75f, 1f));
            man.Type = Man.ManType.Enemy;
            AllEnemy.Add(man);
            AliveEnemy.Add(man);
        }
    }
    public Man CreateBetsEnemy(Vector2 Position, float Power, int index)
    {
        Man man = Instantiate(GameData.active.Enemy[0].Enemy, Position, Quaternion.identity, null).GetComponent<Man>();
        man.MaxHp = Mathf.RoundToInt(15 * Power);
        man.Power = Power;
        man.Size = Power;
        man.name = index.ToString();
        man.Speed = 12 * Power;
        man.Type = Man.ManType.Bets;
        man.Experience = 0;
        man.ForceFlip(index == 0);
        man.SetParams(GameData.active.GetManArmor());
        AiController controller = man.GetComponent<AiController>();
        controller.ViewLenght = 25f * Power;
        Weapon weapon = GameData.active.GetRandomWeapon();
        if (weapon != null)
        {
            weapon.transform.up = ((index == 0 ? Vector2.right : Vector2.left) + Vector2.up).normalized;
            man.TakeWeapon(Instantiate(weapon));
        }
        return man;
    }
    public Man CreateSimpleEnemy(Vector2 Position)
    {
        Man man = Instantiate(GameData.active.Enemy[0].Enemy, Position, Quaternion.identity, null).GetComponent<Man>();
        man.Type = Man.ManType.Enemy;
        man.Experience = 1;
        man.Power = 1;
        man.SetParams(GameData.active.GetManArmor());
        return man;
    }
    public Man CreateNoBrainEnemy(Vector2 Position)
    {
        Man man = Instantiate(GameData.active.Enemy[0].Enemy, Position, Quaternion.identity, null).GetComponent<Man>();
        man.Static = true;
        man.Type = Man.ManType.Player;
        man.Power = 1;
        man.SetParams(GameData.active.GetManArmor());
        return man;
    }
    public Man CreateSpecialEnemy(Vector2 Position, ManInfo info)
    {
        Man man = Instantiate(GameData.active.Enemy[0].Enemy, Position, Quaternion.identity, null).GetComponent<Man>();
        man.MaxHp = info.MaxHp;
        man.Size = info.Size;
        man.name = info.name;
        man.Speed = info.Speed;
        man.Type = info.Type;
        man.Experience = info.Exp;
        man.Power = 1;
        man.SetParams(GameData.active.GetManArmor());
        AiController controller = man.GetComponent<AiController>();
        controller.ViewLenght = info.ViewLength;
        if(info.weapon != null)
        {
            man.TakeWeapon(Instantiate(info.weapon));
        }
        return man;
    }
    public Man CreateLevelEnemy(Vector2 Position, float Power)
    {
        Man man = Instantiate(GameData.active.Enemy[0].Enemy, Position, Quaternion.identity, null).GetComponent<Man>();
        man.MaxHp = Mathf.RoundToInt(MainPlayer.MaxHp * Power * 0.5f);
        man.Size = 1 * Random.Range(0.75f, 1.25f);
        man.name = "Cave Man";
        man.Speed = MainPlayer.Speed * Power;
        man.Power = Power;
        man.Type = Man.ManType.Enemy;
        man.Experience = 1;
        man.SetParams(GameData.active.GetManArmor());
        AiController controller = man.GetComponent<AiController>();
        controller.ViewLenght = 15f * Power;
        Weapon weapon = Random.Range(0, 3) != 0 ? GameData.active.GetRandomWeapon() : null;
        if (weapon != null)
        {
            man.TakeWeapon(Instantiate(weapon));
        }
        return man;
    }
    public Man CreateDuelEnemy(Vector2 Position, float Power)
    {
        Man man = Instantiate(GameData.active.Enemy[0].Enemy, Position, Quaternion.identity, null).GetComponent<Man>();
        man.MaxHp = Mathf.RoundToInt(MainPlayer.MaxHp * Power * 0.5f);
        man.Size = MainPlayer.Size * Power;
        man.name = "Cave Man";
        man.Speed = MainPlayer.Speed * Power;
        man.Type = Man.ManType.Duel;
        man.Experience = 3;
        man.Power = Power;
        man.SetParams(GameData.active.GetManArmor());
        AiController controller = man.GetComponent<AiController>();
        controller.ViewLenght = 25f * Power;
        Weapon weapon = GameData.active.GetRandomWeapon();
        if (weapon != null)
        {
            man.TakeWeapon(Instantiate(weapon));
        }
        return man;
    }
    public Man CreateWavesEnemy(Vector2 Position, float Power)
    {
        Man man = Instantiate(GameData.active.Enemy[0].Enemy, Position, Quaternion.identity, null).GetComponent<Man>();
        man.MaxHp = Mathf.RoundToInt(MainPlayer.MaxHp * Power * 0.5f);
        man.Size = Mathf.Sqrt(MainPlayer.Size) * Random.Range(0.75f, 1.25f);
        man.name = "Cave Man";
        man.Speed = MainPlayer.Speed * Power;
        man.Type = Man.ManType.Enemy;
        man.Experience = 0;
        man.Money = 40;
        man.Power = Power * 0.75f;
        man.SetParams(GameData.active.GetManArmor());
        AiController controller = man.GetComponent<AiController>();
        controller.ViewLenght = 25f * Power;
        Weapon weapon = GameData.active.GetRandomWeapon();
        if (weapon != null)
        {
            man.TakeWeapon(Instantiate(weapon));
        }
        return man;
    }
    public Man CreateManBossEnemy(Vector2[] Pos, float Power)
    {
        Man man = Instantiate(GameData.active.Enemy[1].Enemy, Pos[0], Quaternion.identity, null).GetComponent<Man>();
        man.MaxHp = Mathf.RoundToInt(MainPlayer.MaxHp * Power * 2f);
        man.Size = MainPlayer.Size * Power * 2;
        man.name = "Boss";
        man.Speed = MainPlayer.Speed * Power * 0.75f;
        man.Type = Man.ManType.Boss;
        man.Experience = Mathf.RoundToInt(25 * Power);
        man.Power = Power;
        man.Money = Mathf.RoundToInt(Mathf.Sqrt(GameData.NowLevel + 1) * 50);
        man.SetParams(GameData.active.GetManArmor());
        AiController controller = man.GetComponent<AiController>();
        controller.ViewLenght = 50f * Power;
        Weapon weapon = GameData.active.GetRandomWeapon();
        if (weapon != null)
        {
            man.TakeWeapon(Instantiate(weapon));
        }
        return man;
    }
    public Man CreateFriend(Vector2 Position, float Power)
    {
        Man man = Instantiate(GameData.active.Friend[0].Enemy, Position, Quaternion.identity, null).GetComponent<Man>();
        man.MaxHp = Mathf.RoundToInt(MainPlayer.MaxHp * Power);
        man.Size = MainPlayer.Size * Random.Range(0.9f, 1.25f);
        man.name = "Knight Friend";
        man.Speed = MainPlayer.Speed * Power;
        man.Type = Man.ManType.Player;
        man.Experience = 0;
        man.Power = MainPlayer.Power * Power;
        man.SetParams(GameData.active.playerInfo, GameData.active.GetRandomArmor());
        AiController controller = man.GetComponent<AiController>();
        controller.ViewLenght = 25f * Power;
        Weapon weapon = GameData.active.GetRandomWeapon();
        if (weapon != null)
        {
            man.TakeWeapon(Instantiate(weapon));
        }
        return man;
    }
    public Man CreateSimilarFriend(Vector2 Position, float Power)
    {
        Man man = Instantiate(GameData.active.Friend[0].Enemy, Position, Quaternion.identity, null).GetComponent<Man>();

        man.MaxHp = Mathf.RoundToInt(MainPlayer.MaxHp * Power);
        man.Size = MainPlayer.Size * Random.Range(0.9f, 1.1f);
        man.name = "Knight Friend";
        man.Speed = MainPlayer.Speed * Power;
        man.Type = Man.ManType.Player;
        man.Experience = 0;
        man.Power = MainPlayer.Power * Power;
        man.SetParams(GameData.active.playerInfo, GameData.active.armorInfo);
        AiController controller = man.GetComponent<AiController>();
        controller.ViewLenght = 25f * Power;
        Weapon weapon = GameData.active.GetRandomWeapon();
        if (weapon != null)
        {
            man.TakeWeapon(Instantiate(weapon));
        }
        return man;
    }
    public Man CreateBattleEnemy(Vector2 Position, float Power)
    {
        Man man = Instantiate(GameData.active.Enemy[0].Enemy, Position, Quaternion.identity, null).GetComponent<Man>();
        man.MaxHp = Mathf.RoundToInt(MainPlayer.MaxHp * Power * 0.5f);
        man.Size = MainPlayer.Size * Random.Range(0.75f, 1.5f);
        man.name = "Cave Man";
        man.Speed = MainPlayer.Speed * Power;
        man.Type = Man.ManType.Enemy;
        man.Experience = 3;
        man.Power = Random.Range(1, 2);
        man.SetParams(GameData.active.GetManArmor());
        AiController controller = man.GetComponent<AiController>();
        controller.ViewLenght = 25f * Power;
        Weapon weapon = GameData.active.GetRandomWeapon();
        if (weapon != null)
        {
            man.TakeWeapon(Instantiate(weapon));
        }
        return man;
    }
    public Man CreateKnightEnemy(Vector2 Position, float Power)
    {
        Man man = Instantiate(GameData.active.Friend[0].Enemy, Position, Quaternion.identity, null).GetComponent<Man>();
        man.MaxHp = Mathf.RoundToInt(MainPlayer.MaxHp * Power);
        man.Size = MainPlayer.Size * Random.Range(0.9f, 1.25f);
        man.name = "Knight Enemy";
        man.Speed = MainPlayer.Speed * Power;
        man.Type = Man.ManType.KnightEnemy;
        man.Experience = 3;
        man.Power = MainPlayer.Power * Power;
        man.SetParams(GameData.active.playerInfo, GameData.active.GetEnemyArmor());
        AiController controller = man.GetComponent<AiController>();
        controller.ViewLenght = 25f * Power;
        Weapon weapon = GameData.active.GetRandomWeapon();
        if (weapon != null)
        {
            man.TakeWeapon(Instantiate(weapon));
        }
        return man;
    }

    public void CreateExperience(Man man, float ExpRatio)
    {
        for (int i = 0; i < Mathf.RoundToInt(man.Experience * ExpRatio * (GameData.ExpRatio + 1)); i++)
        {
            Experience.CreateExp(man.transform.position, 1);
        }
    }
    public void CreateExperience(int exp)
    {
        for (int i = 0; i < exp * (GameData.ExpRatio + 1); i++)
        {
            Vector2 Pos = MainPlayer.transform.position + new Vector3(Random.Range(-5, 5), Random.Range(0, 5), 0);
            Experience.CreateExp(Pos, 1);
        }
    }
    public void CreateCoin(Man man)
    {
        if (man.Money == 0)
            return;
        Coin.CreateCoin(man.transform.position, man.Money);
    }
    public void CreateCoin(int num, int cost)
    {
        for(int i = 0; i < num; i++)
        {
            Vector2 Pos = MainPlayer.transform.position + new Vector3(Random.Range(-5f, 5f), 0, 0);
            Coin.CreateCoin(Pos, cost);
        }
    }


    public void OnLevelStart()
    {
        Controller.active.enabled = InGame();
        anim.Play("MaskIdle");
        bool printText = false;
        switch(LevelType)
        {
            case LevelTypes.Idle:
                ShowBanner();
                break;
            case LevelTypes.Menu:
                ShowBanner();
                break;
            case LevelTypes.Learn:
                HideBanner();
                break;
            case LevelTypes.KnightBattle:
                HideBanner();
                break;
            case LevelTypes.Levels:
                HideBanner();
                if (GameData.ShowVideo())
                {
                    PlayAds(AdMob.AdsTypes.Video);
                }
                printText = true;
                break;
            case LevelTypes.Nude:
                HideBanner();
                if (GameData.ShowVideo())
                {
                    PlayAds(AdMob.AdsTypes.Video);
                }
                printText = true;
                break;
            case LevelTypes.Boss:
                HideBanner();
                if (GameData.ShowVideo())
                {
                    PlayAds(AdMob.AdsTypes.Video);
                }
                printText = true;
                break;
            case LevelTypes.Battle:
                HideBanner();
                if (GameData.ShowVideo())
                {
                    PlayAds(AdMob.AdsTypes.Video);
                }
                printText = true;
                break;
            case LevelTypes.Duel:
                HideBanner();
                if (GameData.ShowVideo())
                {
                    PlayAds(AdMob.AdsTypes.Video);
                }
                printText = true;
                break;
            case LevelTypes.Waves:
                HideBanner();
                if (GameData.ShowVideo())
                {
                    PlayAds(AdMob.AdsTypes.Video);
                }
                break;
        }
        if (printText)
        {
            PrintText(Language.Lang.gameTypeText.Name[(int)LevelType] + " " + Language.Lang.basicText.Level + ": " + GameData.NowLevel, 3f);
        }
    }
    public void OnLevelDone(LevelTypes type)
    {
        int exp = 0;
        string text = Language.Lang.gameTypeText.WinText[Random.Range(0, Language.Lang.gameTypeText.WinText.Length)];
        switch (type)
        {
            case LevelTypes.Boss:
                exp = 10;
                break;
            case LevelTypes.Duel:
                exp = 5;
                break;
            case LevelTypes.Levels:
                exp = 3;
                break;
            case LevelTypes.Nude:
                exp = 5;
                break;
        }
        CreateExperience(exp);

        anim.Play("SaveLifeIdle");
        if (LevelFailedCoroutine != null)
        {
            StopCoroutine(LevelFailedCoroutine);
            LevelFailedCoroutine = null;
        }

        GameData.active.DecreaseAttempt();
        cameraMove.TurnAiFollow(LastOfMan, true);
        GameData.IncreaseNowLevel();
        PlayMask(GameData.LevelDelay);
        PrintText(text, 1.5f);
        GameData.Save();

        PlaySound("LevelDone", GameData.LevelDelay);
        Vibration.WinVibrate(GameData.LevelDelay * 0.75f);
    }
    public void OnLevelFailed(LevelTypes type)
    {
        if(LevelFailedCoroutine != null)
        {
            StopCoroutine(LevelFailedCoroutine);
            LevelFailedCoroutine = null;
        }
        LevelFailedCoroutine = StartCoroutine(OnLevelFailedCour(type));
    }
    private IEnumerator OnLevelFailedCour(LevelTypes type)
    {
        string text = Language.Lang.gameTypeText.LoseText[Random.Range(0, Language.Lang.gameTypeText.LoseText.Length)];
        PrintText(text, 3f);
        cameraMove.TurnFailedShow();
        if (!SaveLifeUsed)
        {
            yield return new WaitForSeconds(1f);
            TurnSaveLife();
        }
        yield return new WaitForSeconds(3);

        LevelFailed(type);

        LevelFailedCoroutine = null;
        yield break;
    }
    private void LevelFailed(LevelTypes type)
    {
        switch (type)
        {
            case LevelTypes.Boss:
                break;
            case LevelTypes.Duel:
                break;
            case LevelTypes.Levels:
                break;
            case LevelTypes.Nude:
                break;
            case LevelTypes.Waves:
                GameData.NowWave = 0;
                break;
            case LevelTypes.KnightBattle:
                GameData.NowStage = 0;
                break;
        }
        PlaySound("LevelFailed", 0f);
        Vibration.LoseVibrate(0f);
        GameData.active.IncreaseAttempt();
        PlayMenu(2);
        PlayMask(0);
    }

    public void TurnSaveLife()
    {
        if (NoInternet())
            return;
        anim.Play("SaveLife");
    }
    public void OnSaveLifeButtonClick()
    {
        if(GameData.PremiumOn)
        {
            SaveLife();
        }
        else
        {
            PlayAds(AdMob.AdsTypes.RewardedLife);
        }
    }
    public void SaveLife()
    {
        anim.Play("SaveLifeClick");
        if (LevelFailedCoroutine != null)
        {
            StopCoroutine(LevelFailedCoroutine);
            LevelFailedCoroutine = null;
        }
        for(int i = 0; i < AliveEnemy.Count; i++)
        {
            AliveEnemy[i].StopFun();
        }
        if(BossEnemy != null)
        {
            BossEnemy.StopFun();
        }
        cameraMove.TurnPlayerFollow();
        MainPlayer.SetParams();
        MainPlayer.Hp = Mathf.RoundToInt(MainPlayer.MaxHp / 2);
        SetPlayerRandomOpenedWeapon();
        if(NextLava(MainPlayer))
        {
            MainPlayer.transform.position = Vector3.zero;
        }
        MainPlayer.transform.position += new Vector3(0, 50, 0);

        SaveLifeUsed = true;
    }
    public void SkipSaveLife()
    {
        anim.Play("SaveLifeSkip");
        if (LevelFailedCoroutine != null)
        {
            StopCoroutine(LevelFailedCoroutine);
            LevelFailedCoroutine = null;
        }

        LevelFailed(LevelType);
    }

    public void OnEnemyDie(Man man, Man Enemy, Man.HitType type)
    {
        string DieName = Enemy != null ? (Enemy.name + " " + type.ToString() + " him.") : type.ToString();
        Debug.Log(man.name + " Defeated, cause of " + DieName);
        CurrantState.OnEnemyDie(man, Enemy, type);
        if(Enemy == MainPlayer)
        {
            PrintAttackPhase();
        }
        float ExpRatio = 1;
        switch (type)
        {
            case Man.HitType.Bullet:
                ExpRatio = 0.75f;
                break;
            case Man.HitType.Fall:
                ExpRatio = 1.5f;
                break;
            case Man.HitType.Hit:
                ExpRatio = 1f;
                break;
            case Man.HitType.Lava:
                ExpRatio = 0f;
                break;
            case Man.HitType.Magic:
                ExpRatio = 2f;
                break;
            case Man.HitType.Object:
                ExpRatio = 3f;
                break;
            case Man.HitType.Punch:
                ExpRatio = 1.5f;
                break;
            case Man.HitType.Tackle:
                ExpRatio = 1.5f;
                break;
            case Man.HitType.Throw:
                ExpRatio = 1.25f;
                break;
        }
        CreateExperience(man, ExpRatio);
        CreateCoin(man);
        if (LevelType != LevelTypes.Menu)
        {
            GameData.IncreaseManKilled(type);
        }

        if(Enemy == MainPlayer)
        {
            Vibration.Vibrate(0.5f);
        }
    }
    public void OnFriendDie(Man man, Man Enemy, Man.HitType type)
    {
        string DieName = Enemy != null ? (Enemy.name + " " + type.ToString() + " him.") : type.ToString();
        Debug.Log("Friend " + man.name + " Defeated, cause of " + DieName);
        CurrantState.OnFriendDie(man, Enemy, type);
    }
    public void OnPlayerDie(Man man, Man Enemy, Man.HitType type)
    {
        string DieName = Enemy != null ? (Enemy.name + " " + type.ToString() + " him.") : type.ToString();
        Debug.Log(man.name + " Defeated, cause of " + DieName);
        CurrantState.OnPlayerDie(man, Enemy, type);
        GameData.IncreaseDeath();
        PlayerDeath = type;
    }
    public void OnObjectDestroyed(SceneObject Obj)
    {

    }

    private void OnGamePlayed()
    {
        SaveLifeUsed = false;
    }

    public void OnPlayerUpdateHp()
    {
        GameHp.FillArea((float)MainPlayer.Hp / (float)MainPlayer.MaxHp);
    }
    public void OnPlayerUpdateAmmo()
    {
        StartCoroutine(UpdateAmmoCour());
    }
    private IEnumerator UpdateAmmoCour()
    {
        yield return new WaitForFixedUpdate();
        if (MainPlayer.weapon == null)
        {
            yield break;
        }
        GunWeapon weapon = MainPlayer.weapon.GetComponent<GunWeapon>();
        if (weapon == null)
            yield break;

        if (weapon.NowAmmo > 0)
        {
            Ui_Game_AmmoText.text = "Ammo: " + weapon.NowAmmo + "/" + weapon.MaxAmmo;
        }
        else
        {
            Ui_Game_AmmoText.text = "No Ammo!";
        }
        yield break;
    }
    public void OnPlayerMove(Vector2 Dir)
    {
        CurrantState.OnPlayerMove(Dir);
    }
    public void OnPlayerJump()
    {
        CurrantState.OnPlayerJump();
    }
    public void OnPlayerTackle()
    {
        CurrantState.OnPlayerTackle();
    }
    public void OnPlayerThrow()
    {
        Ui_Game_Ammo.SetActive(false);

        CurrantState.OnPlayerThrow();
    }
    public void OnPlayerTakeWeapon(Weapon weapon)
    {
        CurrantState.OnPlayerTakeWeapon(weapon);

        if(weapon.WeaponType == Weapon.Type.Gun)
        {
            Ui_Game_Ammo.SetActive(true);
            OnPlayerUpdateAmmo();
        }
    }
    public void OnPlayerAttack(Man Enemy, Man.HitType Type)
    {
        if(Enemy != null)
        {
            HitInRow++;
            SetHitinRow(HitInRow);
            if(HitInRow % 7 == 0)
            {
                MainPlayer.GetBuff(Buff.Type.Power, 5f);
            }
            if (HitInRowCoroutine != null)
            {
                StopCoroutine(HitInRowCoroutine);
            }
            HitInRowCoroutine = StartCoroutine(RemoveHitInRow());
            Vibration.Vibrate(Type);
        }
    }
    public void OnPlayerBlock(Man Enemy)
    {
        if (Enemy != null)
        {
            Vibration.Vibrate(0.5f);
        }
    }
    public void OnPlayerGetDamage(Man Enemy, Man.HitType type, Man.EffectType effect)
    {
        SetHitinRow(0);
        float Power = 0.5f;
        switch(type)
        {
            case Man.HitType.Fall:
                Power = 0.25f;
                break;
            case Man.HitType.Hit:
                Power = 0.3f;
                break;
            case Man.HitType.HitFire:
                Power = 0.25f;
                break;
            case Man.HitType.Lava:
                Power = 0.5f;
                break;
            case Man.HitType.Punch:
                Power = 0.33f;
                break;
            case Man.HitType.Throw:
                Power = 0.33f;
                break;
            case Man.HitType.Object:
                Power = 0.5f;
                break;
        }
        PrintDamageText();
        Vibration.Vibrate(Power);
    }


    public void OnSwipe(Vector2 Dir, Vector2 Pos)
    {
        if(CanSwipe(Pos))
        {
            if(Mathf.Abs(Dir.x) > Mathf.Abs(Dir.y * 2))
            {
                ChangeGameType(Dir.x > 0);
            }
        }
    }


    private IEnumerator RemoveHitInRow()
    {
        yield return new WaitForSeconds(1f);
        SetHitinRow(0);
        yield break;
    }

    public void RemoveEnemy_All(Man man, float Delay)
    {
        StartCoroutine(RemoveEnemy_AllCour(man, Delay));
    }
    private IEnumerator RemoveEnemy_AllCour(Man man, float Delay)
    {
        yield return new WaitForSeconds(Delay);
        if (man == null)
            yield break;
        AllEnemy.Remove(man);
        Destroy(man.gameObject, 5f);
        yield break;
    }

    public void ClearGame()
    {
        ClearText();
        GameObject[] Weapon = GameObject.FindGameObjectsWithTag("Weapon");
        for(int i = 0; i < Weapon.Length; i++)
        {
            if (Weapon[i].transform.parent == null)
            {
                Destroy(Weapon[i]);
            }
        }
        GameObject[] Bullets = GameObject.FindGameObjectsWithTag("Bullet");
        for (int i = 0; i < Bullets.Length; i++)
        {
            if (Bullets[i].transform.parent == null || Bullets[i].transform.parent.root.tag != "Weapon")
            {
                Destroy(Bullets[i]);
            }
        }
        GameObject[] Buffs = GameObject.FindGameObjectsWithTag("Buff");
        for (int i = 0; i < Buffs.Length; i++)
        {
            Destroy(Buffs[i]);
        }
        for(int i = 0; i < DefeatedEnemy.Count; i++)
        {
            if(DefeatedEnemy[i] != null)
                Destroy(DefeatedEnemy[i].gameObject);
        }
        for(int i = 0; i < AliveEnemy.Count; i++)
        {
            if (AliveEnemy[i] != null)
                Destroy(AliveEnemy[i].gameObject);
        }
        for(int i = 0; i < AllEnemy.Count; i++)
        {
            if(AllEnemy[i] != null)
                Destroy(AllEnemy[i].gameObject);
        }
        if(BossEnemy != null)
        {
            Destroy(BossEnemy.gameObject);
            BossEnemy = null;
        }
        LastOfMan = null;
        AllEnemy.Clear();
        AliveEnemy.Clear();
        BattleEnemy.Clear();
        DefeatedEnemy.Clear();

        for (int i = 0; i < DefeatedFriends.Count; i++)
        {
            if (DefeatedFriends[i] != null)
                Destroy(DefeatedFriends[i].gameObject);
        }
        for (int i = 0; i < AliveFriends.Count; i++)
        {
            if (AliveFriends[i] != null)
                Destroy(AliveFriends[i].gameObject);
        }
        for (int i = 0; i < AllEnemy.Count; i++)
        {
            if (AllFriends[i] != null)
                Destroy(AllFriends[i].gameObject);
        }

        AllFriends.Clear();
        AliveFriends.Clear();
        BattleFriends.Clear();
        DefeatedFriends.Clear();
        /*
        if(MainPlayer.weapon != null)
        {
            Destroy(MainPlayer.weapon.gameObject);
            MainPlayer.weapon = null;
        }
        */
    }
    public void ClearEnemy()
    {
        for (int i = 0; i < DefeatedEnemy.Count; i++)
        {
            if (DefeatedEnemy[i] != null)
                Destroy(DefeatedEnemy[i].gameObject);
        }
        for (int i = 0; i < AliveEnemy.Count; i++)
        {
            if (AliveEnemy[i] != null)
                Destroy(AliveEnemy[i].gameObject);
        }
        for (int i = 0; i < AllEnemy.Count; i++)
        {
            if (AllEnemy[i] != null)
                Destroy(AllEnemy[i].gameObject);
        }
        if (BossEnemy != null)
        {
            Destroy(BossEnemy.gameObject);
            BossEnemy = null;
        }
        LastOfMan = null;
        AllEnemy.Clear();
        AliveEnemy.Clear();
        BattleEnemy.Clear();
        DefeatedEnemy.Clear();

        for (int i = 0; i < DefeatedFriends.Count; i++)
        {
            if (DefeatedFriends[i] != null)
                Destroy(DefeatedFriends[i].gameObject);
        }
        for (int i = 0; i < AliveFriends.Count; i++)
        {
            if (AliveFriends[i] != null)
                Destroy(AliveFriends[i].gameObject);
        }
        for (int i = 0; i < AllEnemy.Count; i++)
        {
            if (AllFriends[i] != null)
                Destroy(AllFriends[i].gameObject);
        }

        AllFriends.Clear();
        AliveFriends.Clear();
        BattleFriends.Clear();
        DefeatedFriends.Clear();
    }
    public void ClearWeapon()
    {
        GameObject[] Weapon = GameObject.FindGameObjectsWithTag("Weapon");
        for (int i = 0; i < Weapon.Length; i++)
        {
            if (Weapon[i].transform.parent == null)
            {
                Destroy(Weapon[i]);
            }
        }
        GameObject[] Bullets = GameObject.FindGameObjectsWithTag("Bullet");
        for (int i = 0; i < Bullets.Length; i++)
        {
            if (Bullets[i].transform.parent == null || Bullets[i].transform.parent.root.tag != "Weapon")
            {
                Destroy(Bullets[i]);
            }
        }
    }

    public void BoxSpawn()
    {
        if(BoxSpawnCoroutine == null)
        {
            BoxSpawnCoroutine = StartCoroutine(BoxSpawnCour());
        }
    }
    public void StopBoxSpawn()
    {
        if(BoxSpawnCoroutine != null)
        {
            StopCoroutine(BoxSpawnCoroutine);
            BoxSpawnCoroutine = null;
        }
    }
    private IEnumerator BoxSpawnCour()
    {
        while (!CurrantState.isEnd)
        {
            yield return new WaitForSeconds(10);
            if (MainPlayer.weapon != null)
                continue;
            Vector2 Pos = MainPlayer.transform.position + Vector3.right * Random.Range(-2, 2) + Vector3.up * 25f;
            sceneMaker.SpawnBox(Pos, MainPlayer.Rig.velocity * 0.75f);
        }
        BoxSpawnCoroutine = null;
        yield break;
    }

    public void Meteor(int Count, float Delay)
    {
        StartCoroutine(MeteorCour(Count, Delay));
    }
    private IEnumerator MeteorCour(int count, float Delay)
    {
        yield return new WaitForSeconds(Delay);
        for (int i = 0; i < count; i++)
        {
            sceneMaker.SpawnMeteorAtPlayer();
            yield return new WaitForSeconds(1f);
        }
        yield break;
    }

    public void MeteorSpawn()
    {
        if(MeteorSpawnCoroutine == null)
        {
            MeteorSpawnCoroutine = StartCoroutine(MeteorSpawnCour());
        }
    }
    public void HellMeteorSpawn()
    {
        if (MeteorSpawnCoroutine == null)
        {
            MeteorSpawnCoroutine = StartCoroutine(HellMeteorSpawnCour());
        }
    }
    public void StopMeteorSpawn()
    {
        if(MeteorSpawnCoroutine != null)
        {
            StopCoroutine(MeteorSpawnCoroutine);
            MeteorSpawnCoroutine = null;
        }
    }
    private IEnumerator MeteorSpawnCour()
    {
        while (!CurrantState.isEnd)
        {
            yield return new WaitForSeconds(Random.Range(3, 7));
            sceneMaker.SpawnMeteorAtPlayer();
        }
        MeteorSpawnCoroutine = null;
        yield break;
    }
    private IEnumerator HellMeteorSpawnCour()
    {
        while (!CurrantState.isEnd)
        {
            yield return new WaitForSeconds(Random.Range(2, 3));
            sceneMaker.SpawnMeteorAtBattle(AllEnemy);
        }
        MeteorSpawnCoroutine = null;
        yield break;
    }


    public void BuffSpawn()
    {
        if (BuffSpawnCoroutine == null)
        {
            BuffSpawnCoroutine = StartCoroutine(BuffSpawnCour());
        }
    }
    public void StopBuffSpawn()
    {
        if (BuffSpawnCoroutine != null)
        {
            StopCoroutine(BuffSpawnCoroutine);
            BuffSpawnCoroutine = null;
        }
    }
    private IEnumerator BuffSpawnCour()
    {
        while (!CurrantState.isEnd)
        {
            yield return new WaitForSeconds(15);
            Vector2 Pos = MainPlayer.transform.position + Vector3.right * Random.Range(-2, 2) + Vector3.up * 25f;
            sceneMaker.SpawnBuff(Pos, MainPlayer.Rig.velocity * 0.5f);
        }
        BuffSpawnCoroutine = null;
        yield break;
    }

    public void PlayMask(float time)
    {
        anim.ResetTrigger("MaskOn");
        StartCoroutine(PlayMaskCour(time));
    }
    private IEnumerator PlayMaskCour(float time)
    {
        yield return new WaitForSeconds(time * 0.5f);
        anim.SetTrigger("MaskOn");
        yield break;
    }

    public void SetPlayerGame()
    {
        HitInRow = 0;
        MainPlayer.gameObject.SetActive(true);
        MainPlayer.SetParams(GameData.active.playerInfo, GameData.active.SetArmorInfo());
        MainPlayer.transform.position = Vector3.zero;
        MainPlayer.NoThrowOut = false;
    }
    public void SetPlayerKnightBattle()
    {
        HitInRow = 0;
        MainPlayer.gameObject.SetActive(true);
        MainPlayer.SetParams(GameData.active.playerInfo, GameData.active.SetArmorInfo());
        MainPlayer.transform.position = sceneMaker.GetEnemyPos()[0] + new Vector2(Random.Range(6f, 7f), -0.5f);
        MainPlayer.NoThrowOut = false;
    }
    public void SetPlayerBattle()
    {
        HitInRow = 0;
        MainPlayer.gameObject.SetActive(true);
        MainPlayer.SetParams(GameData.active.playerInfo, GameData.active.SetArmorInfo());
        MainPlayer.transform.position = sceneMaker.GetEnemyPos()[0] + new Vector2(Random.Range(5f, 6f), -0.5f);
        MainPlayer.NoThrowOut = false;
    }
    public void SetPlayerLearn()
    {
        if (MainPlayer.weapon != null)
        {
            Destroy(MainPlayer.weapon.gameObject);
            MainPlayer.weapon = null;
        }

        MainPlayer.gameObject.SetActive(true);
        MainPlayer.SetParams(GameData.active.playerInfo, GameData.active.SetArmorInfo());
        MainPlayer.transform.position = Vector3.zero;
        MainPlayer.NoThrowOut = false;
    }
    public void SetPlayerEasy()
    {
        MainPlayer.gameObject.SetActive(true);
        MainPlayer.SetParams(GameData.active.playerInfo, GameData.active.SetArmorInfo());
        MainPlayer.transform.position = Vector3.zero;
        MainPlayer.NoThrowOut = true;
    }
    public void SetPlayerLikeNew()
    {
        MainPlayer.SetParams(GameData.active.playerInfo, GameData.active.SetArmorInfo());
    }
    public void SetPlayerWeapon()
    {
        if(MainPlayer.weapon != null)
        {
            
            WeaponInfo info = GameData.active.GetWeaponInfo(MainPlayer.weapon);
            if (info.Opened)
            {
            }
            else if(info.Premium)
            {
                Destroy(MainPlayer.weapon.gameObject);
                MainPlayer.weapon = null;

                Weapon weapon = Instantiate(GameData.active.GetRandomOpened());
                MainPlayer.TakeWeapon(weapon);
            }
            else
            {

            }

        }
        else
        {
            Weapon weapon = Instantiate(GameData.active.GetSelectedWeapon());
            MainPlayer.TakeWeapon(weapon);
        }
        if (MainPlayer.weapon != null)
        {
            MainPlayer.weapon.OnStart();
        }
    }
    public void SetPlayerRandomWeapon()
    {
        Weapon weapon = Instantiate(GameData.active.GetRandomWeapon());
        MainPlayer.TakeWeapon(weapon);
    }
    public void SetPlayerRandomOpenedWeapon()
    {
        Weapon weapon = Instantiate(GameData.active.GetRandomOpened());
        MainPlayer.TakeWeapon(weapon);
    }
    #endregion
    #region GameInit
    //-------------------------------Game Init-----------------------------------

    private void Init()
    {
        active = this;
    }
    private void LateInit()
    {
        AdMob.active.Init();
        SetPlayerWeapon();
        UpdatePlayerStatsUI();
        LoadLanguage();
        NowArmor = GameData.NowArmor;
        SwipeManager.OnSwipeEvent += OnSwipe;
        PlayMusic();
        StartCoroutine(MusicCheckCour());
        StartCoroutine(PresentTimeCour());
        SwitchPremiumButton();
        Loaded = true;

        anim.Play("StartFade");
    }
    private IEnumerator PresentTimeCour()
    {
        Ui_Menu_Present.SetActive(true);
        while (true)
        {
            float time = GameData.GetTimeToPresent();
            if(time > 3600)
            {
                Ui_Menu_Timer.text = Mathf.CeilToInt(time / 3600).ToString() + " " + Language.Lang.presentText.Hours;
            }
            else if(time > 60)
            {
                Ui_Menu_Timer.text = Mathf.CeilToInt(time / 60).ToString() + " " + Language.Lang.presentText.Minutes;
            }
            else if(time > 0)
            {
                Ui_Menu_Timer.text = Mathf.CeilToInt(time).ToString() + " " + Language.Lang.presentText.Seconds;
            }
            else if(Ui_Menu_Present.activeSelf)
            {
                Ui_Menu_Present.SetActive(false);
            }

            yield return new WaitForSeconds(1f);
        }
    }
    public void LoadLanguage()
    {
        Language.active.LoadLanguage(GameData.Language);
    }
    private void UpdateLanguage()
    {
        //Game
        Ui_Game_SaveLifeNo.text = Language.Lang.premiumText.NoThanks;
        Ui_Game_Skip.text = Language.Lang.gameText.Skip;
        //LevelUp
        Ui_LevelUp_Text.text = Language.Lang.levelUpText.LevelUp;
        Ui_LevelUp_WeaponUnlockText.text = Language.Lang.levelUpText.Unlocked;
        //Weapon
        for (int i = 0; i < Language.Lang.Weapon.Name.Length; i++)
        {
            GameData.active.weapon[i].Name = Language.Lang.Weapon.Name[i];
        }
        //Armor
        for (int i = 0; i < Language.Lang.Armor.Name.Length; i++)
        {
            GameData.active.armor[i].Name = Language.Lang.Armor.Name[i];
        }
        //Phase
        GameData.active.DamagePhase = Language.Lang.gameTypeText.DamagePhase;
        GameData.active.AttackPhase = Language.Lang.gameTypeText.AttackPhase;
        //Menu
        Ui_Menu_PlayText.text = Language.Lang.menuText.Play;
        Ui_Menu_Name.text = Language.Lang.menuText.Name;
        Ui_Menu_Amunition.text = Language.Lang.menuText.Amunition;
        Ui_Menu_Settings.text = Language.Lang.menuText.Settings;
        //Present
        Ui_Menu_PresentGotText.text = Language.Lang.presentText.Get;
        Ui_Menu_PresentWeaponText.text = Language.Lang.levelUpText.Unlocked;
        //Bets
        Ui_Bets_Name.text = Language.Lang.betsText.Name;
        Ui_Bets_Left.text = Language.Lang.betsText.BetLeft;
        Ui_Bets_Right.text = Language.Lang.betsText.BetRight;
        Ui_BetsWin_Text.text = Language.Lang.betsText.IfWin;
        Ui_BetsLose_Text.text = Language.Lang.betsText.IfLose;
        Ui_BetsLose_Close.text = Language.Lang.betsText.Close;
        Ui_BetsWin_Close.text = Language.Lang.betsText.Close;
        //Waves
        Ui_Waves_Name.text = Language.Lang.wavesText.Name;
        Ui_Waves_Play.text = Language.Lang.wavesText.Play;
        //Knights
        Ui_Knights_Name.text = Language.Lang.knightsText.Name;
        Ui_Knights_Play.text = Language.Lang.knightsText.Play;
        //Premiun
        Ui_Store_Name.text = Language.Lang.premiumText.Name;
        Ui_Store_WhatGet.text = Language.Lang.premiumText.WhatGet;
        Ui_Store_NoAds.text = Language.Lang.premiumText.NoAds;
        Ui_Store_CoolWeapon.text = Language.Lang.premiumText.CoolWeapon;
        Ui_Store_Skin.text = Language.Lang.premiumText.Skin;
        Ui_Store_GameMode.text = Language.Lang.premiumText.NewMode;
        Ui_Store_x2Exp.text = Language.Lang.premiumText.x2Exp;
        Ui_Store_Coins.text = GameData.active.premiumInfo.Money + " " + Language.Lang.basicText.Coins;
        Ui_Store_Buy.text = Language.Lang.premiumText.Buy + " " + GameData.active.premiumInfo.Cost + "$";
        Ui_Store_NoThanks.text = Language.Lang.premiumText.NoThanks;
        Ui_Store_Restore.text = Language.Lang.premiumText.Restore;
        Ui_Store_ThanksFor.text = Language.Lang.premiumText.Thanks;
        //Amunition
        
        //Already Bought
        Ui_StoreAlready_Name.text = Language.Lang.premiumText.YouCool;
        Ui_StoreAlready_WhatGot.text = Language.Lang.premiumText.WhatGot;
        Ui_StoreAlready_NoAds.text = Language.Lang.premiumText.NoAds;
        Ui_StoreAlready_Weapon.text = Language.Lang.premiumText.CoolWeapon;
        Ui_StoreAlready_Skin.text = Language.Lang.premiumText.Skin;
        Ui_StoreAlready_GameMode.text = Language.Lang.premiumText.NewMode;
        Ui_StoreAlready_x2Exp.text = Language.Lang.premiumText.x2Exp;
        Ui_StoreAlready_Coins.text = GameData.active.premiumInfo.Money + " " + Language.Lang.basicText.Coins;
        //Pause
        Ui_Pause_ResumeText.text = Language.Lang.settingsText.ResumeGame;
        Ui_Pause_RestartText.text = Language.Lang.settingsText.Restart;
        Ui_Pause_MainMenuText.text = Language.Lang.settingsText.MainMenu;
        Ui_Pause_VolumeText.text = Language.Lang.settingsText.Volume;
        Ui_Pause_MusicText.text = Language.Lang.settingsText.Music;
        //Settings
        Ui_Settings_ResumeText.text = Language.Lang.settingsText.Resume;
        Ui_Settings_VolumeText.text = Language.Lang.settingsText.Volume;
        Ui_Settings_MusicText.text = Language.Lang.settingsText.Music;
        Ui_Settings_Vibration.text = Language.Lang.settingsText.Vibration;

    }
    public void OnLanguageLoaded()
    {
        UpdateLanguage();
    }
    #endregion
    #region Anumition
    //-------------------------------Anumition States-----------------------------
    public void GoAmunition()
    {
        if(GoAmunitionCoroutine != null)
        {
            StopCoroutine(GoAmunitionCoroutine);
        }
        GoAmunitionCoroutine = StartCoroutine(GoAmunitionCour());
        SetAmunition();


        OnButtonClick();
        HideBanner();
    }
    private IEnumerator GoAmunitionCour()
    {
        cameraMove.SetStatic(true);
        Ui_Menu.SetActive(false);
        cameraMove.Cam.orthographicSize = 30;
        MainPlayer.SetSize(GameData.active.playerInfo.Size);
        Vector3 Pos = CameraMenuPos.position + new Vector3(0, (GameData.active.playerInfo.Size - 1) * 1.5f, 0);
        while ((transform.position - Pos).magnitude > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, CameraMenuPos.position, 0.125f);
            cameraMove.Cam.orthographicSize = Mathf.Lerp(cameraMove.Cam.orthographicSize, 6 * GameData.active.playerInfo.Size, 0.1f);
            if(!Ui_Amunition.activeSelf && (transform.position - CameraMenuPos.position).magnitude < 5f)
            {
                Ui_Amunition.SetActive(true);
            }
            yield return new WaitForFixedUpdate();
        }
        transform.position = Pos;
        cameraMove.Cam.orthographicSize = 6 * GameData.active.playerInfo.Size;

        yield break;
    }
    private void SetAmunition()
    {
        Ui_Amunition.SetActive(true);
        for (int i = 1; i < Ui_Anum_ScrollWeapon.transform.childCount; i++)
        {

            Destroy(Ui_Anum_ScrollWeapon.transform.GetChild(i).gameObject);
        }
        WeaponInfo[] info = GameData.active.GetWeaponSortedByLevel();
        Ui_Anum_WeaponIcons = new WeaponIcon[info.Length];
        for (int i = 0; i < Ui_Anum_WeaponIcons.Length; i++)
        {
            Ui_Anum_WeaponIcons[i] = Instantiate(GameData.active.IconPrefab, Ui_Anum_ScrollWeapon.transform);
            Ui_Anum_WeaponIcons[i].SetIcon(info[i], i);
        }

        for (int i = 0; i < Ui_Anum_WeaponIcons.Length; i++)
        {
            Ui_Anum_WeaponIcons[i].SetOpened(info[i].Opened, info[i].Premium, GameData.active.GetAvalibleWeapon(info[i].Index));
            Ui_Anum_WeaponIcons[GameData.NowWeaponPlace].SetSelected(false);
        }

        Ui_Anum_WeaponIcons[GameData.NowWeaponPlace].SetSelected(true);
        Ui_Anum_ScrollWeapon.anchoredPosition = new Vector2(Ui_Anum_WeaponIcons.Length * 120, 0);
        SetPlayerWeapon(info[GameData.NowWeaponPlace].weapon);


        NowArmor = GameData.NowArmor;
        SetArmor();

        Ui_Anum_BackText.text = Language.Lang.ammunitionText.Back;
        Ui_Anum_ArmorName.text = Language.Lang.ammunitionText.Buy + " " + GameData.active.tempArmorInfo.Name;
        Ui_Anum_StartWeaponText.text = Language.Lang.ammunitionText.StartWeapon;

        Ui_Anum_HpLevel.text = Language.Lang.basicText.Level + " " + GameData.active.playerInfo.HpNum.ToString();
        Ui_Anum_SpeedLevel.text = Language.Lang.basicText.Level + " " + GameData.active.playerInfo.SpeedNum.ToString();
        Ui_Anum_SizeLevel.text = Language.Lang.basicText.Level + " " + GameData.active.playerInfo.SizeNum.ToString();
        Ui_Anum_PowerLevel.text = Language.Lang.basicText.Level + " " + GameData.active.playerInfo.PowerNum.ToString();

        Ui_Anum_SizeBar.FillArea(GameData.active.SizeProcent(), GameData.active.SizeSecondProcent());
        Ui_Anum_HpBar.FillArea(GameData.active.HpProcent(), GameData.active.HpSecondProcent());
        Ui_Anum_PowerBar.FillArea(GameData.active.PowerProcent(), GameData.active.PowerSecondProcent());
        Ui_Anum_SpeedBar.FillArea(GameData.active.SpeedProcent(), GameData.active.SpeedSecondProcent());

        if (GameData.active.upInfo.MaxHp > GameData.active.playerInfo.MaxHp)
        {
            Ui_Anum_HpLevel.text = Language.Lang.basicText.Level + " " + GameData.active.playerInfo.HpNum.ToString();
            Ui_Anum_HpCost.text = GameData.active.UpdateCost(GameData.active.playerInfo.HpNum).ToString();
        }
        else
        {
            Ui_Anum_HpLevel.text = Language.Lang.basicText.MaxLevel;
            Ui_Anum_HpCost.text = Language.Lang.basicText.Nice;
        }
        if (GameData.active.upInfo.MaxSpeed > GameData.active.playerInfo.Speed)
        {
            Ui_Anum_SpeedLevel.text = Language.Lang.basicText.Level + " " + GameData.active.playerInfo.SpeedNum.ToString();
            Ui_Anum_SpeedCost.text = GameData.active.UpdateCost(GameData.active.playerInfo.SpeedNum).ToString();
        }
        else
        {
            Ui_Anum_SpeedLevel.text = Language.Lang.basicText.MaxLevel;
            Ui_Anum_SpeedCost.text = Language.Lang.basicText.Nice;
        }
        if (GameData.active.upInfo.MaxPower > GameData.active.playerInfo.Power)
        {
            Ui_Anum_PowerLevel.text = Language.Lang.basicText.Level + " " + GameData.active.playerInfo.PowerNum.ToString();
            Ui_Anum_PowerCost.text = GameData.active.UpdateCost(GameData.active.playerInfo.PowerNum).ToString();
        }
        else
        {
            Ui_Anum_PowerLevel.text = Language.Lang.basicText.MaxLevel;
            Ui_Anum_PowerCost.text = Language.Lang.basicText.Nice;

        }
        Ui_Amunition.SetActive(false);
    }

    public void BackAmunition()
    {
        if (GoAmunitionCoroutine != null)
        {
            StopCoroutine(GoAmunitionCoroutine);
        }
        GoAmunitionCoroutine = StartCoroutine(BackAmunitionCour());
        GameData.Save();

        OnButtonClick();
        ShowBanner();
    }
    private IEnumerator BackAmunitionCour()
    {
        cameraMove.SetStatic(false);
        Ui_Amunition.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        Ui_Menu.SetActive(true);
        yield break;
    }

    

    public void UpdateHp(bool update)
    {
        if (GameData.active.upInfo.MaxHp > GameData.active.playerInfo.MaxHp)
        {
            if (GameData.Money >= GameData.active.UpdateCost(GameData.active.playerInfo.HpNum))
            {
                GameData.active.UpdateHp();
                UpdateMoney(-GameData.active.UpdateCost(GameData.active.playerInfo.HpNum));
                GameData.active.playerInfo.HpNum++;
                Ui_Anum_HpLevel.text = "Level " + GameData.active.playerInfo.HpNum.ToString();
                Ui_Anum_HpCost.text = GameData.active.UpdateCost(GameData.active.playerInfo.HpNum).ToString();
                PlaySound("Buy");
                Vibration.Vibrate(0.25f);
            }
            else
            {
                PlaySound("CantBuy");
            }
        }
        else
        {
            Ui_Anum_HpLevel.text = "Max Level";
            Ui_Anum_HpCost.text = "Nice";
        }
        Ui_Anum_HpBar.FillArea(GameData.active.HpProcent(), GameData.active.HpSecondProcent());
    }
    public void UpdateSpeed(bool update)
    {
        if (GameData.active.upInfo.MaxSpeed > GameData.active.playerInfo.Speed)
        {
            if (GameData.Money >= GameData.active.UpdateCost(GameData.active.playerInfo.SpeedNum))
            {
                GameData.active.UpdateSpeed();
                UpdateMoney(-GameData.active.UpdateCost(GameData.active.playerInfo.SpeedNum));
                GameData.active.playerInfo.SpeedNum++;
                Ui_Anum_SpeedLevel.text = "Level " + GameData.active.playerInfo.SpeedNum.ToString();
                Ui_Anum_SpeedCost.text = GameData.active.UpdateCost(GameData.active.playerInfo.SpeedNum).ToString();
                PlaySound("Buy");
                Vibration.Vibrate(0.25f);
            }
            else
            {
                PlaySound("CantBuy");
            }
        }
        else
        {
            Ui_Anum_SpeedLevel.text = "Max Level";
            Ui_Anum_SpeedCost.text = "Nice";
        }

        Ui_Anum_SpeedBar.FillArea(GameData.active.SpeedProcent(), GameData.active.SpeedSecondProcent());
    }
    public void UpdatePower(bool update)
    {
        if (GameData.active.upInfo.MaxPower > GameData.active.playerInfo.Power)
        {
            if (GameData.Money >= GameData.active.UpdateCost(GameData.active.playerInfo.PowerNum))
            {
                GameData.active.UpdatePower();
                UpdateMoney(-GameData.active.UpdateCost(GameData.active.playerInfo.PowerNum));
                GameData.active.playerInfo.PowerNum++;
                Ui_Anum_PowerLevel.text = "Level " + GameData.active.playerInfo.PowerNum.ToString();
                Ui_Anum_PowerCost.text = GameData.active.UpdateCost(GameData.active.playerInfo.PowerNum).ToString();
                PlaySound("Buy");
                Vibration.Vibrate(0.25f);
            }
            else
            {
                PlaySound("CantBuy");
            }
        }
        else
        {
            Ui_Anum_PowerLevel.text = "Max Level";
            Ui_Anum_PowerCost.text = "Nice";
        }

        Ui_Anum_PowerBar.FillArea(GameData.active.PowerProcent(), GameData.active.PowerSecondProcent());
    }
    public void UpdateSize(bool update)
    {
        if (update)
        {
            GameData.active.UpdateSize();
        }
        else
        {
            GameData.active.playerInfo.Size = GameData.active.upInfo.StandartSize;
        }
        Ui_Anum_SizeBar.FillArea(GameData.active.SizeProcent(), GameData.active.SizeSecondProcent());
        MainPlayer.SetSize(GameData.active.playerInfo.Size);
        cameraMove.Cam.orthographicSize = 6 * GameData.active.playerInfo.Size;
        transform.position = CameraMenuPos.position + new Vector3(0, (GameData.active.playerInfo.Size - 1) * 1.5f, 0);
    }


    public void ChangeArmor(bool right)
    {
        if(right)
        {
            NowArmor++;
            if(NowArmor > GameData.active.armor.Length - 1)
            {
                NowArmor = 0;
            }
        }
        else
        {
            NowArmor--;
            if(NowArmor < 0)
            {
                NowArmor = GameData.active.armor.Length - 1;
            }
        }

        SetArmor();

        if (NoMoneyCoroutine != null)
        {
            StopCoroutine(NoMoneyCoroutine);
            NoMoneyCoroutine = null;
        }
        if (NoLevelCoroutine != null)
        {
            StopCoroutine(NoLevelCoroutine);
            NoLevelCoroutine = null;
        }
        OnButtonClick();
    }
    public void SetArmor()
    {
        GameData.active.SetTempArmorInfo(GameData.active.armor[NowArmor]);

        if (GameData.active.tempArmorInfo.Opened)
        {
            GameData.NowArmor = NowArmor;
            GameData.active.SetArmorInfo();

            Ui_Anum_ArmorCosts.SetActive(false);
            Ui_Anum_ArmorExpText.text = GameData.active.tempArmorInfo.RequiredLevel.ToString();
            Ui_Anum_ArmorCostText.text = GameData.active.tempArmorInfo.Cost.ToString();
            Ui_Anum_ArmorButton.enabled = false;
            Color color = GameData.active.IconWeaponColor[(int)(int)GameData.active.tempArmorInfo.Rare];
            Ui_Anum_ArmorButtonColor.color = new Color(color.r, color.g, color.b, 0.1f);
            Ui_Anum_ArmorName.text = GameData.active.tempArmorInfo.Name;
        }
        else if(GameData.active.tempArmorInfo.Premium)
        {
            Ui_Anum_ArmorCosts.SetActive(false);
            Ui_Anum_ArmorButton.enabled = false;
            Ui_Anum_ArmorName.text = GameData.active.tempArmorInfo.Name;
            StoreOn(1.5f);
        }
        else
        {
            Ui_Anum_ArmorCosts.SetActive(true);
            Ui_Anum_ArmorExpText.text = GameData.active.tempArmorInfo.RequiredLevel.ToString();
            Ui_Anum_ArmorCostText.text = GameData.active.tempArmorInfo.Cost.ToString();
            Ui_Anum_ArmorButton.enabled = true;
            Color color = GameData.active.IconWeaponColor[(int)GameData.active.tempArmorInfo.Rare];
            Ui_Anum_ArmorButtonColor.color = new Color(color.r, color.g, color.b, 0.35f);
            Ui_Anum_ArmorName.text = Language.Lang.ammunitionText.Buy + " " + GameData.active.tempArmorInfo.Name;
        }

        Ui_Anum_HpBar.FillArea(GameData.active.HpProcent(), GameData.active.HpSecondProcent());
        Ui_Anum_SpeedBar.FillArea(GameData.active.SpeedProcent(), GameData.active.SpeedSecondProcent());
        Ui_Anum_SizeBar.FillArea(GameData.active.SizeProcent(), GameData.active.SizeSecondProcent());
        Ui_Anum_PowerBar.FillArea(GameData.active.PowerProcent(), GameData.active.PowerSecondProcent());

        MainPlayer.SetParams(GameData.active.playerInfo, GameData.active.tempArmorInfo);
    }
    public void BuyArmor()
    {
        if(GameData.active.armor[NowArmor].RequiredLevel <= GameData.PlayerLevel)
        {
            if(GameData.active.armor[NowArmor].Cost <= GameData.Money)
            {
                GameData.active.armor[NowArmor].Opened = true;
                UpdateMoney(-GameData.active.armor[NowArmor].Cost);
                SetArmor();

                PlaySound("Buy");
                Vibration.Vibrate(0.25f);
            }
            else
            {
                if(NoMoneyCoroutine == null)
                {
                    NoMoneyCoroutine = StartCoroutine(NoMoneyForArmor());
                }
            }
        }
        else
        {
            if(NoLevelCoroutine == null)
            {
                NoLevelCoroutine = StartCoroutine(NoLevelForArmor());
            }
        }

    }
    private IEnumerator NoLevelForArmor()
    {
        Ui_Anum_ArmorName.text = Language.Lang.ammunitionText.NoLevel;
        PlaySound("CantBuy");
        yield return new WaitForSeconds(1f);
        Ui_Anum_ArmorName.text = Language.Lang.ammunitionText.Buy + " " + GameData.active.tempArmorInfo.Name;
        NoLevelCoroutine = null;
        yield break;
    }
    private IEnumerator NoMoneyForArmor()
    {
        Ui_Anum_ArmorName.text = Language.Lang.ammunitionText.NoMoney;
        PlaySound("CantBuy");
        yield return new WaitForSeconds(1f);
        Ui_Anum_ArmorName.text = Language.Lang.ammunitionText.Buy + " " + GameData.active.tempArmorInfo.Name;
        NoMoneyCoroutine = null;
        yield break;
    }

    public void SelectWeapon(int index, int Place)
    {
        Ui_Anum_WeaponIcons[GameData.NowWeaponPlace].SetSelected(false);
        GameData.NowWeaponPlace = Place;
        if (GameData.active.weapon[index].Opened)
        {
            Ui_Anum_WeaponIcons[GameData.NowWeaponPlace].SetSelected(true);
            GameData.NowWeapon = index;
            SetPlayerWeapon(GameData.active.weapon[index].weapon);
        }
        else if(GameData.active.weapon[index].Premium)
        {
            TempWeapon = index;
            StoreOn(1.5f);
            SetPlayerWeapon(GameData.active.weapon[index].weapon);
        }
        else
        {
            Ui_Anum_WeaponIcons[Place].SetTryToBuy(true);
        }
    }
    private void SelectNowWeapon()
    {
        SetPlayerWeapon(GameData.active.weapon[GameData.NowWeapon].weapon);
    }

    public void TryToBuyWeapon(WeaponIcon icon)
    {
        if(GameData.Money >= GameData.active.weapon[icon.Index].Cost)
        {
            GameData.active.OpenWeapon(icon.Index);
            Ui_Anum_WeaponIcons[icon.Place].OnBought();
            PlaySound("Buy");
            Vibration.Vibrate(0.25f);
        }
        else
        {
            Ui_Anum_WeaponIcons[icon.Place].OnCantBuy();
            PlaySound("CantBuy");
        }
        
    }
    private void SetPlayerWeapon(Weapon weapon)
    {
        if (MainPlayer.weapon != null)
        {
            Destroy(MainPlayer.weapon.gameObject);
            MainPlayer.weapon = null;
        }
        Weapon thisWeapon = Instantiate(weapon);
        MainPlayer.TakeWeapon(thisWeapon);
        Vector2 Dir = new Vector2(Random.Range(-1f, 1f), Random.Range(0.1f, 1f)).normalized;
        MainPlayer.MoveArmForce(Dir);
    }

    #endregion
    #region Bets State

    public void BetsFor(bool right)
    {
        BetForNum = right ? 1 : 0;
        UpdateMoney(-BetMoney);

        CurrantState.Action();
        TurnBetUi(false);

        PlaySound("Play");
        Vibration.Vibrate(0.25f);
        HideBanner();
    }
    public void SetBetMoney()
    {
        BetMoney = Mathf.RoundToInt(Ui_Bets_Slider.value * 1000);
        if(BetMoney > GameData.Money)
        {
            Ui_Bets_Slider.value = (float)GameData.Money / 1000;
        }

        Ui_Bets_MoneySlider.text = BetMoney + " " + Language.Lang.basicText.Coins;
        Ui_Bets_MoneyGetText.text = Language.Lang.betsText.RewardText + " " + BetMoney * 2 + " " + Language.Lang.basicText.Coins;
    }
    private void SetBetMoneyNull()
    {
        Ui_Bets_Slider.value = 0;

        Ui_Bets_MoneySlider.text = 0 + " " + Language.Lang.basicText.Coins;
        Ui_Bets_MoneyGetText.text = Language.Lang.betsText.RewardText + " " + 0 + " " + Language.Lang.basicText.Coins;
    }

    public void TurnBetUi(bool on)
    {
        Ui_Bets.SetActive(on);
    }

    public void OnBetEnd(Man man)
    {
        StartCoroutine(OnBetEndCour(man));
    }
    private IEnumerator OnBetEndCour(Man man)
    {
        yield return new WaitForSeconds(2);
        TurnBetUi(true);
        yield return new WaitForSeconds(1f);
        if (man.name != BetForNum.ToString())
        {
            OnBetWin();
        }
        else
        {
            OnBetLose();
        }
        SetState(BetsState);
        SetBetMoneyNull();
        GameData.Save();
        yield break;
    }

    public void OnBetWin()
    {
        UpdateMoney(BetMoney * 2);
        Ui_BetsWin.SetActive(true);
        Ui_BetsWin_Money.text = "+ " + (BetMoney * 2).ToString() + " " + Language.Lang.basicText.Coins;
        PlaySound("OnBetWin");
        Vibration.WinVibrate(0);
    }
    public void OnBetLose()
    {
        Ui_BetsLose.SetActive(true);
        Ui_BetsLose_Money.text = "- " + (BetMoney).ToString() + " " + Language.Lang.basicText.Coins;
        PlaySound("OnBetLose");
        Vibration.LoseVibrate(0);
    }
    public void CloseBetResult()
    {
        Ui_BetsLose.SetActive(false);
        Ui_BetsWin.SetActive(false);
    }

    #endregion
    #region Waves
    public void SetWavesUi()
    {
        Ui_Waves_Max.text = Language.Lang.wavesText.MaxWave + ": " + (GameData.MaxWave + 1).ToString();
        Ui_Waves_Now.text = Language.Lang.wavesText.NowWave + ": " + (GameData.NowWave + 1).ToString();
    }
    public void SetGameWaves()
    {
        Ui_Game.SetActive(true);
        Ui_Waves.SetActive(false);
        StopBoxSpawn();
        StopBuffSpawn();
        StopMeteorSpawn();
        CloseLevelUp();
    }
    public void PlayWaves()
    {
        GameData.PrevPlayerLevel = GameData.PlayerLevel;
        StartCoroutine(PlayWavesCour());

        PlaySound("Play");
        Vibration.Vibrate(0.25f);
        OnGamePlayed();
    }
    private IEnumerator PlayWavesCour()
    {
        anim.SetTrigger("Fade");
        yield return new WaitForSeconds(0.3f);
        SetState(WavesState);
        yield break;
    }

    public void OnWavesDone(float Delay)
    {
        StartCoroutine(OnWaveDoneCour(Delay));
    }
    private IEnumerator OnWaveDoneCour(float Delay)
    {
        yield return new WaitForSeconds(Delay);
        CurrantState.Action();
        yield break;
    }
    #endregion
    #region Sound
    public void PlaySound(string name)
    {
        ClipInfo info = GameData.active.GetSoundRand(name);
        StartCoroutine(PlaySoundCour(info, 0f));
    }
    public void PlaySound(string name, float delay)
    {
        ClipInfo info = GameData.active.GetSoundRand(name);
        StartCoroutine(PlaySoundCour(info, delay));
    }
    private IEnumerator PlaySoundCour(ClipInfo info, float Delay)
    {
        yield return new WaitForSeconds(Delay);
        AudioSource source = gameObject.AddComponent<AudioSource>();

        source.clip = info.Clip;
        source.spatialBlend = 0f;
        source.volume = info.Volume * GameData.EffectVol;
        source.Play();
        while (source.isPlaying)
        {
            yield return new WaitForFixedUpdate();
        }

        Destroy(source);

        yield break;
    }

    public void PlayMusic()
    {
        ClipInfo info = GameData.active.GetMusicRand();
        musicSource.clip = info.Clip;
        musicSource.volume = GameData.MusicVol * 0.5f;
        musicSource.Play();
    }
    private IEnumerator MusicCheckCour()
    {
        while(true)
        {
            while(musicSource.isPlaying)
            {
                yield return new WaitForFixedUpdate();
            }
            PlayMusic();
            yield return new WaitForFixedUpdate();
        }
    }
    #endregion
    #region KnightBattle
    public void SetKnightsWavesUi()
    {
        Ui_Knights_Max.text = Language.Lang.knightsText.MaxWave + ": " + (GameData.MaxStage + 1).ToString();
        Ui_Knights_Now.text = Language.Lang.knightsText.NowWave + ": " + (GameData.NowStage + 1).ToString();
    }
    public void PlayKnightsWaves()
    {
        if(GameData.active.GameType[NowGameType].Opened)
        {
            GameData.PrevPlayerLevel = GameData.PlayerLevel;
            Ui_Knights.SetActive(false);
            Ui_Game.SetActive(true);
            CurrantState.Action();
            PlaySound("Play");
            Vibration.Vibrate(0.25f);
            OnGamePlayed();
        }
        else if(GameData.active.GameType[NowGameType].Premium)
        {
            StoreOn(0.5f);
        }
        else
        {

        }
    }

    public void OnKnightsWavesDone(float Delay)
    {
        StartCoroutine(OnKnightsWaveDoneCour(Delay));
    }
    private IEnumerator OnKnightsWaveDoneCour(float Delay)
    {
        yield return new WaitForSeconds(Delay);
        CurrantState.Action();
        yield break;
    }
    #endregion
    #region GameStates
    //--------------------------------GameStates----------------------------
    public void SetPlayerMenu()
    {
        MainPlayer.transform.position = PlayerMenuPos.position + Vector3.up;
        MainPlayer.Rig.velocity = Vector2.zero;
        MainPlayer.Rig.angularVelocity = 0f;
        MainPlayer.SetParams();
    }

    public void PlayGame()
    {
        if (GameData.LearningEnded)
        {
            SelectState(0);
        }
        else
        {
            PlayLearn();
        }

        PlaySound("Play");
        Vibration.Vibrate(0.25f);

        OnGamePlayed();
    }
    public void PlayFree()
    {
        SetState(FreeState);
    }
    public void PlayMenu(float Delay)
    {
        if (SelectStateCoroutine == null)
        {
            SelectStateCoroutine = StartCoroutine(PlayMenuCour(Delay));
        }
    }
    private IEnumerator PlayMenuCour(float Delay)
    {
        yield return new WaitForSeconds(Delay);
        SetGameType((GameType)NowGameType);
        SelectStateCoroutine = null;
        yield break;
    }
    private void CheckForLevelUp()
    {
        if(GameData.PlayerLevel > GameData.PrevPlayerLevel)
        {
            StartCoroutine(LevelUpCour());
        }
    }
    private IEnumerator LevelUpCour()
    {
        Ui_LevelUp.SetActive(true);
        Ui_LevelUp_Close.SetActive(false);
        Ui_LevelUp_WeaponUnlock.SetActive(false);
        Ui_LevelUp_From.text = GameData.PrevPlayerLevel.ToString();
        Ui_LevelUp_To.text = GameData.PlayerLevel.ToString();
        Ui_LevelUp_Text.text = Language.Lang.levelUpText.LevelUp;
        int Money = 0;
        for (int i = GameData.PrevPlayerLevel; i < GameData.PlayerLevel; i++)
        {
            Money += GameData.MoneyPerPlayerLevel(i);
        }
        UpdateMoney(Money);
        Ui_LevelUp_Money.text = "+" + Money.ToString();
        for (int i = 1; i < Ui_LevelUp_WeaponUnlockContent.transform.childCount; i++)
        {
            Destroy(Ui_LevelUp_WeaponUnlockContent.transform.GetChild(i).gameObject);
        }
        List<WeaponInfo> info = GameData.active.GetOpenedByLevelUp(GameData.PrevPlayerLevel, GameData.PlayerLevel, 0);
        Ui_LevelUp_WeaponIcons = new WeaponIcon[info.Count];
        Ui_LevelUp_WeaponUnlock.SetActive(info.Count > 0);
        for (int i = 0; i < info.Count; i++)
        {
            Ui_LevelUp_WeaponIcons[i] = Instantiate(GameData.active.IconPrefab, Ui_LevelUp_WeaponUnlockContent.transform);
            Ui_LevelUp_WeaponIcons[i].SetIconLevelUp(info[i]);
        }
        GameData.PrevPlayerLevel = GameData.PlayerLevel;
        GameData.Save();
        PlaySound("LevelUp", 0.25f);

        yield return new WaitForSeconds(2f);
        Ui_LevelUp_Close.SetActive(true);
        Ui_LevelUp_Text.text = Language.Lang.levelUpText.Close;
        yield break;
    }
    public void CloseLevelUp()
    {
        Ui_LevelUp.SetActive(false);
    }

    public void OnButtonClick()
    {
        PlaySound("Click");
    }

    public void PlayFastFade()
    {
        anim.SetTrigger("FastFade");
    }

    private void SetState(GameState state)
    {
        CurrantState = Instantiate(state);
        CurrantState.level = this;
        LevelType = state.Type;
        CurrantState.OnStart();

        GameData.Save();
    }
    private void SelectingState()
    {
        if (!CurrantState.isEnd)
        {
            CurrantState.Run();
        }
        else
        {
            SelectState(GameData.LevelDelay);
        }
    }
    private void SelectState(float Delay)
    {
        if (SelectStateCoroutine == null)
        {
            SelectStateCoroutine = StartCoroutine(SelectStateCour(Delay));
        }
        UpdatePlayerStatsUI();
    }
    private IEnumerator SelectStateCour(float Delay)
    {
        yield return new WaitForSeconds(Delay);
        anim.SetTrigger("Fade");
        yield return new WaitForSeconds(0.3f);

        Debug.Log(GameData.NowLevel);
        switch (GameData.NowLevel % 7)
        {
            case 0:
                SetState(EasyState);
                Debug.Log("Easy");
                break;
            case 1:
                SetState(DuelState);
                Debug.Log("Duel");
                break;
            case 2:
                switch (Random.Range(0, 4))
                {
                    case 0:
                        SetState(EasyState);
                        Debug.Log("R Easy");
                        break;
                    case 1:
                        SetState(LevelState);
                        Debug.Log("R Level");
                        break;
                    case 2:
                        SetState(DuelState);
                        Debug.Log("R Duel");
                        break;
                    case 3:
                        SetState(BattleState);
                        Debug.Log("R Duel");
                        break;
                }
                break;
            case 3:
                SetState(LevelState);
                Debug.Log("Level");
                break;
            case 4:
                SetState(BattleState);
                Debug.Log("Battle");
                break;
            case 5:
                switch (Random.Range(0, 4))
                {
                    case 0:
                        SetState(EasyState);
                        Debug.Log("R Easy");
                        break;
                    case 1:
                        SetState(LevelState);
                        Debug.Log("R Level");
                        break;
                    case 2:
                        SetState(DuelState);
                        Debug.Log("R Duel");
                        break;
                    case 3:
                        SetState(BattleState);
                        Debug.Log("R Duel");
                        break;
                }
                break;
            case 6:
                Debug.Log("Boss");
                SetState(BossState);
                break;
        }
        SelectStateCoroutine = null;
        yield break;
    }
    //------------------------------------------------------------------------
    #endregion
    #region GameTypes
    public void SetGame(bool on)
    {
        if(LevelType != LevelTypes.Menu)
        {
            Ui_Menu.SetActive(false);
            Ui_Bets.SetActive(false);
            Ui_Waves.SetActive(false);
            Ui_Amunition.SetActive(false);
            Ui_Game.SetActive(true);
            CloseLevelUp();
        }
        else
        {
            Ui_Menu.SetActive(true);
            Ui_Game.SetActive(false);
        }

        StopBoxSpawn();
        StopBuffSpawn();
        StopMeteorSpawn();
    }

    public void ChangeGameType(bool Right)
    {
        if(Right)
        {
            NowGameType++;
            if (NowGameType >= GameData.active.GameType.Length)
            {
                NowGameType = 0;
            }
        }
        else
        {
            NowGameType--;
            if (NowGameType < 0)
            {
                NowGameType = GameData.active.GameType.Length - 1;
            }
        }

        SetGameType();
        OnButtonClick();
    }
    private void SetGameType()
    {
        if(SetGameTypeCoroutine == null)
        {
            SetGameTypeCoroutine = StartCoroutine(SetGameTypeCour());
        }
        CloseLevelUp();
    }
    private void SetGameType(GameType type)
    {
        NowGameType = (int)type;

        if (SetGameTypeCoroutine == null)
        {
            SetGameTypeCoroutine = StartCoroutine(SetGameTypeCour());
        }
    }
    private IEnumerator SetGameTypeCour()
    {
        anim.SetTrigger("Fade");
        yield return new WaitForSeconds(0.25f);
        Ui_Menu.SetActive(false);
        Ui_Bets.SetActive(false);
        Ui_Waves.SetActive(false);
        Ui_Game.SetActive(false);
        Ui_Knights.SetActive(false);
        switch ((GameType)NowGameType)
        {
            case GameType.Levels:
                Ui_Menu.SetActive(true);
                SetState(MenuState);
                CheckForLevelUp();
                break;
            case GameType.Bets:
                Ui_Bets.SetActive(true);
                Ui_Bets_Slider.value = 0;
                SetBetMoney();
                SetState(BetsState);
                break;
            case GameType.Waves:
                Ui_Waves.SetActive(true);
                SetWavesUi();
                SetState(IdleBattle);
                break;
            case GameType.Knights:
                Ui_Knights.SetActive(true);
                SetKnightsWavesUi();
                SetState(KnightsState);
                break;
        }

        SetGameTypeCoroutine = null;
        yield break;
    }

    public void DelayGame(float Delay)
    {
        if(DelayGameCoroutine != null)
        {
            StopCoroutine(DelayGameCoroutine);
        }
        DelayGameCoroutine = StartCoroutine(DelayGameCour(Delay));
    }
    private IEnumerator DelayGameCour(float Delay)
    {
        GameData.GameStarted = false;
        yield return new WaitForSecondsRealtime(Delay);
        GameData.GameStarted = true;

        DelayGameCoroutine = null;
        yield break;
    }
    #endregion
    #region Ads
    public void PlayAds(AdMob.AdsTypes type)
    {
        if (GameData.PremiumOn || AdMob.ShowingAds)
            return;
        switch(type)
        {
            case AdMob.AdsTypes.Banner:
                AdMob.active.ShowBanner();
                break;
            case AdMob.AdsTypes.RewardedLife:
                AdMob.active.SetCallback(type);
                AdMob.active.ShowRewarded();
                break;
            case AdMob.AdsTypes.RewardedPresent:
                AdMob.active.SetCallback(type);
                AdMob.active.ShowRewarded();
                break;
            case AdMob.AdsTypes.Video:
                AdMob.active.ShowVideo();
                break;
        }
    }
    public void HideBanner()
    {
        AdMob.active.HideBanner();
    }
    public void ShowBanner()
    {
        if (GameData.PremiumOn)
            return;
        Debug.Log("Show");
        AdMob.active.ShowBanner();
    }
    #endregion
    #region Pause and Settings
    public void Pause()
    {
        OnPause = true;
        Time.timeScale = 0;
        Ui_Pause.SetActive(true);

        Ui_Pause_Music.value = GameData.MusicVol; 
        Ui_Pause_Volume.value = GameData.EffectVol;
        OnButtonClick();
    }
    public void Resume()
    {
        OnPause = false;
        Time.timeScale = 1;
        Ui_Pause.SetActive(false);
        OnButtonClick();
    }
    public void SpecialResume()
    {
        OnPause = false;
        Time.timeScale = 1;
        Ui_Pause.SetActive(false);
        Ui_Settings.SetActive(false);
    }
    public void SpecialPause()
    {
        OnPause = true;
        Time.timeScale = 1;
    }
    public void Restart()
    {
        StartCoroutine(RestartCour(0));
        OnButtonClick();
    }
    public void GoMainMenu()
    {
        PlayMenu(0f);
        Resume();
    }
    private IEnumerator RestartCour(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Resume();
        SetState(CurrantState);
        yield break;
    }

    public void SetEffectVolumeFromPause()
    {
        GameData.EffectVol = Ui_Pause_Volume.value;
    }
    public void SetMusicVolumeFromPause()
    {
        GameData.MusicVol = Ui_Pause_Music.value;
        musicSource.volume = GameData.MusicVol * 0.5f;
    }

    public void SettingsOn()
    {
        Ui_Settings.SetActive(true);

        Ui_Settings_Volume.value = GameData.EffectVol;
        Ui_Settings_Music.value = GameData.MusicVol;
    }
    public void SettingsOff()
    {
        Ui_Settings.SetActive(false);
        GameData.Save();
    }

    public void SetVibration()
    {
        GameData.Vibration = Ui_Setting_Vibration.isOn;
    }
    public void SetEffectVolume()
    {
        GameData.EffectVol = Ui_Settings_Volume.value;
    }
    public void SetMusicVolume()
    {
        GameData.MusicVol = Ui_Settings_Music.value;
        musicSource.volume = GameData.MusicVol * 0.5f;
    }
    #endregion
    #region PremiumStoreAndPresent
    public void StoreOn()
    {
        if(GameData.PremiumOn)
        {
            AlreadyBought();
        }
        else
        {
            Ui_Store.SetActive(true);

            for (int i = 0; i < Ui_Store_Weapon.childCount; i++)
            {
                Destroy(Ui_Store_Weapon.GetChild(i).gameObject);
            }
            List<WeaponInfo> info = GameData.active.GetPremiumWeapon();
            for (int i = 0; i < info.Count; i++)
            {
                WeaponIcon icon = Instantiate(GameData.active.IconPrefab, Ui_Store_Weapon);
                icon.SetIconLevelUp(info[i]);
            }
            HideBanner();
        }
    }
    public void StoreOn(float delay)
    {
        StartCoroutine(StoreOnCour(delay));
    }
    private IEnumerator StoreOnCour(float delay)
    {
        yield return new WaitForSeconds(delay);
        StoreOn();
        yield break;
    }
    public void StoreOff()
    {
        Ui_Store.SetActive(false);
        ShowBanner();
    }

    private void SwitchPremiumButton()
    {
        Ui_StoreOffButton.SetActive(!GameData.PremiumOn);
        Ui_StoreOnButton.SetActive(GameData.PremiumOn);
    }

    public void Buy()
    {
        if(GameData.PremiumOn)
        {
            StoreOff();
            return;
        }
        OnBought();
        StartCoroutine(OnBoughtCour());
    }
    public void OnBought()
    {
        PremiusInfo info = GameData.active.premiumInfo;
        for (int i = 0; i < info.WeaponOpen.Length; i++)
        {
            GameData.active.weapon[info.WeaponOpen[i]].Opened = true;
        }
        for (int i = 0; i < info.ArmorOpen.Length; i++)
        {
            GameData.active.armor[info.ArmorOpen[i]].Opened = true;
        }
        for (int i = 0; i < info.GameTypeOpen.Length; i++)
        {
            GameData.active.GameType[info.GameTypeOpen[i]].Opened = true;
        }
        UpdateMoney(info.Money);
        GameData.PremiumOn = true;
        GameData.Save();

        if (GameData.active.weapon[TempWeapon].Premium)
        {
            SelectWeapon(TempWeapon, GameData.NowWeaponPlace);
        }
        if (GameData.active.tempArmorInfo.Premium)
        {
            NowArmor = GameData.active.tempArmorInfo.Index;
            SetArmor();
        }
        GameData.ExpRatio = 1;

        AdMob.active.DestroyBanner();
        SwitchPremiumButton();
        Vibration.WinVibrate(0);
        PlaySound("Buy");
    }
    private IEnumerator OnBoughtCour()
    {
        Ui_Store_Thanks.SetActive(true);
        yield return new WaitForSeconds(2f);
        Ui_Store_Thanks.SetActive(false);
        StoreOff();
        yield break;
    }

    private void AlreadyBought()
    {
        StartCoroutine(AlreadyBoughtCour());
    }
    private IEnumerator AlreadyBoughtCour()
    {
        Ui_StoreAlready.SetActive(true);
        yield return new WaitForSeconds(4f);
        Ui_StoreAlready.SetActive(false);
        yield break;
    }

    public void NoThanks()
    {
        StoreOff();

        if (GameData.active.weapon[TempWeapon].Premium)
        {
            SelectNowWeapon();
        }
        if(GameData.active.tempArmorInfo.Premium)
        {
            GameData.active.SetTempArmorInfo(GameData.active.armor[GameData.NowArmor]);
            Ui_Anum_ArmorCosts.SetActive(false);
            Ui_Anum_ArmorButton.enabled = false;
            Ui_Anum_ArmorName.text = GameData.active.tempArmorInfo.Name;
            MainPlayer.SetParams(GameData.active.playerInfo, GameData.active.tempArmorInfo);
        }
    }

    public void TakePresent()
    {
        if (GameData.GetTimeToPresent() <= 0)
        {
            if(GameData.PremiumOn || NoInternet())
            {
                OnPresentTaken();
            }
            else
            {
                PlayAds(AdMob.AdsTypes.RewardedPresent);
            }
        }
    }
    public void OnPresentTaken()
    {
        GameData.SetPresentTime(2f);
        Ui_Menu_Present.SetActive(true);
        int Money = Random.Range(GameData.active.MonetForPresent - 100, GameData.active.MonetForPresent + 100);
        UpdateMoney(Money);
        Ui_Menu_Timer.text = Money.ToString();
        Ui_Menu_PresentWeapon.SetActive(false);
        if (Random.Range(0, 5) == 0)
        {
            Weapon weapon = GameData.active.GetRandomPresentWeapon();
            if (weapon != null)
            {
                Ui_Menu_PresentWeapon.SetActive(true);
                if(Ui_Menu_PresentWeaponContent.transform.childCount > 0)
                {
                    Destroy(Ui_Menu_PresentWeaponContent.transform.GetChild(0).gameObject);
                }
                WeaponIcon icon = Instantiate(GameData.active.IconPrefab, Ui_Menu_PresentWeaponContent.transform);
                icon.SetIconLevelUp(GameData.active.weapon[weapon.Index]);
                GameData.active.weapon[weapon.Index].Opened = true;
                GameData.Save();
            }
        }

        PlaySound("TakeCoin");
        anim.SetTrigger("Present");
        Ui_Menu_PresentMoneyText.text = Money.ToString();
    }
    #endregion
    #region Learning
    public void PlayLearn()
    {
        StartCoroutine(PlayLearnCour());
    }
    private IEnumerator PlayLearnCour()
    {
        anim.SetTrigger("Fade");
        yield return new WaitForSeconds(0.3f);
        SetState(LearnState);
        yield break;
    }
    public void SkipLearn()
    {
        GameData.LearningEnded = true;
        if (GameData.NowLevel == 0)
        {
            SetState(LevelState);
        }
        else if (GameData.NowLevel % 2 == 0)
        {
            SetState(BossState);
        }
        else
        {
            SetState(LevelState);
        }
    }
    public void NextLearnState(float Delay)
    {
        if (NextLearnStateCoroutine == null)
        {
            NextLearnStateCoroutine = StartCoroutine(NextLearnStateCour(Delay));
        }
    }
    private IEnumerator NextLearnStateCour(float Delay)
    {
        yield return new WaitForSeconds(Delay);
        CurrantState.Action();
        NextLearnStateCoroutine = null;
        yield break;
    }
    #endregion

    private void CheckButtonClick()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(InGame())
            {
                Pause();
            }
            else if(Ui_Amunition.activeSelf)
            {
                BackAmunition();
            }
            else if(Ui_Settings.activeSelf)
            {
                SettingsOff();
            }
        }
    }
    private void OnApplicationPause(bool pause)
    {
        if(pause && Loaded && !AdMob.ShowingAds)
        {
            Pause();
        }
        if(!pause && !InGame())
        {
            Resume();
        }
    }
    private void OnApplicationQuit()
    {
        GameData.Save();
    }

    public void Awake()
    {
        Init();
    }
    public void Start()
    {
        MainPlayer = Player.active;
        PlayerScript = MainPlayer.GetComponent<Player>();
        SetState(MenuState);
        LateInit();
    }
    public void FixedUpdate()
    {
        GameStuff();
        SelectingState();
    }
    public void Update()
    {
        CheckButtonClick();
    }
}

public static class SideOwn
{
    public static bool isEnemy(Man man1, Man man2)
    {
        if (man1 == null || man2 == null)
            return false;
        if (man1.Type == Man.ManType.Menu || man2.Type == Man.ManType.Menu)
            return true;
        if ((man1.Type == Man.ManType.Boss && man2.Type == Man.ManType.Enemy) ||
           (man2.Type == Man.ManType.Boss && man1.Type == Man.ManType.Enemy))
            return false;
        else
            return man1.Type != man2.Type;
    }
    public static bool isFriend(Man man1, Man man2)
    {
        if (man1 == null || man2 == null)
            return false;
        if (man1.Type == Man.ManType.Menu || man2.Type == Man.ManType.Menu)
            return false;
        if ((man1.Type == Man.ManType.Boss && man2.Type == Man.ManType.Enemy) ||
            (man2.Type == Man.ManType.Boss && man1.Type == Man.ManType.Enemy))
            return true;
        else
            return man1.Type == man2.Type;
    }
}