﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class ButtonRecorder : MonoBehaviour
{
    public KeyCode recoredKey, startKey, stopKey, saveKey;
    public string levelName;

    float timeHeld = 0;
    float timeWhenPressed = 0;
    float timer = 0;
    List<(float, float)> timeline;

    bool recordingStarted = false;
    AudioSource musicPlayer;
    public AudioClip levelMusic;

    UIComponents uIComponents;
    Text UItimer;

    // Start is called before the first frame update
    private void Awake()
    {
        musicPlayer = GetComponent(typeof(AudioSource)) as AudioSource;
        uIComponents = GameObject.FindGameObjectWithTag("UI").GetComponent(typeof(UIComponents)) as UIComponents;
        UItimer = uIComponents.GetTimer;

        timeline = new List<(float, float)> { };
    }

    void Start()
    {
        //LoadRecording(levelName);
        //SaveRecordingToTxt();
    }

    // Update is called once per frame
    void Update()
    {
        if (recordingStarted)
            timer += Time.deltaTime;
        UItimer.text = timer.ToString();

        if (Input.GetKeyDown(startKey) && !recordingStarted)
        {
            recordingStarted = true;
            musicPlayer.PlayOneShot(levelMusic);
        }

        else if (Input.GetKeyDown(stopKey) && recordingStarted)
        {
            recordingStarted = false;
            musicPlayer.Stop();
            timer = 0;
        }
        else if (Input.GetKeyDown(saveKey))
        {
            SaveRecordingToDat();
            SaveRecordingToTxt();
        }


        if (recordingStarted)
        {
            if (Input.GetKeyDown(recoredKey))
            {
                timeWhenPressed = timer;
            }
            else if (Input.GetKeyUp(recoredKey))
            {
                timeHeld = timer - timeWhenPressed;
                if (timeHeld < 0.5)
                    timeHeld = 0;


                timeline.Add((timeWhenPressed, timeHeld));
                Debug.Log(timeWhenPressed + " / " + timeHeld);
            }
        }
    }

    void SaveRecordingToDat()
    {
        string destination = Application.dataPath + "/levels/" + levelName + ".dat";
        FileStream file;

        Directory.CreateDirectory(Application.dataPath + "/levels/");

        if (File.Exists(destination)) 
            file = File.OpenWrite(destination);
        else 
            file = File.Create(destination);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, timeline);
        file.Close();

        for(int i = 0; i< timeline.Count; i++)
        {
            Debug.Log(timeline[i].Item1);
        }
    }

    void SaveRecordingToTxt()
    {
        string destination = Application.dataPath + "/levels/" + levelName + ".txt";
        using (TextWriter tw = new StreamWriter(destination))
        {
            foreach (var tuple in timeline)
            {
                Debug.Log(tuple);
                tw.WriteLine(tuple.Item1.ToString() + " /  " + tuple.Item2.ToString());
            }
        }
    }

    public void LoadRecording(string filename)
    {
        string destination = Application.dataPath + "/levels/" + filename + ".dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError(destination + " - File not found");
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        timeline = (List<(float, float)>)bf.Deserialize(file);
        file.Close();
    }
}
