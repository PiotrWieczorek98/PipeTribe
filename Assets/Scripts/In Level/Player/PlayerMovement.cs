﻿using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	public PipeSystem pipeSystem;
	public AnimationClip comboAnimation;
	public float Velocity { get; private set; }

	Pipe currentPipe;

	float deltaToRotation;
	float systemRotation;
	float worldRotation;

	Transform world;

	public void Awake()
	{
		world = pipeSystem.transform.parent;
	}

	private void Start()
	{
		Velocity = FindObjectOfType<GameSettings>().GetDifficultyValue(GameSettings.DifficultyType.Speed);
		currentPipe = pipeSystem.SetupPipe(true);
		SetupCurrentPipe();
	}

	private void Update()
	{
		float delta = Velocity * Time.deltaTime;
		systemRotation += delta * deltaToRotation;

		if (systemRotation >= currentPipe.CurveAngle)
		{
			delta = (systemRotation - currentPipe.CurveAngle) / deltaToRotation;
			currentPipe = pipeSystem.SetupPipe(false);
			SetupCurrentPipe();
			systemRotation = delta * deltaToRotation;
		}

		pipeSystem.transform.localRotation = Quaternion.Euler(0f, 0f, systemRotation);
	}

	private void SetupCurrentPipe()
	{
		deltaToRotation = 360f / (2f * Mathf.PI * currentPipe.CurveRadius);
		worldRotation += currentPipe.RelativeRotation;

		if (worldRotation < 0f)
			worldRotation += 360f;
		else if (worldRotation >= 360f)
			worldRotation -= 360f;

		world.localRotation = Quaternion.Euler(worldRotation, 0f, 0f);
	}
}