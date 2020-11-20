using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LevelDataPasser : MonoBehaviour
{
    // Save list by serialization
    public List<(float, float)> SaveRecordingToDat(string filename, TimelineIndicator timelineIndicator)
    {
        string destination = Application.dataPath + "/levels/" + transform.root.GetComponent<AudioSource>().clip.name + ".dat";
        FileStream file;

        Directory.CreateDirectory(Application.dataPath + "/levels/");

        if (File.Exists(destination))
            file = File.OpenWrite(destination);
        else
            file = File.Create(destination);

        BinaryFormatter bf = new BinaryFormatter();

        // Transform list from list of objects to list of tuples with only time attributes
        List<(float, float)> timelineNoteTuples = new List<(float, float)>();
        if (timelineIndicator.NotesObjects.Count == 0)
        {
            Debug.Log("List is empty!");
            return null;
        }

        foreach (Transform noteObject in timelineIndicator.NotesObjects)
        {
            NoteTimelineIndicator noteTimelineIndicator = noteObject.GetComponent<NoteTimelineIndicator>();
            timelineNoteTuples.Add((noteTimelineIndicator.Timestamp, noteTimelineIndicator.TimeHeld));
        }

        bf.Serialize(file, timelineNoteTuples);
        file.Close();

        for (int i = 0; i < timelineNoteTuples.Count; i++)
        {
            Debug.Log(timelineNoteTuples[i].Item1);
        }

        return timelineNoteTuples;
    }

    public void SaveRecordingToTxt(string filename, TimelineIndicator timelineIndicator)
    {
        string destination = Application.dataPath + "/levels/" + filename + ".txt";
        using (TextWriter tw = new StreamWriter(destination))
        {
            foreach (Transform noteObject in timelineIndicator.NotesObjects)
            {
                NoteTimelineIndicator noteTimelineIndicator = noteObject.GetComponent<NoteTimelineIndicator>();
                Debug.Log((noteTimelineIndicator.Timestamp, noteTimelineIndicator.TimeHeld));
                tw.WriteLine(noteTimelineIndicator.Timestamp.ToString() + " /  " + noteTimelineIndicator.TimeHeld.ToString());
            }
        }
    }
    public List<(float, float)> LoadRecording(string filename, TimelineIndicator timelineIndicator)
    {
        string destination = Application.dataPath + "/levels/" + filename + ".dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError(destination + " - File not found");
            return null;
        }

        BinaryFormatter bf = new BinaryFormatter();
        List<(float, float)> timelineNoteTuples = (List<(float, float)>)bf.Deserialize(file);

        foreach ((float, float) noteTuple in timelineNoteTuples)
        {
            timelineIndicator.addNoteObject(noteTuple.Item1, noteTuple.Item2);
        }

        file.Close();
        return timelineNoteTuples;
    }
}
