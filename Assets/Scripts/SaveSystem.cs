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
    public int LevelAttempt;
    public int MaxWave;
    public int NowWave;

    public PlayerInfo PlayerData;

    public int level;
    public bool LearningEnded;
    public int NowWeapon;
    public int NowArmor;

    public int DeathNum;
    public int[] MansKilled;

    public bool[] WeaponOpened;

    public bool Vibration;
    public float MusicVol;
    public float EffectVol;

    public SaveData(GameData data)
    {
        level = GameData.NowLevel;
        LearningEnded = GameData.LearningEnded;
        NowWeapon = GameData.NowWeapon;
        NowArmor = GameData.NowArmor;
        LevelAttempt = GameData.AttempForLevel;
        GameHard = data.GameHard;

        NowWave = GameData.NowWave;
        MaxWave = GameData.MaxWave;
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
    }
}