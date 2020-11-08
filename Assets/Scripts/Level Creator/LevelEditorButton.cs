using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class LevelEditorButton : MonoBehaviour
{
    TimelineIndicator timelineIndicator;
    List<(float, float)> timelineNoteTuples;

    public enum TypeOfButton { Load, Save, ZoomIn, ZoomOut, ResetZoom};
    public TypeOfButton typeOfButton;

    public Sprite defaultSprite;
    public Sprite clickedSprite;
    Image buttonImage;

    string levelName;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        buttonImage.sprite = defaultSprite;

        timelineIndicator = FindObjectOfType<TimelineIndicator>();
        levelName = FindObjectOfType<LevelEditorManager>().levelName;
        transform.GetComponentInParent<UIComponents>().levelNameText.text = levelName;
    }

    private void OnMouseDown()
    {
        buttonImage.sprite = clickedSprite;

        float newZoom, zoomCenter;
        switch (typeOfButton)
        { 
            case TypeOfButton.Save:
                SaveRecordingToDat(levelName);
                SaveRecordingToTxt(levelName);
                break;

            case TypeOfButton.Load:
                // Clear timeline befor reloading
                timelineIndicator.DestroyAllNoteObjects();
                LoadRecording(levelName);
                break;
            case TypeOfButton.ResetZoom:
                timelineIndicator.ZoomTimeline(-1, Screen.width / 2, Screen.width);
                break;
            case TypeOfButton.ZoomIn:
                newZoom = (int)(timelineIndicator.CurrentZoom * 0.75);
                newZoom = Mathf.Clamp(newZoom, 0, Screen.width);
                zoomCenter = timelineIndicator.ArrowIndicator.anchoredPosition.x + timelineIndicator.TimelineCanvasRadius;
                timelineIndicator.ZoomTimeline(1, zoomCenter, newZoom);
                break;
            case TypeOfButton.ZoomOut:
                newZoom = (int)(timelineIndicator.CurrentZoom * 1.25);
                newZoom = Mathf.Clamp(newZoom, 0, Screen.width);
                zoomCenter = timelineIndicator.ArrowIndicator.anchoredPosition.x + timelineIndicator.TimelineCanvasRadius;

                timelineIndicator.ZoomTimeline(-1, zoomCenter, newZoom);
                break;
        }

    }
    private void OnMouseUp()
    {
        buttonImage.sprite = defaultSprite;
    }

    // Save list by serialization
    void SaveRecordingToDat(string filename)
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
        timelineNoteTuples = new List<(float, float)>();
        List<Transform> notesObjects = timelineIndicator.NotesObjects;
        if (notesObjects.Count == 0)
        {
            Debug.Log("List is empty!");
            return;
        }

        foreach (Transform noteObject in notesObjects)
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
    }

    void SaveRecordingToTxt(string filename)
    {
        string destination = Application.dataPath + "/levels/" + filename + ".txt";
        using (TextWriter tw = new StreamWriter(destination))
        {
            List<Transform> notesObjects = timelineIndicator.NotesObjects;
            foreach (Transform noteObject in notesObjects)
            {
                NoteTimelineIndicator noteTimelineIndicator = noteObject.GetComponent<NoteTimelineIndicator>();
                Debug.Log((noteTimelineIndicator.Timestamp, noteTimelineIndicator.TimeHeld));
                tw.WriteLine(noteTimelineIndicator.Timestamp.ToString() + " /  " + noteTimelineIndicator.TimeHeld.ToString());
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
        timelineNoteTuples = (List<(float, float)>)bf.Deserialize(file);

        foreach ((float, float) noteTuple in timelineNoteTuples)
        {
            timelineIndicator.addNoteObject(noteTuple.Item1, noteTuple.Item2);
        }

        file.Close();
    }
}
