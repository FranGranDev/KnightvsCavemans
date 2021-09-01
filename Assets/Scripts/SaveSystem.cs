using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveSystem
{
    public static void Save()
    {
        GameData game = GameData.active;
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/GameData.blin";
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData(game);  

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static SaveData Load()
    {
        string path = Application.persistentDataPath + "/GameData.blin";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();
            return data;
        }
        else
        {
            return null;
        }
    }
}
[System.Serializable]
public class SaveData
{
    public int PlayerLevel;
    public int PrevPlayerLevel;
    public int PlayerExperience;
    public int Money;
    public float GameHard;
    public float GameGlobalHard;
    public int LevelAttempt;
    public int MaxWave;
    public int NowWave;
    public int MaxStage;
    public int NowStage;

    public bool PremiumOn;

    public PlayerInfo PlayerData;

    public int level;
    public bool LearningEnded;
    public int NowWeapon;
    public int NowArmor;
    public int NowWeaponPlace;

    public int DeathNum;
    public int[] MansKilled;

    public bool[] WeaponOpened;
    public bool[] ArmorOpened;
    public bool[] GameTypeOpened;

    public bool Vibration;
    public float MusicVol;
    public float EffectVol;

    public SaveData(GameData data)
    {
        level = GameData.NowLevel;
        LearningEnded = GameData.LearningEnded;
        NowWeapon = GameData.NowWeapon;
        NowArmor = GameData.NowArmor;
        NowWeaponPlace = GameData.NowWeaponPlace;
        LevelAttempt = GameData.AttempForLevel;
        GameHard = data.GameHard;
        GameGlobalHard = GameData.GameGlobalHard;

        PremiumOn = GameData.PremiumOn;

        NowWave = GameData.NowWave;
        MaxWave = GameData.MaxWave;
        NowStage = GameData.NowStage;
        MaxStage = GameData.MaxStage;

        DeathNum = GameData.DeathNum;
        PlayerData = data.playerInfo;
        MansKilled = data.MansKilled;

        PlayerLevel = GameData.PlayerLevel;
        PrevPlayerLevel = GameData.PrevPlayerLevel;
        PlayerExperience = GameData.PlayerExperience;
        Money = GameData.Money;

        Vibration = GameData.Vibration;
        MusicVol = GameData.MusicVol;
        EffectVol = GameData.EffectVol;

        WeaponOpened = new bool[data.weapon.Length];
        for(int i = 0; i < WeaponOpened.Length; i++)
        {
            WeaponOpened[i] = data.weapon[i].Opened;
        }
        ArmorOpened = new bool[data.armor.Length];
        for(int i = 0; i < ArmorOpened.Length; i++)
        {
            ArmorOpened[i] = data.armor[i].Opened;
        }
        GameTypeOpened = new bool[data.GameType.Length];
        for(int i = 0; i < GameTypeOpened.Length; i++)
        {
            GameTypeOpened[i] = data.GameType[i].Opened;
        }
    }
}