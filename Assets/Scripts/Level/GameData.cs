using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{

    public static GameData active;
    //-------------GameData--------------
    public static int NowLevel { get; private set; }
    public static void IncreaseNowLevel()
    {
        NowLevel++;
        GameGlobalHard = 0.5f + (float)NowLevel > 15 ? 0.5f : (float)NowLevel / 30;
    }
    public static int NowWeapon;
    public static int NowWeaponPlace;
    public static int NowArmor;
    public static int NowKnightEnemyArmor;
    public static int AttempForLevel;
    public static int NowWave;
    public static int MaxWave;
    public static void IncreaseMaxWave()
    {
        if (NowWave > MaxWave)
        {
            MaxWave = NowWave;
        }
    }
    public static int NowStage;
    public static int MaxStage;
    public static void IncreaseMaxStage()
    {
        if (NowStage > MaxStage)
        {
            MaxStage = NowStage;
        }
    }
    public void IncreaseAttempt()
    {
        AttempForLevel++;
        GameHard = 1 / (AttempForLevel * 0.25f + 1);
    }
    public void DecreaseAttempt()
    {
        AttempForLevel--;
        if (AttempForLevel < 0)
            AttempForLevel = 0;
        GameHard = 1 / (AttempForLevel * 0.25f + 1);
    }
    public static int BossCompleated;
    public static int DeathNum;
    public static void IncreaseDeath()
    {
        DeathNum++;
    }

    public static int Language;

    //----------------Time-----------------------
    public static System.DateTime PresentTime;
    public static void SetPresentTime(float Hours)
    {
        System.DateTime nowtime = System.DateTime.Now;
        PresentTime = nowtime.AddSeconds(Hours * 60 * 60);
    }
    public static float GetTimeToPresent() //In seconds
    {
        System.TimeSpan time = PresentTime - System.DateTime.Now;
        return time.Hours * 60 * 60 + time.Minutes * 60 + time.Seconds;
    }
    //-------------Settings--------------
    public static bool PremiumOn;
    public static float ExpRatio;
    public static float EffectVol;
    public static float MusicVol;
    public static bool Vibration;
    [Header("Game Stats")]
    public int[] MansKilled;
    public static void IncreaseManKilled(Man.HitType hit)
    {
        if(active.MansKilled.Length == 0)
        {
            active.MansKilled = new int[9];
        }
        active.MansKilled[(int)hit]++;
    }
    public static float Speed;
    public static float Acceleration;
    public static int NowGround;
    public static bool LearningEnded;
    public static bool GameStarted;
    //--------Settings-----------
    public static bool GameDelay;
    public const float LevelDelay = 3;
    //---------------------------
    [Header("Game Setting's")]
    [Range(0, 1f)]
    public float GameHard;
    public static float GameGlobalHard;
    [Header("Premium")]
    public PremiusInfo premiumInfo;
    [Header("Game Types")]
    public GameTypeInfo[] GameType;
    [Header("Player")]
    public PlayerInfo playerInfo;
    public ArmorInfo armorInfo;
    public ArmorInfo SetArmorInfo()
    {
        armorInfo = armor[NowArmor];
        return armorInfo;
    }
    public ArmorInfo tempArmorInfo;
    public ArmorInfo GetRandomArmor()
    {
        return armor[Random.Range(0, armor.Length)];
    }
    public ArmorInfo GetEnemyArmor()
    {
        return armor[NowKnightEnemyArmor];
    }
    public void SetRandomEnemyArmor()
    {
        NowKnightEnemyArmor = Random.Range(0, armor.Length);
        while(NowKnightEnemyArmor == NowArmor)
        {
            NowKnightEnemyArmor = Random.Range(0, armor.Length);
        }
    }
    public void SetTempArmorInfo(ArmorInfo info)
    {
        tempArmorInfo = info;
    }
    public int UpdateCost(int level)
    {
        return Mathf.RoundToInt(Mathf.Pow(level + 1, 0.75f) * 100);
    }

    public void UpdateHp()
    {
        playerInfo.MaxHp += upInfo.HpAdd;
        if(playerInfo.MaxHp > upInfo.MaxHp)
        {
            playerInfo.MaxHp = upInfo.MaxHp;
        }

        Save();
    }
    public void UpdateSize()
    {
        playerInfo.Size += upInfo.SizeAdd;
        if (playerInfo.Size > upInfo.MaxSize)
        {
            playerInfo.Size = upInfo.MaxSize;
        }

        Save();
    }
    public void UpdateSpeed()
    {
        playerInfo.Speed += upInfo.SpeedAdd;
        if (playerInfo.Speed > upInfo.MaxSpeed)
        {
            playerInfo.Speed = upInfo.MaxSpeed;
        }

        Save();
    }
    public void UpdateJump()
    {
        playerInfo.JumpForce += upInfo.JumpAdd;
        if (playerInfo.JumpForce > upInfo.MaxJump)
        {
            playerInfo.JumpForce = upInfo.MaxJump;
        }

        Save();
    }
    public void UpdatePower()
    {
        playerInfo.Power += upInfo.PowerAdd;
        if (playerInfo.Power > upInfo.MaxPower)
        {
            playerInfo.Power = upInfo.MaxPower;
        }

        Save();
    }
    public float HpProcent()
    {
        return (float)playerInfo.MaxHp / (float)upInfo.MaxHp * 0.65f;
    }
    public float HpSecondProcent()
    {
        return (float)playerInfo.MaxHp * (tempArmorInfo.Hp + 1) / (float)upInfo.MaxHp * 0.65f;
    }
    public float SpeedProcent()
    {
        return playerInfo.Speed / upInfo.MaxSpeed * 0.65f;
    }
    public float SpeedSecondProcent()
    {
        return playerInfo.Speed * (tempArmorInfo.Speed + 1) / upInfo.MaxSpeed * 0.65f;
    }
    public float SizeProcent()
    {
        return playerInfo.Size / upInfo.MaxSize * 0.65f;
    }
    public float SizeSecondProcent()
    {
        return playerInfo.Size * (tempArmorInfo.Size + 1) / upInfo.MaxSize * 0.65f;
    }
    public float PowerProcent()
    {
        return playerInfo.Power / upInfo.MaxPower * 0.65f;
    }
    public float PowerSecondProcent()
    {
        return playerInfo.Size * (tempArmorInfo.Power + 1) / upInfo.MaxSize * 0.65f;
    }

    public UpInfo upInfo;
    public static int PlayerExperience;
    public static int NextLevelExperience()
    {
        return Mathf.RoundToInt(Mathf.Pow(PlayerLevel + 1, 0.75f) * 25);
    }
    public static int PlayerLevel;
    public static int PrevPlayerLevel;
    public static void UpdateExperience(int Up)
    {
        PlayerExperience += Up;
        if(PlayerExperience >= NextLevelExperience())
        {
            PlayerExperience = 0;
            PlayerLevel++;
            Level.active.OnLevelUp();
            Save();
        }
    }
    public static void UpdateMoney(int Up)
    {
        Money += Up;
    }
    public static int MoneyPerPlayerLevel(int level)
    {
        return Mathf.RoundToInt(Mathf.Sqrt(level + 1) * 150);
    }
    public static int Money;
    [Header("Game Info")]
    public int MonetForPresent;
    [Header("Man's")]
    public EnemyInfo[] Enemy;
    public EnemyInfo[] Friend;
    [Header("Attack Phase")]
    public string[] AttackPhase;
    [Header("Damage Phase")]
    public string[] DamagePhase;
    [Header("Armor")]
    public ArmorInfo[] armor;
    public ArmorInfo GetManArmor()
    {
        return new ArmorInfo(ManHead[Random.Range(0, ManHead.Length)], ManBody[Random.Range(0, ManBody.Length)], ManLLeg0, ManLLeg1, ManRLeg0, ManRLeg1);
    }
    public Sprite[] ManHead;
    public Sprite[] ManBody;
    public Sprite ManLLeg0;
    public Sprite ManLLeg1;
    public Sprite ManRLeg0;
    public Sprite ManRLeg1;
    [Header("Weapon")]
    public WeaponIcon IconPrefab;
    public WeaponInfo[] weapon;
    public AnimationCurve WeaponRand;
    public void OpenWeapon(int index)
    {
        weapon[index].Opened = true;
        Level.active.UpdateMoney(-weapon[index].Cost);
    }
    public WeaponInfo[] GetWeaponSortedByCost()
    {
        WeaponInfo[] info = (WeaponInfo[])weapon.Clone();

        for(int i = 0; i < info.Length - 1; i++)
        {
            for(int a = i + 1; a < info.Length; a++)
            {
                if(info[a].Cost < info[i].Cost)
                {
                    WeaponInfo temp = info[i];
                    info[i] = info[a];
                    info[a] = temp;
                }
            }
        }
        return info;
    }
    public WeaponInfo[] GetWeaponSortedByLevel()
    {
        WeaponInfo[] info = (WeaponInfo[])weapon.Clone();

        for (int i = 0; i < info.Length - 1; i++)
        {
            for (int a = i + 1; a < info.Length; a++)
            {
                if (info[a].RequiredLevel < info[i].RequiredLevel)
                {
                    WeaponInfo temp = info[i];
                    info[i] = info[a];
                    info[a] = temp;
                }
            }
        }
        return info;
    }
    public List<WeaponInfo> GetPremiumWeapon()
    {
        WeaponInfo[] info = GetWeaponSortedByLevel();
        List<WeaponInfo> list = new List<WeaponInfo>();
        for (int i = 0; i < info.Length; i++)
        {
            if(info[i].Premium)
            {
                list.Add(info[i]);
            }
        }
        return list;
    }
    public List<WeaponInfo> GetOpenedByLevelUp(int from, int to, int add)
    {
        List<WeaponInfo> info = new List<WeaponInfo>();

        for (int i = 0; i < weapon.Length; i++)
        {
            if(weapon[i].RequiredLevel > from &&
               weapon[i].RequiredLevel <= to + add)
            {
                info.Add(weapon[i]);
            }
        }
        return info;
    }
    public bool GetAvalibleWeapon(int Index)
    {
        return weapon[Index].RequiredLevel <= PlayerLevel;
    }
    public WeaponInfo GetWeaponInfo(Weapon obj)
    {
        for(int i = 0; i < weapon.Length; i++)
        {
            if (weapon[i].weapon.Index == obj.Index)
                return weapon[i];
        }
        return this.weapon[0];
    }
    public Weapon GetWeapon(int index)
    {
        return weapon[index].weapon;
    }
    public Weapon GetSelectedWeapon()
    {
        return weapon[NowWeapon].weapon;
    }
    public Weapon GetLastOpenedWeapon()
    {
        WeaponInfo temp = weapon[NowWeapon];
        for (int i = 0; i < weapon.Length; i++)
        {
            if (weapon[i].Opened && weapon[i].RequiredLevel > temp.RequiredLevel)
                temp = weapon[i];
        }
        return temp.weapon;
    }
    public Weapon GetRandomWeapon()
    {
        int index = Mathf.RoundToInt(WeaponRand.Evaluate(Random.Range(0f, 1f)) * (weapon.Length - 1));
        return weapon[index].weapon;

    }
    public Weapon GetRandomOpened()
    {
        List<WeaponInfo> info = new List<WeaponInfo>();
        for(int i = 0; i < weapon.Length; i++)
        {
            if(weapon[i].Opened)
            {
                info.Add(weapon[i]);
            }
        }

        return info[Random.Range(0, info.Count)].weapon;
    }
    [Header("Sounds")]
    public SoundInfo[] soundInfo;
    public SoundInfo[] musicInfo;
    public ClipInfo GetSound(string name, int num)
    {
        for (int i = 0; i < soundInfo.Length; i++)
        {
            if (soundInfo[i].Name == name)
            {
                AudioClip clip = num < soundInfo[i].Clip.Length ? soundInfo[i].Clip[num] : soundInfo[i].Clip[0];
                return new ClipInfo(clip, soundInfo[i].Volume);
            }
        }
        return new ClipInfo(soundInfo[0].Clip[0], 0);
    }
    public ClipInfo GetSoundRand(string name)
    {
        for (int i = 0; i < soundInfo.Length; i++)
        {
            if (soundInfo[i].Name == name)
            {
                return new ClipInfo(soundInfo[i].Clip[Random.Range(0, soundInfo[i].Clip.Length)], soundInfo[i].Volume);
            }
        }
        return new ClipInfo(soundInfo[0].Clip[0], 0);
    }
    public ClipInfo GetMusic(int num)
    {
        return new ClipInfo(musicInfo[num].Clip[0], musicInfo[num].Volume);
    }
    public ClipInfo GetMusicRand()
    {
        int num = Random.Range(0, musicInfo.Length);
        return new ClipInfo(musicInfo[num].Clip[0], musicInfo[num].Volume);
    }

    [Header("Effects")]
    public EffectInfo[] Effects;
    public GameObject GetEffect(string name)
    {
        for(int i = 0; i < Effects.Length; i++)
        {
            if (Effects[i].Name == name)
                return Effects[i].Effect;
        }
        return null;
    }
    public Color GetGroundColor()
    {
        return Ground[NowGround].color;
    }
    public void CreateLandEffect(Transform transform, float Size)
    {
        Vector2 Position = new Vector2(transform.position.x, -0.1f);
        ParticleSystem ThisEffect = Instantiate(GetEffect("Collide"), Position, Quaternion.identity, transform).GetComponent<ParticleSystem>();
        ThisEffect.transform.localScale = new Vector3(Size + 1, 1, 1);
        var Main = ThisEffect.main;
        Main.startColor = Ground[NowGround].color;
    }
    [Header("Scene Look")]
    public GroundInfo[] Ground;
    [Header("Buffs")]
    public BuffInfo[] Buffs;
    public Buff GetRandomBuff()
    {
        return Buffs[Random.Range(0, Buffs.Length)].obj;
    }
    [Header("Other")]
    public Experience Exp;
    public Coin coin;
    [Header("Colors")]
    public Color WeaponTrail;
    public Color WeaponRedTrail;
    public Color FastRun;
    public Color[] MansColor;
    public Color GetRandColorHVS()
    {
        Color HVS = Color.HSVToRGB((float)Random.Range(25, 60) / 360f, (float)Random.Range(30, 70) / 100f, (float)Random.Range(30, 70) / 100f, true);
        return new Color(HVS.r, HVS.g, HVS.b, 0);
    }
    public Color GetRandColor()
    {
        return MansColor[Random.Range(0, MansColor.Length)];
    }
    public Color[] IconWeaponColor;

    public static Color ClearColor(Color color)
    {
        return new Color(color.r, color.g, color.b, 0);
    }
    //------------------------------SaveLoad--------------------------------------
    public static void Save()
    {
        SaveSystem.Save();
    }
    private void Load()
    {
        SaveData data = SaveSystem.Load();
        if (data == null)
        {
            NowWeapon = 0;
            Money = 100;
            GameHard = 0.5f;
            AttempForLevel = 3;
            Vibration = true;
            MusicVol = 1f;
            EffectVol = 1f;
            GameGlobalHard = 0.5f;
            PresentTime = System.DateTime.Now;
        }
        else
        {
            Language = data.Language;
            ExpRatio = data.ExpRatio;
            NowWave = data.NowWave;
            MaxWave = data.MaxWave;
            NowStage = data.NowStage;
            MaxStage = data.MaxStage;
            NowLevel = data.level;
            NowWeapon = data.NowWeapon;
            NowWeaponPlace = data.NowWeaponPlace;
            NowArmor = data.NowArmor;
            LearningEnded = data.LearningEnded;
            playerInfo = data.PlayerData;
            GameHard = data.GameHard;
            GameGlobalHard = data.GameGlobalHard;
            AttempForLevel = data.LevelAttempt;
            PremiumOn = data.PremiumOn;

            Vibration = data.Vibration;
            MusicVol = data.MusicVol;
            EffectVol = data.EffectVol;


            PresentTime = data.PresentTime;

            DeathNum = data.DeathNum;
            MansKilled = data.MansKilled;
            Money = data.Money;
            PlayerLevel = data.PlayerLevel;
            PrevPlayerLevel = data.PrevPlayerLevel;
            PlayerExperience = data.PlayerExperience;

            for(int i = 0; i < data.WeaponOpened.Length; i++)
            {
                weapon[i].Opened = data.WeaponOpened[i];
            }
            for(int i = 0; i < data.ArmorOpened.Length; i++)
            {
                armor[i].Opened = data.ArmorOpened[i];
            }
            for(int i = 0; i < data.GameTypeOpened.Length; i++)
            {
                GameType[i].Opened = data.GameTypeOpened[i];
            }
        }
    }
    //-----------------------------------------------------------------------------
    private void Init()
    {
        active = this;
        Speed = 1;
    }
    private void Awake()
    {
        Init();
        Load();
    }
    private void Start()
    {

    }
}
[System.Serializable]
public struct GameTypeInfo
{
    public string Name;
    public bool Opened;
    public bool Premium;
    public Level.GameType type;
    public int Index;
}
[System.Serializable]
public struct PlayerInfo
{
    public string Name;

