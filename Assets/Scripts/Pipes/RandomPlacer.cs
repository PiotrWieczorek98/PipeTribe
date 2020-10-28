using System.Collections.Generic;
using UnityEngine;

public class RandomPlacer : MonoBehaviour
{
	public MusicNote[] itemPrefabs;

	// Minimum time between notes to allow change of segment the note is placed on
	public float minDeltaTimeToSwitch = 1f;

	public void GenerateItems(Pipe pipe, List<(float, float, float)> notesToGenerate)
	{
		//float angleStep = pipe.GetCurveAngle / pipe.GetCurveSegmentCount;
		float angleStep = pipe.GetCurveAngle / pipe.GetCurveSegmentCount;

		int pipeSegment = Random.Range(0, pipe.pipeSegmentCount);

		for (int i = 0; i < notesToGenerate.Count; i++)
		{
			MusicNote item = Instantiate<MusicNote>(itemPrefabs[Random.Range(0, itemPrefabs.Length)]);
			float curveSegment = notesToGenerate[i].Item1;

			if(i > 0 && notesToGenerate[i].Item3 - notesToGenerate[i - 1].Item3 > minDeltaTimeToSwitch)
            {
				Debug.Log(notesToGenerate[i].Item3 - notesToGenerate[i - 1].Item3 + " > " + minDeltaTimeToSwitch);
				pipeSegment = Random.Range(0, pipe.pipeSegmentCount);
			}
			float pipeRotation =(pipeSegment + 0.5f) * 360f / pipe.pipeSegmentCount;

			//item.Position(pipe, i * angleStep, pipeRotation, segment);
			item.Position(pipe, curveSegment, pipeRotation, pipeSegment);
		}
	}


}
