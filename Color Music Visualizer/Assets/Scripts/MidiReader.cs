using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAudio;
using NAudio.Midi;
using System;
using System.Linq;

public class MidiReader : MonoBehaviour
{
    private void Awake()
    {
        ReadMidi("Assets/Resources/" + gameObject.GetComponent<MIDIPlayer>().midiFilePath + ".txt");
    }

    void ReadMidi(string fileName)
    {
        var strictMode = false;
        var midiFile = new MidiFile(fileName, strictMode);

        Debug.LogFormat("Format {0}, Tracks {1}, Delta Ticks Per Quarter Note {2}",
            midiFile.FileFormat, midiFile.Tracks, midiFile.DeltaTicksPerQuarterNote);

        var timeSignature = midiFile.Events[0].OfType<TimeSignatureEvent>().FirstOrDefault();
        for (int n = 0; n < midiFile.Tracks; n++)
            foreach (var midiEvent in midiFile.Events[n])
                Debug.LogFormat("{0}\r\n", midiEvent);
    }

    private string ToMBT(long eventTime, int ticksPerQuarterNote, TimeSignatureEvent timeSignature)
    {
        int beatsPerBar = timeSignature == null ? 4 : timeSignature.Numerator;
        int ticksPerBar = timeSignature == null ? ticksPerQuarterNote * 4 : (timeSignature.Numerator * ticksPerQuarterNote * 4) / (1 << timeSignature.Denominator);
        int ticksPerBeat = ticksPerBar / beatsPerBar;
        long bar = 1 + (eventTime / ticksPerBar);
        long beat = 1 + ((eventTime % ticksPerBar) / ticksPerBeat);
        long tick = eventTime % ticksPerBeat;
        return String.Format("{0}:{1}:{2}", bar, beat, tick);
    }
}
