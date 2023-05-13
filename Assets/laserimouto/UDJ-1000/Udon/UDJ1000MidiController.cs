using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using UnityEngine.UI;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class UDJ1000MidiController : UdonSharpBehaviour
{
    private const float MAX_VALUE = 127f;
    private const int START_IN_MIDDLE = 64;

    private const int CH_FADER = 19;
    private const float CH_FADER_START = 0.0971f;
    private const float CH_FADER_END = 0.0527f;

    private const int CROSSFADER = 31;
    private const int CROSSFADER_MAX = 127;
    private const float CROSSFADER_START = 0.022f;
    private const float CROSSFADER_END = -0.022f;

    private const int CHANNEL_CUE_BUTTON = 84;
    private const int MASTER_CUE_BUTTON = 99;
    private const int SAMPLER_CUE_BUTTON = 105;

    private const int EQ_TRIM = 4;
    private const int EQ_HIGH = 7;
    private const int EQ_MID = 11;
    private const int EQ_LOW = 15;

    private const int COLOR_FX_CHANNEL = 6;
    private const int COLOR_FX1 = 23;
    private const int COLOR_FX2 = 24;
    private const int COLOR_FX3 = 25;
    private const int COLOR_FX4 = 26;
    private const int COLOR_FX1_BUTTON = 0;
    private const int COLOR_FX2_BUTTON = 1;
    private const int COLOR_FX3_BUTTON = 2;
    private const int COLOR_FX4_BUTTON = 3;

    private const int CUE_CHANNEL = 6;
    private const int SAMPLER_VOLUME = 3;
    private const int HEADPHONE_MIX = 12;
    private const int HEADPHONE_LEVEL = 13;

    private const int FX_CHANNEL = 4;
    private const int FX_DEPTH = 2;
    private const int FX_ONOFF_BUTTON_1 = 70;
    private const int FX_ONOFF_BUTTON_2 = 71;

    private const int FX_SELECT_MIN = 32;
    private const int FX_SELECT_MAX = 45;
    private const int FX_CHANNEL_MIN = 16;
    private const int FX_CHANNEL_MAX = 22;
    private const int FX_VALUE_MAX = 13;

    private const int TEMPO = 0;
    private const float TEMPO_SLIDER_START = 0.0395f;
    private const float TEMPO_SLIDER_END = 0.145f;

    private const int JOGWHEEL_SIDE = 33;
    private const int JOGWHEEL_TOP = 34;
    private const int JOGWHEEL_VINYL = 35;
    private const int JOGWHEEL_TOUCH = 54;

    private const int PLAY_BUTTON = 11;
    private const int CUE_BUTTON = 12;

    private const int SHIFT_BUTTON = 63;
    private const int SYNC_BUTTON = 88;
    private const int SEARCH_NEXT_BUTTON = 113;
    private const int SEARCH_PREV_BUTTON = 112;

    private const int LOOP_IN_BUTTON = 16;
    private const int LOOP_OUT_BUTTON = 17;
    private const int FOUR_BEAT_LOOP_BUTTON = 20;



    /*************
     * INSPECTOR *
     *************/
    [Header("EQ")]
    public Transform[] eqTrimBones;
    public Transform[] eqHighBones;
    public Transform[] eqMidBones;
    public Transform[] eqLowBones;

    [Header("Mixer - Channel Faders")]
    public Transform[] channelFaderBones;
    public Transform crossfaderBone;

    [Space(10)]
    public Renderer[] cueButtons;
    public Renderer cueSamplerButton;

    [Header("Mixer - Effects/Cue")]
    public Renderer[] colorFxButtons;
    public Transform[] colorFxBones;

    [Space(10)]
    public Renderer masterCueButton;

    [Space(10)]
    public Transform samplerVolumeBone;
    public Transform headphoneMixBone;
    public Transform headphoneLevelBone;

    [Space(10)]
    public Transform fxSelectBone;
    public Transform fxChannelBone;
    public Transform fxDepthBone;
    public Renderer fxOnOffButton;

    [Header("Left Player")]
    public Renderer buttonPlayLeft;
    public Renderer buttonCueLeft;

    [Space(10)]
    public Transform jogwheelBoneLeft;
    public Renderer buttonSyncLeft;
    public Transform tempoSliderBoneLeft;

    [Space(10)]
    public Renderer[] padsLeft;

    [Space(10)]
    public Renderer buttonShiftLeft;
    public Renderer searchNextButtonLeft;
    public Renderer searchPrevButtonLeft;

    [Space(10)]
    public Renderer loopInButtonLeft;
    public Renderer loopOutButtonLeft;
    public Renderer fourBeatLoopButtonLeft;

    [Header("Right Player")]
    public Renderer buttonPlayRight;
    public Renderer buttonCueRight;
    
    [Space(10)]
    public Transform jogwheelBoneRight;
    public Renderer buttonSyncRight;
    public Transform tempoSliderBoneRight;

    [Space(10)]
    public Renderer[] padsRight;

    [Space(10)]
    public Renderer buttonShiftRight;
    public Renderer searchNextButtonRight;
    public Renderer searchPrevButtonRight;

    [Space(10)]
    public Renderer loopInButtonRight;
    public Renderer loopOutButtonRight;
    public Renderer fourBeatLoopButtonRight;

    [Header("Misc")]
    public float jogwheelSens = 0.5f;
    public Text fxText;
    public Text effectChannelText;


    [UdonSynced]
    private int[] faderValues = new int[4];
    [UdonSynced]
    private int crossfaderValue = CROSSFADER_MAX / 2;
    [UdonSynced]
    private int[] eqTrimValues = new int[4];
    [UdonSynced]
    private int[] eqHighValues = new int[4];
    [UdonSynced]
    private int[] eqMidValues = new int[4];
    [UdonSynced]
    private int[] eqLowValues = new int[4];
    [UdonSynced]
    private int[] eqColorFxValues = new int[4];

    [UdonSynced]
    private int cueSamplerValue = START_IN_MIDDLE;
    [UdonSynced]
    private int cueMixValue = START_IN_MIDDLE;
    [UdonSynced]
    private int cueLevelValue = START_IN_MIDDLE;

    [UdonSynced]
    private int effectValue = FX_VALUE_MAX / 2;
    [UdonSynced]
    private int effectChannelValue;
    [UdonSynced]
    private int effectDepthValue;

    [UdonSynced]
    private int tempoLeftValue = START_IN_MIDDLE;
    [UdonSynced]
    private int tempoRightValue = START_IN_MIDDLE;

    [UdonSynced]
    private int jogwheelValueLeft;
    [UdonSynced]
    private float jogwheelRotationLeft;
    [UdonSynced]
    private int jogwheelValueRight;
    [UdonSynced]
    private float jogwheelRotationRight;

    private Animator anim;

    private string[] FX_NAMES =
    {
        "LOW CUT ECHO",
        "ECHO",
        "MT DELAY",
        "SPIRAL",
        "REVERB",
        "TRANS",
        "ENIGMA JET",
        "FLANGER",
        "PHASER",
        "PITCH",
        "SLIP ROLL",
        "ROLL",
        "MOBIUS SAW",
        "MOBIUS TRI"
    };



    private void Start()
    {
        WarnIfNotEnoughConfigured(eqTrimBones, 4, "eqTrimBones");
        WarnIfNotEnoughConfigured(eqHighBones, 4, "eqHighBones");
        WarnIfNotEnoughConfigured(eqMidBones, 4, "eqMidBones");
        WarnIfNotEnoughConfigured(eqLowBones, 4, "eqLowBones");
        WarnIfNotEnoughConfigured(colorFxBones, 4, "eqColorFxBones");
        WarnIfNotEnoughConfigured(channelFaderBones, 4, "channelFaderBones");
        WarnIfNotEnoughConfigured(cueButtons, 4, "cueButtons");
        WarnIfNotEnoughConfigured(padsLeft, 4, "padsLeft");
        WarnIfNotEnoughConfigured(padsRight, 4, "padsRight");

        for (int i = 0; i < 4; i++)
        {
            eqTrimValues[i] = START_IN_MIDDLE;
            eqHighValues[i] = START_IN_MIDDLE;
            eqMidValues[i] = START_IN_MIDDLE;
            eqLowValues[i] = START_IN_MIDDLE;
            eqColorFxValues[i] = START_IN_MIDDLE;
        }

        anim = GetComponent<Animator>();

        OnDeserialization();
    }

    private void WarnIfNotEnoughConfigured(Component[] components, int expected, string name)
    {
        if (components.Length < expected)
            Debug.LogWarning(string.Format("Missing {0}! Expected {1}, found {2}", name, expected, components.Length));
    }



    /**************************************
     * MIDI CC (Faders, Knobs & Jogwheel) *
     **************************************/
    public override void MidiControlChange(int channel, int control, int value)
    {
        if (!Networking.IsOwner(gameObject))
        {
            Debug.Log("Not Owner, skipping CC");
            return;
        }

        if (channel >= 0 && channel <= 3)
        {
            HandleChannelCC(channel, control, value);
        }
        else if (IsColorFxMessage(channel, control))
        {
            HandleColorFxCC(control, value);
        }
        else if (channel == FX_CHANNEL)
        {
            HandleEffectCC(control, value);
        }
        else if (channel == CUE_CHANNEL)
        {
            HandleCueCC(control, value);
        }
    }

    private void HandleChannelCC(int channel, int control, int value)
    {
        bool serializationRequired = true;

        switch (control)
        {
            case CH_FADER:
                faderValues[channel] = value;
                MoveFaders();
                break;
            case EQ_TRIM:
                eqTrimValues[channel] = value;
                MoveEqKnobs();
                break;
            case EQ_HIGH:
                eqHighValues[channel] = value;
                MoveEqKnobs();
                break;
            case EQ_MID:
                eqMidValues[channel] = value;
                MoveEqKnobs();
                break;
            case EQ_LOW:
                eqLowValues[channel] = value;
                MoveEqKnobs();
                break;
            case TEMPO:
                if (IsLeftDeck(channel))
                    tempoLeftValue = value;
                else
                    tempoRightValue = value;
                MoveFaders();
                break;
            case JOGWHEEL_SIDE:
            case JOGWHEEL_TOP:
            case JOGWHEEL_VINYL:
                if (IsLeftDeck(channel))
                {
                    jogwheelRotationLeft += CalculateJogwheelRotation(value, jogwheelValueLeft);
                    jogwheelValueLeft = value;
                }
                else
                {
                    jogwheelRotationRight += CalculateJogwheelRotation(value, jogwheelValueRight);
                    jogwheelValueRight = value;
                }
                MoveJogwheels();
                break;
            default:
                serializationRequired = false;
                break;
        }

        if (serializationRequired)
            RequestSerialization();
    }

    private void HandleColorFxCC(int control, int value)
    {
        int colorFxChannel = GetColorFxChannel(control);
        if (colorFxChannel == -1)
        {
            Debug.LogError("Could not map ColorFxChannel for CC: control=" + control + ", value=" + value);
            return;
        }

        eqColorFxValues[colorFxChannel] = value;

        MoveEqKnobs();
        RequestSerialization();
    }

    private void HandleEffectCC(int control, int value)
    {
        if (control != FX_DEPTH)
            return;

        effectDepthValue = value;
        MoveEffectKnobs();
        RequestSerialization();
    }

    private void HandleCueCC(int control, int value)
    {
        bool serializationRequired = true;
        switch (control)
        {
            case CROSSFADER:
                crossfaderValue = value;
                MoveFaders();
                break;
            case SAMPLER_VOLUME:
                cueSamplerValue = value;
                MoveCueKnobs();
                break;
            case HEADPHONE_MIX:
                cueMixValue = value;
                MoveCueKnobs();
                break;
            case HEADPHONE_LEVEL:
                cueLevelValue = value;
                MoveCueKnobs();
                break;
            default:
                serializationRequired = false;
                break;
        }

        if (serializationRequired)
            RequestSerialization();
    }



    /****************
     * MIDI NOTE ON *
     ****************/
    public override void MidiNoteOn(int channel, int control, int velocity)
    {
        if (!Networking.IsOwner(gameObject))
        {
            Debug.Log("Not Owner, skipping Note On");
            return;
        }

        if (channel >= 0 && channel <= 3)
        {
            HandleChannelNoteOn(channel, control, velocity);
        }
        else if (channel == FX_CHANNEL)
        {
            HandleEffectNoteOn(control, velocity);
        }
        else if (channel == CUE_CHANNEL)
        {
            HandleCueNoteOn(control, velocity);
        }
        else if (channel >= 7 && channel <= 14)
        {
            HandlePadNoteOn(channel, control, velocity);
        }
    }

    private void HandleChannelNoteOn(int channel, int control, int velocity)
    {
        switch (control)
        {
            case PLAY_BUTTON:
                SendCustomNetworkEvent(NetworkEventTarget.All, "TriggerPlay" + DeckName(channel) + OnOff(velocity));
                break;
            case CUE_BUTTON:
                SendCustomNetworkEvent(NetworkEventTarget.All, "TriggerCue" + DeckName(channel) + OnOff(velocity));
                break;
            case CHANNEL_CUE_BUTTON:
                SendCustomNetworkEvent(NetworkEventTarget.All, "TriggerCue" + channel + OnOff(velocity));
                break;
            case SYNC_BUTTON:
                SendCustomNetworkEvent(NetworkEventTarget.All, "TriggerSync" + DeckName(channel) + OnOff(velocity));
                break;
            case SHIFT_BUTTON:
                SendCustomNetworkEvent(NetworkEventTarget.All, "TriggerShift" + DeckName(channel) + OnOff(velocity));
                break;
            case SEARCH_NEXT_BUTTON:
                SendCustomNetworkEvent(NetworkEventTarget.All, "TriggerSeekForwards" + DeckName(channel) + OnOff(velocity));
                break;
            case SEARCH_PREV_BUTTON:
                SendCustomNetworkEvent(NetworkEventTarget.All, "TriggerSeekBackwards" + DeckName(channel) + OnOff(velocity));
                break;
            case LOOP_IN_BUTTON:
                SendCustomNetworkEvent(NetworkEventTarget.All, "TriggerLoopIn" + DeckName(channel) + OnOff(velocity));
                break;
            case LOOP_OUT_BUTTON:
                SendCustomNetworkEvent(NetworkEventTarget.All, "TriggerLoopOut" + DeckName(channel) + OnOff(velocity));
                break;
            case FOUR_BEAT_LOOP_BUTTON:
                SendCustomNetworkEvent(NetworkEventTarget.All, "TriggerFourBeatLoop" + DeckName(channel) + OnOff(velocity));
                break;
            case JOGWHEEL_TOUCH:
                SendCustomNetworkEvent(NetworkEventTarget.All, "TriggerJogwheelTouch" + DeckName(channel) + OnOff(velocity));
                break;
        }
    }

    private void HandleCueNoteOn(int control, int velocity)
    {
        switch (control)
        {
            case MASTER_CUE_BUTTON:
                SendCustomNetworkEvent(NetworkEventTarget.All, "TriggerCueBooth" + OnOff(velocity));
                break;
            case SAMPLER_CUE_BUTTON:
                SendCustomNetworkEvent(NetworkEventTarget.All, "TriggerCueSampler" + OnOff(velocity));
                break;
            case COLOR_FX1_BUTTON:
                SendCustomNetworkEvent(NetworkEventTarget.All, "TriggerColorFx0" + OnOff(velocity));
                break;
            case COLOR_FX2_BUTTON:
                SendCustomNetworkEvent(NetworkEventTarget.All, "TriggerColorFx1" + OnOff(velocity));
                break;
            case COLOR_FX3_BUTTON:
                SendCustomNetworkEvent(NetworkEventTarget.All, "TriggerColorFx2" + OnOff(velocity));
                break;
            case COLOR_FX4_BUTTON:
                SendCustomNetworkEvent(NetworkEventTarget.All, "TriggerColorFx3" + OnOff(velocity));
                break;
        }
    }

    private void HandleEffectNoteOn(int control, int velocity)
    {
        if (control >= FX_SELECT_MIN && control <= FX_SELECT_MAX)
        {
            effectValue = control - FX_SELECT_MIN;
            MoveEffectKnobs();
            RequestSerialization();
        }
        else if (control >= FX_CHANNEL_MIN && control <= FX_CHANNEL_MAX)
        {
            effectChannelValue = control - FX_CHANNEL_MIN;
            MoveEffectKnobs();
            RequestSerialization();
        }
        else if (control == FX_ONOFF_BUTTON_1 || control == FX_ONOFF_BUTTON_2)
        {
            Debug.Log("Toggle FX On/Off");
            SendCustomNetworkEvent(NetworkEventTarget.All, "TriggerEffect" + OnOff(velocity));
        }
    }

    private void HandlePadNoteOn(int channel, int control, int velocity)
    {
        int pad = control % 8;
        SendCustomNetworkEvent(NetworkEventTarget.All, "TriggerPad" + pad + DeckName(channel) + OnOff(velocity));
    }



    /********************
     * UPDATE POSITIONS *
     ********************/
    public override void OnDeserialization()
    {
        MoveFaders();
        MoveEqKnobs();
        MoveCueKnobs();
        MoveEffectKnobs();
        MoveJogwheels();
    }

    private void MoveFaders()
    {
        for (int i = 0; i < 4; i++)
        {
            Vector3 t = channelFaderBones[i].localPosition;
            channelFaderBones[i].localPosition = new Vector3(t.x, t.y, Mathf.Lerp(CH_FADER_START, CH_FADER_END, faderValues[i] / MAX_VALUE));
        }

        crossfaderBone.localPosition = new Vector3(Mathf.Lerp(CROSSFADER_START, CROSSFADER_END, (float)crossfaderValue / CROSSFADER_MAX), crossfaderBone.localPosition.y, crossfaderBone.localPosition.z);

        tempoSliderBoneLeft.localPosition = new Vector3(tempoSliderBoneLeft.localPosition.x, tempoSliderBoneLeft.localPosition.y, Mathf.Lerp(TEMPO_SLIDER_START, TEMPO_SLIDER_END, tempoLeftValue / MAX_VALUE));
        tempoSliderBoneRight.localPosition = new Vector3(tempoSliderBoneRight.localPosition.x, tempoSliderBoneRight.localPosition.y, Mathf.Lerp(TEMPO_SLIDER_START, TEMPO_SLIDER_END, tempoRightValue / MAX_VALUE));
    }

    private void MoveEqKnobs()
    {
        for (int i = 0; i < 4; i++)
        {
            eqTrimBones[i].localRotation = CalculateKnobRotation(eqTrimValues[i]);
            eqHighBones[i].localRotation = CalculateKnobRotation(eqHighValues[i]);
            eqMidBones[i].localRotation = CalculateKnobRotation(eqMidValues[i]);
            eqLowBones[i].localRotation = CalculateKnobRotation(eqLowValues[i]);
            colorFxBones[i].localRotation = CalculateKnobRotation(eqColorFxValues[i]);
        }
    }

    private void MoveCueKnobs()
    {
        samplerVolumeBone.localRotation = CalculateKnobRotation(cueSamplerValue);
        headphoneMixBone.localRotation = CalculateKnobRotation(cueMixValue);
        headphoneLevelBone.localRotation = CalculateKnobRotation(cueLevelValue);
    }

    private void MoveJogwheels()
    {
        jogwheelBoneLeft.localRotation = Quaternion.Euler(0f, jogwheelRotationLeft, 0f);
        jogwheelBoneRight.localRotation = Quaternion.Euler(0f, jogwheelRotationRight, 0f);
    }

    private void MoveEffectKnobs()
    {
        fxSelectBone.localRotation = CalculateKnobRotation(effectValue, FX_VALUE_MAX);
        fxDepthBone.localRotation = CalculateKnobRotation(effectDepthValue);
        fxText.text = string.Format("{0}\n{1,3}%", FX_NAMES[effectValue % FX_NAMES.Length], (int)(effectDepthValue / MAX_VALUE * 100));

        float channelRotation;
        switch (effectChannelValue)
        {
            case 0:
                channelRotation = -18f;
                effectChannelText.text = "CH1";
                break;
            case 1:
                channelRotation = 18f;
                effectChannelText.text = "CH2";
                break;
            case 2:
                channelRotation = -54f;
                effectChannelText.text = "CH3";
                break;
            case 3:
                channelRotation = 54f;
                effectChannelText.text = "CH4";
                break;
            case 4:
                channelRotation = 90f;
                effectChannelText.text = "MST";
                break;
            case 5:
                channelRotation = -90f;
                effectChannelText.text = "MIC";
                break;
            case 6:
                channelRotation = -108f;
                effectChannelText.text = "SP";
                break;
            default:
                channelRotation = -18f;
                break;
        }
        fxChannelBone.localRotation = Quaternion.Euler(0f, channelRotation, 0f);
    }



    /********
     * UTIL *
     ********/
    public void SetOwner()
    {
        VRCPlayerApi lp = Networking.LocalPlayer;
        Debug.Log(string.Format("Setting owner to {0}", lp.displayName));
        Networking.SetOwner(lp, gameObject);
    }

    private bool IsColorFxMessage(int channel, int control)
    {
        return channel == COLOR_FX_CHANNEL && (control >= COLOR_FX1 && control <= COLOR_FX4);
    }

    private int GetColorFxChannel(int control)
    {
        switch (control)
        {
            case COLOR_FX1:
                return 0;
            case COLOR_FX2:
                return 1;
            case COLOR_FX3:
                return 2;
            case COLOR_FX4:
                return 3;
            default:
                return -1;
        }
    }
    private bool IsLeftDeck(int channel)
    {
        return channel == 0 || channel == 2 || channel == 7 || channel == 8 || channel == 11 || channel == 12;
    }

    private string DeckName(int channel)
    {
        return IsLeftDeck(channel) ? "Left" : "Right";
    }

    private string OnOff(int velocity)
    {
        return velocity > 0 ? "On" : "Off";
    }

    private float CalculateJogwheelRotation(int newValue, int currentValue)
    {
        int rawChange = currentValue - newValue;
        int jogChange;
        if (newValue > 65)
            // Spinning clockwise, only consider positive rotation
            jogChange = Mathf.Max(0, rawChange);
        else
            // Spinning counterclockwise, only consider negative rotation
            jogChange = Mathf.Min(0, rawChange);

        return jogChange * jogwheelSens;
    }

    private Quaternion CalculateKnobRotation(int lerpAmount)
    {
        return CalculateKnobRotation(lerpAmount, MAX_VALUE);
    }

    private Quaternion CalculateKnobRotation(int lerpAmount, float max)
    {
        return Quaternion.Euler(0f, Mathf.Lerp(-150f, 150f, lerpAmount / max), 0f);
    }

    private void EnableEmission(Renderer renderer)
    {
        EnableEmission(renderer, Color.white * 3f);
    }

    private void EnablePadEmission(Renderer renderer)
    {
        EnableEmission(renderer, new Color(0.6f, 0f, 0.8f) * 3f);
    }

    private void EnableEmission(Renderer renderer, Color color)
    {
        renderer.material.SetColor("_EmissionColor", color);
        renderer.material.EnableKeyword("_EMISSION");
        RendererExtensions.UpdateGIMaterials(renderer);
    }

    private void DisableEmission(Renderer renderer)
    {
        renderer.material.SetColor("_EmissionColor", new Color(0.75f, 0.75f, 0.75f));
        RendererExtensions.UpdateGIMaterials(renderer);
    }



    /**************************
     * UDON NETWORKING MIDI ON EVENTS *
     **************************/

    /*****************
     * DECK PLAY/CUE *
     *****************/
    public void TriggerPlayLeftOn()
    {
        EnableEmission(buttonPlayLeft);
    }

    public void TriggerPlayLeftOff()
    {
        DisableEmission(buttonPlayLeft);
    }

    public void TriggerPlayRightOn()
    {
        EnableEmission(buttonPlayRight);
    }

    public void TriggerPlayRightOff()
    {
        DisableEmission(buttonPlayRight);
    }

    public void TriggerCueLeftOn()
    {
        EnableEmission(buttonCueLeft);
    }

    public void TriggerCueLeftOff()
    {
        DisableEmission(buttonCueLeft);
    }

    public void TriggerCueRightOn()
    {
        EnableEmission(buttonCueRight);
    }

    public void TriggerCueRightOff()
    {
        DisableEmission(buttonCueRight);
    }



    /*************
     * DECK MISC *
     *************/
    public void TriggerShiftLeftOn()
    {
        EnableEmission(buttonShiftLeft);
    }

    public void TriggerShiftLeftOff()
    {
        DisableEmission(buttonShiftLeft);
    }

    public void TriggerShiftRightOn()
    {
        EnableEmission(buttonShiftRight);
    }

    public void TriggerShiftRightOff()
    {
        DisableEmission(buttonShiftRight);
    }

    public void TriggerSyncLeftOn()
    {
        EnableEmission(buttonSyncLeft);
    }

    public void TriggerSyncLeftOff()
    {
        DisableEmission(buttonSyncLeft);
    }

    public void TriggerSyncRightOn()
    {
        EnableEmission(buttonSyncRight);
    }

    public void TriggerSyncRightOff()
    {
        DisableEmission(buttonSyncRight);
    }

    public void TriggerSeekForwardsLeftOn()
    {
        Debug.Log("FLOn");
        EnableEmission(searchNextButtonLeft);
    }

    public void TriggerSeekForwardsLeftOff()
    {
        Debug.Log("FLOff");
        DisableEmission(searchNextButtonLeft);
    }

    public void TriggerSeekBackwardsLeftOn()
    {
        Debug.Log("BLOn");
        EnableEmission(searchPrevButtonLeft);
    }

    public void TriggerSeekBackwardsLeftOff()
    {
        Debug.Log("BLOff");
        DisableEmission(searchPrevButtonLeft);
    }

    public void TriggerSeekForwardsRightOn()
    {
        EnableEmission(searchNextButtonRight);
    }

    public void TriggerSeekForwardsRightOff()
    {
        DisableEmission(searchNextButtonRight);
    }

    public void TriggerSeekBackwardsRightOn()
    {
        EnableEmission(searchPrevButtonRight);
    }

    public void TriggerSeekBackwardsRightOff()
    {
        DisableEmission(searchPrevButtonRight);
    }

    public void TriggerJogwheelTouchLeftOn()
    {
        anim.SetFloat("jogwheelSpeedL", 0f);
    }

    public void TriggerJogwheelTouchLeftOff()
    {
        anim.SetFloat("jogwheelSpeedL", 1f);
    }

    public void TriggerJogwheelTouchRightOn()
    {
        anim.SetFloat("jogwheelSpeedR", 0f);
    }

    public void TriggerJogwheelTouchRightOff()
    {
        anim.SetFloat("jogwheelSpeedR", 1f);
    }



    /****************
     * LOOP BUTTONS *
     ****************/
    public void TriggerLoopInLeftOn()
    {
        EnableEmission(loopInButtonLeft);
    }

    public void TriggerLoopInLeftOff()
    {
        DisableEmission(loopInButtonLeft);
    }

    public void TriggerLoopOutLeftOn()
    {
        EnableEmission(loopOutButtonLeft);
    }

    public void TriggerLoopOutLeftOff()
    {
        DisableEmission(loopOutButtonLeft);
    }

    public void TriggerFourBeatLoopLeftOn()
    {
        EnableEmission(fourBeatLoopButtonLeft);
    }

    public void TriggerFourBeatLoopLeftOff()
    {
        DisableEmission(fourBeatLoopButtonLeft);
    }

    public void TriggerLoopInRightOn()
    {
        EnableEmission(loopInButtonRight);
    }

    public void TriggerLoopInRightOff()
    {
        DisableEmission(loopInButtonRight);
    }

    public void TriggerLoopOutRightOn()
    {
        EnableEmission(loopOutButtonRight);
    }

    public void TriggerLoopOutRightOff()
    {
        DisableEmission(loopOutButtonRight);
    }

    public void TriggerFourBeatLoopRightOn()
    {
        EnableEmission(fourBeatLoopButtonRight);
    }

    public void TriggerFourBeatLoopRightOff()
    {
        DisableEmission(fourBeatLoopButtonRight);
    }



    /*********************
     * MIXER CUE BUTTONS *
     *********************/
    public void TriggerCue0On()
    {
        EnableEmission(cueButtons[0]);
    }

    public void TriggerCue0Off()
    {
        DisableEmission(cueButtons[0]);
    }

    public void TriggerCue1On()
    {
        EnableEmission(cueButtons[1]);
    }

    public void TriggerCue1Off()
    {
        DisableEmission(cueButtons[1]);
    }

    public void TriggerCue2On()
    {
        EnableEmission(cueButtons[2]);
    }

    public void TriggerCue2Off()
    {
        DisableEmission(cueButtons[2]);
    }

    public void TriggerCue3On()
    {
        EnableEmission(cueButtons[3]);
    }
    public void TriggerCue3Off()
    {
        DisableEmission(cueButtons[3]);
    }

    public void TriggerCueSamplerOn()
    {
        EnableEmission(cueSamplerButton);
    }

    public void TriggerCueSamplerOff()
    {
        DisableEmission(cueSamplerButton);
    }

    public void TriggerCueBoothOn()
    {
        EnableEmission(masterCueButton);
    }

    public void TriggerCueBoothOff()
    {
        DisableEmission(masterCueButton);
    }



    /*****************
     * MIXER EFFECTS *
     *****************/
    public void TriggerColorFx0On()
    {
        EnableEmission(colorFxButtons[0]);
    }

    public void TriggerColorFx0Off()
    {
        DisableEmission(colorFxButtons[0]);
    }

    public void TriggerColorFx1On()
    {
        EnableEmission(colorFxButtons[1]);
    }

    public void TriggerColorFx1Off()
    {
        DisableEmission(colorFxButtons[1]);
    }

    public void TriggerColorFx2On()
    {
        EnableEmission(colorFxButtons[2]);
    }

    public void TriggerColorFx2Off()
    {
        DisableEmission(colorFxButtons[2]);
    }

    public void TriggerColorFx3On()
    {
        EnableEmission(colorFxButtons[3]);
    }

    public void TriggerColorFx3Off()
    {
        DisableEmission(colorFxButtons[3]);
    }

    public void TriggerEffectOn()
    {
        EnableEmission(fxOnOffButton);
    }

    public void TriggerEffectOff()
    {
        DisableEmission(fxOnOffButton);
    }


    /*************
     * PADS LEFT *
     *************/
    public void TriggerPad0LeftOn()
    {
        EnablePadEmission(padsLeft[0]);
    }

    public void TriggerPad0LeftOff()
    {
        DisableEmission(padsLeft[0]);
    }

    public void TriggerPad1LeftOn()
    {
        EnablePadEmission(padsLeft[1]);
    }

    public void TriggerPad1LeftOff()
    {
        DisableEmission(padsLeft[1]);
    }

    public void TriggerPad2LeftOn()
    {
        EnablePadEmission(padsLeft[2]);
    }

    public void TriggerPad2LeftOff()
    {
        DisableEmission(padsLeft[2]);
    }

    public void TriggerPad3LeftOn()
    {
        EnablePadEmission(padsLeft[3]);
    }

    public void TriggerPad3LeftOff()
    {
        DisableEmission(padsLeft[3]);
    }

    public void TriggerPad4LeftOn()
    {
        EnablePadEmission(padsLeft[4]);
    }

    public void TriggerPad4LeftOff()
    {
        DisableEmission(padsLeft[4]);
    }

    public void TriggerPad5LeftOn()
    {
        EnablePadEmission(padsLeft[5]);
    }

    public void TriggerPad5LeftOff()
    {
        DisableEmission(padsLeft[5]);
    }

    public void TriggerPad6LeftOn()
    {
        EnablePadEmission(padsLeft[6]);
    }

    public void TriggerPad6LeftOff()
    {
        DisableEmission(padsLeft[6]);
    }

    public void TriggerPad7LeftOn()
    {
        EnablePadEmission(padsLeft[7]);
    }

    public void TriggerPad7LeftOff()
    {
        DisableEmission(padsLeft[7]);
    }



    /**************
     * PADS RIGHT *
     **************/
    public void TriggerPad0RightOn()
    {
        EnablePadEmission(padsRight[0]);
    }

    public void TriggerPad0RightOff()
    {
        DisableEmission(padsRight[0]);
    }

    public void TriggerPad1RightOn()
    {
        EnablePadEmission(padsRight[1]);
    }

    public void TriggerPad1RightOff()
    {
        DisableEmission(padsRight[1]);
    }

    public void TriggerPad2RightOn()
    {
        EnablePadEmission(padsRight[2]);
    }

    public void TriggerPad2RightOff()
    {
        DisableEmission(padsRight[2]);
    }

    public void TriggerPad3RightOn()
    {
        EnablePadEmission(padsRight[3]);
    }

    public void TriggerPad3RightOff()
    {
        DisableEmission(padsRight[3]);
    }

    public void TriggerPad4RightOn()
    {
        EnablePadEmission(padsRight[4]);
    }

    public void TriggerPad4RightOff()
    {
        DisableEmission(padsRight[4]);
    }

    public void TriggerPad5RightOn()
    {
        EnablePadEmission(padsRight[5]);
    }

    public void TriggerPad5RightOff()
    {
        DisableEmission(padsRight[5]);
    }

    public void TriggerPad6RightOn()
    {
        EnablePadEmission(padsRight[6]);
    }

    public void TriggerPad6RightOff()
    {
        DisableEmission(padsRight[6]);
    }

    public void TriggerPad7RightOn()
    {
        EnablePadEmission(padsRight[7]);
    }

    public void TriggerPad7RightOff()
    {
        DisableEmission(padsRight[7]);
    }
}
