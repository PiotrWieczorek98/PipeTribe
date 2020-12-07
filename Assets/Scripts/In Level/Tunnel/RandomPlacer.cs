using System.Collections.Generic;
using UnityEngine;

public class RandomPlacer : MonoBehaviour
{
	public MusicNote[] itemPrefabs;

	public void GenerateItems(Pipe pipe, List<(float, float, float)> notesToGenerate)
	{
		int pipeSegment = Random.Range(0, pipe.pipeSegmentCount);

		// Generate notes for this pipe
		for (int i = 0; i < notesToGenerate.Count; i++)
		{
			MusicNote note = Instantiate(itemPrefabs[Random.Range(0, itemPrefabs.Length)]);
			float curveSegment = notesToGenerate[i].Item1;
			float noteDuration = notesToGenerate[i].Item2;
			//if(noteDuration == 0)
			//         {
			//	note.noteType = MusicNote.NoteType.Tap;
			//         }
			//         else
			//         {
			//	note.noteType = MusicNote.NoteType.Hold;
			//	note.Prolong(noteDuration);
			//         }


			// Allow to change segment only if time between notes is big enough - if notes are too close it would break gameplay
			if (i > 0 && notesToGenerate[i].Item3 - notesToGenerate[i - 1].Item3 > FindObjectOfType<GameSettings>().Randomness)
			{
				pipeSegment = Random.Range(0, pipe.pipeSegmentCount);
			}
			// Rotation of music note in a pipe
			float pipeRotation = (pipeSegment + 0.5f) * 360f / pipe.pipeSegmentCount;
			// Restriction to limit rotation in <-180, 180)
			if (pipeRotation > 360)
			{
				int tmp = (int)(pipeRotation / 360f);
				tmp *= 360;
				pipeRotation -= tmp;
			}
			if (pipeRotation >= 180)
				pipeRotation -= 360;

			note.Position(pipe, curveSegment, pipeRotation);
		}
	}


}
