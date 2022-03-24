using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSource : MonoBehaviour
{
    public AudioSource audio_;
    public float realVolume;

    public int startSong = 0;
    public int endSong;
    public int endLoop;

    public bool playonawake;

    void Awake()
    {
        audio_ = GetComponent<AudioSource>();
        realVolume = audio_.volume;
        audio_.timeSamples = startSong;
    }

    private void Start()
    {
        if (playonawake)
        {
            Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (audio.isPlaying) { Debug.Log(audio.timeSamples); }
        if (endSong == 0) { return; }

        if (audio_.timeSamples >= endSong)
        {
            audio_.timeSamples = endLoop;
        }
    }

    // AudioSource Methods
    public void PlayORStop()
    {
        if (audio_.isPlaying)
        {
            Stop();
        }
        else
        {
            audio_.timeSamples = startSong;
            Play();
        }
    }
    public void Play()
    {
        audio_.Play();
    }

    public void Stop()
    {
        audio_.Stop();
    }

    public void Pause()
    {
        audio_.Pause();
    }
}
