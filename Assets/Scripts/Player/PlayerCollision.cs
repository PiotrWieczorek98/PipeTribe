using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCollision : MonoBehaviour
{
	UIComponents uIComponents;
	RingManager ringManager;
	GameManager gameManager;

	Text comboCounterUI;
	Animator animator;
	Text scorePercentageUI;
	RectTransform healthPointsBarUI;
	float comboCounter = 0;
	float scorePercentage = 0;
	int noteHits = 0, noteCounter;
	float healthPoints = 100;

	// Start is called before the first frame update
	void Start()
    {
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent(typeof(GameManager)) as GameManager;
		uIComponents = GameObject.FindGameObjectWithTag("UI").GetComponent(typeof(UIComponents)) as UIComponents;

		healthPointsBarUI = uIComponents.HealthPointsBar;
		scorePercentageUI = uIComponents.ScorePercentage;
		ringManager = uIComponents.RingManager;
		comboCounterUI = uIComponents.ComboCounter;
		animator = comboCounterUI.GetComponent<Animator>();

		comboCounterUI.text = "0x";
		scorePercentageUI.text = "100%";
	}

	private void OnTriggerEnter(Collider other)
	{
		MusicNote noteHit = other.GetComponentInParent(typeof(MusicNote)) as MusicNote;
		noteCounter++;

		if (noteHit.ColorOfNote.ringElement == ringManager.SelectedRingElement.ToString("g"))
		{
			noteHits++;
			comboCounter++;

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
		// TODO LOSE SCREEN
		if (healthPoints <= 0)
			gameManager.EndGameScreen(true);
		else if (healthPoints < 20)
			healthPointsBarUI.GetComponent<Image>().color = Color.red;
		else if (healthPoints < 40)
			healthPointsBarUI.GetComponent<Image>().color = Color.yellow;
		else
			healthPointsBarUI.GetComponent<Image>().color = Color.white;

		scorePercentage = (float)noteHits * 100 / (float)noteCounter;
		scorePercentageUI.text = scorePercentage.ToString("F2") + "%"; // Two decimal points
		comboCounterUI.text = comboCounter + "x";
		healthPointsBarUI.localScale = new Vector3(1, healthPoints / 100, 1);

	}

	IEnumerator setBoolParameterNextFrame(string parameter, bool value)
    {
        yield return new WaitForEndOfFrame();
        animator.SetBool(parameter, value);
    }
}
