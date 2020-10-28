using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    // Start is called before the first frame update
    public string levelName;
    public AudioClip music;

    AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent(typeof(AudioSource)) as AudioSource;
        audioSource.PlayOneShot(music);
    }

    void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Time.time);
    }
}
