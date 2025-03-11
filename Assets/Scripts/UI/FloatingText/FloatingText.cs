using Febucci.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public TMP_Text label;
    public string takeDamage = "<bounce><waitfor=1><fade>";

    public void SendText(string template, string value, Color color)
    {
        label.text = template + value;
        label.color = color;        
    }
}
