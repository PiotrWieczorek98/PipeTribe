using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public string levelName;
    public AudioClip music;
    public float MusicNotesOffset;
    // Minimum time between notes to allow change of segment the note is placed on
    public float minDeltaTimeToSwitch = 0.5f;

    AudioSource audioSource;
    GameObject canvas;
    Text UItimer;
    Component[] texts;
    void Start()
    {
        audioSource = GetComponent(typeof(AudioSource)) as AudioSource;
        audioSource.PlayOneShot(music);
    }

    void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;

        canvas = GameObject.FindGameObjectWithTag("UI");
        texts = canvas.GetComponentsInChildren(typeof(Text));
        foreach(Text text in texts)
        {
            if(text.name == "Timer")
            {
                UItimer = text;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        UItimer.text = Time.time.ToString();
    }
    public float GetMusicNotesOffset { get { return MusicNotesOffset; } }
    public float GetMinDeltaTimeToSwitch { get { return minDeltaTimeToSwitch; } }


}
