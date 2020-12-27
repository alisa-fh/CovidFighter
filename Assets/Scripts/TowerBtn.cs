using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TowerBtn : MonoBehaviour
{
    private void Start()
    {
        priceText.text = price + " coins";
        GameManager.Instance.Changed += new CurrencyChanged(PriceCheck); //pricecheck executed every time changed is
    }
    [SerializeField]
    private GameObject towerPrefab;
    [SerializeField]
    private Sprite sprite;
    [SerializeField]
    private int price;
    public int Price
    {
        get
        {
            return price;
        }
    }
    
    [SerializeField]
    private Text priceText;

    public Sprite Sprite
    {
        get
        {
            return sprite;
        }

    } 

    public GameObject TowerPrefab
    {
        get
        {
            return towerPrefab;
        }
    }

    private void PriceCheck()
    {
        if (price <= GameManager.Instance.Currency)
        {
            GetComponent<Image>().color = Color.white;
            priceText.color = Color.white;
        }
        else
        {
            GetComponent<Image>().color = Color.grey;
            priceText.color = Color.grey; 
        }
    }

    public void ShowTooltipInfo(string type)
    {
        string ttText = string.Empty;
        switch(type)
        {
            case "Fire":
                FireTower fire = towerPrefab.GetComponentInChildren<FireTower>();
                ttText = string.Format("<color=#ffa500ff><size=20><b>Fire</b></size></color>\n Has a chance of repeatedly damaging the target even out of range \nDamage: {0} \nChance of special attack: {1}%\nSpecial attack duration: {2} sec \nSpecial damage: {3}", fire.Damage, fire.Proc, fire.DebuffDuration, fire.TickDamage);
                break;
            case "Stone":
                StoneTower stone = towerPrefab.GetComponentInChildren<StoneTower>();
                ttText = string.Format("<color=#36BAF3><size=20><b>Stone</b></size></color>\n Has a chance of slowing down the target \nDamage: {0} \nChance of special attack: {1}%\nSpecial attack duration: {2} seconds\nSlowing Factor {3}%", stone.Damage, stone.Proc, stone.DebuffDuration, stone.SlowingFactor);
                break;
        }
        GameManager.Instance.SetTooltipText(ttText); 
        GameManager.Instance.ShowTooltip();  
    }
}
