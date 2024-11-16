using NAudio.Midi;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;
using System.Text;
using TMPro;

public class Visualizer : MonoBehaviour
{
    public SpriteRenderer[] sprites;
    public TextMeshPro[] channelOnButtons;
    public SpriteRenderer[] channelButtonBG;
    //string[] noteNames = new[] { "A", "E", "C#", "F#", "B", "G#", "D#", "A#", "G", "C", "F", "D" }; // Mali mode
    //string[] noteNames = new[] { "A", "E", "C#", "F", "C", "G#", "D#", "A#", "G", "B", "F#", "D" }; // amali mode 2
    //string[] noteNames = new[] { "A", "E", "D", "B", "F#", "G", "D#", "A#", "G#", "F", "C", "C#" }; // amali mode 3
    //string[] noteNames = new[] { "A", "C#", "D", "F", "B", "G#", "D#", "A#", "F#", "G", "E", "C" }; // amali mode 4
    string[] noteNames = new[] { "A", "E", "B", "F#", "C#", "G#", "D#", "A#", "F", "C", "G", "D" }; // major 5ths - 7 semitones
    //string[] noteNames = new[] { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#" }; // in order

    int[] channelsActive = new int[16];
    int[,] noteCountPerChannel = new int[16,12];
    int[] noteCount = new int[12];
    [HideInInspector]
    public List<int> unmutedChannels;
    private List<int> invisibleChannels = new();

    private void Awake()
    {
        for (int i = 0; i < noteCount.Length; i++)
            sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, 0);
        unmutedChannels = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        invisibleChannels.Add(9);
    }

    public void MuteChannel(bool mute, int channel)
    {
        if(mute)
        {
            unmutedChannels.Remove(channel);
            ClearChannel(channel);
            channelOnButtons[channel].color = new Color(1, 1, 1, .01f);
        }
        else
        {
            unmutedChannels.Add(channel);
            channelOnButtons[channel].color = new Color(1, 1, 1, .5f);
        }
    }

    public void ClearChannel(int channel)
    {
        for (int i = 0; i < noteCountPerChannel.GetLength(1); i++)
            noteCountPerChannel[channel, i] = 0;
    }

    public void ClearAllChannels()
    {
        for (int i = 0; i < noteCountPerChannel.GetLength(0); i++)
            for (int j = 0; j < noteCountPerChannel.GetLength(1); j++)
                noteCountPerChannel[i, j] = 0;
    }

    public void ProcessNote(Note note, bool noteOn, int channel)
    {
        if (noteOn)
            channelsActive[channel] = 1;

        if (!string.IsNullOrEmpty(note.name) && unmutedChannels.Contains(channel) && noteOn ? !invisibleChannels.Contains(channel) : true)
        {
            noteCountPerChannel[channel, Array.IndexOf(noteNames, note.name)] += noteOn ? 1 : -1;
            noteCountPerChannel[channel, Array.IndexOf(noteNames, note.name)] = Mathf.Max(noteCountPerChannel[channel, Array.IndexOf(noteNames, note.name)], 0);
            //Debug.Log($"Index of {note.name}: {Array.IndexOf(noteNames, note.name)}");
            //if(channel != 9)
            //{
            //    noteCount[Array.IndexOf(noteNames, note.name)] += noteOn ? 1 : -1;
            //    noteCount[Array.IndexOf(noteNames, note.name)] = Mathf.Max(noteCount[Array.IndexOf(noteNames, note.name)], 0);
            //}
            //else
            //{
            //    noteCount[12] += noteOn ? 1 : -1;
            //    noteCount[12] = Mathf.Max(noteCount[12], 0);
            //}
        }
    }

    public void LateUpdate()
    {
        SumNoteCounts();
        //Debug.Log($"{noteCount[0]} {noteCount[1]} {noteCount[2]} {noteCount[3]} {noteCount[4]} {noteCount[5]} {noteCount[6]} {noteCount[7]} {noteCount[8]} {noteCount[9]} {noteCount[10]} {noteCount[11]}");
        for (int i = 0; i < noteCount.Length; i++)
        {
            if(noteCount[i] > 0)
                sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, 1);
            else
                sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, Mathf.Clamp01(sprites[i].color.a * .85f));
        }

        for (int i = 0; i < channelButtonBG.Length; i++)
        {
            //bool activeOnChannel = false;
            //for (int j = 0; j < noteCountPerChannel.GetLength(1); j++)
            //{
            //    if (noteCountPerChannel[i,j] > 0)
            //    {
            //        activeOnChannel = true;
            //        break;
            //    }
            //}
            //channelButtonBG[i].color = new Color(1,1,1, activeOnChannel ? 0.2f : Mathf.MoveTowards(channelButtonBG[i].color.a, 0f, .85f));
            channelButtonBG[i].color = new Color(1, 1, 1, channelsActive[i] != 0 ? 0.25f : Mathf.MoveTowards(channelButtonBG[i].color.a, 0f, 1 * Time.deltaTime));
            channelsActive[i] = 0;
        }
    }

    void SumNoteCounts()
    {
        for (int i = 0; i < noteCountPerChannel.GetLength(1); i++) //12
        {
            int count = 0;
            for (int j = 0; j < noteCountPerChannel.GetLength(0); j++) //16
            {
                count += noteCountPerChannel[j, i];
            }

            noteCount[i] = count;
        }
    }
}
