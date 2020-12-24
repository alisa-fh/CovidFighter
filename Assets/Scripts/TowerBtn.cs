using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerBtn : MonoBehaviour
{
    private void Start()
    {
        priceText.text = price + " coins";
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
}
