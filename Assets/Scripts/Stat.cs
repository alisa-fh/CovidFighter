using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Stat
{
    [SerializeField]
    private BarScript bar;

    public BarScript Bar 
    {
        get 
        {
            return bar;
        }
    }
    [SerializeField]
    private float maxVal;
    public float MaxVal 
    {
        get
        {
            return maxVal;
        }
        set 
        {
            bar.MaxValue = value;
            this.maxVal = value;
        }
    }
    [SerializeField]
    private float currentVal;
    public float CurrentVal
    {
        get
        {
            return currentVal;
        }
        set 
        {       
            this.currentVal = Mathf.Clamp(value, 0, MaxVal);
            bar.BarValue = currentVal;
        }
    }
    public void Initialise()
    {
        this.MaxVal = maxVal;
        this.CurrentVal = currentVal;
    }
}
