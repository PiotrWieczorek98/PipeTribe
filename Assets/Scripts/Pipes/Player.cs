using System.Collections;
using System.Dynamic;
using UnityEditorInternal;
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
	GameManager gameSettings;
	Text comboCounterUI;
	float comboCounter = 0;

    public void Awake() 
	{
		world = pipeSystem.transform.parent;
		currentPipe = pipeSystem.SetupPipe(true);
		SetupCurrentPipe();

	}
    private void Start()
    {
		gameSettings = GameObject.FindGameObjectWithTag("GameManager").GetComponent(typeof(GameManager)) as GameManager;
		uIComponents = GameObject.FindGameObjectWithTag("UI").GetComponent(typeof(UIComponents)) as UIComponents;
		ringManager = uIComponents.ringManager;
		comboCounterUI = uIComponents.combo;
		comboCounterUI.text = "0x";
	}

	private void Update () 
	{
		float delta = velocity * Time.deltaTime;
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

    private void OnTriggerEnter(Collider other)
    {
		MusicNote noteHit = other.GetComponentInParent(typeof(MusicNote)) as MusicNote;
		if (noteHit.ColorOfNote.ringElement == ringManager.SelectedRingElement.ToString("g"))
        {
			comboCounter++;
		}
        else
        {
			comboCounter = 0;
		}
		comboCounterUI.text = comboCounter + "x";
	}

    //private void OnTriggerStay(Collider other)
    //{
        
    //}

    private void SetupCurrentPipe () 
	{
		deltaToRotation = 360f / (2f * Mathf.PI * currentPipe.CurveRadius);
		worldRotation += currentPipe.RelativeRotation;

		if (worldRotation < 0f) 
			worldRotation += 360f;
		else if (worldRotation >= 360f) 
			worldRotation -= 360f;

		world.localRotation = Quaternion.Euler(worldRotation, 0f, 0f);
	}

	public float GetPlayerVelocity { get { return velocity; } }
}