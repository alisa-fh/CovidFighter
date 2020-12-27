using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDebuff : Debuff
{   
    private float tickTime;
    private float timeSinceTick;
    private float tickDamage; 
    //constructor + executes constructor of parent 
    public FireDebuff(float tickDamage, float tickTime, float duration, Enemy target) :base(target, duration)
    {
        this.tickDamage = tickDamage;
        this.tickTime = tickTime;

    }
    public override void Update()
    {
        if (target != null) //if debuff has a target
        {
            //increase time
            timeSinceTick += Time.deltaTime;
            if (timeSinceTick >= tickTime) //ticktime is how often debuff ticks
            {
                timeSinceTick = 0;
                target.TakeDamage(tickDamage, Element.FIRE);
            }
        }
        base.Update();
    }
}
