# UDJ-1000
MIDI-controlled 4-channel DJ controller prop for VRChat worlds. The UDJ-1000's knobs, faders, buttons & jogwheels are synced to reflect the positions of a physical DDJ-1000 in real time.

[![UDJ-1000 next to real life DJ controller](./Docs/img/youtube-thumbnail.jpg)](https://www.youtube.com/watch?v=S0iXARL-Q10)

Watch the [demo video on Youtube](https://www.youtube.com/watch?v=lzRvLQKQWvw)


## Setup
### Requirements
- Unity 2019.4.31f1
- [VRChat SDK3 Worlds](https://vrchat.com/home/download)
- [UdonSharp](https://github.com/vrchat-community/UdonSharp)
- Python 3.6+ for [mido](https://pypi.org/project/mido/) & rtmidi backend
- [loopMIDI](https://www.tobias-erichsen.de/software/loopmidi.html) or equivalent virtual MIDI loopback
- (Optional) [Poiyomi Toon Shader](https://github.com/poiyomi/PoiyomiToonShader)


### Getting Started
1. Import VRChat SDK3 Worlds & UdonSharp
2. Import UDJ-1000 unitypackage
3. Install mido & rtmidi backend:
```bash
pip install mido python-rtmidi
```
4. Install loopMidi & create a virtual MIDI port
5. Run midifilter.py (no arguments lists all available MIDI port names)
```bash
> python midifilter.py
MIDI Inputs:  ['DDJ-1000 0', 'loopMIDI Port 1']
MIDI Outputs:  ['Microsoft GS Wavetable Synth 0', 'DDJ-1000 1', 'loopMIDI Port 2']
> python midifilter.py "DDJ-1000 0" "loopMIDI Port 2"
Running...
```
6. In Unity, select the loopMIDI port under `VRChat SDK > Utilities > MIDI`
7. Play

## Detailed Setup Tutorials
- [MIDI Sync + Rekordbox Control using Separate MacOS DJ Computer](https://github.com/laserimouto/UDJ-1000/wiki/Using-a-separate-Mac-for-MIDI-Sync-&-Rekordbox-control) (Recommended)
- [Windows Only MIDI Sync + Rekordbox Control](https://github.com/laserimouto/UDJ-1000/wiki/Windows-only-MIDI-Sync-&--Rekordbox-control)

## Demo
Watch the [demo video on Youtube](https://www.youtube.com/watch?v=lzRvLQKQWvw).

You can also try it out in VRChat using the link below:

https://vrchat.com/home/launch?worldId=wrld_a3f6be81-cf26-4d45-b7e5-5817ed70914c

(Note: You may need to change your [VRChat launch options](https://docs.vrchat.com/docs/realtime-midi) if you have multiple MIDI devices connected)


## Known Issues/Bugs
### VRChat Client / Unity crashes when faders are moved too quickly
Due to an [existing bug with VRChat's MIDI implementation](https://feedback.vrchat.com/vrchat-udon-closed-alpha-bugs/p/when-many-midi-events-are-received-references-outside-the-buffer-range-occur), if too many MIDI CC messages are received too quickly the Unity Editor will crash on exiting play mode, and the VRChat client will crash on changing/reloading the world. As a workaround, `midifilter.py` selectively drops MIDI CC messages from the crossfader, channel faders and tempo sliders to remain under a safe threshold.

#### Usage
```bash
python midifilter.py "DDJ-1000" "<MIDI OUTPUT NAME>"
```

where `<MIDI OUTPUT NAME>` is the name of the loopMIDI virtual port or equivalent. This will be the device used by VRChat to receive MIDI.