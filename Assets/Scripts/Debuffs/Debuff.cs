using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Debuff
{
    protected Enemy target;
    private float duration;
    private float elapsed;
    //constructor
    public Debuff(Enemy target, float duration)
    {
        this.target = target;
        this.duration = duration;
    }

    public virtual void Update() //virtual bc overwritten in children
    {
        elapsed += Time.deltaTime;
        if (elapsed >= duration)
        {
            Remove();
        }
    }

    public virtual void Remove()
    {
        if (target != null)
        {
            target.RemoveDebuff(this);
        }
    }
}
