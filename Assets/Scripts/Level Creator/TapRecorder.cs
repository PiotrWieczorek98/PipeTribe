using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapRecorder : MonoBehaviour
{
    public KeyCode recoredKey, startKey, stopKey;

    float timeHeld = 0;
    float timeWhenPressed = 0;

    bool isRecording = false;
    AudioSource audioSource;
    TimelineIndicator timelineIndicator;

    // Start is called before the first frame update
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        timelineIndicator = GetComponentInChildren<TimelineIndicator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(startKey) && !isRecording)
        {
            isRecording = true;
            audioSource.Play();
        }

        else if (Input.GetKeyDown(stopKey) && isRecording)
        {
            isRecording = false;
            audioSource.Pause();
        }


        if (Input.GetKeyDown(recoredKey))
        {
            timeWhenPressed = audioSource.time;
        }
        else if (Input.GetKeyUp(recoredKey))
        {
            timeHeld = audioSource.time - timeWhenPressed;
            if (timeHeld < 0.5)
                timeHeld = 0;

            timelineIndicator.addNoteObject(timeWhenPressed, timeHeld);
            Debug.Log(timeWhenPressed + " / " + timeHeld);
        }
    }

    public bool IsRecording { get { return isRecording; } }
}
