using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PipeSystem : MonoBehaviour
{

	public Pipe pipePrefab;
	public int pipesDisplayedAtTheSameTime;
	public float WorldAbsoluteRotation { get; private set; } = 0;

	Pipe[] pipes;

	List<(float, float)> musicNotesTimeline;
	float timeWhenPipeEntered = 0;

	public void Awake()
	{
		string destination = CrossSceneData.LevelDir + "/" + CrossSceneData.SelectedLevelName + "/" + CrossSceneData.SelectedLevelName;
		musicNotesTimeline = FindObjectOfType<LevelDataPasser>().LoadLevelDataFromDat(destination, true);
		// First tuple contains bpm  and offset
		musicNotesTimeline.RemoveAt(0);

		pipes = new Pipe[pipesDisplayedAtTheSameTime];
	}

	private void Start()
	{
		for (int i = 0; i < pipes.Length; i++)
		{
			Pipe pipe = pipes[i] = Instantiate<Pipe>(pipePrefab);
			pipe.transform.SetParent(transform, false);

			// Since we start the game at the second pipe, do not generate notes for the first one
			if (i == 0)
			{
				pipe.GeneratePipe();
				pipe.GenerateMusicNotes(musicNotesTimeline, timeWhenPipeEntered, true);
			}
			else
			{
				pipe.GeneratePipe();
				WorldAbsoluteRotation = pipe.AlignWith(pipes[i - 1], WorldAbsoluteRotation);
				timeWhenPipeEntered = pipe.GenerateMusicNotes(musicNotesTimeline, timeWhenPipeEntered);
			}
		}
		AlignNextPipeWithOrigin();
	}

	public Pipe SetupPipe(bool isFirstPipe)
	{
		if (isFirstPipe)
			transform.localPosition = new Vector3(0f, -pipes[1].CurveRadius);
		else
		{
			ShiftPipes();
			AlignNextPipeWithOrigin();

			// Regenerate the last pipe to avoid looping pipes
			pipes[pipes.Length - 1].GeneratePipe();
			// Align newly generated pipe
			WorldAbsoluteRotation = pipes[pipes.Length - 1].AlignWith(pipes[pipes.Length - 2], WorldAbsoluteRotation);
			transform.localPosition = new Vector3(0f, -pipes[1].CurveRadius);
			// Generate music notes for new pipe
			timeWhenPipeEntered = pipes[pipes.Length - 1].GenerateMusicNotes(musicNotesTimeline, timeWhenPipeEntered);
		}

		return pipes[1];
	}

	private void ShiftPipes()
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

	private void AlignNextPipeWithOrigin()
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


		for (int i = 0; i < pipes.Length; i++)
		{
			if (i != 1)
			{
				pipes[i].transform.SetParent(transform);
			}
		}
	}

}