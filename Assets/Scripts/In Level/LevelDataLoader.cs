using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LevelDataLoader : MonoBehaviour
{
	List<(float, float)> musicNotesTimeline;

	// Loads data about note objects - amount and time they appear
    public List<(float, float)> LoadRecording()
	{
		musicNotesTimeline = new List<(float, float)>();
		GameManager gameManager = FindObjectOfType<GameManager>();
		string filename = gameManager.LevelName;
		string destination = Application.dataPath + "/levels/" + filename + ".dat";

		// Open file
		FileStream file;
		if (File.Exists(destination)) 
			file = File.OpenRead(destination);
		else
		{
			Debug.LogError(destination + " - File not found");
			return null;
		}

		// Deserialize data
		BinaryFormatter bf = new BinaryFormatter();
		List<(float, float)> originalTimeline = (List<(float, float)>)bf.Deserialize(file);
		file.Close();


		// Add offset to each note - user preferences
		foreach (var note in originalTimeline)
			musicNotesTimeline.Add((note.Item1 + gameManager.MusicNotesOffset, note.Item2));

		return musicNotesTimeline;
	}
}
