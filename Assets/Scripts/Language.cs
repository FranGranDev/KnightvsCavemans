using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class Language : MonoBehaviour
{
    public static Language active;
    private static string Json;
    private static string NowLanguage = "en_US";
    public static string[] LanguagesArray = { "en_US", "ru_RU", "by_BY", "zh_CN" };
    public static LanguageData Lang = new LanguageData();

    private void Awake()
    {
        active = this;
    }
    public void LoadLanguage(int Num)
    {
        if (Num > LanguagesArray.Length)
        {
            GameData.Language = 0;
            NowLanguage = LanguagesArray[0];
            Debug.Log("Missing Language. Set to English");
        }
        else
        {
            GameData.Language = Num;
            NowLanguage = LanguagesArray[Num];
        }

        string path = "Languages/" + NowLanguage;
        LoadData(path);
    }
    private void LoadData(string Path)
    {
        TextAsset file = Resources.Load<TextAsset>(Path);

        Json = file.text;
        Lang = JsonUtility.FromJson<LanguageData>(Json);

        Level.active.OnLanguageLoaded();
    }
}
[System.Serializable]
public struct LanguageData
{
    public Basic basicText;
    public UiMenu menuText;
    public UiMenuBets betsText;
    public UiMenuWaves wavesText;
    public UiMenuWaves knightsText;
    public UiSettings settingsText;
    public UiAmunition ammunitionText;
    public UiGame gameText;
    public UiLevelUp levelUpText;
    public UiPresent presentText;
    public UiPremium premiumText;
    public WeaponText Weapon;
    public ArmorText Armor;
    public GameTypeText gameTypeText;
    public LearningStageText[] learningText;
}

[System.Serializable]
public struct Basic
{
    public string Coin;
    public string Coins;
    public string Buy;
    public string Level;
    public string MaxLevel;
    public string Nice;
    public string Caveman;
    public string Cavemans;
}
[System.Serializable]
public struct UiMenu
{
    public string Name;
    public string Play;
    public string Amunition;
    public string Settings;
}
[System.Serializable]
public struct UiMenuBets
{
    public string Name;
    public string BetLeft;
    public string BetRight;
    public string IfWin;
    public string IfLose;
    public string RewardText;
    public string Close;
}
[System.Serializable]
public struct UiMenuWaves
{
    public string Name;
    public string Play;
    public string MaxWave;
    public string NowWave;
    public string WaveDone;
    public string Remaining;
}
[System.Serializable]
public struct UiMenuKnights
{
    public string Name;
    public string Play;
    public string MaxWave;
    public string NowWave;
    public string WaveDone;
}
[System.Serializable]
public struct UiSettings
{
    public string Resume;
    public string ResumeGame;
    public string Restart;
    public string Volume;
    public string Music;
    public string Vibration;
    public string MainMenu;
}
[System.Serializable]
public struct UiAmunition
{
    public string Back;
    public string StartWeapon;
    public string Buy;
    public string NoLevel;
    public string NoMoney;
}
[System.Serializable]
public struct UiGame
{
    public string Combo;
    public string MegaCombo;
    public string BossHp;
    public string Skip;
}
[System.Serializable]
public struct UiLevelUp
{
    public string LevelUp;
    public string Close;
    public string Unlocked;
}
[System.Serializable]
public struct UiPremium
{
    public string Name;
    public string WhatGet;
    public string NoAds;
    public string CoolWeapon;
    public string Skin;
    public string NewMode;
    public string x2Exp;
    public string Buy;
    public string NoThanks;
    public string Restore;
    public string Thanks;

    public string YouCool;
    public string WhatGot;
}
[System.Serializable]
public struct UiPresent
{
    public string Get;
    public string Hours;
    public string Hour;
    public string Minutes;
    public string Minute;
    public string Seconds;
    public string Second;
}
[System.Serializable]
public struct WeaponText
{
    public string[] Name;
}
[System.Serializable]
public struct ArmorText
{
    public string[] Name;
}
[System.Serializable]
public struct GameTypeText
{
    public string[] Name;
    public string[] WinText;
    public string[] LoseText;
    public string[] AttackPhase;
    public string[] DamagePhase;
}

[System.Serializable]
public struct LearningStageText
{
    public string StartText;
    public string DoneText;
}