    public int MaxHp;
    public int HpNum;
    public float Speed;
    public int SpeedNum;
    public float JumpForce;
    public int JumpNum;
    public float Power;
    public int PowerNum;
    public float Size;
    public float SizeNum;

    public float MaxHpBuff;
    public float SpeedBuff;
    public float JumpForceBuff;
    public float PowerBuff;
    public float SizeBuff;

    
}
[System.Serializable]
public struct UpInfo
{
    public int MaxHp;
    public int HpAdd;
    public int StandartHp;

    public float MaxSize;
    public float SizeAdd;
    public int StandartSize;

    public float MaxPower;
    public float PowerAdd;
    public float StandartPower;

    public float MaxSpeed;
    public float SpeedAdd;
    public int StandartSpeed;

    public float MaxJump;
    public float JumpAdd;
    public int StandartJump;
}

[System.Serializable]
public struct EnemyInfo
{
    public string Name;
    public int Index;
    public Man Enemy;
}
[System.Serializable]
public struct WeaponInfo
{
    public string Name;
    public int Index;
    public int Cost;
    public int RequiredLevel;
    public bool Opened;
    public bool Premium;
    public Weapon weapon;
    public Sprite Icon;
    public enum RareType { Common, Rare, Epic, Legendary }
    public RareType Rare;
    public int Place;
}
[System.Serializable]
public struct ArmorInfo
{
    public enum EffectType { Null, Fire }
    public string Name;
    public int Index;

