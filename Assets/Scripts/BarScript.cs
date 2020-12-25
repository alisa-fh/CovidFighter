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
    private Text valueText;

    [SerializeField]
    private float lerpSpeed;

    [SerializeField]
    private Color lowColor;

    [SerializeField]
    private Color fullColor;

    [SerializeField]
    private bool lerpColors;


    public float MaxValue {get; set;}

    public float Value 
    {
        set 
        {
            // string[] tmp = valueText.text.Split(':');
            // valueText.text = tmp[0] + ": " + value;
            fillAmount = Map(value, 0, MaxValue, 0, 1);
            
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
        Value = MaxValue;
        content.fillAmount = 1;
        
    }
}
