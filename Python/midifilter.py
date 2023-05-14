import mido
import sys
import time

if len(sys.argv) < 3:
    print("MIDI Inputs: ", mido.get_input_names())
    print("MIDI Outputs: ", mido.get_output_names())
    exit()

MIDI_IN_NAME = sys.argv[1]
MIDI_OUT_NAME = sys.argv[2]
FADER_CH_LSB = [32, 51, 63]
FADER_CH_MSB = [0, 19, 31]

last_value_sent = {}
for channel in range(7):
    for control in FADER_CH_MSB:
        last_value_sent[(channel, control)] = -1

with mido.open_input(MIDI_IN_NAME) as midi_in, mido.open_output(MIDI_OUT_NAME) as midi_out:
    print("Running...")
    while True:
        for msg in midi_in.iter_pending():
            if msg.is_cc() and msg.control in FADER_CH_LSB:
                continue

            if msg.is_cc() and msg.control in FADER_CH_MSB:
                if msg.value == last_value_sent[(msg.channel, msg.control)]:
                    continue
                else:
                    last_value_sent[(msg.channel, msg.control)] = msg.value

            print(msg)
            midi_out.send(msg)
        
        time.sleep(1 / 1000)
