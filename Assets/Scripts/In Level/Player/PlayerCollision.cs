using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCollision : MonoBehaviour
{
	UIComponents uIComponents;
	RingManager ringManager;
	InLevelManager gameManager;

	Text comboCounterUI;
	Animator animator;
	Text scorePercentageUI;
	RectTransform healthPointsBarUI;

	int comboCounter = 0;
    int noteCounter = 0;
	int totalPossibleScore = 0;
	float healthPoints = 100;

	// Start is called before the first frame update
	void Start()
    {
		gameManager = FindObjectOfType<InLevelManager>();
		uIComponents = FindObjectOfType<UIComponents>();

		healthPointsBarUI = uIComponents.HealthPointsBar;
		scorePercentageUI = uIComponents.ScorePercentage;
		ringManager = uIComponents.RingManager;
		comboCounterUI = uIComponents.ComboCounter;
		animator = comboCounterUI.GetComponent<Animator>();

		comboCounterUI.text = "0x";
		scorePercentageUI.text = "100%";
	}

	// When note block hits player collider - where segments are placed
	private void OnTriggerEnter(Collider other)
	{
		MusicNote noteHit = other.GetComponentInParent<MusicNote>();
		noteCounter++;

		if (noteHit.ColorOfNote.ringElement == ringManager.SelectedRingElement.RingElement())
		{
			comboCounter++;
			if (comboCounter > MaxComboAchieved)
				MaxComboAchieved = comboCounter;

			animator.SetBool("Jump", true);
			StartCoroutine(setBoolParameterNextFrame("Jump", false));

			if (healthPoints < 100)
				healthPoints += gameManager.healthRegainOnHit;
		}
		else
		{
			comboCounter = 0;
			healthPoints -= gameManager.healthLossOnFail;
		}

		healthPoints = Mathf.Clamp(healthPoints, 0, 100);
		if (healthPoints <= 0)
			gameManager.EndGameScreen(true);
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
		ScorePercentage = (float)TotalPlayerScore * 100 / (float)totalPossibleScore;

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
