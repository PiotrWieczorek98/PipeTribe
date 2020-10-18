using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlacer : MonoBehaviour
{
	public MusicNote[] itemPrefabs;

	public void GenerateItems(Pipe pipe)
	{
		float angleStep = pipe.CurveAngle / pipe.CurveSegmentCount;
		for (int i = 0; i < pipe.CurveSegmentCount; i++)
		{
			MusicNote item = Instantiate<MusicNote>(itemPrefabs[Random.Range(0, itemPrefabs.Length)]);
			float pipeRotation =(Random.Range(0, pipe.pipeSegmentCount) + 0.5f) * 360f / pipe.pipeSegmentCount;
			item.Position(pipe, i * angleStep, pipeRotation);
		}
	}
}
