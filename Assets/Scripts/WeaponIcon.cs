using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponIcon : MonoBehaviour
{
    public Button button;
    public Image background;
    public Image icon;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI BuyText;
    public TextMeshProUGUI Cost;
    public Image TryToBoughtUI;
    public Image BuyButtonUI;
    public GameObject OnBoughtUI;
    public GameObject OnNoMoneyUI;
    public int Place;
    public int Index;
    private WeaponInfo.RareType rare;
    private const float IdleColorA = 0.1f;
    private const float SelectedColorA = 0.35f;
    public void SetIconLevelUp(WeaponInfo info)
    {
        icon.sprite = info.Icon;
        rare = info.Rare;
        this.Index = info.Index;
        SetOpened(true, false, true);
    }
    public void SetIcon(WeaponInfo info, int Place)
    {
        if (GameData.active.GetAvalibleWeapon(info.Index) || info.Premium)
        {
            button.onClick.AddListener(delegate { Level.active.SelectWeapon(info.Index, Place); });
        }
        BuyText.text = Language.Lang.basicText.Buy;
        icon.sprite = info.Icon;
        rare = info.Rare;
        this.Place = Place;
        this.Index = info.Index;
        SetSelected(false);
    }
    public void SetOpened(bool Opened, bool Premium, bool Avalible)
    {
        if(Opened || Premium)
        {
            icon.color = Color.white;
        }
        else if(Avalible)
        {
            icon.color = new Color(1f, 1f, 1f, 0.4f);
        }
        else
        {
            icon.color = Color.black;
        }
    }
    public void SetSelected(bool on)
    {
        Color color = GameData.active.IconWeaponColor[(int)rare];
        background.color = new Color(color.r, color.g, color.b, on ? SelectedColorA : IdleColorA);
        if(!on)
        {
            SetTryToBuy(false);
        }
    }
    public void SetTryToBuy(bool on)
    {
        TryToBoughtUI.gameObject.SetActive(on);
        if(on)
        {
            BuyButtonUI.color = GameData.active.IconWeaponColor[(int)rare];
            Name.text = GameData.active.weapon[Index].Name;
            Cost.text = GameData.active.weapon[Index].Cost.ToString();
        }
    }
    public void Buy()
    {
        Level.active.TryToBuyWeapon(this);
    }
    public void OnBought()
    {
        StartCoroutine(OnBoughtCour());
    }
    private IEnumerator OnBoughtCour()
    {
        TryToBoughtUI.gameObject.SetActive(false);
        OnBoughtUI.SetActive(true);
        yield return new WaitForSeconds(0.75f);
        OnBoughtUI.SetActive(false);
        SetOpened(true, false, true);
        Level.active.SelectWeapon(Index, Place);
        yield break;
    }
    public void OnCantBuy()
    {
        StartCoroutine(OnCantBuyCour());
    }
    private IEnumerator OnCantBuyCour()
    {
        OnNoMoneyUI.SetActive(true);
        yield return new WaitForSeconds(0.75f);
        OnNoMoneyUI.SetActive(false);
        yield break;
    }
}
