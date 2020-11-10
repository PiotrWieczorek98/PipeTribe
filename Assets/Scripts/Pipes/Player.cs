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
	Text scorePercentageUI;
	RectTransform healthPointsBarUI;
	float comboCounter = 0;
	float scorePercentage = 0;
	int noteHits = 0, noteCounter;
	float healthPoints = 100;

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

		healthPointsBarUI = uIComponents.HealthPointsBar;
		scorePercentageUI = uIComponents.ScorePercentage;
		ringManager = uIComponents.RingManager;
		comboCounterUI = uIComponents.ComboCounter;
		
		comboCounterUI.text = "0x";
		scorePercentageUI.text = "100%";
		
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
		noteCounter++;

		if (noteHit.ColorOfNote.ringElement == ringManager.SelectedRingElement.ToString("g"))
        {
			noteHits++;
			comboCounter++;
			if(healthPoints < 100)
            {
				healthPoints += gameSettings.healthRegainOnHit;
            }
		}
        else
        {
			comboCounter = 0;
			healthPoints -= gameSettings.healthLossOnFail;
		}

		healthPoints = Mathf.Clamp(healthPoints, 0, 100);
		// TODO LOSE SCREEN
		if (healthPoints <= 0)
			Debug.Log("Game Lost");
		else if (healthPoints < 20)
			healthPointsBarUI.GetComponent<Image>().color = Color.red;
		else if (healthPoints < 40)
			healthPointsBarUI.GetComponent<Image>().color = Color.yellow;
		else
			healthPointsBarUI.GetComponent<Image>().color = Color.white;

		scorePercentage = (float)noteHits * 100 / (float)noteCounter;
		scorePercentageUI.text = scorePercentage.ToString("F2") + "%"; // Two decimal points
		comboCounterUI.text = comboCounter + "x";
		healthPointsBarUI.localScale = new Vector3(1, healthPoints/100, 1);

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