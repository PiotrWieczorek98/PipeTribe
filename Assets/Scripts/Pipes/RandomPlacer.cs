using System.Collections.Generic;
using UnityEngine;

public class RandomPlacer : MonoBehaviour
{
	public MusicNote[] itemPrefabs;

	// Minimum time between notes to allow change of segment the note is placed on
	public float minDeltaTimeToSwitch = 1.0f;

	public void GenerateItems(Pipe pipe, List<(float, float, float)> notesToGenerate)
	{
		int pipeSegment = Random.Range(0, pipe.pipeSegmentCount);

		// Generate notes for this pipe
		for (int i = 0; i < notesToGenerate.Count; i++)
		{
			MusicNote item = Instantiate(itemPrefabs[Random.Range(0, itemPrefabs.Length)]);
			float curveSegment = notesToGenerate[i].Item1;

			// Allow to change segment only if time between notes is big enough - if notes are too close it would break gameplay 
			if(i > 0 && notesToGenerate[i].Item3 - notesToGenerate[i - 1].Item3 > minDeltaTimeToSwitch)
            {
				pipeSegment = Random.Range(0, pipe.pipeSegmentCount);
			}

			float pipeRotation =(pipeSegment + 0.5f) * 360f / pipe.pipeSegmentCount;
			// Restriction to limit rotation in <-180, 180)
			if (pipeRotation > 360)
			{
				int tmp = (int)(pipeRotation / 360f);
				tmp *= 360;
				pipeRotation -= tmp;
			}
			if (pipeRotation >= 180)
				pipeRotation -= 360;

			item.Position(pipe, curveSegment, pipeRotation);
		}
	}


}
