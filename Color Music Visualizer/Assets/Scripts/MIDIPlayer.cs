using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CSharpSynth.Effects;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using CSharpSynth.Midi;
using System.Text;

[RequireComponent(typeof(AudioSource))]
public class MIDIPlayer : MonoBehaviour
{
    //Public
    //Check the Midi's file folder for different songs
    public string midiFilePath = "Midis/Groove.mid";
    public bool ShouldPlayFile = true;

    //Try also: "FM Bank/fm" or "Analog Bank/analog" for some different sounds
    public string bankFilePath = "GM Bank/gm";
    public int bufferSize = 1024;
    public int midiNote = 60;
    public int midiNoteVolume = 100;
    [Range(0, 127)] //From Piano to Gunshot
    public int midiInstrument = 0;
    //Private 
    private float[] sampleBuffer;
    private float gain = 1f;
    private MidiSequencer midiSequencer;
    private StreamSynthesizer midiStreamSynthesizer;

    private float sliderValue = 1.0f;
    private float maxSliderValue = 127.0f;

    Visualizer visualizer;
    bool playMode = false;

    // Awake is called when the script instance
    // is being loaded.
    void Awake()
    {
        midiStreamSynthesizer = new StreamSynthesizer(44100, 2, bufferSize, 40);
        sampleBuffer = new float[midiStreamSynthesizer.BufferSize];
        
        midiStreamSynthesizer.LoadBank(bankFilePath);

        midiSequencer = new MidiSequencer(midiStreamSynthesizer);

        //These will be fired by the midiSequencer when a song plays. Check the console for messages if you uncomment these
        midiSequencer.NoteOnEvent += new MidiSequencer.NoteOnEventHandler (MidiNoteOnHandler);
        midiSequencer.NoteOffEvent += new MidiSequencer.NoteOffEventHandler (MidiNoteOffHandler);

        visualizer = gameObject.GetComponent<Visualizer>();
    }

    void LoadSong(string midiPath)
    {
        midiSequencer.LoadMidi(midiPath, false);
        //midiSequencer.MuteAllChannels();
        //midiSequencer.UnMuteChannel(1);
        for(int i = 0; i < 16; i++)
            if(!visualizer.unmutedChannels.Contains(i))
                midiSequencer.MuteChannel(i);
        midiSequencer.Play();
    }

