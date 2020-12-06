using System.Collections.Generic;
using UnityEngine;

public class MusicNote : MonoBehaviour
{
	Transform rotater;
	MeshRenderer mesh;
	Dictionary<int, CubeValues> cubeColors;
	PipeSystem pipeSystem;
	public enum NoteType { Tap, Hold };

	public struct CubeValues
	{
		public Color colorValue;
		public Color emissionValue;
		public string ringElement;

		public CubeValues(Color colorValue, Color emissionValues, string ringElement)
		{
			this.colorValue = colorValue;
			this.emissionValue = emissionValues;
			this.ringElement = ringElement;
		}
	}
	CubeValues noteColorValues;

	private void Awake()
	{
		rotater = transform.GetChild(0);
		mesh = rotater.GetComponentInChildren(typeof(MeshRenderer)) as MeshRenderer;
		cubeColors = new Dictionary<int, CubeValues>
		{
			//{30,new CubeValues(new Color(.98f, 1f, 0f), new Color(.5f, .5f, 0f), "BottomRight") },			// yellow
			//{90,new CubeValues(new Color(.45f, .25f, .55f), new Color(.18f, .05f, .25f), "Right") },		// purple
			//{150,new CubeValues(new Color(.87f, 0.44f, .15f), new Color(.73f, 0.16f, .02f), "TopRight") },	// orange
			//{-150,new CubeValues(new Color(.41f, .74f, .18f), new Color(.14f, .5f, .027f), "TopLeft") },	// green
			//{-90,new CubeValues(new Color(.76f, .14f, .14f), new Color(.53f, .01f, .01f), "Left") },		// red
			//{-30,new CubeValues(new Color(.38f, 0.60f, 1f), new Color(.12f, 0.32f, 1f), "BottomLeft") },	// blue
			{30,new CubeValues(new Color(1.0f, 1f, 0f), new Color(.55f, .55f, 0f), "BottomRight") },	// yellow
			{90,new CubeValues(new Color(0.47f, 0f, 1f), new Color(0.47f, 0f, 1f), "Right") },			// purple
			{150,new CubeValues(new Color(1f, 0.4f, 0f), new Color(1f, 0.4f, 0f), "TopRight") },		// orange
			{-150,new CubeValues(new Color(0f, 1f, 0f), new Color(0f, .8f, 0f), "TopLeft") },			// green
			{-90,new CubeValues(new Color(1f, 0f, 0f), new Color(.44f, 0f, 0f), "Left") },				// red
			{-30,new CubeValues(new Color(0f, 0.25f, 1f), new Color(0f, 0.25f, 1f), "BottomLeft") },	// blue
		};
	}

	public void Position(Pipe pipe, float curveRotation, float pipeRotation)
	{
		noteColorValues = cubeColors[(int)pipeRotation];

		mesh.material.color = noteColorValues.colorValue;
		mesh.material.SetColor("_EmissionColor", noteColorValues.emissionValue);


		transform.SetParent(pipe.transform, false);
		transform.localRotation = Quaternion.Euler(0f, 0f, -curveRotation);

		// Pipe system is an only child of root which is the "world" game object
		pipeSystem = transform.root.GetChild(0).GetComponent(typeof(PipeSystem)) as PipeSystem;
		// Futher rotation based on absolute world rotation to avoid color shift when entering next pipe
		pipeRotation -= pipeSystem.WorldAbsoluteRotation;

		rotater.localPosition = new Vector3(0f, pipe.CurveRadius);
		rotater.localRotation = Quaternion.Euler(pipeRotation, 0f, 0f);
	}

	public CubeValues ColorOfNote { get { return noteColorValues; } }
}
