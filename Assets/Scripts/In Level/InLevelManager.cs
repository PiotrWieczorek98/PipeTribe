using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InLevelManager : MonoBehaviour
{
    public string MusicName { get; private set; }
    public string MusicDir { get; private set; }
    public AudioClip music;
    public AudioClip levelComplete;
    public AudioClip levelFailed;
    public float MusicNotesOffset;
    // Minimum time between notes to allow change of segment the note is placed on
    public float minDeltaTimeToSwitch;
    public float healthRegainOnHit;
    public float healthLossOnFail;

    public float loadingTime;
    public Transform endGamePanel;
    float timeSinceLoaded = 0;

    AudioSource audioSource;
    UIComponents uiComponents;
    Text currentTimeUI;
    Text totalTimeUI;
    RectTransform currentTimeBar;

    void Awake()
    {
        uiComponents = FindObjectOfType<UIComponents>();

        currentTimeUI = uiComponents.Timer.GetChild(0).GetComponent<Text>();
        totalTimeUI = uiComponents.Timer.GetChild(1).GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
        currentTimeBar = uiComponents.TimerBar;
    }

    void Start()
    {
        StartCoroutine(FinishLoading(loadingTime));
        totalTimeUI.text = music.length.ToString("F0") + " sec";
    }


    // Update is called once per frame
    void Update()
    {
        float timePassedPercent = timeSinceLoaded / music.length;
        currentTimeBar.localScale = new Vector3(timePassedPercent,1,1);
        
        timeSinceLoaded += Time.deltaTime;
        currentTimeUI.text = timeSinceLoaded.ToString("F0");

        if(timeSinceLoaded > music.length)
            EndGameScreen();
    }

    // This method is required to avoid desync and lag at the beggining of play when entities still load;
    public IEnumerator FinishLoading(float delay)
    {
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        // Disable player to stop movement of the pipe system
        playerMovement.enabled = false;

        // Wait to sync
        yield return new WaitForSeconds(delay);

        // Set everything ready to start
        playerMovement.enabled = true;
        audioSource.PlayOneShot(music);
        timeSinceLoaded = 0;
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
    }
}
