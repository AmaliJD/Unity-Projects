using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayNotes : MonoBehaviour
{
    [Range(110, 512)]  //Creates a slider in the inspector
    public float frequency;
    float root; float octave = 1;

    [Range(0f, 1f)]
    public float volume;

    [Range(1, 16)]
    public int periods;

    public float sampleRate = 44100;
    public float waveLengthInSeconds = 2.0f;
    private float TWELVE_TONE_INTERVAL = Mathf.Pow(2, (1f/12f));

    AudioSource audioSource;
    int timeIndex = 0;

    bool[] audioPlaying;
    float numNotes = 0;

    public enum State { just, twelve, auto };
    public State state;

    public LineRenderer line;
    int indexes = 120;
    float offset = 0;
    private int maxoffset = 16;

    public Transform KeyParent;
    SpriteRenderer[] keySprites;
    TextMeshPro[] intervals;
    string[] intervalsTwelve = new string[] { "1", "1.059", "1.122", "1.189", "1.260", "1.335", "1.414", "1.498", "1.587", "1.682", "1.782", "1.888", "2" };
    string[] intervalsJust = new string[] {   "1", "1.067", "1.125", "1.2",   "1.25",  "1.33",  "1.4",   "1.5",   "1.6",   "1.67",  "1.8",   "1.875", "2" };

    //public TMP_Dropdown dropdown;
    public Image justButtonImage, twelveButtonImage;
    public Slider periodSlider;
    TextMeshProUGUI periodSliderText;
    int mouseScroll = 0;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0; //force 2D sound
        audioSource.Stop(); //avoids audiosource from starting to play automatically

        line.positionCount = indexes;
        maxoffset = 350 * (int)sampleRate;
        periodSliderText = periodSlider.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        audioPlaying = new bool[13];
        audioSource.Play();

        keySprites = new SpriteRenderer[13];
        intervals = new TextMeshPro[13];
        for (int i = 0; i < 13; i++)
        {
            keySprites[i] = KeyParent.GetChild(i).GetChild(0).GetComponent<SpriteRenderer>();
            intervals[i] = KeyParent.GetChild(i).GetChild(2).GetComponent<TextMeshPro>();
        }

        updateIntervals();

        root = frequency;
    }

    void Update()
    {
        audioSource.volume = volume;
        //state = (State)dropdown.value;
        //indexes = Mathf.Clamp(50 * (periods/4), 50, 300);
        //line.positionCount = indexes;

        getInput(KeyCode.A, 0);
        getInput(KeyCode.W, 1);
        getInput(KeyCode.S, 2);
        getInput(KeyCode.E, 3);
        getInput(KeyCode.D, 4);
        getInput(KeyCode.F, 5);
        getInput(KeyCode.T, 6);
        getInput(KeyCode.G, 7);
        getInput(KeyCode.Y, 8);
        getInput(KeyCode.H, 9);
        getInput(KeyCode.U, 10);
        getInput(KeyCode.J, 11);
        getInput(KeyCode.K, 12);

        if (numNotes > 0 && !audioSource.isPlaying)
        {
            timeIndex = 0;
            offset = 0;
            audioSource.Play();
        }
        else if(numNotes == 0 && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            state = State.just;
            updateIntervals();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            state = State.twelve;
            updateIntervals();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            audioPlaying = new bool[13];
            numNotes = 0;
        }

        if (Input.GetKeyDown(KeyCode.Minus))
        {
            octave /= 2;
            octave = Mathf.Clamp(octave, 0.5f, 2f);
        }
        else if (Input.GetKeyDown(KeyCode.Equals))
        {
            octave *= 2;
            octave = Mathf.Clamp(octave, 0.5f, 2f);
        }

        mouseScroll += (int)Input.mouseScrollDelta.y;

        if (mouseScroll >= 4)
        {
            periodSlider.value = Mathf.Clamp(periods + 1, 1, 16);
            mouseScroll = 2;
        }
        else if (mouseScroll <= 0)
        {
            periodSlider.value = Mathf.Clamp(periods - 1, 1, 16);
            mouseScroll = 2;
        }

        periods = (int)periodSlider.value;
        periodSliderText.text = "" + periods;

        //Debug.Log(numNotes);

        indicateKeys();
        setLinePositions();
        //updateIntervals();

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void getInput(KeyCode key, int index)
    {
        if (Input.GetKeyDown(key) && !audioPlaying[index])
        {
            audioPlaying[index] = true;
            numNotes++;
        }
        else if (Input.GetKeyUp(key) && audioPlaying[index])
        {

            audioPlaying[index] = false;
            numNotes--;
        }

        /*if (Input.GetKeyDown(key))
        {
            if (!audioPlaying[index])
            {
                audioPlaying[index] = true;
                numNotes++;
            }
            else
            {
                audioPlaying[index] = false;
                numNotes--;
            }
        }*/
    }

    public void pressOnScreenKey(int index)
    {
        if (!audioPlaying[index])
        {
            audioPlaying[index] = true;
            numNotes++;
        }
        else
        {
            audioPlaying[index] = false;
            numNotes--;
        }
    }

    public void pressResetKey()
    {
        audioPlaying = new bool[13];
        numNotes = 0;
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (numNotes > 0)
        {
            for (int i = 0; i < data.Length; i += channels)
            {
                data[i] = 0;
                data[i + 1] = 0;

                for (int j = 0; j < 13; j++)
                {
                    if (audioPlaying[j])
                    {
                        data[i] += CreateSine(timeIndex, GetFrequency(j), sampleRate, 0) / numNotes;

                        if (channels == 2)
                        {
                            data[i + 1] += CreateSine(timeIndex, GetFrequency(j), sampleRate, 0) / numNotes;
                        }
                    }
                }
                /*data[i] = CreateSine(timeIndex, GetFrequency(0), sampleRate) / numNotes;

                if (channels == 2)
                {
                    data[i + 1] = CreateSine(timeIndex, GetFrequency(0), sampleRate) / numNotes;
                }*/

                timeIndex++;

                //if timeIndex gets too big, reset it to 0
                if (timeIndex >= (sampleRate * waveLengthInSeconds))
                {
                    timeIndex = 0;
                }
            }
        }
    }

    public float GetFrequency(int tone)
    {
        float adjustedFrequency = frequency * octave;
        switch (state)
        {
            case State.twelve:
                adjustedFrequency = adjustedFrequency * Mathf.Pow(TWELVE_TONE_INTERVAL, tone);
                break;

            case State.just:
                switch(tone)
                {
                    case 0:
                        adjustedFrequency = adjustedFrequency * 1f; break; // Root, 1
                    case 1:
                        adjustedFrequency = adjustedFrequency * (16f / 15f); break; // Semitone, minor second - 1.067
                    case 2:
                        adjustedFrequency = adjustedFrequency * (9f / 8f); break; // Tone, major second - 1.125
                    case 3:
                        adjustedFrequency = adjustedFrequency * (6f / 5f); break; // minor third - 1.2
                    case 4:
                        adjustedFrequency = adjustedFrequency * (5f / 4f); break; // major third - 1.25
                    case 5:
                        adjustedFrequency = adjustedFrequency * (4f / 3f); break; // perfect fourth - 1.33
                    case 6:
                        adjustedFrequency = adjustedFrequency * (7f / 5f); break; // Tritone - 1.4
                    case 7:
                        adjustedFrequency = adjustedFrequency * (3f / 2f); break; // perfect fifth - 1.5
                    case 8:
                        adjustedFrequency = adjustedFrequency * (8f / 5f); break; // minor sixth - 1.6
                    case 9:
                        adjustedFrequency = adjustedFrequency * (5f / 3f); break; // major sixth - 1.67
                    case 10:
                        adjustedFrequency = adjustedFrequency * (9f / 5f); break; // minor seventh - 1.8
                    case 11:
                        adjustedFrequency = adjustedFrequency * (15f / 8f); break; // major seventh - 1.875
                    case 12:
                        adjustedFrequency = adjustedFrequency * 2f; break;  // Octave - 2
                }
                break;
        }

        return adjustedFrequency;
    }

    //Creates a sinewave
    public float CreateSine(int timeIndex, float frequency, float sampleRate, float offset)
    {
        return Mathf.Sin((2 * Mathf.PI * timeIndex * frequency / sampleRate) + offset);
    }

    public void setLinePositions()
    {
        for (int i = 0; i < indexes; i++)
        {
            float amplitude = 0;
            if (numNotes > 0)
            {
                for (int j = 0; j < 13; j++)
                {
                    if (audioPlaying[j]) amplitude += CreateSine(periods * ((int)((sampleRate / frequency / indexes) * i + (offset % (sampleRate / GetFrequency(j))))), GetFrequency(j), sampleRate, 0) / numNotes;
                    //if (audioPlaying[j]) amplitude += CreateSine(periods * ((int)((sampleRate / frequency / indexes) * i)), GetFrequency(j), sampleRate, offset) / numNotes;
                }
            }           
            line.SetPosition(i, new Vector3(-8.4f + (16.94f / ((float)indexes) * i), 2f * amplitude + 2.5f, 0));
        }

        offset = (offset + (int)sampleRate) % maxoffset;
        //offset = (offset + ((2 * Mathf.PI) * 1.3333f));
        Debug.Log(offset);
    }

    public void indicateKeys()
    {
        for (int j = 0; j < 13; j++)
        {
            if (audioPlaying[j])
            {
                keySprites[j].color = new Color(keySprites[j].color.r, keySprites[j].color.g, keySprites[j].color.b, 0.6f);
            }
            else
            {
                keySprites[j].color = new Color(keySprites[j].color.r, keySprites[j].color.g, keySprites[j].color.b, 1f);
            }
        }
    }

    public void updateIntervals()
    {
        int i = 0;
        foreach(TextMeshPro tmp in intervals)
        {
            switch(state)
            {
                case State.twelve:
                    tmp.text = intervalsTwelve[i];
                    twelveButtonImage.color = new Color(.87f, .87f, .87f);
                    justButtonImage.color = new Color(.11f, .11f, .11f);
                    twelveButtonImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;
                    justButtonImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
                    break;

                case State.just:
                    tmp.text = intervalsJust[i];
                    justButtonImage.color = new Color(.87f, .87f, .87f);
                    twelveButtonImage.color = new Color(.11f, .11f, .11f);
                    justButtonImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;
                    twelveButtonImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
                    break;
            }

            i++;
        }
    }

    public void SetState(int i)
    {
        state = (State)i;
        updateIntervals();
    }
}