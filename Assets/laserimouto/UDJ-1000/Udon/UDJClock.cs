
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class UDJClock : UdonSharpBehaviour
{
    public Text textL;
    public Text textR;

    private void Update()
    {
        TimeSpan t = TimeSpan.FromSeconds(Time.fixedTime);
        string s = t.ToString(@"hh\:mm\:ss");
        textL.text = s;
        textR.text = s;
    }
}
