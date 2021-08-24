﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{

    public static GameData active;
    //-------GameData------------
    public static int NowLevel;
    public static int NowWeapon;
    public static int NowArmor;
    public static int AttempForLevel;
    public static int NowWave;
    public static int MaxWave;
    public static void IncreaseMaxWave()
    {
        if(NowWave > MaxWave)
        {
            MaxWave = NowWave;
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
    public void SetTempArmorInfo(ArmorInfo info)
    {
        tempArmorInfo = info;
    }
    public int UpdateCost(int level)
    {
        return Mathf.RoundToInt(Mathf.Sqrt(level + 1) * 100);
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
        return playerInfo.Size / upInfo.MaxSize * 0.65f;
    }
    public float PowerSecondProcent()
    {
        return playerInfo.Size * (tempArmorInfo.Power + 1) / upInfo.MaxSize * 0.65f;
    }

    public UpInfo upInfo;
    public static int PlayerExperience;
    public static int NextLevelExperience()
    {
        return Mathf.RoundToInt(Mathf.Pow(PlayerLevel + 1, 0.75f) * 20);
    }
    public static int PlayerLevel;
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
        return Mathf.RoundToInt(Mathf.Sqrt(level + 1) * 250);
    }
    public static int Money;
    [Header("Man's")]
    public EnemyInfo[] Enemy;
    [Header("Attack Phase")]
    public string[] AttackPhase;
    [Header("Damage Phase")]
    public string[] DamagePhase;
    [Header("Armor")]
    public ArmorInfo[] armor;
    [Header("Weapon")]
    public WeaponIcon IconPrefab;
    public WeaponInfo[] weapon;
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
    public List<WeaponInfo> GetOpenedByLevelUp(int from, int to)
    {
        List<WeaponInfo> info = new List<WeaponInfo>();

        for (int i = 0; i < weapon.Length; i++)
        {
            if(weapon[i].RequiredLevel > from &&
               weapon[i].RequiredLevel <= to)
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
    public Weapon GetWeapon(int index)
    {
        return weapon[index].weapon;
    }
    public Weapon GetSelectedWeapon()
    {
        return weapon[NowWeapon].weapon;
    }
    public Weapon GetRandomWeapon()
    {
        return weapon[Random.Range(0, weapon.Length)].weapon;
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
            NowWeapon = 14;
            Money = 1000243;
            GameHard = 0.8f;
            AttempForLevel = 1;
        }
        else
        {
            NowWave = data.NowWave;
            MaxWave = data.MaxWave;
            NowLevel = data.level;
            NowWeapon = data.NowWeapon;
            NowArmor = data.NowArmor;
            LearningEnded = data.LearningEnded;
            playerInfo = data.PlayerData;
            GameHard = data.GameHard;
            AttempForLevel = data.LevelAttempt;

            DeathNum = data.DeathNum;
            MansKilled = data.MansKilled;
            Money = data.Money;
            PlayerLevel = data.PlayerLevel;
            PlayerExperience = data.PlayerExperience;

            for(int i = 0; i < data.WeaponOpened.Length; i++)
            {
                weapon[i].Opened = data.WeaponOpened[i];
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
    public bool Opened;

    public float Hp;
    public float Power;
    public float Size;
    public float Speed;
    public float Jump;
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