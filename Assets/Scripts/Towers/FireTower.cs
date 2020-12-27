using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTower : Tower
{
    [SerializeField]
    private float tickTime; //upgrade tik more often
    public float TickTime 
    {
        get 
        {
            return tickTime;
        }
    }
    [SerializeField]
    private float tickDamage;
    public float TickDamage 
    {
        get 
        {
            return tickDamage;
        }
    }
    private void Start()
    {
        ElementType = Element.FIRE;
        Upgrades = new TowerUpgrade []
        {
            new TowerUpgrade(2, 1, 1, 10, -0.1f, 1),
            new TowerUpgrade(5, 2, 1, 30, -0.1f, 1)
        };
    } 

    public override Debuff GetDebuff()
    {
        return new FireDebuff(TickDamage, tickTime, DebuffDuration, Target);
    }

    public override string GetStats()
    {
        if (NextUpgrade != null) //If the next is avaliable
        {
            return string.Format("<color=#ffa500ff>{0}</color>{1} \n <color=#00ff00ff></color>\nSpecial damage: {2} <color=#00ff00ff>+{3}</color>", "<size=20><b>Fire</b></size> ", base.GetStats(), TickDamage, NextUpgrade.SpecialDamage);
        }
 
        //Returns the current upgrade
        return string.Format("<color=#ffa500ff>{0}</color>{1} \nSpecial damage: {2}", "<size=20><b>Fire</b></size> ", base.GetStats(), TickDamage);
    }

    public override void Upgrade()
    {
        this.tickTime -= NextUpgrade.TickTime;
        this.tickDamage += NextUpgrade.SpecialDamage;
        base.Upgrade();
    }

}
