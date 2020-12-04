using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class LevelDataPasser : MonoBehaviour
{
	// Save list by serialization
	public List<(float, float)> SaveRecordingToDat(string filename, List<Transform> notesObjects)
	{
		string destination = FindObjectOfType<GameSettings>().LevelDir + "/" + filename + "/" + filename + ".dat";
		FileStream file;

		Directory.CreateDirectory(Application.dataPath + "/levels/");

		if (File.Exists(destination))
			file = File.OpenWrite(destination);
		else
			file = File.Create(destination);

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

	public void SaveRecordingToTxt(string filename, List<Transform> notesObjects)
	{
		string destination = FindObjectOfType<GameSettings>().LevelDir + "/" + filename + "/" + filename + ".txt";
		using (TextWriter tw = new StreamWriter(destination))
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
	public List<(float, float)> LoadRecordingFromDat(string filename)
	{
		string destination = FindObjectOfType<GameSettings>().LevelDir + "/" + filename + "/" + filename + ".dat";
		FileStream file;

		if (File.Exists(destination)) file = File.OpenRead(destination);
		else
		{
			Debug.Log(destination + " - File not found");
			return null;
		}

		BinaryFormatter bf = new BinaryFormatter();
		List<(float, float)> timelineNoteTuples = (List<(float, float)>)bf.Deserialize(file);

		return timelineNoteTuples;
	}

	public List<(float, float)> LoadRecordingFromTxt(string filename)
	{
		string destination = FindObjectOfType<GameSettings>().LevelDir + "/" + filename + "/" + filename + ".txt";
		List<(float, float)> timelineNoteTuples = new List<(float, float)>();

		using (TextReader tr = new StreamReader(destination))
		{
			string line;
			while ((line = tr.ReadLine()) != null)
			{
				line = line.Trim();
				string[] words = line.Split('/');
				timelineNoteTuples.Add((float.Parse(words[0]), float.Parse(words[1])));
			}
		}

		return timelineNoteTuples;
	}
}
