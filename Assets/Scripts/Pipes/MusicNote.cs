using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class MusicNote : MonoBehaviour
{
	Transform rotater;
	MeshRenderer cube;
	Dictionary<int, Color> cubeColor;

	private void Awake()
	{
		rotater = transform.GetChild(0);
		cube = rotater.GetChild(0).GetComponent(typeof(MeshRenderer)) as MeshRenderer;
		cubeColor = new Dictionary<int, Color>
		{
			{0, new Color(251/255f,135/255f,57/255f) },
			{1, new Color(121/255f,55/255f,172/255f) },
			{2, new Color(255/255f,255/255f,6/255f) },
			{3, new Color(251/255f,135/255f,57/255f) },
			{4, new Color(76/255f,123/255f,209/255f) },
			{5, new Color(255/255f,255/255f,21/255f) },
			{6, new Color(122/255f,184/255f,80/255f) },
		};
	}

	public void Position(Pipe pipe, float curveRotation, float ringRotation, int pipeSegment)
	{
		transform.SetParent(pipe.transform, false);
		transform.localRotation = Quaternion.Euler(0f, 0f, -curveRotation);

		rotater.localPosition = new Vector3(0f, pipe.GetCurveRadius);
		rotater.localRotation = Quaternion.Euler(ringRotation, 0f, 0f);

		cube.material.color = cubeColor[pipeSegment];
	}

}
