using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCollision : MonoBehaviour
{
	public AudioClip hitSound;
	public AudioClip missSound;
	public Text comboCounterUI;
	public Text scorePercentageUI;
	public RectTransform healthPointsBarUI;

	Animator animator;
	AudioSource audioSource;
	RingManager ringManager;
	GameSettings gameSettings;

	int comboCounter = 0;
	int noteCounter = 0;
	int notesHit = 0;
	int totalPossibleScore = 0;
	float healthPoints = 100;

	private void Awake()
	{
		gameSettings = FindObjectOfType<GameSettings>();
		ringManager = FindObjectOfType<RingManager>();
		animator = comboCounterUI.GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();

		comboCounterUI.text = "0x";
		scorePercentageUI.text = "100%";
	}


	// When note block hits player collider - where segments are placed
	private void OnTriggerEnter(Collider other)
	{
		MusicNote noteHit = other.GetComponentInParent<MusicNote>();
		noteCounter++;

		// Player was correct
		if (ringManager.SelectedRingElement != null && noteHit.ColorOfNote.ringElement == ringManager.SelectedRingElement.RingElement())
		{
			comboCounter++;
			notesHit++;
			if (comboCounter > MaxComboAchieved)
				MaxComboAchieved = comboCounter;

			animator.SetBool("Jump", true);
			StartCoroutine(setBoolParameterNextFrame("Jump", false));

			healthPoints += gameSettings.HealthRegainOnHit;
			if (healthPoints > 100)
				healthPoints = 100;

			audioSource.PlayOneShot(hitSound);
		}
		// Player missed
		else
		{
			comboCounter = 0;
			healthPoints -= gameSettings.HealthLossOnFail;
			if (healthPoints < 0)
				healthPoints = 0;

			audioSource.PlayOneShot(missSound);
		}

		if (healthPoints <= 0)
			FindObjectOfType<InLevelManager>().EndGameScreen(true);
		else if (healthPoints < 20)
			healthPointsBarUI.GetComponent<Image>().color = Color.red;
		else if (healthPoints < 40)
			healthPointsBarUI.GetComponent<Image>().color = Color.yellow;
		else
			healthPointsBarUI.GetComponent<Image>().color = Color.white;

		// Calculate scores
		int baseScore = 100;
		TotalPlayerScore += comboCounter * baseScore;
		totalPossibleScore += noteCounter * baseScore;
		ScorePercentage = (float)notesHit * 100 / (float)noteCounter;

		// Show on UI
		scorePercentageUI.text = ScorePercentage.ToString("F2") + "%"; // Two decimal points
		comboCounterUI.text = comboCounter + "x";
		healthPointsBarUI.localScale = new Vector3(1, healthPoints / 100, 1);

	}

	IEnumerator setBoolParameterNextFrame(string parameter, bool value)
	{
		yield return new WaitForEndOfFrame();
		animator.SetBool(parameter, value);
	}

	public int TotalPlayerScore { get; private set; } = 0;
	public int MaxComboAchieved { get; private set; } = 0;
	public float ScorePercentage { get; private set; } = 0;

}
