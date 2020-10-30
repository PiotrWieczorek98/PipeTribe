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
    UIComponents uIComponents;
    Text UItimer;
    void Start()
    {
        audioSource = GetComponent(typeof(AudioSource)) as AudioSource;
        audioSource.PlayOneShot(music);

        uIComponents = GameObject.FindGameObjectWithTag("UI").GetComponent(typeof(UIComponents)) as UIComponents;
        UItimer = uIComponents.GetTimer;
    }

    void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        UItimer.text = Time.time.ToString();
    }
    public float GetMusicNotesOffset { get { return MusicNotesOffset; } }
    public float GetMinDeltaTimeToSwitch { get { return minDeltaTimeToSwitch; } }


}
