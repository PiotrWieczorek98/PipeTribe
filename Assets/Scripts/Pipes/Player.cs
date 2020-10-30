using System.Dynamic;
using UnityEngine;

public class Player : MonoBehaviour {

	public PipeSystem pipeSystem;

	public float velocity;

	private Pipe currentPipe;

	private float deltaToRotation;
	private float systemRotation;
	private float worldRotation;

	private Transform world;

	private void Start () 
	{
		world = pipeSystem.transform.parent;
		currentPipe = pipeSystem.SetupFirstPipe();
		SetupCurrentPipe();
	}

	private void Update () 
	{
		float delta = velocity * Time.deltaTime;
		systemRotation += delta * deltaToRotation;

		if (systemRotation >= currentPipe.GetCurveAngle) 
		{
			delta = (systemRotation - currentPipe.GetCurveAngle) / deltaToRotation;
			currentPipe = pipeSystem.SetupNextPipe();
			SetupCurrentPipe();
			systemRotation = delta * deltaToRotation;
		}

		pipeSystem.transform.localRotation = Quaternion.Euler(0f, 0f, systemRotation);
	}

	private void SetupCurrentPipe () 
	{
		deltaToRotation = 360f / (2f * Mathf.PI * currentPipe.GetCurveRadius);
		worldRotation += currentPipe.GetRelativeRotation;

		if (worldRotation < 0f) 
			worldRotation += 360f;
		else if (worldRotation >= 360f) 
			worldRotation -= 360f;

		world.localRotation = Quaternion.Euler(worldRotation, 0f, 0f);
	}

	public float GetPlayerVelocity { get { return velocity; } }
}