﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class MusicNote : MonoBehaviour
{
	Transform rotater;
	MeshRenderer cube;
	Dictionary<int, Color> cubeColor;
	PipeSystem pipeSystem;

	private void Awake()
	{
		rotater = transform.GetChild(0);
		cube = rotater.GetChild(0).GetComponent(typeof(MeshRenderer)) as MeshRenderer;
		cubeColor = new Dictionary<int, Color>
		{
			{30, new Color(1.0f, 1f, 0f) },		// yellow
			{90, new Color(0.15f, 0f, 1f) },	// purple
			{150, new Color(1f, 0.4f, 0f) },	// orange
			{-150, new Color(0f, 1f, 0f) },		// green
			{-90, new Color(1f, 0f, 0f) },		// red
			{-30, new Color(0f, 0.25f, 1f) },	// blue
		};
	}

	public void Position(Pipe pipe, float curveRotation, float pipeRotation)
	{
		cube.material.color = cubeColor[(int)pipeRotation];
		cube.material.SetColor("_EmissionColor", cubeColor[(int)pipeRotation]);

		transform.SetParent(pipe.transform, false);
		transform.localRotation = Quaternion.Euler(0f, 0f, -curveRotation);

		// Pipe system is an only child of root which is the "world" game object
		pipeSystem = transform.root.GetChild(0).GetComponent(typeof(PipeSystem)) as PipeSystem;
		// Futher rotation based on absolute world rotation to avoid color shift when entering next pipe
		pipeRotation -= pipeSystem.GetWorldAbsoluteRotation;

		rotater.localPosition = new Vector3(0f, pipe.GetCurveRadius);
		rotater.localRotation = Quaternion.Euler(pipeRotation, 0f, 0f);

		//Debug.Log(transform.root.rotation.x);
	}

}
