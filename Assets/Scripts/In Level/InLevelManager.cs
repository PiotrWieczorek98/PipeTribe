using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InLevelManager : MonoBehaviour
{
	public AudioClip levelComplete;
	public AudioClip levelFailed;

	public Text currentTimeUI;
	public Text totalTimeUI;
	public float loadingTime;
	public Transform endGamePanel;
	public Transform effectsPanel;
	public AnimationClip countdown;
	public RectTransform currentTimeBar;

	float timeSinceLoaded = 0;
	AudioSource audioSource;


	void Awake()
	{
		audioSource = GetComponent<AudioSource>();

		string uri = "file://" + CrossSceneData.LevelDir + "/" + CrossSceneData.SelectedLevelName + "/" + CrossSceneData.SelectedLevelName + ".ogg";
		StartCoroutine(GetComponent<MusicLoader>().PlayMusic(audioSource, uri, false));
	}

	void Start()
	{
		StartCoroutine(FinishLoading());
	}


	// Update is called once per frame
	void Update()
	{
		float timePassedPercent = timeSinceLoaded / audioSource.clip.length;
		currentTimeBar.localScale = new Vector3(timePassedPercent, 1, 1);

		timeSinceLoaded += Time.deltaTime;
		currentTimeUI.text = timeSinceLoaded.ToString("F0");

		if (timeSinceLoaded > audioSource.clip.length)
			EndGameScreen();
	}

	// This method is required to avoid desync and lag at the beggining of play when entities still load;
	public IEnumerator FinishLoading()
	{
		PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
		// Disable player to stop movement of the pipe system
		playerMovement.enabled = false;

		// Wait to sync
		Text text = effectsPanel.GetComponentInChildren<Text>();
		effectsPanel.GetComponent<Animator>().Play(countdown.name);
		Vector3 scale = text.transform.localScale;
		text.transform.localScale = scale * 3;
		text.text = "3";
		yield return new WaitForSeconds(1);
		text.text = "2";
		yield return new WaitForSeconds(1);
		text.text = "1";
		yield return new WaitForSeconds(1);
		text.text = "GO!";
		yield return new WaitForSeconds(1);

		// Set everything ready to start
		text.transform.localScale = scale;
		text.text = "Loading...";
		playerMovement.enabled = true;
		audioSource.PlayOneShot(audioSource.clip);
		timeSinceLoaded = 0;
		totalTimeUI.text = audioSource.clip.length.ToString("F0") + " sec";
	}

	public void EndGameScreen(bool failed = false)
	{
		PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
		//Enable end game panel
		endGamePanel.gameObject.SetActive(true);

		// Disable game components
		playerMovement.enabled = false;
		audioSource.Stop();
		FindObjectOfType<RingManager>().gameObject.SetActive(false);

		// Get references
		PlayerCollision playerCollision = FindObjectOfType<PlayerCollision>();
		Text levelResult = endGamePanel.GetChild(0).GetComponent<Text>();
		Text[] scores = endGamePanel.GetChild(1).GetComponentsInChildren<Text>();
		Text totalScore = scores[0];
		Text maxCombo = scores[1];
		Text percentage = scores[2];

		if (failed)
		{
			audioSource.PlayOneShot(levelFailed);
			levelResult.text = "FAILED";
			levelResult.color = new Color(.9f, 0, 0);
		}
		else
		{
			audioSource.PlayOneShot(levelComplete);
			levelResult.text = "COMPLETE";
			levelResult.color = new Color(1, 0.74f, 0);
		}

		// Show on UI
		totalScore.text = playerCollision.TotalPlayerScore.ToString();
		maxCombo.text = playerCollision.MaxComboAchieved.ToString();
		percentage.text = playerCollision.ScorePercentage.ToString("F2") + "%";

		string fileLoc = CrossSceneData.ScoresDir + "/" + CrossSceneData.SelectedLevelName;
		FindObjectOfType<LevelDataPasser>().SaveScore(fileLoc, playerCollision.TotalPlayerScore, playerCollision.MaxComboAchieved);
	}
}
