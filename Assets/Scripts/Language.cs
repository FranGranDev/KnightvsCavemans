using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
        string path = Application.streamingAssetsPath + "/Languages/" + NowLanguage + ".json";
        #if UNITY_ANDROID && !UNITY_EDITOR
            StartCoroutine(LoadDataAndroid(path));
        #endif
        #if UNITY_EDITOR
            LoadDataPC(path);
        #endif
    }
    private void LoadDataPC(string Path)
    {
        Json = File.ReadAllText(Path);
        Lang = JsonUtility.FromJson<LanguageData>(Json);
    }
    private IEnumerator LoadDataAndroid(string Path)
    {
        WWW www = new WWW(Path);
        yield return www;
        Lang = JsonUtility.FromJson<LanguageData>(www.text);
        yield break;
    }
}
[System.Serializable]
public struct LanguageData
{
    public Basic basicText;
    public UiMenu menuText;
    public UiMenuBets betsText;
    public UiMenuWaves wavesText;
    public UiSettings settingsText;
    public UiAmunition ammunitionText;
    public UiGame gameText;
    public UiLevelUp levelUpText;
    public UiPresent presentText;
    public WeaponText[] Weapon;
    public ArmorText[] Armor;
}

[System.Serializable]
public struct Basic
{
    public string Coin;
    public string Coins;
    public string Buy;
    public string Level;
}
[System.Serializable]
public struct UiMenu
{
    public string Name;
    public string Play;
    public string Amunittion;
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
}
[System.Serializable]
public struct UiMenuWaves
{
    public string Name;
    public string Play;
    public string MaxWave;
    public string NowWave;
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
}
[System.Serializable]
public struct UiLevelUp
{
    public string Close;
    public string Unlocked;
}
[System.Serializable]
public struct UiPresent
{
    public string Hours;
    public string Hour;
    public string Minutes;
    public string Minute;
    public string Seconds;
}
[System.Serializable]
public struct WeaponText
{
    public string Name;
}
[System.Serializable]
public struct ArmorText
{
    public string Name;
}
[System.Serializable]
public struct GameTypeText
{
    public string WinText;
    public string LoseText;
    public string[] AttackPhase;
    public string[] DamagePhase;
}