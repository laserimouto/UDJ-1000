using System;
using UnityEditor;
using UnityEngine;

public class UDJBoneAutoAssign
{
    [MenuItem("CONTEXT/UDJ1000MidiController/Auto-assign Bones")]
    public static void Foo(MenuCommand menuCommand)
    {
        var target = menuCommand.context as UDJ1000MidiController;
        var armature = target.transform.GetChild(0).Find("Armature");

        // EQ
        Transform eqBones = armature.Find("Bones.EQ");
        target.eqTrimBones = FindMany<Transform>(eqBones, 4, n => "EQ.Trim." + n);
        target.eqHighBones = FindMany<Transform>(eqBones, 4, n => "EQ.High." + n);
        target.eqMidBones = FindMany<Transform>(eqBones, 4, n => "EQ.Mid." + n);
        target.eqLowBones = FindMany<Transform>(eqBones, 4, n => "EQ.Low." + n);

        // MIXER
        Transform mixerBones = armature.Find("Bones.Mixer");
        Transform mixerButtons = armature.Find("Buttons.Mixer");

        target.channelFaderBones = FindMany<Transform>(mixerBones, 4, n => "ChannelFader." + n);
        target.crossfaderBone = mixerBones.Find("Crossfader");
        target.cueButtons = FindMany<Renderer>(mixerButtons, 4, n => "Cue." + n);
        target.cueSamplerButton = Find<Renderer>(mixerButtons, "Cue.Sampler");

        target.colorFxButtons = FindMany<Renderer>(mixerButtons, 4, n => "ColorFX.Button." + n);
        target.colorFxBones = FindMany<Transform>(mixerBones, 4, n => "ColorFX.Knob." + n);

        target.masterCueButton = Find<Renderer>(mixerButtons, "MasterCue");
        target.samplerVolumeBone = mixerBones.Find("Sampler.Volume");
        target.headphoneMixBone = mixerBones.Find("Headphones.Mix");
        target.headphoneLevelBone = mixerBones.Find("Headphones.Level");

        target.fxSelectBone = mixerBones.Find("FX.Select");
        target.fxChannelBone = mixerBones.Find("FX.Channel");
        target.fxDepthBone = mixerBones.Find("FX.Depth");
        target.fxOnOffButton = Find<Renderer>(mixerButtons, "FX.OnOff");

        // PLAYERS
        Transform playerBones = armature.Find("Bones.Players");
        Transform playerLeftButtons = armature.Find("Buttons.Player.L");
        target.buttonPlayLeft = Find<Renderer>(playerLeftButtons, "Play.L");
        target.buttonCueLeft = Find<Renderer>(playerLeftButtons, "Cue.L");
        target.jogwheelBoneLeft = playerBones.Find("Jogwheel.L");
        target.buttonSyncLeft = Find<Renderer>(playerLeftButtons, "BeatSync.L");
        target.tempoSliderBoneLeft = playerBones.Find("Tempo.L");

        target.padsLeft = FindMany<Renderer>(playerLeftButtons, 8, n => string.Format("Pad.{0}.L", n));

        target.buttonShiftLeft = Find<Renderer>(playerLeftButtons, "Shift.L");
        target.searchNextButtonLeft = Find<Renderer>(playerLeftButtons, "Search.Next.L");
        target.searchPrevButtonLeft = Find<Renderer>(playerLeftButtons, "Search.Prev.L");

        target.loopInButtonLeft = Find<Renderer>(playerLeftButtons, "Loop.In.L");
        target.loopOutButtonLeft = Find<Renderer>(playerLeftButtons, "Loop.Out.L");
        target.fourBeatLoopButtonLeft = Find<Renderer>(playerLeftButtons, "Loop.4Beat.L");

        Transform playerRightButtons = armature.Find("Buttons.Player.R");
        target.buttonPlayRight = Find<Renderer>(playerRightButtons, "Play.R");
        target.buttonCueRight = Find<Renderer>(playerRightButtons, "Cue.R");
        target.jogwheelBoneRight = playerBones.Find("Jogwheel.R");
        target.buttonSyncRight = Find<Renderer>(playerRightButtons, "BeatSync.R");
        target.tempoSliderBoneRight = playerBones.Find("Tempo.R");

        target.padsRight = FindMany<Renderer>(playerRightButtons, 8, n => string.Format("Pad.{0}.R", n));

        target.buttonShiftRight = Find<Renderer>(playerRightButtons, "Shift.R");
        target.searchNextButtonRight = Find<Renderer>(playerRightButtons, "Search.Next.R");
        target.searchPrevButtonRight = Find<Renderer>(playerRightButtons, "Search.Prev.R");

        target.loopInButtonRight = Find<Renderer>(playerRightButtons, "Loop.In.R");
        target.loopOutButtonRight = Find<Renderer>(playerRightButtons, "Loop.Out.R");
        target.fourBeatLoopButtonRight = Find<Renderer>(playerRightButtons, "Loop.4Beat.R");
    }

    private static T Find<T>(Transform parent, string name)
    {
        var transform = parent.Find(name);
        return transform != null ? transform.GetComponent<T>() : default(T);
    }

    private static T[] FindMany<T>(Transform parent, int number, Func<int, string> namer)
    {
        T[] res = new T[number];
        for (int i = 0; i < number; i++)
            res[i] = Find<T>(parent, namer(i + 1));

        return res;
    }
}
