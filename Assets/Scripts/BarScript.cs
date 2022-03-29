using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarScript : MonoBehaviour
{
    //to remove serialize
    private float fillAmount;

    [SerializeField]
    private Image content;

    [SerializeField]
    private float lerpSpeed;

    [SerializeField]
    private Color lowColor;

    [SerializeField]
    private Color fullColor;

    [SerializeField]
    private bool lerpColors;
    [SerializeField]
    private float maxValue;
    public bool Running;
    public float MaxValue 
    { 
        get
        {
            return maxValue;
        } 
        set 
        {
            maxValue = value;
        } 
    }
    private float barValue;

    public float BarValue 
    {
        get 
        {
            return barValue;
        }
        set 
        {
            barValue = value;
            fillAmount = Map(value, 0, maxValue, 0, 1);
            
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (lerpColors)
        {
            content.color = fullColor;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleBar();
    }


    private void HandleBar()
    {
        if (fillAmount != content.fillAmount)
        {
            //changing from first param to second with third param's speed
            content.fillAmount = Mathf.Lerp(content.fillAmount, fillAmount, Time.deltaTime * lerpSpeed);
        } 
        if (lerpColors)
        {
            content.color = Color.Lerp(lowColor, fullColor, fillAmount); 
        }
    }

    //value is current health
    private float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
       return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin; 
    }

    public void Reset()
    {
        BarValue = 0;
        content.fillAmount = 0;
        
    }
}
