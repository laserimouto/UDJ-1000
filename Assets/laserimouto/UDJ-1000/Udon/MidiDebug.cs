
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using System.Collections.Generic;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MidiDebug : UdonSharpBehaviour
{
    public Text midiDisplay;
    public Text owner;

    [UdonSynced]
    private string text;

    public void SetOwner()
    {
        VRCPlayerApi lp = Networking.LocalPlayer;
        Networking.SetOwner(lp, gameObject);

        text = "Type:\nChannel:\nControl:\nValue:";
        RequestSerialization();
        UpdateDisplay(text);
    }

    public override void MidiControlChange(int channel, int number, int value)
    {
        UpdateText("CC", channel, number, value);
    }

    public override void MidiNoteOn(int channel, int number, int velocity)
    {
        UpdateText("ON", channel, number, velocity);
    }

    public override void MidiNoteOff(int channel, int number, int velocity)
    {
        UpdateText("OFF", channel, number, velocity);
    }

    public override void OnDeserialization()
    {
        UpdateDisplay(text);
    }

    private void UpdateText(string type, int channel, int number, int velocity)
    {
        if (!Networking.IsOwner(gameObject))
            return;

        text = string.Format("Type: {0}\nChannel: {1}\nControl: {2}\nValue: {3}", type, channel, number, velocity);
        RequestSerialization();
        UpdateDisplay(text);
    }

    private void UpdateDisplay(string text)
    {
        midiDisplay.text = text;

        var player = Networking.GetOwner(gameObject);
        if (player != null)
            owner.text = "Current:" + player.displayName;
    }
}