    [Header("Sprites")]
    public Sprite Head;
    public int HeadLayer;

    public Sprite Body;
    public int BodyLayer;

    public Sprite LeftLeg0;
    public int LeftLeg0Layer;

    public Sprite LeftLeg1;
    public int LeftLeg1Layer;

    public Sprite RightLeg0;
    public int RightLeg0Layer;

    public Sprite RightLeg1;
    public int RightLeg1Layer;

    [Header("Params")]
    public EffectType Effect;
    public WeaponInfo.RareType Rare;
    public int RequiredLevel;
    public int Cost;
    public bool Premium;
    public bool Opened;

    public float Hp;
    public float Power;
    public float Size;
    public float Speed;
    public float Jump;

    public ArmorInfo(Sprite Head, Sprite Body, Sprite LeftLeg0, Sprite LeftLeg1, Sprite RightLeg0, Sprite RightLeg1)
    {
        Index = -1;
        Effect = EffectType.Null;
        Rare = WeaponInfo.RareType.Common;
        RequiredLevel = 0;
        Cost = 0;
        Opened = false;
        Premium = false;

        this.Name = "Custom";
        this.Body = Body;
        BodyLayer = 5;
        this.Head = Head;
        HeadLayer = 7;
        this.LeftLeg0 = LeftLeg0;
        LeftLeg0Layer = 2;
        this.LeftLeg1 = LeftLeg1;
        LeftLeg1Layer = 3;
        this.RightLeg0 = RightLeg0;
        RightLeg0Layer = 2;
        this.RightLeg1 = RightLeg1;
        RightLeg1Layer = 3;

        Hp = 0;
        Power = 0;
        Size = 0;
        Speed = 0;
        Jump = 0;
    }
}
[System.Serializable]
public struct EffectInfo
{
    public string Name;
    public int Index;
    public GameObject Effect;
}
[System.Serializable]
public struct SceneInfo
{
    public GameObject[] BackGround;
    public GameObject[] SceneObject;
    
}
[System.Serializable]
public struct GroundInfo
{
    public string Name;
    public int Index;
    public Sprite Ground;
    public Color color;
    public Color UiBackColor;

    public GameObject[] FarBackGround;
    public GameObject[] BackGround;
    public GameObject[] SceneObject;

    [Range(0, 1)]
    public float Speed;

    [Range(0, 1)]
    public float Acceleration;
}
[System.Serializable]
public struct BuffInfo
{
    public string Name;
    public int Index;
    public Buff obj;
}
[System.Serializable] 
public struct PremiusInfo
{
    public int[] WeaponOpen;
    public int[] ArmorOpen;
    public int[] GameTypeOpen;
    public bool NoAdd;
    public int Money;
}
[System.Serializable]
public struct SoundInfo
{
    public string Name;
    public AudioClip[] Clip;
    [Range(0, 1f)]
    public float Volume;
}
[System.Serializable]
public struct ClipInfo
{
    public AudioClip Clip;
    public float Volume;

    public ClipInfo(AudioClip clip, float volume)
    {
        Clip = clip;
        Volume = volume;
    }
}