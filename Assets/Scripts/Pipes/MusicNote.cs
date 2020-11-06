using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class MusicNote : MonoBehaviour
{
	Transform rotater, scaler;
	//Component[] cubes;
	MeshRenderer mesh;
	Dictionary<int, ColorElement> cubeColors;
	PipeSystem pipeSystem;
	public enum NoteType { Tap, Hold };
	//public NoteType noteType { get; set; }

	public struct ColorElement
    {
		public Color colorValue;
		public string ringElement;

		public ColorElement(Color colorValue, string ringElement)
		{
			this.colorValue = colorValue;
			this.ringElement = ringElement;
		}
	}
	ColorElement colorOfNote;

	private void Awake()
	{
		rotater = transform.GetChild(0);
		//scaler = rotater.GetChild(1);
		//cubes = rotater.GetComponentsInChildren(typeof(MeshRenderer));
		mesh = rotater.GetComponentInChildren(typeof(MeshRenderer)) as MeshRenderer;
		cubeColors = new Dictionary<int, ColorElement>
		{
			{30,new ColorElement(new Color(1.0f, 1f, 0f), "BottomRight") },		// yellow
			{90,new ColorElement(new Color(0.47f, 0f, 1f), "Right") },			// purple
			{150,new ColorElement(new Color(1f, 0.4f, 0f), "TopRight") },		// orange
			{-150,new ColorElement(new Color(0f, 1f, 0f), "TopLeft") },			// green
			{-90,new ColorElement(new Color(1f, 0f, 0f), "Left") },				// red
			{-30,new ColorElement(new Color(0f, 0.25f, 1f), "BottomLeft") },	// blue
		};
	}

	public void Position(Pipe pipe, float curveRotation, float pipeRotation)
	{
		colorOfNote = cubeColors[(int)pipeRotation];
		//foreach (MeshRenderer mesh in cubes)
       // {
			mesh.material.color = colorOfNote.colorValue;
			mesh.material.SetColor("_EmissionColor", colorOfNote.colorValue);
		//}


		transform.SetParent(pipe.transform, false);
		transform.localRotation = Quaternion.Euler(0f, 0f, -curveRotation);

		// Pipe system is an only child of root which is the "world" game object
		pipeSystem = transform.root.GetChild(0).GetComponent(typeof(PipeSystem)) as PipeSystem;
		// Futher rotation based on absolute world rotation to avoid color shift when entering next pipe
		pipeRotation -= pipeSystem.GetWorldAbsoluteRotation;

		rotater.localPosition = new Vector3(0f, pipe.CurveRadius);
		rotater.localRotation = Quaternion.Euler(pipeRotation, 0f, 0f);
	}

	public ColorElement ColorOfNote { get { return colorOfNote; } }
}
