using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class LevelDataPasser : MonoBehaviour
{
	// Save list by serialization
	public List<(float, float)> SaveRecordingToDat(string fileLoc, List<Transform> notesObjects)
	{
		fileLoc += ".dat";
		FileStream file;

		Directory.CreateDirectory(Application.dataPath + "/levels/");

		if (File.Exists(fileLoc))
			file = File.OpenWrite(fileLoc);
		else
			file = File.Create(fileLoc);

		BinaryFormatter bf = new BinaryFormatter();

		// Transform list from list of objects to list of tuples with only time attributes
		List<(float, float)> timelineNoteTuples = new List<(float, float)>();
		if (notesObjects.Count == 0)
		{
			Debug.Log("List is empty!");
			return null;
		}

		// Add info about offset and bpm first
		LevelCreatorManager levelCreatorManager = FindObjectOfType<LevelCreatorManager>();
		timelineNoteTuples.Add((levelCreatorManager.BPMValue, levelCreatorManager.OffsetValue));

		// Pass notes data
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

		return timelineNoteTuples;
	}

	public void SaveRecordingToTxt(string fileLoc, List<Transform> notesObjects)
	{
		fileLoc += ".txt";
		using (TextWriter tw = new StreamWriter(fileLoc))
		{
			// Add info about offset and bpm first
			LevelCreatorManager levelCreatorManager = FindObjectOfType<LevelCreatorManager>();
			tw.WriteLine(levelCreatorManager.BPMValue.ToString() + " /  " + levelCreatorManager.OffsetValue.ToString());

			foreach (Transform noteObject in notesObjects)
			{
				NoteTimelineIndicator noteTimelineIndicator = noteObject.GetComponent<NoteTimelineIndicator>();
				tw.WriteLine(noteTimelineIndicator.Timestamp.ToString() + " /  " + noteTimelineIndicator.TimeHeld.ToString());

				Debug.Log((noteTimelineIndicator.Timestamp, noteTimelineIndicator.TimeHeld));
			}
		}
	}

	public List<(float, float)> LoadLevelDataFromDat(string fileLoc, bool addOffset = false)
	{
		fileLoc += ".dat";
		FileStream file;

		if (File.Exists(fileLoc))
		{
			file = File.OpenRead(fileLoc);
		}
		else
		{
			Debug.Log(fileLoc + " - File not found");
			return null;
		}

		BinaryFormatter bf = new BinaryFormatter();
		List<(float, float)> tupleList;
		try
		{
			tupleList = (List<(float, float)>)bf.Deserialize(file);
		}
		catch
		{
			tupleList = null;
		}

		if (addOffset)
		{
			// Add offset to each note - user preferences
			float offset = FindObjectOfType<GameSettings>().NotesOffset;
			List<(float, float)> tupleListOffset = new List<(float, float)>();
			foreach (var note in tupleList)
			{
				tupleListOffset.Add((note.Item1 + offset, note.Item2));
			}
			// Set reference
			tupleList.Clear();
			tupleList = tupleListOffset;
		}

		return tupleList;
	}

	public List<(float, float)> LoadLevelDataFromTxt(string fileLoc)
	{
		fileLoc += ".txt";
		List<(float, float)> tupleList = new List<(float, float)>();

		using (TextReader tr = new StreamReader(fileLoc))
		{
			string line;
			while ((line = tr.ReadLine()) != null)
			{
				line = line.Trim();
				string[] words = line.Split('/');
				tupleList.Add((float.Parse(words[0]), float.Parse(words[1])));
			}
		}

		return tupleList;
	}

	public void SaveScore(string fileLoc, float score, float combo)
	{
		fileLoc += ".dat";
		FileStream file;

		Directory.CreateDirectory(Application.dataPath + "/levels/");

		if (File.Exists(fileLoc))
			file = File.OpenWrite(fileLoc);
		else
			file = File.Create(fileLoc);

		BinaryFormatter bf = new BinaryFormatter();
		List<(float, float)> scoreList;
		try
		{
			scoreList = (List<(float, float)>)bf.Deserialize(file);
		}
		catch
		{
			scoreList = new List<(float, float)>();
		}
		scoreList.Add((score, combo));

		bf.Serialize(file, scoreList);
		file.Close();
	}
}