    // Update is called every frame, if the
    // MonoBehaviour is enabled.
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ShouldPlayFile = !ShouldPlayFile;
            visualizer.ClearAllChannels();
        }

        if (!midiSequencer.isPlaying)
        {
            //if (!GetComponent<AudioSource>().isPlaying)
            if (ShouldPlayFile)
            {
                LoadSong(midiFilePath);
            }
        }
        else if (!ShouldPlayFile)
        {
            midiSequencer.Stop(true);
        }

        CheckKey(KeyCode.A, 0);
        CheckKey(KeyCode.W, 1);
        CheckKey(KeyCode.S, 2);
        CheckKey(KeyCode.E, 3);
        CheckKey(KeyCode.D, 4);
        CheckKey(KeyCode.F, 5);
        CheckKey(KeyCode.T, 6);
        CheckKey(KeyCode.G, 7);
        CheckKey(KeyCode.Y, 8);
        CheckKey(KeyCode.H, 9);
        CheckKey(KeyCode.U, 10);
        CheckKey(KeyCode.J, 11);
        CheckKey(KeyCode.K, 12);

        if (!ShouldPlayFile)
        {
            CheckKey(KeyCode.Alpha1, -12);
            CheckKey(KeyCode.Alpha2, -11);
            CheckKey(KeyCode.Alpha3, -10);
            CheckKey(KeyCode.Alpha4, -9);
            CheckKey(KeyCode.Alpha5, -8);
            CheckKey(KeyCode.Alpha6, -7);
            CheckKey(KeyCode.Alpha7, -6);
            CheckKey(KeyCode.Alpha8, -5);
            CheckKey(KeyCode.Alpha9, -4);
            CheckKey(KeyCode.Alpha0, -3);
            CheckKey(KeyCode.Minus, -2);
            CheckKey(KeyCode.Equals, -1);
            CheckKey(KeyCode.Backspace, 0);
        }
        else if (ShouldPlayFile)
        {
            CheckChannelMuted(KeyCode.BackQuote, 0);
            CheckChannelMuted(KeyCode.Alpha1, 1);
            CheckChannelMuted(KeyCode.Alpha2, 2);
            CheckChannelMuted(KeyCode.Alpha3, 3);
            CheckChannelMuted(KeyCode.Alpha4, 4);
            CheckChannelMuted(KeyCode.Alpha5, 5);
            CheckChannelMuted(KeyCode.Alpha6, 6);
            CheckChannelMuted(KeyCode.Alpha7, 7);
            CheckChannelMuted(KeyCode.Alpha8, 8);
            CheckChannelMuted(KeyCode.Alpha9, 9);
            CheckChannelMuted(KeyCode.Alpha0, 10);
            CheckChannelMuted(KeyCode.Minus, 11);
            CheckChannelMuted(KeyCode.Equals, 12);
            CheckChannelMuted(KeyCode.LeftBracket, 13);
            CheckChannelMuted(KeyCode.RightBracket, 14);
            CheckChannelMuted(KeyCode.Backslash, 15);
        }
    }

    void CheckChannelMuted(KeyCode keyCode, int channel)
    {
        if (Input.GetKeyDown(keyCode))
        {
            if (midiSequencer.isChannelMuted(channel))
            {
                midiSequencer.UnMuteChannel(channel);
                visualizer.MuteChannel(false, channel);
            }
            else
            {
                midiSequencer.MuteChannel(channel);
                visualizer.MuteChannel(true, channel);
            }
        }
    }

    void CheckKey(KeyCode keyCode, int offset)
    {
        if (Input.GetKeyDown(keyCode))
        {
            midiStreamSynthesizer.NoteOn(0, midiNote + offset, midiNoteVolume, midiInstrument);
            MidiNoteOnHandler(0, midiNote + offset, midiNoteVolume);
        }
        else if (Input.GetKeyUp(keyCode))
        {
            midiStreamSynthesizer.NoteOff(0, midiNote + offset);
            MidiNoteOffHandler(0, midiNote + offset);
        }
    }

    // See http://unity3d.com/support/documentation/ScriptReference/MonoBehaviour.OnAudioFilterRead.html for reference code
    //	If OnAudioFilterRead is implemented, Unity will insert a custom filter into the audio DSP chain.
    //
    //	The filter is inserted in the same order as the MonoBehaviour script is shown in the inspector. 	
    //	OnAudioFilterRead is called everytime a chunk of audio is routed thru the filter (this happens frequently, every ~20ms depending on the samplerate and platform). 
    //	The audio data is an array of floats ranging from [-1.0f;1.0f] and contains audio from the previous filter in the chain or the AudioClip on the AudioSource. 
    //	If this is the first filter in the chain and a clip isn't attached to the audio source this filter will be 'played'. 
    //	That way you can use the filter as the audio clip, procedurally generating audio.
    //
    //	If OnAudioFilterRead is implemented a VU meter will show up in the inspector showing the outgoing samples level. 
    //	The process time of the filter is also measured and the spent milliseconds will show up next to the VU Meter 
    //	(it turns red if the filter is taking up too much time, so the mixer will starv audio data). 
    //	Also note, that OnAudioFilterRead is called on a different thread from the main thread (namely the audio thread) 
    //	so calling into many Unity functions from this function is not allowed ( a warning will show up ). 	
    private void OnAudioFilterRead(float[] data, int channels)
    {
        //This uses the Unity specific float method we added to get the buffer
        midiStreamSynthesizer.GetNext(sampleBuffer);

        for (int i = 0; i < data.Length; i++)
        {
            data[i] = sampleBuffer[i] * gain;
        }
    }

    public void MidiNoteOnHandler(int channel, int note, int velocity)
    {
        //Debug.Log($"NoteOn: {NoteConverter(note)}, Channel: {channel}");
        visualizer.ProcessNote(NoteConverter(note), true, channel);
    }

    public void MidiNoteOffHandler(int channel, int note)
    {
        //Debug.Log($"NoteOff: {NoteConverter(note)}, Channel: {channel}");
        visualizer.ProcessNote(NoteConverter(note), false, channel);
    }

    string[] noteNames = new[] {"A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#"};
    Note NoteConverter(int rawNote)
    {
        if(rawNote < 0 || rawNote > 127 /*|| (rawNote >= 108 && rawNote <= 128)*/)
            return new Note();

        Note note = new Note();
        note.raw = rawNote;
        note.index = ((rawNote) % 12);
        note.octave = (rawNote) / 12;
        note.name = noteNames[note.index];

        return note;
    }
}
