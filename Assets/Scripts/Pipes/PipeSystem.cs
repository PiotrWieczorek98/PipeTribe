using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class PipeSystem : MonoBehaviour 
{

	public Pipe pipePrefab;
	public int pipesDisplayedAtTheSameTime;

	Pipe[] pipes;

	List<(float, float)> loadedTimeline;
	GameSettings gameSettings;
	float timeWhenPipeEntered = 0;

	private void Awake () 
	{
		gameSettings = GameObject.FindGameObjectWithTag("GameManager").GetComponent(typeof(GameSettings)) as GameSettings;
		LoadRecording(gameSettings.levelName);

		pipes = new Pipe[pipesDisplayedAtTheSameTime];
		for (int i = 0; i < pipes.Length; i++) 
		{
			Pipe pipe = pipes[i] = Instantiate<Pipe>(pipePrefab);
			pipe.transform.SetParent(transform, false);

			// Since we start the game at the second pipe, do not generate notes for the first one
			if (i==0)
            {
				pipe.Generate(loadedTimeline, timeWhenPipeEntered, true);
			}
			else 
			{
				timeWhenPipeEntered += pipe.Generate(loadedTimeline, timeWhenPipeEntered);
				pipe.AlignWith(pipes[i - 1]);
			}
		}
		AlignNextPipeWithOrigin();
	}

	public Pipe SetupFirstPipe () 
	{
		transform.localPosition = new Vector3(0f, -pipes[1].GetCurveRadius);
		return pipes[1];
	}

	public Pipe SetupNextPipe () 
	{
		ShiftPipes();
		AlignNextPipeWithOrigin();

		// Regenerate the last pipe to avoid looping pipes
		timeWhenPipeEntered += pipes[pipes.Length - 1].Generate(loadedTimeline, timeWhenPipeEntered);
		// Align newly generated pipe
		pipes[pipes.Length - 1].AlignWith(pipes[pipes.Length - 2]);
		transform.localPosition = new Vector3(0f, -pipes[1].GetCurveRadius);
		return pipes[1];
	}

	private void ShiftPipes () 
	{
		// Destroy notes from pipe that disappears
		pipes[0].destroyChildren();
		// Shift pipes
		Pipe temp = pipes[0];
		for (int i = 1; i < pipes.Length; i++) 
		{
			pipes[i - 1] = pipes[i];
		}
		pipes[pipes.Length - 1] = temp;
	}

	private void AlignNextPipeWithOrigin () 
	{
		Transform transformToAlign = pipes[1].transform;
		for (int i = 0; i < pipes.Length; i++) 
		{
			if (i != 1) 
			{
				pipes[i].transform.SetParent(transformToAlign);
			}
		}
		
		transformToAlign.localPosition = Vector3.zero;
		transformToAlign.localRotation = Quaternion.identity;
		
		for (int i = 0; i < pipes.Length; i++) {
			if (i != 1) {
				pipes[i].transform.SetParent(transform);
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
		loadedTimeline = (List<(float, float)>)bf.Deserialize(file);

		file.Close();

		using (TextWriter tw = new StreamWriter(destination + ".txt"))
		{
			foreach (var note in loadedTimeline)
				tw.WriteLine(note);
		}
	}
}