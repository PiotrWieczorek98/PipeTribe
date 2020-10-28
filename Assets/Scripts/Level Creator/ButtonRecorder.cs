using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class ButtonRecorder : MonoBehaviour
{
    public KeyCode recoredKey, startKey, stopKey;
    public string levelName;

    float timeHeld = 0;
    float timeWhenPressed = 0;
    List<(float, float)> timeline;

    bool recordingStarted = false;
    AudioSource musicPlayer;
    public AudioClip levelMusic;

    // Start is called before the first frame update
    void Start()
    {
        musicPlayer = GetComponent(typeof(AudioSource)) as AudioSource;
        timeline = new List<(float, float)> { };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(startKey) && !recordingStarted)
        {
            recordingStarted = true;
            musicPlayer.PlayOneShot(levelMusic);
        }

        else if (Input.GetKeyDown(stopKey) && recordingStarted)
        {
            SaveRecording();
            recordingStarted = false;
            musicPlayer.Stop();
        }

        if (recordingStarted)
        {
            if (Input.GetKeyDown(recoredKey))
            {
                timeWhenPressed = Time.time;
            }
            else if (Input.GetKeyUp(recoredKey))
            {
                timeHeld = Time.time - timeWhenPressed;
                if (timeHeld < 0.5)
                    timeHeld = 0;


                timeline.Add((timeWhenPressed, timeHeld));
                //Debug.Log(timeWhenPressed.ToString() + " - " + timeHeld.ToString());
            }
        }

    }

    void SaveRecording()
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
        List<(float, float)> loadedTimeline = (List<(float, float)>)bf.Deserialize(file);
        file.Close();
    }
}
