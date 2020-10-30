using System.Dynamic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	public PipeSystem pipeSystem;

	public float velocity;

	Pipe currentPipe;

	float deltaToRotation;
	float systemRotation;
	float worldRotation;

	Transform world;

	UIComponents uIComponents;
	RingManager ringManager;
	Text comboCounterUI;
	float comboCounter = 0;

    void Start () 
	{
		world = pipeSystem.transform.parent;
		uIComponents = GameObject.FindGameObjectWithTag("UI").GetComponent(typeof(UIComponents)) as UIComponents;
		ringManager = uIComponents.GetRingManager;
		comboCounterUI = uIComponents.GetCombo;
		comboCounterUI.text = "0x";

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

    private void OnTriggerEnter(Collider other)
    {
		MusicNote noteHit = other.GetComponentInParent(typeof(MusicNote)) as MusicNote;
		if (noteHit.GetColorOfNote.ringElement == ringManager.GetSelectedElement.ToString("g"))
        {
			comboCounter++;
		}
        else
        {
			comboCounter = 0;
		}
		comboCounterUI.text = comboCounter + "x";
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