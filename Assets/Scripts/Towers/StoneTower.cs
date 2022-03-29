using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StoneTower : Tower
{
    [SerializeField]
    private float slowingFactor;
    public float SlowingFactor 
    {
        get 
        {
            return slowingFactor;
        }
    }
    private void Start()
    {
        ElementType = Element.STONE;
        Upgrades = new TowerUpgrade[] 
        {
            new TowerUpgrade(2, 1, 1, 20, 25),
            new TowerUpgrade(4, 1, 2, 20, 25),
        };
    }

    public override Debuff GetDebuff()
    {
        return new StoneDebuff(slowingFactor, DebuffDuration, Target);
    }

    public override string GetStats()
    {
        if (NextUpgrade != null)  //If the next is avaliable
        {
            return String.Format("<color=#36BAF3>{0}</color>{1} \nPercentage slowed: {2}% <color=#00ff00ff>+{3}%</color>", "<size=30><b>Stone</b></size>", base.GetStats(), SlowingFactor, NextUpgrade.SlowingFactor);
        }   

        //Returns the current upgrade
        return String.Format("<color=#36BAF3>{0}</color>{1} \nPercentage slowed: {2}%", "<size=30><b>Stone</b></size>", base.GetStats(), SlowingFactor);
    }

    public override void Upgrade()
    {
        this.slowingFactor += NextUpgrade.SlowingFactor;
        base.Upgrade();
    }
}
